using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace CH.Spawner.Test
{
    [TestFixture]
    public class SpawnerTestFixture
    {
        private static readonly string _spawnerTestExe = Path.Combine("..", "..", "..", "CH.Spawner.TestExe", "bin",
                                                                      "Release",
                                                                      "CH.Spawner.TestExe.exe");

        [Test]
        public void ShouldThrowExceptionIfProgramDoesntExist()
        {
            using (var spawner = new Spawner())
            {
                Assert.Throws(typeof (ArgumentException), () => spawner.Spawn(new StartInfo {ProgramName = "asdf"}));
            }
        }

        [Test]
        public void ShouldCaptureOutputFromProcess()
        {
            var stringWriter = new StringWriter();
            using (var spawner = new Spawner())
            {
                var processInfo =
                    spawner.Spawn(
                        new StartInfo
                            {
                                ProgramName = _spawnerTestExe,
                                Arguments = "--large-output",
                                StdOut = stringWriter,
                            });
                Assert.That(spawner.Wait(TimeSpan.MaxValue, processInfo), Is.True);
            }
            var outputLine = stringWriter.ToString().Split(new[] {Environment.NewLine},
                                                           StringSplitOptions.RemoveEmptyEntries);

            Assert.That(outputLine.Length, Is.EqualTo(10000));
        }

        [Test]
        public void ShouldTrackRunningProcesses()
        {
            using (var spawner = new Spawner())
            {
                spawner.Spawn(
                    new StartInfo
                        {
                            ProgramName = _spawnerTestExe,
                            Arguments = "--wait",
                        });
                spawner.Spawn(
                    new StartInfo
                        {
                            ProgramName = _spawnerTestExe,
                            Arguments = "--wait",
                        });
                Assert.That(spawner.Tracked.Count(), Is.EqualTo(2));
                Assert.That(spawner.Wait(TimeSpan.MaxValue, spawner.Tracked.First()), Is.True);
                Assert.That(spawner.Tracked.Count(), Is.EqualTo(1));
                Assert.That(spawner.Wait(TimeSpan.MaxValue, spawner.Tracked.First()), Is.True);
                Assert.That(spawner.Tracked.Count(), Is.EqualTo(0));
            }
        }
    }
}