using System;
using System.Globalization;
using System.Windows.Controls;

namespace MathCore.WPF.ValidationRules
{
    public class ValueInInterval : ValidationRule
    {
        public double Min { get; set; } = double.NegativeInfinity;
        public double Max { get; set; } = double.PositiveInfinity;
        public bool MinEquals { get; set; } = true;
        public bool MaxEquals { get; set; } = true;

        public string ErrorMessage { get; set; }

        public override ValidationResult Validate(object value, CultureInfo CultureInfo)
        {
            if(value is null) return new ValidationResult(false, "Значение не указно");
            try
            {
                var v = Convert.ToDouble(value);
                if(v < Min || !MinEquals && v.Equals(Min)) return new ValidationResult(false, ErrorMessage ?? $"Значение {v} меньше чем {Min}");
                if(v > Max || !MaxEquals && v.Equals(Max)) return new ValidationResult(false, ErrorMessage ?? $"Значение {v} больше чем {Max}");
                return ValidationResult.ValidResult;
            }
            catch(Exception e)
            {
                return new ValidationResult(false, $"Ошибка преобразования {value} к вещественному типу: {e.Message}");
            }
        }
    }
}