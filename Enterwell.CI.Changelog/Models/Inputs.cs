using System;

namespace Enterwell.CI.Changelog.Models
{
    /// <summary>
    /// Class representing the program inputs.
    /// </summary>
    public class Inputs
    {
        /// <summary>
        /// Initializes a new instance of of the Inputs class.
        /// </summary>
        /// <param name="changelogLocation">Changelog location path.</param>
        /// <param name="changesLocation">Changes location path.</param>
        public Inputs(string changelogLocation, string changesLocation)
        {
            this.ChangelogLocation = changelogLocation ?? throw new ArgumentNullException(nameof(changelogLocation));
            this.ChangesLocation = changesLocation ?? throw new ArgumentNullException(nameof(changesLocation));    
        }

        /// <summary>
        /// Changelog location path.
        /// </summary>
        public string ChangelogLocation { get; }

        /// <summary>
        /// Changes location path.
        /// </summary>
        public string ChangesLocation { get; }
    }
}