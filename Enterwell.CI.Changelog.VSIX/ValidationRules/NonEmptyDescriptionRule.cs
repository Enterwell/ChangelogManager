using System.Globalization;
using System.Windows.Controls;

namespace Enterwell.CI.Changelog.VSIX.ValidationRules
{
    /// <summary>
    /// Class that represents our Non Empty Description Validation Rule.
    /// Change Description text should not be empty.
    /// </summary>
    public class NonEmptyDescriptionRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace((string) value))
            {
                return new ValidationResult(false, "Change description should not be empty!");
            }

            return ValidationResult.ValidResult;
        }
    }
}