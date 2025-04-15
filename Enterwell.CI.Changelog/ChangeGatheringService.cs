using Enterwell.CI.Changelog.Models;
using Enterwell.CI.Changelog.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Configuration = Enterwell.CI.Changelog.Models.Configuration;

namespace Enterwell.CI.Changelog
{
    /// <summary>
    /// Service that gathers changes from the changes directory and returns them to the user.
    /// </summary>
    public class ChangeGatheringService
    {
        private const string ChangelogFileName = "Changelog.md";
        private const string ConfigurationFileName = ".changelog.json";

        private readonly string[] acceptableChanges = ["added", "changed", "deprecated", "removed", "fixed", "security"];

        /// <summary>
        /// Gathers the current application version information that includes the current semantic version number and a dictionary of changes being made.
        /// </summary>
        /// <param name="changelogLocation">Path to the directory containing the changelog file.</param>
        /// <param name="changesLocation">Path to the changes directory.</param>
        /// <returns><see cref="VersionInformation"/> instance.</returns>
        public async Task<VersionInformation> GatherVersionInformation(string changelogLocation, string changesLocation)
        {
            var latestVersionInformation = await this.GatherVersionNumber(changelogLocation);

            // Load user configuration.
            var configuration = await this.LoadConfiguration(changelogLocation);
            var currentChanges = this.GatherChanges(configuration, changesLocation);

            return new VersionInformation(latestVersionInformation.major, latestVersionInformation.minor, latestVersionInformation.patch, currentChanges, configuration?.BumpingRule);
        }

        /// <summary>
        /// Reads the changelog to parse the current application's semantic version.
        /// </summary>
        /// <param name="changelogLocation">Path to the directory containing the changelog file.</param>
        /// <returns>Tuple containing the application's parsed semantic version.</returns>
        private async Task<(int major, int minor, int patch)> GatherVersionNumber(string changelogLocation)
        {
            if (string.IsNullOrWhiteSpace(changelogLocation))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(changelogLocation));

            // Correctly-cased path for a changelog file
            var changelogFilePath = FileSystemHelper.GetFilePathCaseInsensitive(Path.Combine(changelogLocation, ChangelogFileName));

            if (!File.Exists(changelogFilePath))
            {
                throw new FileNotFoundException("Changelog file is not found.");
            }

            var changelogText = await File.ReadAllTextAsync(changelogFilePath);
            var groupMatch = Regex.Match(changelogText, @"\[(\d+.\d+.\d+)\]");

            var latestVersionMatch = groupMatch.Groups[1].Value;    
            if (string.IsNullOrWhiteSpace(latestVersionMatch)) latestVersionMatch = "0.0.0";

            var versionParts = latestVersionMatch.Split('.');
            if (versionParts.Length != 3)
            {
                throw new ArgumentException($"Expected version format: <major.minor.patch>. Got: '{latestVersionMatch}'.");
            }

            return (int.Parse(versionParts[0]), int.Parse(versionParts[1]), int.Parse(versionParts[2]));
        }

        /// <summary>
        /// Reads the changes from the changes folder in the repository on a given changes location and returns the dictionary with change type keys.
        /// </summary>
        /// <param name="configuration">User changelog manager configuration.</param>
        /// <param name="changesLocation">Path to the changes directory.</param>
        /// <returns>Returns the dictionary whose keys are change types and values are all the changes of the corresponding change type.</returns>
        private Dictionary<string, List<ChangeInfo>> GatherChanges(Configuration? configuration, string changesLocation)
        {
            var changes = new Dictionary<string, List<ChangeInfo>>();

            var filesPath = Directory.GetFiles(changesLocation);

            foreach (string filePath in filesPath)
            {
                var fileName = Path.GetFileName(filePath);
                string[] fileNameSplit = fileName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

                // If the file can't be split into two parts at this stage, it really shouldn't be accepted. It contains only one word.
                if (fileNameSplit.Length == 1) continue;

                var changeType = fileNameSplit[0].ToLower();

                // If the change type is not acceptable, skip it.
                if (!this.acceptableChanges.Contains(changeType)) continue;

                var changeDescription = fileNameSplit[1];

                // Replace multiple spaces with a single space for consistency.
                changeDescription = Regex.Replace(changeDescription, @"\s+", " ");

                // Remove all the whitespaces inside the [ ] angle brackets.
                changeDescription = Regex.Replace(changeDescription, @"\[\s*(\w+)\s*\]", "[$1]");

                // If configuration exists and our change is not valid, ignore the change file. Else just ignore all validation.
                if (configuration != null && !configuration.IsValid(changeDescription)) continue;

                // Changing the change type so that the first letter is upper case and the rest are lower case.
                var changeKey = char.ToUpper(changeType[0]) + changeType[1..].ToLower();

                if (!changes.ContainsKey(changeKey))
                {
                    changes.Add(changeKey, new List<ChangeInfo>());
                }

                changes[changeKey].Add(new ChangeInfo(changeDescription, filePath));
            }

            return changes;
        }

        /// <summary>
        /// Deletes all accepted change files.
        /// </summary>
        /// <param name="acceptedChanges">Changes being made in the current version.</param>
        public void RemoveAcceptedChanges(Dictionary<string, List<ChangeInfo>> acceptedChanges)
        {
            var allChangeInfos = acceptedChanges.SelectMany(c => c.Value);

            foreach (var changeInfo in allChangeInfos)
            {
                File.Delete(changeInfo.ChangeFilePath);
            }
        }

        /// <summary>
        /// Loads the configuration from the changelog location.
        /// </summary>
        /// <param name="changelogLocation">Path to the directory where the configuration file is located.</param>
        /// <returns>Returns the <code>Configuration</code> object deserialized from the configuration file.</returns>
        /// <exception cref="ArgumentException">Thrown when argument is not valid.</exception>
        private async Task<Configuration?> LoadConfiguration(string changelogLocation)
        {
            if (string.IsNullOrWhiteSpace(changelogLocation))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(changelogLocation));

            var configurationFilePath = FileSystemHelper.GetFilePathCaseInsensitive(Path.Combine(changelogLocation, ConfigurationFileName));

            // First check to see if the configuration file exists.
            if (File.Exists(configurationFilePath))
            {
                return JsonSerializer.Deserialize<Configuration>(await File.ReadAllTextAsync(configurationFilePath));
            }

            return null;
        }
    }
}