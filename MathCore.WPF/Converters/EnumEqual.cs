using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(object), typeof(bool))]
[MarkupExtensionReturnType(typeof(EnumEqual))]
public class EnumEqual : ValueConverter
{
    public bool Inverted { get; set; }

    public Enum? Value { get; set; }

    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c)
    {
        if (v is not Enum enter || Value is not { } value || enter.GetType() != value.GetType())
            return Binding.DoNothing;

        return Inverted ^ Equals(enter, value);
    }
}