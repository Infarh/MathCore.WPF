using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(IsNull))]
public class IsNull(bool Inverted) : ValueConverter
{
    public IsNull() : this(false) { }

    [ConstructorArgument(nameof(Inverted))]
    public bool Inverted { get; set; } = Inverted;

    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) => Inverted ^ (v is null);
}