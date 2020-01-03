using System;
using System.Globalization;
using System.Windows.Controls;

namespace MathCore.WPF.ValidationRules
{
    public class IsNumeric : ValidationRule
    {
        public bool AllowNull { get; set; }

        public bool IntegerOnly { get; set; }

        public string ErrorMessage { get; set; }

        public override ValidationResult Validate(object value, CultureInfo CultureInfo)
        {
            if (value is null) return AllowNull ? ValidationResult.ValidResult : new ValidationResult(false, "Значение не указно");
            try
            {
                var unused = IntegerOnly ? Convert.ToInt32(value) : Convert.ToDouble(value);
                return ValidationResult.ValidResult;
            }
            catch (Exception e)
            {
                return new ValidationResult(false, ErrorMessage ?? $"Ошибка преобразования {value} к вещественному типу: {e.Message}");
            }
        }
    }
}