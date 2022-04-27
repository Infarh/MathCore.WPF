using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(SubtractionMulti))]
public class SubtractionMulti : MultiDoubleValueValueConverter
{
    /// <inheritdoc />
    protected override double Convert(double[]? vv)
    {
        if (vv is not { Length: > 0 and var length } values)
            return double.NaN;

        var result = values[0];

        for (var i = 0; i < length; i++)
            result -= values[i];

        return result;
    }
}

[MarkupExtensionReturnType(typeof(TemperatureF2C))]
public class TemperatureF2C : Linear
{
    public TemperatureF2C() : base(1.8, 32) { }
}

[MarkupExtensionReturnType(typeof(TemperatureC2F))]
public class TemperatureC2F : Linear
{
    public TemperatureC2F() : base(1 / 1.8, - 32 / 1.8) { }
}