using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Enterwell.CI.Changelog.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            changelogText.Insert(index, $"\r\n${textToWrite}");

            await File.WriteAllLinesAsync(changelogFilePath, changelogText);
        }

        /// <summary>
        /// Method that will set the new application's version to the project file.
        /// </summary>
        /// <param name="newVersion">Application's bumped version.</param>
        /// <param name="projectFilePath">Path to the project file.</param>
        /// <returns>An asynchronous task.</returns>
        public async Task BumpProjectFileVersion(string newVersion, string projectFilePath)
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

            // Manage the package.json file
            if (projectFilePath.EndsWith("package.json"))
            {
                var jsonString = await File.ReadAllTextAsync(projectFilePath);

                if (JsonConvert.DeserializeObject(jsonString) is not JObject jsonObject)
                {
                    throw new InvalidCastException("Could not deserialize the 'package.json' file.");
                }

                // Replacing the JSON 'version' entry
                jsonObject["version"]?.Replace(newVersion);

                await File.WriteAllTextAsync(projectFilePath, jsonObject.ToString());
            }

            // Manage the .csproj file
            if (projectFilePath.EndsWith(".csproj"))
            {
                var csprojString = await File.ReadAllTextAsync(projectFilePath);
                var csprojXml = XDocument.Parse(csprojString);

                var xmlVersionTag = csprojXml
                    .Descendants()
                    .FirstOrDefault(e =>
                        e.Name.ToString().ToLowerInvariant() == "version" &&
                        e.Parent?.Name.ToString().ToLowerInvariant() == "propertygroup");

                // Replace the .csproj 'Version' entry
                xmlVersionTag!.Value = newVersion;

                await File.WriteAllTextAsync(projectFilePath, csprojXml.ToString());
            }
        }
    }
}