using System.Globalization;
using System.Windows.Data;

namespace MathCore.WPF.Converters;

public class JoinStringConverter : IMultiValueConverter
{
    public object Convert(object[]? values, Type TargetType, object? parameter, CultureInfo culture)
    {
        var separator = parameter as string ?? " ";
        return string.Join(separator, values);
    }

    public object[]? ConvertBack(object? value, Type[] TargetTypes, object? parameter, CultureInfo culture)
    {
        if (value is not string { } str) return null;

        var separator = parameter as string ?? " ";

        return str.Split(new[] { separator }, StringSplitOptions.None).Cast<object>().ToArray();
    }
}