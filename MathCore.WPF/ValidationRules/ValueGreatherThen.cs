using System;
using System.Globalization;
using System.Windows.Controls;

namespace MathCore.WPF.ValidationRules
{
    public class ValueGreatherThen : ValidationRule
    {
        public double Value { get; set; }
#pragma warning disable CS0108 // Член скрывает унаследованный член: отсутствует новое ключевое слово
        public bool Equals { get; set; }
#pragma warning restore CS0108 // Член скрывает унаследованный член: отсутствует новое ключевое слово

        public string ErrorMessage { get; set; }

        public ValueGreatherThen() { }

        public ValueGreatherThen(double value) { Value = value; }

        public override ValidationResult Validate(object value, CultureInfo CultureInfo)
        {
            if(value == null) return new ValidationResult(false, "Значение не указно");
            try
            {
                var v = Convert.ToDouble(value);
                return v > Value || Equals && v.Equals(Value)
                    ? ValidationResult.ValidResult
                    : new ValidationResult(false, ErrorMessage ?? $"Значение {value} меньше чем {Value}");
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Ошибка преобразования {value} к вещественному типу: {e.Message}");
            }
        }
    }
}