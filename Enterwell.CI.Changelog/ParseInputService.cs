using System;
using System.IO;
using Enterwell.CI.Changelog.Models;

namespace Enterwell.CI.Changelog
{
    /// <summary>
    /// Service that parses input arguments and returns them to the user.
    /// </summary>
    public class ParseInputService
    {
        /// <summary>
        /// Method that parses application input arguments.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when arguments are not valid.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <c>null</c> is passed instead of input arguments.</exception>
        /// <param name="inputArguments">Input arguments passed to the application.</param>
        /// <returns>An <see cref="Inputs"/> instance.</returns>
        public Inputs ParseInputs(string[] inputArguments)
        {
            if (inputArguments == null) throw new ArgumentNullException(nameof(inputArguments));

            // Checking for exactly all inputs (changelog location and changes location).
            if (inputArguments.Length != typeof(Inputs).GetProperties().Length)
            {
                throw new ArgumentException("Correct usage: <changelog location> <changes location>");
            }

            var changelogPath = inputArguments[0].Trim();
            var changesPath = inputArguments[1].Trim();

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

            return new Inputs(changelogPath, changesPath);
        }
    }
}