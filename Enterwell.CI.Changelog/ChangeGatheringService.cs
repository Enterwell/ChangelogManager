using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Enterwell.CI.Changelog
{
    /// <summary>
    /// Service that gathers changes from the repository and returns them to the user.
    /// </summary>
    public class ChangeGatheringService
    {
        private const string ChangesFolderName = "changes";
        private const string ConfigurationFileName = ".changelog.json";

        private readonly string[] acceptableChanges = { "added", "changed", "deprecated", "removed", "fixed", "security" };

        /// <summary>
        /// Reads the changes from the changes folder in the repository on a given repository path and returns the dictionary with change type keys.
        /// </summary>
        /// <param name="repositoryPath">Path to the repository where the folder with changes is located.</param>
        /// <exception cref="DirectoryNotFoundException">Thrown when directory containing changes does not exist.</exception>
        /// <returns>Returns the dictionary whose keys are change types and values are all the changes of the corresponding change type.</returns>
        public async Task<Dictionary<string, List<string>>> GatherChanges(string repositoryPath)
        {
            var changes = new Dictionary<string, List<string>>();

            // Load user configuration.
            var configuration = await LoadConfiguration(repositoryPath);

            var changesDirectoryPath = Path.Combine(repositoryPath, ChangesFolderName);

            if (!Directory.Exists(changesDirectoryPath))
            {
                throw new DirectoryNotFoundException("Directory 'changes' not found.");
            }

            var filesPath = Directory.GetFiles(changesDirectoryPath);

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
                    changes.Add(changeKey, new List<string>());
                }

                changes[changeKey].Add(changeDescription);
            }

            return changes;
        }

        /// <summary>
        /// Deletes all the change files from the changes folder in the repository.
        /// </summary>
        /// <param name="repositoryPath">Path to the repository where the folder with changes is located.</param>
        public void EmptyChangesFolder(string repositoryPath)
        {
            var changesDirectoryPath = Path.Combine(repositoryPath, ChangesFolderName);

            var changesDirectory = new DirectoryInfo(changesDirectoryPath);

            foreach (FileInfo file in changesDirectory.GetFiles())
            {
                file.Delete();
            }

        }

        /// <summary>
        /// Loads the configuration from the repository root.
        /// </summary>
        /// <param name="repositoryPath">Path to the repository where the configuration file is located.</param>
        /// <returns>Returns the <code>Configuration</code> object deserialized from the configuration file in repository root.</returns>
        /// <exception cref="ArgumentException">Thrown when arguments are not valid.</exception>
        private async Task<Configuration?> LoadConfiguration(string repositoryPath)
        {
            if (string.IsNullOrWhiteSpace(repositoryPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(repositoryPath));


            var configurationFilePath = Path.Combine(repositoryPath, ConfigurationFileName);

            // First check to see if the configuration file exists.
            if (File.Exists(configurationFilePath))
            {
                return JsonConvert.DeserializeObject<Configuration>(await File.ReadAllTextAsync(configurationFilePath));
            }

            return null;
        }

        
    }
}