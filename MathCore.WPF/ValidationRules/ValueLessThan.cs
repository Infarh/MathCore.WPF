using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.ValidationRules
{
    public class ValueLessThan : ValidationRule
    {
        [ConstructorArgument(nameof(Value))]
        public double Value { get; set; }

        public bool IsEquals { get; set; }

        public string? ErrorMessage { get; set; }

        public ValueLessThan() { }

        public ValueLessThan(double value) => Value = value;

        /// <inheritdoc />
        [NotNull]
        public override ValidationResult Validate(object value, CultureInfo CultureInfo)
        {
            if (value is null) return new ValidationResult(false, "Значение не указано");
            try
            {
                var v = Convert.ToDouble(value);
                return v < Value || IsEquals && v.Equals(Value)
                    ? ValidationResult.ValidResult
                    : new ValidationResult(false, ErrorMessage ?? $"Значение {value} больше чем {Value}");
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Значение {value} не может быть преобразовано в вещественное число: {e.Message}");
            }
        }
    }
}