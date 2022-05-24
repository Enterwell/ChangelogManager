namespace Enterwell.CI.Changelog.Models;

/// <summary>
/// Change information class representing the change description and the path to the change file.
/// </summary>
public class ChangeInfo
{
    /// <summary>
    /// User's change description.
    /// </summary>
    public string ChangeDescription { get; set; }

    /// <summary>
    /// Change's file path.
    /// </summary>
    public string ChangeFilePath { get; set; }

    /// <summary>
    /// Initializes a new instance of the <c>VersionInformation</c> class.
    /// </summary>
    /// <param name="changeDescription">User's change description.</param>
    /// <param name="changeFilePath">Change's file path.</param>
    public ChangeInfo(string changeDescription, string changeFilePath)
    {
        this.ChangeDescription = changeDescription;
        this.ChangeFilePath = changeFilePath;
    }
}