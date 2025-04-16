using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Enterwell.CI.Changelog.Shared;

namespace Enterwell.CI.Changelog
{
    /// <summary>
    /// Service that writes new changelog section to changelog file.
    /// </summary>
    public class FileWriterService
    {
        private const string ChangelogFileName = "Changelog.md";

        /// <summary>
        /// Method that writes to changelog file.
        /// </summary>
        /// <param name="textToWrite">Text to write to the changelog file.</param>
        /// <param name="changelogLocation">Path to the directory containing the changelog file.</param>
        /// <param name="insertBefore">Text before which the method will insert <code>textToWrite</code>.</param>
        /// <exception cref="ArgumentException">Thrown when arguments are not valid.</exception>
        /// <exception cref="FileNotFoundException">Thrown when changelog file does not exist at the <see cref="changelogLocation"/>.</exception>
        /// <returns></returns>
        public async Task WriteToChangelog(string textToWrite, string changelogLocation, string insertBefore)
        {
            if (string.IsNullOrWhiteSpace(textToWrite))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(textToWrite));
            if (string.IsNullOrWhiteSpace(changelogLocation))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(changelogLocation));
            if (string.IsNullOrWhiteSpace(insertBefore))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(insertBefore));

            // Correctly-cased path for a changelog file 
            var changelogFilePath = FileSystemHelper.GetFilePathCaseInsensitive(Path.Combine(changelogLocation, ChangelogFileName));
            var changelogText = (await File.ReadAllLinesAsync(changelogFilePath)).ToList();

            // Get the index of a line before H2 with older version.
            var index = changelogText.FindIndex(0, line => line.StartsWith(insertBefore)) - 1;

            // Remove that blank like before the older version because our TextToWrite already has blank lines before and after.
            changelogText.RemoveAt(index);
            changelogText.Insert(index, $"\r\n{textToWrite}");

            await File.WriteAllLinesAsync(changelogFilePath, changelogText);
        }

        /// <summary>
        /// Method that will set the new application's version to the project file.
        /// </summary>
        /// <param name="newVersion">Application's bumped version.</param>
        /// <param name="projectFilePath">Path to the project file.</param>
        /// <param name="revisionNumber">(Optional) revision number.</param>
        /// <returns>An asynchronous task.</returns>
        public async Task BumpProjectFileVersion(string newVersion, string projectFilePath, int? revisionNumber)
        {
            // Try to automatically determine the project file
            if (string.IsNullOrWhiteSpace(projectFilePath))
            {
                var determinedProjectFilePath = FileSystemHelper.GetTheProjectFile();

                // If no file was found, throw
                if (string.IsNullOrWhiteSpace(determinedProjectFilePath))
                {
                    throw new FileNotFoundException("Could not find a 'package.json' file or a '.csproj' file with a 'Version' tag.");
                }

                projectFilePath = determinedProjectFilePath;
            }

            // If the explicit project file does not exist, throw
            if (!File.Exists(projectFilePath))
            {
                throw new FileNotFoundException("Could not find the given project file.");
            }

            // Handle the file contents
            var fileContent = await File.ReadAllTextAsync(projectFilePath);

            var universalVersionPattern = """(?<=("version"\s*:\s*"|<Version>|<PackageVersion>|Version:\s*|\[assembly:\s*AssemblyVersion\("|<Identity[^>]*\sVersion="))(\d+\.\d+\.\d+)(?:\.(\d+))?""";
            var patternRegex = new Regex(universalVersionPattern, RegexOptions.IgnoreCase);

            var updatedContent = patternRegex.Replace(fileContent, match =>
            {
                var baseVersion = newVersion;
                var existingRevision = match.Groups[3].Success ? match.Groups[3].Value : "";

                // If we have explicit revision number, use it
                if (revisionNumber.HasValue)
                {
                    return $"{baseVersion}.{revisionNumber.Value}";
                }

                // Otherwise, use the revision number we found in the file
                if (!string.IsNullOrWhiteSpace(existingRevision))
                {
                    return $"{baseVersion}.{existingRevision}";
                }

                // Otherwise, just use the base version
                return baseVersion;
            }, 1);

            if (updatedContent != fileContent)
            {
                await File.WriteAllTextAsync(projectFilePath, updatedContent);
            }
        }
    }
}