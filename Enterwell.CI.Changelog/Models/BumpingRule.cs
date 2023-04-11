using System;

namespace Enterwell.CI.Changelog.Models
{
    /// <summary>
    /// Class representing the version bumping rule in the configuration file.
    /// </summary>
    public class BumpingRule
    {
        /// <summary>
        /// Change description keyword that will bump the major version.
        /// </summary>
        public string BreakingKeyword = "BREAKING CHANGE";

        /// <summary>
        /// Change types that bump the major version.
        /// </summary>
        public string[] Major { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Change types that bump the minor version.
        /// </summary>
        public string[] Minor { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Change types that bump the patch version.
        /// </summary>
        public string[] Patch { get; set; } = Array.Empty<string>();
    }
}