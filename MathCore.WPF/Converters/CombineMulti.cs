using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(CombineMulti))]
public class CombineMulti(IMultiValueConverter First, IValueConverter Then) : MultiValueValueConverter
{
    public CombineMulti() : this(null, null) { }

    public CombineMulti(IMultiValueConverter First) : this(First, null) { }

    [ConstructorArgument("First")]
    public IMultiValueConverter? First { get; set; } = First;

    [ConstructorArgument("Then")]
    public IValueConverter? Then { get; set; } = Then;

    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c)
    {
        var result = (First ?? throw new InvalidOperationException("Не задан первичный конвертер значений")).Convert(vv, t, p, c);
        return Then is { } then 
            ? then.Convert(result, t, p, c) 
            : result;
    }

    protected override object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c)
    {
        if (Then  is { } then)
            v = then.ConvertBack(v, v != null ? v.GetType() : typeof(object), p, c);
        return (First ?? throw new InvalidOperationException("Не задан первичный конвертер значений")).ConvertBack(v, tt, p, c);
    }
}