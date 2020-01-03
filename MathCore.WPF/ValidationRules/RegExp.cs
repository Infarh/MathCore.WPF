using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace MathCore.WPF.ValidationRules
{
    public class RegExp : ValidationRule
    {
        public bool AllowNull { get; set; }
        public bool AllowNotString { get; set; }

        public string Expression { get; set; }

        public string ErrorMessage { get; set; }

        public RegExp() { }
        public RegExp(string expression) { Expression = expression; }

        /// <inheritdoc />
        public override ValidationResult Validate(object value, CultureInfo CultureInfo)
        {
            var valid = ValidationResult.ValidResult;
            if(value is null) return AllowNull ? valid : new ValidationResult(false, ErrorMessage ?? "Значение не указно");
            if(!(value is string)) return AllowNotString ? valid : new ValidationResult(false, $"Значение {value} не является строкой");

            var str = (string)value;

            var match = str.FindRegEx(Expression);
            return match.Success ? valid : new ValidationResult(false, ErrorMessage ?? $"Выражение {Expression} не найдено в строке {str}");
        }
    }
}