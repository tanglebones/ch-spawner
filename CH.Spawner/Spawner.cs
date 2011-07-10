using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CH.Spawner
{
    // NOTE: This doesn't implement the .net security model. Mainly because the docs for the .Net security model as horrible and everything I've tried to get FxCop to pass
    // doesn't work. If someone can figure out the correct mix of attributes to sprinkle about please let me know.
    public sealed class Spawner : ISpawner, IDisposable
    {
        private const int StreamReadBufferSize = 1024;
        private readonly IDictionary<int, ProgramInfo> _trackedProcessDictionary = new Dictionary<int, ProgramInfo>();

        public IEnumerable<IProgramInfo> Tracked
        {
            get { return _trackedProcessDictionary.Values; }
        }

        public bool Wait(TimeSpan timeout, params IProgramInfo[] programInfoArray)
        {
            foreach (var programInfo in programInfoArray.Cast<ProgramInfo>())
            {
                Trace.WriteLine("Waiting on pid " + programInfo.Process.Id + " to exit.");
                var requestedTimeoutMilliseconds = timeout.TotalMilliseconds;
                var timeoutMilliseconds = requestedTimeoutMilliseconds > int.MaxValue ? int.MaxValue : (int)requestedTimeoutMilliseconds;
                if (!programInfo.Process.WaitForExit(timeoutMilliseconds))
                    return false;
                for (;;)
                {
                    var waitHandles =
                        programInfo.StreamArray.Where(x => !x.Complete).Select(x => x.AsyncResult.AsyncWaitHandle).
                            ToArray();
                    if (waitHandles.Length == 0) break;
                    WaitHandle.WaitAll(waitHandles);
                }
                Trace.WriteLine("Removing pid " + programInfo.Process.Id + " from tracked.");
                _trackedProcessDictionary.Remove(programInfo.Process.Id);
                programInfo.Process.Dispose();
            }
            return true;
        }

        public IProgramInfo Spawn(IStartInfo startInfo)
        {
            var processStartInfo =
                new ProcessStartInfo
                    {
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        ErrorDialog = false,
                        UseShellExecute = false,
                        FileName = startInfo.ProgramName,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                    };

            if (startInfo.Arguments != null) processStartInfo.Arguments = startInfo.Arguments;
            if (startInfo.WorkingDirectory != null) processStartInfo.WorkingDirectory = startInfo.WorkingDirectory;
            if (!File.Exists(startInfo.ProgramName))
                throw new ArgumentException(
                    "Could not find startInfo.ProgramName \""
                    + startInfo.ProgramName
                    + "\" to spawn."
                    );
// ReSharper disable UseObjectOrCollectionInitializer
            // Object Collection Initializer can cause the creation of a temporary that doesn't get Dispose() called on it.
            var process = new Process();
// ReSharper restore UseObjectOrCollectionInitializer

            process.StartInfo = processStartInfo;

            if (process.Start())
            {
                process.StandardInput.Write(startInfo.StdIn);
                process.StandardInput.Close();

                var streamArray =
                    new[]
                        {
                            new AsyncStreamReadState
                                {
                                    Stream = process.StandardOutput.BaseStream,
                                    Buffer = new byte[StreamReadBufferSize],
                                    AsyncResult = null,
                                    Writer = startInfo.StdOut,
                                },
                            new AsyncStreamReadState
                                {
                                    Stream = process.StandardError.BaseStream,
                                    Buffer = new byte[StreamReadBufferSize],
                                    AsyncResult = null,
                                    Writer = startInfo.StdErr,
                                }
                        };

                var pid = process.Id;
                var programInfo = new ProgramInfo(startInfo, process, streamArray);
                _trackedProcessDictionary[pid] = programInfo;

                foreach (var s in streamArray)
                    s.AsyncResult = s.Stream.BeginRead(s.Buffer, 0, s.Buffer.Length, ContinueRead, s);

                return programInfo;
            }
            throw new Exception("Could not start process: " + startInfo.ProgramName);
        }

        private static void ContinueRead(IAsyncResult ar)
        {
            var s = (AsyncStreamReadState) ar.AsyncState;
            var nr = s.Stream.EndRead(ar);
            if (nr <= 0)
            {
                s.Complete = true;
                return;
            }
            if (s.Writer != null)
                s.Writer.Write(Encoding.ASCII.GetString(s.Buffer, 0, nr));
            s.AsyncResult = s.Stream.BeginRead(s.Buffer, 0, s.Buffer.Length, ContinueRead, s);
        }

        public void Dispose()
        {
            foreach (var programInfo in _trackedProcessDictionary.Values.ToArray())
            {
                programInfo.Process.Kill();
                programInfo.Process.WaitForExit();
                for (;;)
                {
                    var waitHandles =
                        programInfo.StreamArray.Where(x => !x.Complete).Select(x => x.AsyncResult.AsyncWaitHandle).
                            ToArray();
                    if (waitHandles.Length == 0) break;
                    WaitHandle.WaitAll(waitHandles);
                }
                _trackedProcessDictionary.Remove(programInfo.Process.Id);
                programInfo.Process.Dispose();
            }
        }
    }
}