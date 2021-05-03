using System;
using System.Globalization;
using System.Windows.Controls;
using MathCore.Annotations;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.ValidationRules
{
    public class ValueInInterval : ValidationRule
    {
        public double Min { get; set; } = double.NegativeInfinity;

        public double Max { get; set; } = double.PositiveInfinity;

        public bool MinEquals { get; set; } = true;

        public bool MaxEquals { get; set; } = true;

        public string? ErrorMessage { get; set; }

        /// <inheritdoc />
        [NotNull]
        public override ValidationResult Validate(object? value, CultureInfo c)
        {
            if (value is null) return new ValidationResult(false, "Значение не указано");
            try
            {
                var v = Convert.ToDouble(value, c);
                if (v < Min || !MinEquals && v.Equals(Min)) return new ValidationResult(false, ErrorMessage ?? $"Значение {v} меньше чем {Min}");
                if (v > Max || !MaxEquals && v.Equals(Max)) return new ValidationResult(false, ErrorMessage ?? $"Значение {v} больше чем {Max}");
                return ValidationResult.ValidResult;
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