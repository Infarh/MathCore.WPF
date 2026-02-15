using System.Globalization;
using System.Windows.Controls;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.ValidationRules;

/// <summary>Проверка значения на принадлежность интервалу</summary>
public class ValueInInterval : ValidationRule
{
    /// <summary>Минимальная граница интервала</summary>
    public double Min { get; set; } = double.NegativeInfinity;

    /// <summary>Максимальная граница интервала</summary>
    public double Max { get; set; } = double.PositiveInfinity;

    /// <summary>Разрешить равенство минимальной границе</summary>
    public bool MinEquals { get; set; } = true;

    /// <summary>Разрешить равенство максимальной границе</summary>
    public bool MaxEquals { get; set; } = true;

    /// <summary>Сообщение об ошибке проверки</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Проверка значения на принадлежность интервалу</summary>
    /// <param name="value">Проверяемое значение</param>
    /// <param name="c">Сведения о текущей культуре</param>
    /// <returns>Результат проверки на принадлежность интервалу</returns>
    /// <inheritdoc />
    public override ValidationResult Validate(object? value, CultureInfo c)
    {
        if (value is null) return new(false, "Значение не указано");
        try
        {
            var v = Convert.ToDouble(value, c);
            if (v < Min || !MinEquals && v.Equals(Min)) return new(false, ErrorMessage ?? $"Значение {v} меньше чем {Min}");
            if (v > Max || !MaxEquals && v.Equals(Max)) return new(false, ErrorMessage ?? $"Значение {v} больше чем {Max}");
            return ValidationResult.ValidResult;
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
            return new(false, ErrorMessage ?? $"Ошибка формата данных {value} при преобразовании к вещественному типу: {e.Message}");
        }
    }
}