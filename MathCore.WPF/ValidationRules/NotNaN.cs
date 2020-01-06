using System;
using System.Globalization;
using System.Windows.Controls;

using MathCore.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global

// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.ValidationRules
{
    public class NotNaN : ValidationRule
    {
        public bool AllowNull { get; set; }

        public string? ErrorMessage { get; set; }

        /// <inheritdoc />
        [NotNull]
        public override ValidationResult Validate(object value, CultureInfo c)
        {
            var valid = ValidationResult.ValidResult;
            if (value is null) return AllowNull ? valid : new ValidationResult(false, ErrorMessage ?? "Значение не указано");
            try
            {
                return !double.IsNaN(Convert.ToDouble(value, c)) 
                    ? valid 
                    : new ValidationResult(false, ErrorMessage ?? "Значение является не-числом");
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