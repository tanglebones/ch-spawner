using System.Text;

namespace CH.Spawner
{
    public class StringWriter : IWriter
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public void Write(string text)
        {
            _stringBuilder.Append(text);
        }

        public void Write(string formatString, params object[] formatArguments)
        {
            _stringBuilder.AppendFormat(formatString, formatArguments);
        }

        public void WriteLine(string text)
        {
            _stringBuilder.AppendLine(text);
        }

        public void WriteLine(string formatString, params object[] formatArguments)
        {
            _stringBuilder.AppendFormat(formatString, formatArguments);
            _stringBuilder.AppendLine();
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
}