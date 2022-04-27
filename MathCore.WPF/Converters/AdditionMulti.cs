using System.Linq;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(AdditionMulti))]
public class AdditionMulti : MultiDoubleValueValueConverter
{
    /// <inheritdoc />
    protected override double Convert(double[]? vv) =>
        vv is { Length: > 0 } values 
            ? values.Sum() 
            : double.NaN;
}