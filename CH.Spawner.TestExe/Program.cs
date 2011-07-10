using System;
using System.Collections.Generic;
using System.Linq;

namespace CH.Spawner.TextExe
{
    public static class Program
    {
        private static readonly IDictionary<string, Action> Actions =
            new Dictionary<string, Action>(StringComparer.InvariantCultureIgnoreCase)
                {
                    {"--large-output", LargeOutput},
                    {"--wait", Wait}
                };

        public static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                Action action;
                if (Actions.TryGetValue(arg, out action))
                {
                    action();
                }
            }
        }

        private static void LargeOutput()
        {
            foreach (var line in Enumerable.Range(0, 10000))
            {
                Console.WriteLine("test line: " + line);
            }
        }
        private static void Wait()
        {
            System.Threading.Thread.Sleep(1000);
        }
    }
}