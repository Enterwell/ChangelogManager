using System;

namespace Enterwell.CI.Changelog.CLI
{
    public class ConsoleLogger
    {
        public void LogResult(bool changeCreated, string reason)
        {
            if (!changeCreated)
            {
                LogError($"Adding Change Failed. Reason: {reason}");
            }
            else
            {
                LogSuccess("Change Added Successfully");
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