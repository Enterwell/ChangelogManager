using Enterwell.CI.Changelog.CLI.ValidationRules;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

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

        /// <summary>
        /// First argument to the CLI application representing the change type. Required. Checking for allowed values.
        /// </summary>
        [Argument(0, "Change Type", "Change type following the 'Keep a Changelog' guiding principle.", ShowInHelpText = true)]
        [Required]
        [AllowedValues(StringComparison.InvariantCultureIgnoreCase,
            "Added", "a", "Changed", "c", "Deprecated", "d", "Removed", "r", "Fixed", "f", "Security", "s",
            IgnoreCase = true)]
        public string Type { get; set; }

        /// <summary>
        /// Second argument to the CLI application representing the change description. Required.
        /// </summary>
        [Argument(1, "Change Description", "Change description that describes the changes made.", ShowInHelpText = true)]
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Option parameter to the CLI application representing the change category. Required only if the configuration file exists.
        /// </summary>
        [Option(Description = "One of the valid change categories determined in the configuration file, or arbitrary if configuration does not exist.", ShowInHelpText = true)]
        [ValidCategory]
        public string Category { get; set; }

        /// <summary>
        /// Allowed types of changes. Used in the program for linking the one-letter versions to their whole-word equivalents.
        /// </summary>
        // Not sure about this. Now we have 2 sources of truth, in AllowedValues attribute and here. But the attribute is nice because it helps generate nice -h | --help and 
        // documentation about this CLI application
        public List<string> AllowedTypes = new() { "Added", "a", "Changed", "c", "Deprecated", "d", "Removed", "r", "Fixed", "f", "Security", "s" };

        /// <summary>
        /// Method that runs when the arguments are provided and they pass their "initial" validation.
        /// </summary>
        private void OnExecute()
        {
            var logger = new ConsoleLogger();

            // Because 'ValidCategory' attribute validation only fires IF user passed in something for the category option, we have to manually validate 
            // it in the case category was not assigned, but should have been assigned.
            ValidateCategory(logger);

            EnsureChangesDirectoryExists();

            var inputType = FormatTypeCorrectly();
            var fileName = ConstructFileName(inputType);

            var creationResult = CreateFile(Path.Combine(Directory.GetCurrentDirectory(), "changes", fileName));

            logger.LogResult(creationResult.isSuccessfull, creationResult.reason);
        }

        /// <summary>
        /// Formats the type entered by the user so that it is correctly spelled and saved as a file later.
        /// </summary>
        /// <returns></returns>
        private string FormatTypeCorrectly()
        {
            // If the input type is only one word, replace it with its fully named equivalent.
            string inputType = Type;
            if (inputType.Length == 1)
            {
                var inputTypeIndex = AllowedTypes.IndexOf(inputType);

                inputType = AllowedTypes[inputTypeIndex - 1];
            }

            // Ensure that the first letter is upper case and the rest are lower case.
            return inputType.First().ToString().ToUpper() + inputType[1..].ToLower();
        }

        /// <summary>
        /// Constructs the file name using the arguments passed in by the user to the CLI application and the manually formatted change type.
        /// </summary>
        /// <param name="inputType">Manually formatted change type so that it is spelled correctly.</param>
        /// <returns>Name of the file to be saved in the folder where changes are stored.</returns>
        private string ConstructFileName(string inputType)
        {
            var description = Description.Trim();

            var category = Category.Trim();
            if (string.IsNullOrWhiteSpace(category))
            {
                return $"{inputType} {description}";
            }
            else
            {
                return $"{inputType} [{category}] {description}";
            }
        }

        /// <summary>
        /// Creates the file on the given file path.
        /// </summary>
        /// <param name="filePath">Path to the file that needs to be created.</param>
        /// <returns>A value <see cref="Tuple"/> with 2 components. A <see cref="bool"/> that represents if the file was successfully created and a
        /// <see cref="string"/> that specifies the reason if not.</returns>
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

        /// <summary>
        /// Used to ensure that the changes directory exists. If it does not exist, the directory is created.
        /// </summary>
        private void EnsureChangesDirectoryExists()
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
        private void ValidateCategory(ConsoleLogger logger)
        {
            if (string.IsNullOrWhiteSpace(Category) && ConfigurationExists())
            {
                logger.LogError("The -c|--category field is required.");
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
