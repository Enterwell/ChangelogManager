using System;

namespace Enterwell.CI.Changelog.CLI
{
    public class ConsoleLogger
    {
        public void LogResult(bool changeCreated, string reason)
        {
            var statusText = changeCreated ? "Change Added Successfully" : $"Adding Change Failed. Reason: {reason}";

            if (!changeCreated)
            {
                LogError(statusText);
            }
            else
            {
                LogSuccess(statusText);
            }
        }

        public void LogError(string statusText)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(statusText);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Error.WriteLine("Specify --help for a list of available options and commands");
        }

        private void LogSuccess(string statusText)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Error.WriteLine(statusText);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}