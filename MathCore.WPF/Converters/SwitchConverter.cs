using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters;

/// <summary>Конвертер, который принимает <see cref="SwitchConverterCase"/> и использует их для преобразования значения</summary>
[ContentProperty("Cases")]
public class SwitchConverter : IValueConverter
{
    #region Public Properties.

    /// <summary>Установка, или чтение списка элементов <see cref="SwitchConverterCase"/></summary>
    public List<SwitchConverterCase> Cases { get; set; } = new();

    #endregion

    /// <inheritdoc />
    public object? Convert(object? value, Type TargetType, object parameter, CultureInfo c)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        return Cases is { Count: > 0 } cases
            ? cases
               .Where(Case => Equals(value, Case) || string.Equals(value.ToString(), Case.When, StringComparison.OrdinalIgnoreCase))
               .Select(Case => Case.Then)
               .FirstOrDefault()
            : null;
    }

    public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo c) => throw new NotSupportedException();
}

/// <summary>Вариант значения</summary>
[ContentProperty("Then")]
public class SwitchConverterCase
{

    /// <summary>Значение аргумента данного варианта</summary>
    public string When { get; set; }

    /// <summary>Подставляемое значение <see cref="SwitchConverter"/></summary>
    public object Then { get; set; }

    /// <summary>Инициализация нового пустого <see cref="SwitchConverterCase"/></summary>
    public SwitchConverterCase() { }

    /// <summary>Инициализация нового <see cref="SwitchConverterCase"/></summary>
    /// <param name="when">Вариант аргумента</param>
    /// <param name="then">Значение <see cref="SwitchConverter"/></param>
    public SwitchConverterCase(string when, object then) => (When, Then) = (when, then);

    public override string ToString() => $"When={When}; Then={Then}";
}
