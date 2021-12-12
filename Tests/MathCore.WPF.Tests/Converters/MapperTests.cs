using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters
{
    [TestClass]
    public class MapperTests
    {
        [TestMethod]
        public void Convert()
        {
            const double min_scale = 15;
            const double max_scale = 345;
            const double min_value = 10;
            const double max_value = 220;
            IValueConverter converter = new Mapper
            {
                MinScale = min_scale,
                MaxScale = max_scale,
                MinValue = min_value,
                MaxValue = max_value
            };

            var at_0 = converter.Convert(0, typeof(double), null, CultureInfo.CurrentCulture);
            var at_min_value = converter.Convert(min_value, typeof(double), null, CultureInfo.CurrentCulture);
            var at_max_value = converter.Convert(max_value, typeof(double), null, CultureInfo.CurrentCulture);
            var zero = (double)converter.Convert(0.45454545454545, typeof(double), null, CultureInfo.CurrentCulture)!;

            Assert.That.Value(at_0).IsEqual(-0.7142857142857135);
            Assert.That.Value(at_min_value).IsEqual(min_scale);
            Assert.That.Value(at_max_value).IsEqual(max_scale);
            Assert.That.Value(zero).IsEqual(0, 1e-14);
        }

        [TestMethod]
        public void ConvertBack()
        {
            const double min_scale = 15;
            const double max_scale = 345;
            const double min_value = 10;
            const double max_value = 220;
            IValueConverter converter = new Mapper
            {
                MinScale = min_scale,
                MaxScale = max_scale,
                MinValue = min_value,
                MaxValue = max_value
            };

            var at_scale_0 = (double)converter.ConvertBack(0, typeof(double), null, CultureInfo.CurrentCulture)!;
            var at_scale_min = converter.ConvertBack(min_scale, typeof(double), null, CultureInfo.CurrentCulture);
            var at_scale_max = converter.ConvertBack(max_scale, typeof(double), null, CultureInfo.CurrentCulture);
            var at_scale_360 = (double)converter.ConvertBack(360, typeof(double), null, CultureInfo.CurrentCulture)!;

            Assert.That.Value(at_scale_0).IsEqual(0.45454545454545, 5.1e-15);
            Assert.That.Value(at_scale_min).IsEqual(min_value);
            Assert.That.Value(at_scale_max).IsEqual(max_value);
            Assert.That.Value(at_scale_360).IsEqual(229.54545454545456);
        }
    }
}
