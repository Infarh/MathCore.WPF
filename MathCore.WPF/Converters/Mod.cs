using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Mod))]
public class Mod(double M) : DoubleValueConverter
{
    public Mod() : this(double.NaN) { }

    public double M { get; set; } = M;

    /// <inheritdoc />
    protected override double Convert(double v, double? p = null) => 
        (p ?? v).IsNaN() 
            ? double.NaN 
            : M.IsNaN() 
                ? p ?? v 
                : (p ?? v) % M;
}