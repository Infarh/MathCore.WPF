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
    /// <summary>Проверка, что значение является числом типа <see cref="double"/></summary>
    public class IsDouble : Base.FormattedValueValidation
    {
        /// <summary>Проверка значения на возможность его преобразования в тип <see cref="double"/></summary>
        /// <param name="value">Проверяемое значение</param>
        /// <param name="c">Сведения о текущей культуре</param>
        /// <returns>Результат проверки валидный, если проверяемое значение может быть представлено в виде <see cref="double"/></returns>
        [NotNull]
        public override ValidationResult Validate(object? value, CultureInfo c)
        {
            if (value is null) 
                return AllowNull 
                    ? ValidationResult.ValidResult 
                    : new ValidationResult(false, NullReferenceMessage ?? ErrorMessage ?? "Значение не указано");
            try
            {
                _ = Convert.ToDouble(value, c);
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
                return new ValidationResult(false, FormatErrorMessage ?? ErrorMessage ?? $"Ошибка формата данных {value} при преобразовании к вещественному типу: {e.Message}");
            }
        }
    }
}