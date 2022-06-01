using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace Enterwell.CI.Changelog.Shared
{
    /// <summary>
    /// Class that is used for Json to Deserialize configuration file to.
    /// </summary>
    public class Configuration
    {
        public static string ConfigurationName = ".changelog.json";

        /// <summary>
        /// Categories from the configuration json file.
        /// Only changes with these categories are accepted. 
        /// </summary>
        public string[] Categories { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Validates a change based on Configuration object properties.
        /// </summary>
        /// <param name="changeCategory">Category of the user change.</param>
        /// <returns>Returns <c>true</c> if the change category is valid and <c>false</c> otherwise.</returns>
        public bool IsValid(string changeCategory)
        {
            return 
                this.Categories.Length == 0 || 
                this.Categories.Any(c => string.Equals(c, changeCategory, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Indicates if the configuration file is empty and has no rules to validate against.
        /// </summary>
        /// <returns>A <see cref="bool"/> representing if the configuration is empty or not.</returns>
        public bool IsEmpty()
        {
            return this.Categories.Length == 0;
        }

        /// <summary>
        /// Formats the category entered by the user so that it is correctly spelled and saved as a file later.
        /// </summary>
        /// <param name="category">User entered change category to format.</param>
        /// <returns>Correctly formatted change category.</returns>
        public string FormatCategoryCorrectly(string category)
        {
            var correctCategory = this.Categories.FirstOrDefault(c => string.Equals(c, category, StringComparison.InvariantCultureIgnoreCase));

            if (correctCategory != null)
            {
                return correctCategory;
            }

            return string.Empty;
        }

        /// <summary>
        /// Loads the configuration from the directory root.
        /// </summary>
        /// <param name="directoryPath">Path to the directory where the changelog configuration file should be located.</param>
        /// <returns><see cref="Configuration"/> object deserialized from the changelog configuration file.</returns>
        /// <exception cref="ArgumentException">Thrown when arguments are not valid.</exception>
        public static Configuration? LoadConfiguration(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(directoryPath));

            var currentDir = new DirectoryInfo(directoryPath);
            while (currentDir != null)
            {
                var possibleConfigFilePath = Path.Combine(currentDir.FullName, ConfigurationName);
                if (File.Exists(possibleConfigFilePath))
                {
                    return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(possibleConfigFilePath));
                }

                currentDir = currentDir.Parent;
            }

            return null;
        }
    }
}