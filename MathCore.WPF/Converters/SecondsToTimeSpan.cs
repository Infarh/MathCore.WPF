using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Конвертер преобразования секунд в TimeSpan и обратно</summary>
/// <remarks>
/// Преобразует числовое значение секунд в TimeSpan и обратно.
/// Поддерживает различные числовые типы (byte, short, int, long, float, double).
/// Поддерживает двустороннее связывание.
/// <example>
/// <code>
/// &lt;TextBlock Text="{Binding DurationInSeconds, Converter={converters:SecondsToTimeSpan}}" /&gt;
/// &lt;Slider Value="{Binding TimeSpan, Converter={converters:SecondsToTimeSpan}}" /&gt;
/// </code>
/// </example>
/// </remarks>
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
    /// <summary>Преобразование числовых секунд в TimeSpan или TimeSpan в секунды</summary>
    /// <param name="v">Число секунд или TimeSpan</param>
    /// <param name="t">Целевой тип</param>
    /// <param name="p">Параметр конвертера (не используется)</param>
    /// <param name="c">Информация о культуре</param>
    /// <returns>TimeSpan для числового входа, double для TimeSpan входа</returns>
    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) => v switch
    {
        float x => TimeSpan.FromSeconds(x),
        double x => TimeSpan.FromSeconds(x),
        long x => TimeSpan.FromSeconds(x),
        int x => TimeSpan.FromSeconds(x),
        short x => TimeSpan.FromSeconds(x),
        byte x => TimeSpan.FromSeconds(x),
        TimeSpan time => time.TotalSeconds,
        _ => throw new InvalidOperationException()
    };

    /// <summary>Обратное преобразование (идентично прямому)</summary>
    /// <param name="v">Число секунд или TimeSpan</param>
    /// <param name="t">Целевой тип</param>
    /// <param name="p">Параметр конвертера (не используется)</param>
    /// <param name="c">Информация о культуре</param>
    /// <returns>TimeSpan для числового входа, double для TimeSpan входа</returns>
    protected override object? ConvertBack(object? v, Type t, object? p, CultureInfo c) => v switch
    {
        float x => TimeSpan.FromSeconds(x),
        double x => TimeSpan.FromSeconds(x),
        long x => TimeSpan.FromSeconds(x),
        int x => TimeSpan.FromSeconds(x),
        short x => TimeSpan.FromSeconds(x),
        byte x => TimeSpan.FromSeconds(x),
        TimeSpan time => time.TotalSeconds,
        _ => throw new InvalidOperationException()
    };
}
