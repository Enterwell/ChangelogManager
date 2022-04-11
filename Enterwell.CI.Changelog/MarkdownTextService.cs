using System;
using System.Linq;
using System.Text;
using Enterwell.CI.Changelog.Models;

namespace Enterwell.CI.Changelog
{
    /// <summary>
    /// Services that knows how to transform text into markdown.
    /// </summary>
    public class MarkdownTextService
    {
        /// <summary>
        /// Transforms text into markdown H2.
        /// </summary>
        /// <param name="input">Text to transform.</param>
        /// <returns>Text in markdown H2.</returns>
        public string ToH2(string input)
        {
            return $"## {input}";
        }

        /// <summary>
        /// Transforms text into markdown H3.
        /// </summary>
        /// <param name="input">Text to transform.</param>
        /// <returns>Text in markdown H3.</returns>
        public string ToH3(string input)
        {
            return $"### {input}";
        }

        /// <summary>
        /// Transforms text into markdown UL list item.
        /// </summary>
        /// <param name="input">Text to transform.</param>
        /// <returns>Text in markdown UL list item.</returns>
        public string ToUnorderedListItem(string input)
        {
            return $"- {input}";
        }

        /// <summary>
        /// Builds the text section in markdown specifying version with date containing all of the changes in the current version.
        /// </summary>
        /// <param name="versionInformation">Information about the application's current version.</param>
        /// <returns>Text section in markdown.</returns>
        public string BuildChangelogSection(VersionInformation versionInformation)
        {
            var builder = new StringBuilder();

            builder.AppendLine();
            builder.AppendLine(ToH2($"[{versionInformation.SemanticVersion}] - {DateTime.Now:yyyy-MM-dd}"));

            var orderedChanges = versionInformation.Changes.OrderBy(c => c.Key).ToList();

            if (orderedChanges.Count == 0)
            {
                builder.AppendLine(ToUnorderedListItem("No new changes"));
            }

            for (var changeTypeIndex = 0; changeTypeIndex < orderedChanges.Count; changeTypeIndex++)
            {
                builder.AppendLine(ToH3(orderedChanges[changeTypeIndex].Key));

                foreach (var changeDescription in orderedChanges[changeTypeIndex].Value)
                {
                    builder.AppendLine(ToUnorderedListItem(changeDescription));
                }

                // If we are not at the last change, we add a blank line separator. (empty line after last change is not necessary).
                if (changeTypeIndex != orderedChanges.Count - 1)
                {
                    builder.AppendLine();
                }
            }

            return builder.ToString();
        }
    }
}