using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters;

/// <summary>Объединяет массив значений в строку</summary>
public class JoinStringConverter : IMultiValueConverter
{
    /// <summary>Преобразует массив значений в строку, используя параметр как разделитель</summary>
    public object Convert(object[]? values, Type TargetType, object? parameter, CultureInfo culture)
    {
        var separator = parameter as string ?? " ";
        if (values is null) return Binding.DoNothing;

        var items = values.Select(v => v?.ToString() ?? string.Empty);
        return string.Join(separator, items);
    }

    /// <summary>Разбивает строку по разделителю и возвращает массив объектов</summary>
    public object[]? ConvertBack(object? value, Type[] TargetTypes, object? parameter, CultureInfo culture)
    {
        if (value is not string str) return null;

        var separator = parameter as string ?? " ";
        var parts = str.Split(new[] { separator }, StringSplitOptions.None);
        return parts.Cast<object>().ToArray();
    }
}