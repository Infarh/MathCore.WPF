using System.Linq;
using System.Windows.Markup;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(AdditionMulti))]
    public class AdditionMulti : MultiDoubleValueValueConverter
    {
        /// <inheritdoc />
        protected override double Convert([CanBeNull] double[]? vv) => vv?.Sum() ?? double.NaN;
    }
}