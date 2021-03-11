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
        /// <returns>Returns true if the change category is valid and false otherwise.</returns>
        public bool IsValid(string changeCategory)
        {
            if (Categories.Length == 0) return true;

            return Categories.Contains(changeCategory);
        }

        /// <summary>
        /// Indicates if the configuration file is empty and has no rules to validate against.
        /// </summary>
        /// <returns>A <see cref="bool"/> representing if the configuration is empty or not.</returns>
        public bool IsEmpty()
        {
            return Categories.Length == 0;
        }

        /// <summary>
        /// Loads the configuration from the directory root.
        /// </summary>
        /// <param name="directoryPath">Path to the directory where the changelog configuration file should be located.</param>
        /// <returns><see cref="Configuration"/> object deserialized from the changelog configuration file.</returns>
        /// <exception cref="ArgumentException">Thrown when arguments are not valid.</exception>
        public static Configuration LoadConfiguration(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(directoryPath));

            var configurationFilePath = Path.Combine(directoryPath, ConfigurationName);

            // First check to see if the configuration file exists.
            if (File.Exists(configurationFilePath))
            {
                return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(configurationFilePath));
            }

            return null;
        }
    }
}