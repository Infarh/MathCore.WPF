using System;
using System.Globalization;
using System.Windows.Controls;

namespace MathCore.WPF.ValidationRules
{
    public class ValueLessThen : ValidationRule
    {
        public double Value { get; set; }
        public bool IsEquals { get; set; }

        public string ErrorMessage { get; set; }

        public ValueLessThen() { }

        public ValueLessThen(double value) => Value = value;

        public override ValidationResult Validate(object value, CultureInfo CultureInfo)
        {
            if(value == null) return new ValidationResult(false, "Значение не указно");
            try
            {
                var v = Convert.ToDouble(value);
                return v < Value || IsEquals && v.Equals(Value)
                    ? ValidationResult.ValidResult
                    : new ValidationResult(false, ErrorMessage ?? $"Значение {value} больше чем {Value}");
            }
            catch(Exception e)
            {
                return new ValidationResult(false, $"Значение {value} не может быть преобразовано в вещественное число: {e.Message}");
            }
        }
    }
}