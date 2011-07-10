namespace CH.Spawner
{
    public class TextWriter : IWriter
    {
        private readonly System.IO.TextWriter _systemIoTextWriter;
        public TextWriter(System.IO.TextWriter systemIoTextWriter)
        {
            _systemIoTextWriter = systemIoTextWriter;
        }

        public void Write(string text)
        {
            _systemIoTextWriter.Write(text);
        }

        public void Write(string formatString, params object[] formatArguments)
        {
            _systemIoTextWriter.Write(formatString, formatArguments);
        }

        public void WriteLine(string text)
        {
            _systemIoTextWriter.WriteLine(text);
        }

        public void WriteLine(string formatString, params object[] formatArguments)
        {
            _systemIoTextWriter.WriteLine(formatString, formatArguments);
        }
    }
}