using System;
using System.Linq;

namespace Enterwell.CI.Changelog.CLI
{
    /// <summary>
    /// Class that is used for Json to Deserialize configuration file to.
    /// </summary>
    public class Configuration
    {
        public static string ConfigurationName = ".changelog.json";

        /// <summary>
        /// Only changes with these categories are accepted. 
        /// </summary>
        public string[] Categories { get; set; } = new string[0];

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
    }
}