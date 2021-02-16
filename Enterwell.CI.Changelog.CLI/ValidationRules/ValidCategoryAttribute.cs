using System.ComponentModel.DataAnnotations;
using System.IO;
using Newtonsoft.Json;

namespace Enterwell.CI.Changelog.CLI.ValidationRules
{
    /// <summary>
    /// Specifies that the data must be valid according to the configuration file if one exists. Otherwise, everything is valid.
    /// </summary>
    public class ValidCategoryAttribute : ValidationAttribute
    {
        public ValidCategoryAttribute() : base("The change category is not valid based on the configuration file."){}

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var pathToConfig = Path.Combine(Directory.GetCurrentDirectory(), Configuration.ConfigurationName);

            Configuration config = null;

            if (File.Exists(pathToConfig))
            {
                config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(pathToConfig));
            }

            if (config == null)
            {
                return ValidationResult.Success;
            }
            else
            {
                var inputString = (string) value;

                if (config.IsValid(inputString?.Trim()))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
        }
    }
}