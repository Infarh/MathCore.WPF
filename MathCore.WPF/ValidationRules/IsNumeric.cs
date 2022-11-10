using System.Globalization;
using System.Windows.Controls;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global
// ReSharper disable AssignmentIsFullyDiscarded

namespace MathCore.WPF.ValidationRules;

/// <summary>Проверка, что значение является числом (<see cref="double"/> или <see cref="int"/>)</summary>
public class IsNumeric : Base.FormattedValueValidation
{
    /// <summary>Значение должно быть исключительно целочисленным (<see cref="int"/>)</summary>
    public bool IntegerOnly { get; set; }

    /// <summary>Проверка значения на возможность его преобразования в тип <see cref="double"/> или <see cref="int"/></summary>
    /// <param name="value">Проверяемое значение</param>
    /// <param name="c">Сведения о текущей культуре</param>
    /// <returns>Результат проверки валидный, если проверяемое значение может быть представлено в виде <see cref="double"/> или <see cref="int"/></returns>
    public override ValidationResult Validate(object? value, CultureInfo c)
    {
        if (value is null)
            return AllowNull
                ? ValidationResult.ValidResult
                : new ValidationResult(false, NullReferenceMessage ?? ErrorMessage ?? "Значение не указано"); 
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
            return new ValidationResult(false, FormatErrorMessage ?? ErrorMessage ?? $"Ошибка формата данных {value} при преобразовании к {(IntegerOnly ? "целочисленному" : "числовому")} типу: {e.Message}");
        }
    }
}