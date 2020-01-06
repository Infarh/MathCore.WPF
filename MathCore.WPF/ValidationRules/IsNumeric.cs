using System;
using System.Globalization;
using System.Windows.Controls;

using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global

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
            try
            {
                _ = IntegerOnly ? Convert.ToInt32(value, c) : Convert.ToDouble(value, c);
                return ValidationResult.ValidResult;
            }
            catch (OverflowException e)
            {
                return new ValidationResult(false, ErrorMessage ?? $"Ошибка переполнения при преобразовании {value} к числовому типу: {e.Message}");
            }
            catch (InvalidCastException e)
            {
                return new ValidationResult(false, ErrorMessage ?? $"Ошибка приведения {value} к числовому типу: {e.Message}");
            }
            catch (FormatException e)
            {
                return new ValidationResult(false, ErrorMessage ?? $"Ошибка формата данных {value} при преобразовании к числовому типу: {e.Message}");
            }
        }
    }
}