using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace MathCore.WPF.ValidationRules;

/// <summary>Проверка длины строки</summary>
public class StringLength : ValidationRule
{
    /// <summary>Разрешить пустое значение</summary>
    public bool AllowNull { get; set; }

    /// <summary>Разрешить значения, не являющиеся строкой</summary>
    public bool AllowNotString { get; set; }

    /// <summary>Эталонная длина строки</summary>
    [ConstructorArgument(nameof(Length))]
    public int Length { get; set; }

    /// <summary>Разрешить равенство длины эталонной</summary>
    public bool Equal { get; set; } = true;

    /// <summary>Разрешить длину меньше эталонной</summary>
    public bool Less { get; set; }

    /// <summary>Разрешить длину больше эталонной</summary>
    public bool Gatherer { get; set; }

    /// <summary>Инициализация нового экземпляра <see cref="StringLength"/></summary>
    public StringLength() { }

    /// <summary>Инициализация нового экземпляра <see cref="StringLength"/></summary>
    /// <param name="Length">Эталонная длина строки</param>
    public StringLength(int Length) => this.Length = Length;

    /// <summary>Проверка длины строки</summary>
    /// <param name="value">Проверяемое значение</param>
    /// <param name="c">Сведения о текущей культуре</param>
    /// <returns>Результат проверки на соответствие длины</returns>
    public override ValidationResult Validate(object? value, CultureInfo c)
    {
        var valid = ValidationResult.ValidResult;
        if (value is null) return AllowNull ? valid : new(false, "Значение не указано");
        if (value is not string str) return AllowNotString ? valid : new(false, $"Значение {value} не является строкой");

        var len = Length;
        return (str.Length - len) switch
        {
            0 when Equal      => valid,
            < 0 when Less     => valid,
            > 0 when Gatherer => valid,

            0   => new(false, $"Длина строки {str} не равна {len}"),
            < 0 => new(false, $"Длина строки {str} меньше чем {len}"),
            > 0 => new(false, $"Длина строки {str} больше чем {len}"),
        };

        //if (str_length == len) return Equal ? valid : new ValidationResult(false, $"Длина строки {value} не равна {len}");
        //if (str_length < len) return Less ? valid : new ValidationResult(false, $"Длина строки {value} меньше чем {len}");
        //if (str_length > len) return Gatherer ? valid : new ValidationResult(false, $"Длина строки {value} больше чем {len}");
        //return new ValidationResult(false, $"Длина строки {value} равна {len}");
    }
}