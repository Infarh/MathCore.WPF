using System.Linq;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(AdditionMulty))]
    public class AdditionMulty : MultiDoubleValueValueConverter
    {
        /// <inheritdoc />
        protected override double Convert(double[] vv) => vv.Sum();
    }
}