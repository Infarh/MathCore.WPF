using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

public class DefaultIfNaN(double DefaultValue) : ValueConverter
{
    [ConstructorArgument(nameof(DefaultValue))]
    public double DefaultValue { get; set; } = DefaultValue;

    public DefaultIfNaN() : this(default) { }

    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) => v switch
    {
        double d => double.IsNaN(d) ? DefaultValue : d,
        _ => v
    };
}