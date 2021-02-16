using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using Enterwell.CI.Changelog.CLI.ValidationRules;
using McMaster.Extensions.CommandLineUtils;

namespace Enterwell.CI.Changelog.CLI
{
    /// <summary>
    /// Entry point to the CLI application.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main method that starts the CLI application passing it the input arguments.
        /// </summary>
        /// <param name="args">Arguments user provided through command line.</param>
        /// <returns></returns>
        static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Argument(0, "Change Type", "Change type following the 'Keep a Changelog' guiding principle.", ShowInHelpText = true)]
        [Required]
        [AllowedValues(StringComparison.InvariantCultureIgnoreCase,
            "added", "a", "changed", "c", "deprecated", "d", "removed", "r", "fixed", "f", "security", "s",
            IgnoreCase = true)]
        public string Type { get; set; }

        [Argument(1, "Change Description", "Change description that describes the changes made.", ShowInHelpText = true)]
        [Required]
        public string Description { get; set; }

        [Option(Description = "One of the valid change categories determined in the configuration file, or arbitrary if configuration does not exist.", ShowInHelpText = true)]
        [ValidCategory]
        public string Category { get; set; }

        public List<string> CLIAllowedTypes = new() {"added", "a", "changed", "c", "deprecated", "d", "removed", "r", "fixed", "f", "security", "s"};

        /// <summary>
        /// Method that runs when the arguments are provided and they pass "initial" validation.
        /// </summary>
        private void OnExecute()
        {
            // Because 'ValidCategory' attribute validation only fires IF user passed in something for the category option, we have to manually validate 
            // it in the case category was not assigned, but should have been assigned.
            ValidateCategory();

            EnsureChangesDirectoryExist();

            string fileName;

            string type = Type;

            if (type.Length == 1)
            {
                var typeIndex = CLIAllowedTypes.FindIndex(s => s == Type.ToLower());

                type = CLIAllowedTypes[typeIndex - 1];
                type = char.ToUpper(type[0]) + type[1..];
            }

            var description = Description.Trim();
            var category = Category.Trim();

            if (string.IsNullOrWhiteSpace(Category))
            {
                fileName = $"{type} {description}";
            }
            else
            {
                fileName = $"{type} [{category}] {description}";
            }

            var creationResult = CreateFile(Path.Combine(Directory.GetCurrentDirectory(), "changes", fileName));

            LogResult(creationResult.isSuccessfull, creationResult.reason);
        }

        private void LogResult(bool changeCreated, string reason)
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

        private void LogSuccess(string statusText)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Error.WriteLine(statusText);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private void LogError(string statusText)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(statusText);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Error.WriteLine("Specify --help for a list of available options and commands");
        }

        private (bool isSuccessfull, string reason) CreateFile(string filePath)
        {
            try
            {
                var file = File.Create(filePath);
                file.Close();

                return (true, string.Empty);
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }

        private void EnsureChangesDirectoryExist()
        {
            var changesDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "changes");

            if (!Directory.Exists(changesDirectoryPath))
            {
                Directory.CreateDirectory(changesDirectoryPath);
            }
        }

        /// <summary>
        /// Validating <see cref="Category"/> property in the case user did not specify it, but should have because the configuration file with allowed categories exists.
        /// </summary>
        private void ValidateCategory()
        {
            if (string.IsNullOrWhiteSpace(Category) && ConfigurationExists())
            {
                LogError("The --category field is required.");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Checks to see if the configuration file exists in the current directory.
        /// </summary>
        /// <returns>A <see cref="bool"/> representing the if the configuration file exists or not.</returns>
        private bool ConfigurationExists()
        {
            var pathToConfig = Path.Combine(Directory.GetCurrentDirectory(), Configuration.ConfigurationName);

            if (File.Exists(pathToConfig)) return true;
            
            return false;
        }
    }
}
