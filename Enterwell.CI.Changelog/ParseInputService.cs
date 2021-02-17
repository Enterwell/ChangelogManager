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
        public (string semanticVersion, string repositoryPath) ParseInputs(string[] inputArguments)
        {
            if (inputArguments == null) throw new ArgumentNullException(nameof(inputArguments));

            // Checking for exactly two inputs (semantic version and repo directory).
            if (inputArguments.Length != 2)
            {
                throw new ArgumentException("Correct usage: <version[major.minor.patch]> <repository location>");
            }

            var semanticVersion = inputArguments[0].Trim();
            var repositoryPath = inputArguments[1].Trim();

            // Checking if all three components are present in the version input.
            if (semanticVersion.Split('.').Length != 3)
            {
                throw new ArgumentException($"Expected input format: <major.minor.patch>. Got: '{semanticVersion}'. Check your separation dots!");
            }

            // Checking if repository directory exists.
            if (!Directory.Exists(repositoryPath))
            {
                throw new ArgumentException($"Repository does not exist on path: {repositoryPath}");
            }

            return (semanticVersion, repositoryPath);
        }
    }
}