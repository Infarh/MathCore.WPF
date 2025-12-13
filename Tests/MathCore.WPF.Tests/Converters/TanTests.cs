using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера тангенса</summary>
[TestClass]
public class TanTests
{
    /// <summary>Конвертация нуля возвращает ноль</summary>
    [TestMethod]
    public void Convert_Zero_ReturnsZero()
    {
        IValueConverter converter = new Tan();
        var result = converter.Convert(0.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(0.0, (double)result, 0.0001);
    }

    /// <summary>Конвертация NaN возвращает NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNaN()
    {
        IValueConverter converter = new Tan();
        var result = converter.Convert(double.NaN, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.IsTrue(double.IsNaN((double)result));
    }
}
