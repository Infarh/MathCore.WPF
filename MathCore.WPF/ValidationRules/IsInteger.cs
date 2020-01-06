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
            Exception? error;
            try
            {
                _ = Convert.ToInt32(value, c);
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
            return new ValidationResult(false, ErrorMessage ?? $"Ошибка преобразования {value} к 4х-байтному целому типу: {error.Message}");
        }
    }
}