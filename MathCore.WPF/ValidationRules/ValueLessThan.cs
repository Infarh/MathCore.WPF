using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

// ReSharper disable UnusedType.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.ValidationRules;

/// <summary>Проверка значения меньше заданного</summary>
public class ValueLessThan : ValidationRule
{
    /// <summary>Эталонное значение</summary>
    [ConstructorArgument(nameof(Value))]
    public double Value { get; set; }

    /// <summary>Разрешить равенство эталонному значению</summary>
    public bool IsEquals { get; set; }

    /// <summary>Сообщение об ошибке проверки</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Инициализация нового экземпляра <see cref="ValueLessThan"/></summary>
    public ValueLessThan() { }

    /// <summary>Инициализация нового экземпляра <see cref="ValueLessThan"/></summary>
    /// <param name="value">Эталонное значение</param>
    public ValueLessThan(double value) => Value = value;

    /// <summary>Проверка значения на меньшее или равное эталонному</summary>
    /// <param name="value">Проверяемое значение</param>
    /// <param name="CultureInfo">Сведения о текущей культуре</param>
    /// <returns>Результат проверки на меньшее или равное значение</returns>
    public override ValidationResult Validate(object? value, CultureInfo CultureInfo)
    {
        if (value is null) return new(false, "Значение не указано");
        try
        {
            var v = Convert.ToDouble(value);
            return v < Value || IsEquals && v.Equals(Value)
                ? ValidationResult.ValidResult
                : new(false, ErrorMessage ?? $"Значение {value} больше чем {Value}");
        }
        catch (Exception e)
        {
            return new(false, $"Значение {value} не может быть преобразовано в вещественное число: {e.Message}");
        }
    }
}