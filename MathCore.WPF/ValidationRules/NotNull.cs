using System.Globalization;
using System.Windows.Controls;

namespace MathCore.WPF.ValidationRules
{
    public class NotNull : ValidationRule
    {
        public string ErrorMessage { get; set; }

        /// <inheritdoc />
        public override ValidationResult Validate(object value, CultureInfo CultureInfo) => value == null ? new ValidationResult(false, ErrorMessage ?? "Значение не указно") : ValidationResult.ValidResult;
    }
}