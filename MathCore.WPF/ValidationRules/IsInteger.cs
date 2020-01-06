using System;
using System.Globalization;
using System.Windows.Controls;

using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.ValidationRules
{
    public class IsInteger : ValidationRule
    {
        public bool AllowNull { get; set; }

        public string? ErrorMessage { get; set; }

        /// <inheritdoc />
        [NotNull]
        public override ValidationResult Validate(object value, CultureInfo c)
        {
            if (value is null) return AllowNull ? ValidationResult.ValidResult : new ValidationResult(false, "Значение не указано");
            try
            {
                _ = Convert.ToInt32(value, c);
                return ValidationResult.ValidResult;
            }
            catch (OverflowException e)
            {
                return new ValidationResult(false, ErrorMessage ?? $"Ошибка переполнения при преобразовании {value} к целочисленному (4 байта) типу: {e.Message}");
            }
            catch (InvalidCastException e)
            {
                return new ValidationResult(false, ErrorMessage ?? $"Ошибка приведения {value} к целочисленному (4 байта) типу: {e.Message}");
            }
            catch (FormatException e)
            {
                return new ValidationResult(false, ErrorMessage ?? $"Ошибка формата данных {value} при преобразовании к целочисленному (4 байта) типу: {e.Message}");
            }
        }
    }
}