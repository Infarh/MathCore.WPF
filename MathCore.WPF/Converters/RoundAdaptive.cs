using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(RoundAdaptive))]
public class RoundAdaptive(int Digits, MidpointRounding Rounding) : DoubleValueConverter
{
    public RoundAdaptive() : this(0) { }

    public RoundAdaptive(int Digits) : this(Digits, default) { }


    [ConstructorArgument(nameof(Digits))]
    public int Digits { get; set; } = Digits;

    [ConstructorArgument(nameof(Rounding))]
    public MidpointRounding Rounding { get; set; } = Rounding;

    /// <inheritdoc />
    protected override double Convert(double v, double? p = null) => v.RoundAdaptive(Digits);

    /// <inheritdoc />
    protected override double ConvertBack(double v, double? p = null) => v;
}