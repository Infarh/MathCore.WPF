using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Комбинирует многозначный конвертер с одиночным конвертером</summary>
[MarkupExtensionReturnType(typeof(CombineMulti))]
public class CombineMulti(IMultiValueConverter First, IValueConverter Then) : MultiValueValueConverter
{
    /// <summary>Комбинирует многозначный конвертер с одиночным конвертером</summary>
    public CombineMulti() : this(null, null) { }

    /// <summary>Комбинирует многозначный конвертер с одиночным конвертером</summary>
    public CombineMulti(IMultiValueConverter First) : this(First, null) { }

    /// <summary>Первичный многозначный конвертер</summary>
    [ConstructorArgument(nameof(First))]
    public IMultiValueConverter? First { get; set; } = First;

    /// <summary>Последующий одиночный конвертер</summary>
    [ConstructorArgument(nameof(Then))]
    public IValueConverter? Then { get; set; } = Then;

    /// <inheritdoc />
    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c)
    {
        var primary = First ?? throw new InvalidOperationException("Не задан первичный конвертер значений");
        var result = primary.Convert(vv, t, p, c);
        return Then is { } then
            ? then.Convert(result, t, p, c)
            : result;
    }

    /// <inheritdoc />
    protected override object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c)
    {
        var result = v;

        if (Then is { } then)
        {
            // Для одиночного конвертера передаём тип целевого значения как Type (в случае null используем typeof(object))
            var targetType = result != null ? result.GetType() : typeof(object);
            result = then.ConvertBack(result, targetType, p, c);
        }

        return (First ?? throw new InvalidOperationException("Не задан первичный конвертер значений")).ConvertBack(result, tt, p, c);
    }
}