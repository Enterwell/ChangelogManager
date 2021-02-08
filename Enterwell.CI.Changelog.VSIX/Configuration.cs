namespace Enterwell.CI.Changelog.VSIX
{
    /// <summary>
    ///     Class that is used for Json to Deserialize configuration file to.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        ///     Categories from the configuration json file.
        /// </summary>
        public string[] Categories { get; set; } = new string[0];
    }
}