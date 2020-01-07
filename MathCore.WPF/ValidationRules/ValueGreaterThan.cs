using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.ValidationRules
{
    public class ValueGreaterThan : ValidationRule
    {
        [ConstructorArgument(nameof(Value))]
        public double Value { get; set; }

        public bool IsEqual { get; set; }

        public string? ErrorMessage { get; set; }

        public ValueGreaterThan() { }

        public ValueGreaterThan(double value) => Value = value;

        /// <inheritdoc />
        [NotNull]
        public override ValidationResult Validate(object value, CultureInfo c)
        {
            if (value is null) return new ValidationResult(false, "Значение не указано");
            try
            {
                var v = Convert.ToDouble(value, c);
                return v > Value || IsEqual && v.Equals(Value)
                    ? ValidationResult.ValidResult
                    : new ValidationResult(false, ErrorMessage ?? $"Значение {value} меньше чем {Value}");
            }
            catch (OverflowException e)
            {
                return new ValidationResult(false, ErrorMessage ?? $"Ошибка переполнения при преобразовании {value} к вещественному типу: {e.Message}");
            }
            catch (InvalidCastException e)
            {
                return new ValidationResult(false, ErrorMessage ?? $"Ошибка приведения {value} к вещественному типу: {e.Message}");
            }
            catch (FormatException e)
            {
                return new ValidationResult(false, ErrorMessage ?? $"Ошибка формата данных {value} при преобразовании к вещественному типу: {e.Message}");
            }
        }
    }
}