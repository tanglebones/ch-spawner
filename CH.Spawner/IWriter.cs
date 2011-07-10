namespace CH.Spawner
{
    public interface IWriter
    {
        void Write(string text);
        void Write(string formatString, params object[] formatArguments);
        void WriteLine(string text);
        void WriteLine(string formatString, params object[] formatArguments);
    }
}