using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedType.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Round))]
public class Round(int Digits, MidpointRounding Rounding) : DoubleValueConverter
{
    public double K { get; set; } = 1;

    public Round() : this(0) { }

    public Round(int Digits) : this(Digits, default) { }

    [ConstructorArgument(nameof(Digits))]
    public int Digits { get; set; } = Digits;

    [ConstructorArgument(nameof(Rounding))]
    public MidpointRounding Rounding { get; set; } = Rounding;

    /// <inheritdoc />
    protected override double Convert(double v, double? p = null) => Digits > 0 
        ? Math.Round(v * K, Digits) / K
        : Math.Round(v * K) / K;

    /// <inheritdoc />
    protected override double ConvertBack(double v, double? p = null) => v;
}