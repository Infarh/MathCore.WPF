using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(IsNull))]
public class IsNull : ValueConverter
{
    [ConstructorArgument(nameof(Inverted))]
    public bool Inverted { get; set; }

    public IsNull() { }

    public IsNull(bool Inverted) => this.Inverted = Inverted;

    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) => Inverted ^ (v is null);
}