using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

            // Path to the changelog file.
            var changelogPath = Path.Combine(changelogLocation, ChangelogFileName);
            var changelogText = (await File.ReadAllLinesAsync(changelogPath)).ToList();

            // Get the index of a line before H2 with older version.
            var index = changelogText.FindIndex(0, line => line.StartsWith(insertBefore)) - 1;

            // Remove that blank like before the older version because our TextToWrite already has blank lines before and after.
            changelogText.RemoveAt(index);
            changelogText.Insert(index, textToWrite);

            await File.WriteAllLinesAsync(changelogPath, changelogText);
        }
    }
}