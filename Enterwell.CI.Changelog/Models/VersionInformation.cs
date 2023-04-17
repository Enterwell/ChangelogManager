using System.Collections.Generic;
using System.Linq;

namespace Enterwell.CI.Changelog.Models
{
    /// <summary>
    /// Class representing the information about the current version and its changes.
    /// </summary>
    public class VersionInformation
    {
        /// <summary>
        /// Major version number.
        /// </summary>
        private int Major { get; set; }

        /// <summary>
        /// Minor version number.
        /// </summary>
        private int Minor { get; set; }

        /// <summary>
        /// Patch version number.
        /// </summary>
        private int Patch { get; set; }

        /// <summary>
        /// String representing the semantic version.
        /// </summary>
        public string SemanticVersion => $"{Major}.{Minor}.{Patch}";

        /// <summary>
        /// Changes being made in the current version.
        /// Dictionary whose keys are the change types and their values are all the changes of the corresponding change type.
        /// </summary>
        public Dictionary<string, List<ChangeInfo>> Changes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <c>VersionInformation</c> class.
        /// </summary>
        /// <param name="major">Major version number.</param>
        /// <param name="minor">Minor version number.</param>
        /// <param name="patch">Patch version number.</param>
        /// <param name="changes">Changes being made in the current version.</param>
        /// <param name="bumpingRule">User's custom bumping application's version rule.</param>
        public VersionInformation(int major, int minor, int patch, Dictionary<string, List<ChangeInfo>> changes, BumpingRule? bumpingRule)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
            this.Changes = changes;

            this.DetermineBumpVersion(bumpingRule);
        }

        /// <summary>
        /// Determine which version part to bump according to the made changes.
        /// </summary>
        /// <param name="bumpingRule">User's custom bumping application's version rule.</param>
        private void DetermineBumpVersion(BumpingRule? bumpingRule)
        {
            var shouldBumpMajor = false;
            var shouldBumpMinor = false;
            var shouldBumpPatch = false;

            var changeInfos = this.Changes.SelectMany(kvp => kvp.Value);

            if (bumpingRule == null)
            {
                if (changeInfos.Any(c => c.ChangeDescription.ToLower().Contains("breaking change")))
                {
                    shouldBumpMajor = true;
                }
                else if (!this.Changes.Keys.Any() || this.Changes.ContainsKey("Added") || this.Changes.ContainsKey("Changed") || this.Changes.ContainsKey("Removed") || this.Changes.ContainsKey("Deprecated"))
                {
                    shouldBumpMinor = true;
                }
                else if (this.Changes.ContainsKey("Fixed") || this.Changes.ContainsKey("Security"))
                {
                    shouldBumpPatch = true;
                }
            }
            else
            {
                if (changeInfos.Any(c => c.ChangeDescription.ToLower().Contains(bumpingRule.BreakingKeyword.ToLower())) ||
                    bumpingRule.Major.Any(changeType => this.Changes.ContainsKey(changeType)) ||
                    bumpingRule.Major.Contains("NoChanges") && !this.Changes.Any())
                {
                    shouldBumpMajor = true;
                }
                else if (bumpingRule.Minor.Any(changeType => this.Changes.ContainsKey(changeType)) ||
                         bumpingRule.Minor.Contains("NoChanges") && !this.Changes.Any())
                {
                    shouldBumpMinor = true;
                }
                else if (bumpingRule.Patch.Any(changeType => this.Changes.ContainsKey(changeType)) ||
                         bumpingRule.Patch.Contains("NoChanges") && !this.Changes.Any())
                {
                    shouldBumpPatch = true;
                }
            }

            if (shouldBumpMajor)
            {
                this.Major += 1;
                this.Minor = 0;
                this.Patch = 0;
            }
            else if (shouldBumpMinor)
            {
                this.Minor += 1;
                this.Patch = 0;
            }
            else if (shouldBumpPatch)
            {
                this.Patch += 1;
            }
        }
    }
}