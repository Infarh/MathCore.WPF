using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера усечения дробной части</summary>
[TestClass]
public class TruncateTests
{
    /// <summary>Конвертация положительного числа усекает дробную часть</summary>
    [TestMethod]
    public void Convert_PositiveNumber_TruncatesFractionalPart()
    {
        IValueConverter converter = new Truncate();
        var result = converter.Convert(3.7, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(3.0, (double)result);
    }

    /// <summary>Конвертация отрицательного числа усекает дробную часть</summary>
    [TestMethod]
    public void Convert_NegativeNumber_TruncatesFractionalPart()
    {
        IValueConverter converter = new Truncate();
        var result = converter.Convert(-3.7, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(-3.0, (double)result);
    }
}
