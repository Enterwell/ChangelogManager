using Enterwell.CI.Changelog.CLI.ValidationRules;
using Enterwell.CI.Changelog.Shared;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Enterwell.CI.Changelog.CLI
{
    /// <summary>
    /// Entry point to the CLI application.
    /// </summary>
    [Command(Name = "cc",
            FullName = "Changelog Create CLI Tool",
            Description = "A CLI tool to be used for creating `changes` files.")]
    [VersionOptionFromMember("-v|--version", MemberName = "GetVersion")]
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
        [Argument(0, "Change Type", "Change type following the 'Keep a Changelog' guiding principle.")]
        [Required]
        [AllowedValues(StringComparison.InvariantCultureIgnoreCase,
            "Added", "a", "Changed", "c", "Deprecated", "d", "Removed", "r", "Fixed", "f", "Security", "s",
            IgnoreCase = true)]
        public string Type { get; set; }

        /// <summary>
        /// Second argument to the CLI application representing the change description. Required.
        /// </summary>
        [Argument(1, "Change Description", "Quoted change description that describes the changes made.")]
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Option parameter to the CLI application representing the change category. Required only if the configuration file exists.
        /// </summary>
        [Option(Description = "One of the valid change categories defined in the configuration file, or arbitrary if configuration does not exist. Needs to be quoted if the name is longer than one word.")]
        [ValidCategory]
        public string? Category { get; set; }

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

            // Manually validate the category in the case it was not assigned, but should have been assigned and format it correctly.
            var formattedCategory = this.ValidateAndFormatCategory(logger, this.Category?.Trim());

            var changesDirectory = FileSystemHelper.FindNearestChangesFolder();

            if (string.IsNullOrWhiteSpace(changesDirectory))
            {
                FileSystemHelper.EnsureChangesDirectoryExists(Directory.GetCurrentDirectory());
            }

            string inputType = this.FormatTypeCorrectly(this.Type);
            string fileName = FileSystemHelper.ConstructFileName(inputType, formattedCategory, this.Description);

            string filePath;
            if (!string.IsNullOrWhiteSpace(changesDirectory))
            {
                filePath = Path.Combine(changesDirectory, fileName);
            }
            else
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), FileSystemHelper.ChangeDirectoryName, fileName);
            }

            (bool isSuccessful, string reason) = FileSystemHelper.CreateFile(filePath);

            logger.LogResult(isSuccessful, reason, filePath);
        }

        /// <summary>
        /// Formats the type entered by the user so that it is correctly spelled and saved as a file later.
        /// </summary>
        /// <param name="type">User entered change type to format.</param>
        /// <returns>Correctly formatted change type.</returns>
        private string FormatTypeCorrectly(string type)
        {
            // If the input type is only one word, replace it with its fully named equivalent.
            string inputType = type.Trim().ToLower();
            if (inputType.Length == 1)
            {
                var inputTypeIndex = this.AllowedTypes.IndexOf(inputType);

                inputType = this.AllowedTypes[inputTypeIndex - 1];
            }

            // Ensure that the first letter is upper case and the rest are lower case.
            return inputType.First().ToString().ToUpper() + inputType[1..].ToLower();
        }

        /// <summary>
        /// Validating <see cref="Category"/> property in the case user did not specify it, but should have because the configuration file with allowed categories exists.
        /// </summary>
        /// <param name="logger">The console logger.</param>
        /// <param name="category">User entered change category to format.</param>
        /// <returns>Correctly formatted change category.</returns>
        private string ValidateAndFormatCategory(ConsoleLogger logger, string? category)
        {
            var config = Configuration.LoadConfiguration(Directory.GetCurrentDirectory());

            if (string.IsNullOrWhiteSpace(category) && config != null && !config.IsEmpty())
            {
                logger.LogError("The -c|--category field is required.");
                Environment.Exit(1);
            }

            if (config == null)
            {
                return string.IsNullOrWhiteSpace(category) ? string.Empty : category;
            }

            return config.FormatCategoryCorrectly(category!);
        }

        /// <summary>
        /// Reads an application version from the .csproj file.
        /// </summary>
        /// <returns><see cref="string"/> representing the application version.</returns>
        private string? GetVersion()
        {
            return typeof(Program)
                .Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
        }
    }
}