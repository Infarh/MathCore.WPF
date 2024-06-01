using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(SingleValue))]
public class SingleValue(int Index, IValueConverter Next) : MultiValueValueConverter
{
    public SingleValue() : this(0, null!) { }

    public SingleValue(int Index) : this(Index, null) { }

    public int Index { get; set; } = Index;

    public IValueConverter? Next { get; set; } = Next;

    /// <inheritdoc />
    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c)
    {
        var v = vv is null || Index >= vv.Length ? null : vv[Index];
        return Next is null ? v : Next.Convert(v, t, p, c);
    }
}