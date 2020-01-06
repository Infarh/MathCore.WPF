using System;
using System.Globalization;
using System.Windows.Controls;

using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable AssignmentIsFullyDiscarded

namespace MathCore.WPF.ValidationRules
{
    public class IsNumeric : ValidationRule
    {
        public bool AllowNull { get; set; }

        public bool IntegerOnly { get; set; }

        public string? ErrorMessage { get; set; }

        [NotNull]
        public override ValidationResult Validate(object value, CultureInfo c)
        {
            if (value is null) return AllowNull ? ValidationResult.ValidResult : new ValidationResult(false, "Значение не указано");
            Exception? error;
            try
            {
                _ = IntegerOnly ? Convert.ToInt32(value, c) : Convert.ToDouble(value, c);
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