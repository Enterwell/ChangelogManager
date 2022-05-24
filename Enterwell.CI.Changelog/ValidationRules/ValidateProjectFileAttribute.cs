using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Enterwell.CI.Changelog.ValidationRules;

/// <summary>
/// Validates the project file at the given path.
/// </summary>
public class ValidateProjectFileAttribute : ValidationAttribute
{
    /// <summary>
    /// Custom validation attribute constructor. Sets the base error message that will be displayed to the user if the validation fails.
    /// </summary>
    public ValidateProjectFileAttribute() : base("The project file is not valid. Reason: {0}") { }

    /// <summary>
    /// Checks if the target property contains the valid data.
    /// </summary>
    /// <param name="value">Property value.</param>
    /// <param name="validationContext">Validation context containing the information about the property.</param>
    /// <returns>Result of the validation; can be either success or fail.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var projectFilePath = (string?)value;

        // If the user did not enter a project file path, that means he wants an automatic project file detection. Don't fail the validation
        if (projectFilePath == null)
        {
            return ValidationResult.Success;
        }

        // Check if the file exists
        if (!File.Exists(projectFilePath))
        {
            return new ValidationResult(this.FormatErrorMessage("File does not exist."));
        }

        // Check if the file type is supported
        if (!projectFilePath.EndsWith("package.json") && !projectFilePath.EndsWith(".csproj"))
        {
            return new ValidationResult(this.FormatErrorMessage("Only 'package.json' and '.csproj' files are supported."));
        }

        // Check if the '.csproj' file contains the 'Version' tag
        if (projectFilePath.EndsWith(".csproj"))
        {
            var xmlVersionTag = XElement.Load(projectFilePath)
                .Descendants()
                .FirstOrDefault(e =>
                    e.Name.ToString().ToLowerInvariant() == "version" &&
                    e.Parent?.Name.ToString().ToLowerInvariant() == "propertygroup");

            // Check if not found
            if (xmlVersionTag == null)
            {
                return new ValidationResult(this.FormatErrorMessage("Given '.csproj' file does not contain a 'Version' tag."));
            }
        }

        return ValidationResult.Success;
    }
}