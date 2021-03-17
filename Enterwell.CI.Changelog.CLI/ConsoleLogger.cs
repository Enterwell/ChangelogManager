using System;

namespace Enterwell.CI.Changelog.CLI
{
    /// <summary>
    /// Class used to log to the console with appropriate foreground colors.
    /// </summary>
    public class ConsoleLogger
    {
        /// <summary>
        /// Uses the console to notify the user if the change file was successfully created and added. 
        /// </summary>
        /// <param name="changeCreated"><see cref="bool"/> that is used to determine if the file was created or not.</param>
        /// <param name="reason"><see cref="string"/> that specifies the reason if the file was not created.</param>
        /// <param name="filePath"><see cref="string"/> that specifies the path on which the file was created or tried to be created.</param>
        public void LogResult(bool changeCreated, string reason, string filePath)
        {
            if (!changeCreated)
            {
                LogError($"Adding Change Failed. Reason: {reason}\nTried to add change file to: {filePath}");
            }
            else
            {
                LogSuccess($"Change Added Successfully\nPath: {filePath}");
            }
        }

        /// <summary>
        /// Logs an error to the console using the appropriate foreground color and shows how to look for help in this CLI application.
        /// </summary>
        /// <param name="statusText"><see cref="string"/> to log to the console.</param>
        public void LogError(string statusText)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(statusText);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Error.WriteLine("Specify --help for a list of available options and commands");
        }

        /// <summary>
        /// Logs a success to the console using the appropriate foreground color.
        /// </summary>
        /// <param name="statusText"><see cref="string"/> to log to the console.</param>
        private void LogSuccess(string statusText)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Error.WriteLine(statusText);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}