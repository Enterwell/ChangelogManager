using System;
using System.IO;

namespace Enterwell.CI.Changelog
{
    /// <summary>
    /// Service that parses input arguments and returns them to the user.
    /// </summary>
    public class ParseInputService
    {
        /// <summary>
        /// Method that parses application input arguments and returns them to the caller.
        /// </summary>
        /// <param name="inputArguments">Input arguments passed to the application.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when arguments are not valid.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <code>NULL</code> is passed instead of input arguments.</exception>
        public (string semanticVersion, string changelogLocation, string changesLocation) ParseInputs(string[] inputArguments)
        {
            if (inputArguments == null) throw new ArgumentNullException(nameof(inputArguments));

            // Checking for exactly three inputs (semantic version, changelog location and changes location).
            if (inputArguments.Length != 3)
            {
                throw new ArgumentException("Correct usage: <version[major.minor.patch]> <changelog location> <changes location>");
            }

            var semanticVersion = inputArguments[0].Trim();
            var changelogPath = inputArguments[1].Trim();
            var changesPath = inputArguments[2].Trim();

            // Checking if all three components are present in the version input.
            if (semanticVersion.Split('.').Length != 3)
            {
                throw new ArgumentException($"Expected input format: <major.minor.patch>. Got: '{semanticVersion}'. Check your separation dots!");
            }

            // Checking if changelog directory exists.
            if (!Directory.Exists(changelogPath))
            {
                throw new ArgumentException($"Changelog location does not exist on path: {changelogPath}");
            }

            // Checking if changes directory exists.
            if (!Directory.Exists(changesPath))
            {
                throw new ArgumentException($"Changes location does not exist on path: {changesPath}");
            }

            return (semanticVersion, changelogPath, changesPath);
        }
    }
}