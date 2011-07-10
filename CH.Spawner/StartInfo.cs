namespace CH.Spawner
{
    public class StartInfo : IStartInfo
    {
        public string ProgramName { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }
        public string StdIn { get; set; }
        public IWriter StdOut { get; set; }
        public IWriter StdErr { get; set; }
    }
}