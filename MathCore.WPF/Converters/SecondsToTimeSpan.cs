using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Преобразует значение в секунды в TimeSpan и обратно</summary>
[MarkupExtensionReturnType(typeof(SecondsToTimeSpan))]
[ValueConversion(typeof(double), typeof(TimeSpan))]
[ValueConversion(typeof(float), typeof(TimeSpan))]
[ValueConversion(typeof(long), typeof(TimeSpan))]
[ValueConversion(typeof(int), typeof(TimeSpan))]
[ValueConversion(typeof(short), typeof(TimeSpan))]
[ValueConversion(typeof(byte), typeof(TimeSpan))]
[ValueConversion(typeof(TimeSpan), typeof(double))]
public class SecondsToTimeSpan : ValueConverter
{
    /// <summary>Преобразует числовое значение в TimeSpan или TimeSpan в число секунд</summary>
    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) => v switch
    {
        null => Binding.DoNothing,
        float x => TimeSpan.FromSeconds(x),
        double x => TimeSpan.FromSeconds(x),
        long x => TimeSpan.FromSeconds(x),
        int x => TimeSpan.FromSeconds(x),
        short x => TimeSpan.FromSeconds(x),
        byte x => TimeSpan.FromSeconds(x),
        TimeSpan time => time.TotalSeconds,
        _ => Binding.DoNothing
    };

    /// <summary>Обратное преобразование, идентично Convert</summary>
    protected override object? ConvertBack(object? v, Type t, object? p, CultureInfo c) => v switch
    {
        null => Binding.DoNothing,
        float x => TimeSpan.FromSeconds(x),
        double x => TimeSpan.FromSeconds(x),
        long x => TimeSpan.FromSeconds(x),
        int x => TimeSpan.FromSeconds(x),
        short x => TimeSpan.FromSeconds(x),
        byte x => TimeSpan.FromSeconds(x),
        TimeSpan time => time.TotalSeconds,
        _ => Binding.DoNothing
    };
}
