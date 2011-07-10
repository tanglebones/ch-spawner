namespace CH.Spawner
{
    public interface IStartInfo
    {
        string ProgramName { get; set; }
        string Arguments { get; set; }
        string WorkingDirectory { get; set; }
        string StdIn { get; set; }
        IWriter StdOut { get; set; }
        IWriter StdErr { get; set; }
    }
}