using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Комбинирует последовательность вложенных конвертеров в один</summary>
[MarkupExtensionReturnType(typeof(ExConverter))]
public class ExConverter : ValueConverter
{
    /// <summary>Внешний преобразователь: выполняет последовательное применение вложенных конвертеров</summary>
    public IValueConverter? From { get; set; }

    /// <summary>Внешний преобразователь: применяется после получения значения</summary>
    public IValueConverter? To { get; set; }

    /// <inheritdoc />
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) =>
        GetConverters().Aggregate(v, (value, converter) => To?.Convert(converter.Convert(value, t, p, c), t, p, c));

    /// <inheritdoc />
    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) =>
        GetConverters().Reverse().Aggregate(v, (value, converter) => converter.ConvertBack(To?.Convert(value, t, p, c), t, p, c));

    private IEnumerable<IValueConverter> GetConverters()
    {
        var converter = From;
        while(converter != null)
        {
            yield return converter;
            var ex_converter = converter as ExConverter;
            converter = ex_converter?.From;
        }
    }
}