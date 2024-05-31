using System.Globalization;
using System.Windows.Controls;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.ValidationRules;

/// <summary>Проверка, что значение является числом типа <see cref="double"/> и при этом не является <see cref="double.NaN"/></summary>
public class NotNaN : Base.FormattedValueValidation
{
    /// <summary>Проверка, что значение является вещественным числом (<see cref="double"/>) и при этом не является <see cref="double.NaN"/></summary>
    /// <param name="value">Проверяемое значение</param>
    /// <param name="c">Сведения о текущей культуре</param>
    /// <returns>Результат проверки валидный, если проверяемое значение может быть представлено в виде <see cref="double"/></returns>
    public override ValidationResult Validate(object? value, CultureInfo c)
    {
        if (value is null)
            return AllowNull
                ? ValidationResult.ValidResult 
                : new(false, NullReferenceMessage ?? ErrorMessage ?? "Значение не указано");
        try
        {
            return !double.IsNaN(Convert.ToDouble(value, c)) 
                ? ValidationResult.ValidResult 
                : new(false, ErrorMessage ?? "Значение является не-числом");
        }
        catch (OverflowException e)
        {
            return new(false, ErrorMessage ?? $"Ошибка переполнения при преобразовании {value} к вещественному типу: {e.Message}");
        }
        catch (InvalidCastException e)
        {
            return new(false, ErrorMessage ?? $"Ошибка приведения {value} к вещественному типу: {e.Message}");
        }
        catch (FormatException e)
        {
            return new(false, FormatErrorMessage ?? ErrorMessage ?? $"Ошибка формата данных {value} при преобразовании к вещественному типу: {e.Message}");
        }
    }
}