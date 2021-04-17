using System.Linq;
using System.Windows.Markup;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(AdditionMulti))]
    public class AdditionMulti : MultiDoubleValueValueConverter
    {
        /// <inheritdoc />
        protected override double Convert(double[]? vv) => vv?.Sum() ?? double.NaN;
    }
}