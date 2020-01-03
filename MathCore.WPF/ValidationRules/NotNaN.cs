using System;
using System.Globalization;
using System.Windows.Controls;

namespace MathCore.WPF.ValidationRules
{
    public class NotNaN : ValidationRule
    {
        public bool AllowNull { get; set; }

        public string ErrorMessage { get; set; }

        /// <inheritdoc />
        public override ValidationResult Validate(object value, CultureInfo CultureInfo)
        {
            var valid = ValidationResult.ValidResult;
            if(value is null) return AllowNull ? valid : new ValidationResult(false, ErrorMessage ?? "Значение не указно");
            try
            {
                return !double.IsNaN(Convert.ToDouble(value)) ? valid : new ValidationResult(false, ErrorMessage ?? "Значение является не-числом");
            }
            catch(Exception e)
            {
                return new ValidationResult(false, ErrorMessage ?? $"Невозможно преобразовано {value} в вещественное число: {e.Message}");
            }
        }
    }
}