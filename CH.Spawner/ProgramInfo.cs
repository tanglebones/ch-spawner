using System.Diagnostics;

namespace CH.Spawner
{
    internal class ProgramInfo : IProgramInfo
    {
        public IStartInfo StartInfo {  get;  private set; }
        public Process Process { get; private set; }
        public AsyncStreamReadState[] StreamArray { get; private set; }
        
        public ProgramInfo(IStartInfo startInfo, Process process, AsyncStreamReadState[] streamArray)
        {
            StartInfo = startInfo;
            Process = process;
            StreamArray = streamArray;
        }
    }
}