using System;
using System.Globalization;
using System.Windows.Controls;

using MathCore.Annotations;
// ReSharper disable AssignmentIsFullyDiscarded
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.ValidationRules
{
    public class IsDouble : ValidationRule
    {
        public bool AllowNull { get; set; }

        public string? ErrorMessage { get; set; }

        [NotNull]
        public override ValidationResult Validate(object value, CultureInfo c)
        {
            if (value is null) return AllowNull ? ValidationResult.ValidResult : new ValidationResult(false, "Значение не указано");
            Exception? error;
            try
            {
                _ = Convert.ToDouble(value, c);
                return ValidationResult.ValidResult;
            }
            catch (OverflowException e)
            {
                error = e;

            }
            catch (InvalidCastException e)
            {
                error = e;
            }
            catch (FormatException e)
            {
                error = e;
            }
            return new ValidationResult(false, ErrorMessage ?? $"Ошибка преобразования {value} к вещественному типу: {error.Message}");
        }
    }
}