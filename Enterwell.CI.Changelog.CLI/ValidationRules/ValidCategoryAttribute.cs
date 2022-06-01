using Enterwell.CI.Changelog.Shared;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Enterwell.CI.Changelog.CLI.ValidationRules
{
    /// <summary>
    /// Specifies that the data must be valid according to the configuration file if one exists. Otherwise, everything is valid.
    /// </summary>
    public class ValidCategoryAttribute : ValidationAttribute
    {
        /// <summary>
        /// Custom validation attribute constructor. Passes the error message that will be displayed to the user if the validation fails to its base class.
        /// </summary>
        public ValidCategoryAttribute() : base("The change category is not valid based on the configuration file.") { }

        /// <summary>
        /// Checks if the target property contains the valid data.
        /// </summary>
        /// <param name="value">Property value.</param>
        /// <param name="validationContext">Validation context containing the information about the property.</param>
        /// <returns>Result of the validation; can be either success or fail.</returns>
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var config = Configuration.LoadConfiguration(Directory.GetCurrentDirectory());

            // Any input data is valid if the configuration file does not exist or if it is empty.
            if (config == null || config.IsEmpty())
            {
                return ValidationResult.Success;
            }

            var inputString = (string?)value;

            if (config.IsValid(inputString?.Trim()))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}