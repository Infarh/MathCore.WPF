using System.Linq;
using System.Windows.Markup;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(SubtractionMulti))]
    public class SubtractionMulti : MultiDoubleValueValueConverter
    {
        /// <inheritdoc />
        protected override double Convert(double[]? vv) => vv?.Select((o, i) => new { v = o, i }).Sum(v => v.i > 0 ? -v.v : v.v) ?? double.NaN;
    }
}