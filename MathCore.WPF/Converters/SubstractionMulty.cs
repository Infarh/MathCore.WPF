using System.Linq;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(SubstractionMulty))]
    public class SubstractionMulty : MultiDoubleValueValueConverter
    {
        /// <inheritdoc />
        protected override double Convert(double[] vv) => vv.Select((o, i) => new { v = o, i }).Sum(v => v.i > 0 ? -v.v : v.v);
    }
}