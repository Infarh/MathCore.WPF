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
        public override ValidationResult Validate(object value, CultureInfo с)
        {
            var valid = ValidationResult.ValidResult;
            if (value is null) return AllowNull ? valid : new ValidationResult(false, ErrorMessage ?? "Значение не указано");
            try
            {
                return !double.IsNaN(Convert.ToDouble(value, c)) ? valid : new ValidationResult(false, ErrorMessage ?? "Значение является не-числом");
            }
            catch (Exception e)
            {
                return new ValidationResult(false, ErrorMessage ?? $"Невозможно преобразовано {value} в вещественное число: {e.Message}");
            }
        }
    }
}