using System;
using System.Linq;

namespace Enterwell.CI.Changelog.Models
{
    /// <summary>
    /// Class used for Json to deserialize the configuration file into.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Only changes with these categories are accepted. 
        /// </summary>
        public string[] Categories { get; set; } = [];

        /// <summary>
        /// Custom rules with which to bump the application's semantic version.
        /// </summary>
        public BumpingRule? BumpingRule { get; set; }

        /// <summary>
        /// Validates a change based on Configuration object properties.
        /// </summary>
        /// <param name="changeDescription">User change description.</param>
        /// <returns>Returns <c>true</c> if the change description is valid and <c>false</c> otherwise.</returns>
        public bool IsValid(string changeDescription)
        {
            // If there are no validation categories think of it as true.
            if (this.Categories.Length == 0) return true;

            var descriptionSplit = changeDescription.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            // Removing [ and ] around the change type. Ex. [API] --> API
            var categoryType = descriptionSplit[0][1..^1];

            return this.Categories.Contains(categoryType);
        }
    }
}