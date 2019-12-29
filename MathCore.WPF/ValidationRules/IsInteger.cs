using System;
using System.Globalization;
using System.Windows.Controls;

namespace MathCore.WPF.ValidationRules
{
    public class IsInteger : ValidationRule
    {
        public bool AllowNull { get; set; }

        public string ErrorMessage { get; set; }

        /// <inheritdoc />
        public override ValidationResult Validate(object value, CultureInfo CultureInfo)
        {
            var valid = ValidationResult.ValidResult;
            if (value == null) return AllowNull ? valid : new ValidationResult(false, ErrorMessage ?? "Значение не указно");
            try
            {
                var unused = Convert.ToInt32(value);
                return valid;
            }
            catch (Exception e)
            {
                return new ValidationResult(false, ErrorMessage ?? $"Невозможно преобразовать {value} в целое число: {e.Message}");
            }
        }
    }  
}