using System;
using System.Linq;

namespace Enterwell.CI.Changelog
{
    /// <summary>
    /// Class that is used for Json to Deserialize configuration file to.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Only changes with these categories are accepted. 
        /// </summary>
        public string[] Categories { get; set; } = new string[0];

        /// <summary>
        /// Validates a change based on Configuration object properties.
        /// </summary>
        /// <param name="changeDescription">Description of user change.</param>
        /// <returns>Returns true if the change description is valid and false otherwise.</returns>
        public bool IsValid(string changeDescription)
        {
            // If there are no validation categories think of it as true.
            if (Categories.Length == 0) return true;

            var descriptionSplit = changeDescription.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            // Removing the [ and ] around change type. Ex. [API] --> API
            var categoryType = descriptionSplit[0][1..^1];

            return Categories.Contains(categoryType);
        }
    }
}