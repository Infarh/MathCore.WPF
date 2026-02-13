using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.ValidationRules;

/// <summary>Проверка значения больше заданного</summary>
public class ValueGreaterThan : ValidationRule
{
    /// <summary>Эталонное значение</summary>
    [ConstructorArgument(nameof(Value))]
    public double Value { get; set; }

    /// <summary>Разрешить равенство эталонному значению</summary>
    public bool IsEqual { get; set; }

    /// <summary>Сообщение об ошибке проверки</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Инициализация нового экземпляра <see cref="ValueGreaterThan"/></summary>
    public ValueGreaterThan() { }

    /// <summary>Инициализация нового экземпляра <see cref="ValueGreaterThan"/></summary>
    /// <param name="value">Эталонное значение</param>
    public ValueGreaterThan(double value) => Value = value;

    /// <summary>Проверка значения на превышение эталонного</summary>
    /// <param name="value">Проверяемое значение</param>
    /// <param name="c">Сведения о текущей культуре</param>
    /// <returns>Результат проверки на превышение или равенство</returns>
    public override ValidationResult Validate(object? value, CultureInfo c)
    {
        if (value is null) return new(false, "Значение не указано");
        try
        {
            var v = Convert.ToDouble(value, c);
            return v > Value || IsEqual && v.Equals(Value)
                ? ValidationResult.ValidResult
                : new(false, ErrorMessage ?? $"Значение {value} меньше чем {Value}");
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