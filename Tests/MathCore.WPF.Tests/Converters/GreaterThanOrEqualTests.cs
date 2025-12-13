using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class GreaterThanOrEqualTests
{
    /// <summary>Проверка что значение больше порога возвращает true</summary>
    [TestMethod]
    public void Convert_ValueGreaterThanThreshold_ReturnsTrue()
    {
        var converter = new GreaterThanOrEqual { Value = 10.0 };

        var result = ((IValueConverter)converter).Convert(15.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка что значение равное порогу возвращает true</summary>
    [TestMethod]
    public void Convert_ValueEqualToThreshold_ReturnsTrue()
    {
        var converter = new GreaterThanOrEqual { Value = 10.0 };

        var result = ((IValueConverter)converter).Convert(10.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка что значение меньше порога возвращает false</summary>
    [TestMethod]
    public void Convert_ValueLessThanThreshold_ReturnsFalse()
    {
        var converter = new GreaterThanOrEqual { Value = 10.0 };

        var result = ((IValueConverter)converter).Convert(5.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка что NaN возвращает null</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNull()
    {
        var converter = new GreaterThanOrEqual { Value = 10.0 };

        var result = ((IValueConverter)converter).Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.IsNull(result, "NaN должен возвращать null");
    }

    /// <summary>Проверка с положительной бесконечностью</summary>
    [TestMethod]
    public void Convert_PositiveInfinity_ReturnsTrue()
    {
        var converter = new GreaterThanOrEqual { Value = 100.0 };

        var result = ((IValueConverter)converter).Convert(double.PositiveInfinity, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка с отрицательной бесконечностью</summary>
    [TestMethod]
    public void Convert_NegativeInfinity_ReturnsFalse()
    {
        var converter = new GreaterThanOrEqual { Value = -100.0 };

        var result = ((IValueConverter)converter).Convert(double.NegativeInfinity, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка с нулевым порогом и нулевым значением</summary>
    [TestMethod]
    public void Convert_ZeroValue_ZeroThreshold_ReturnsTrue()
    {
        var converter = new GreaterThanOrEqual { Value = 0.0 };

        var result = ((IValueConverter)converter).Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка с очень малыми значениями</summary>
    [TestMethod]
    public void Convert_VerySmallDifference_WorksCorrectly()
    {
        var converter = new GreaterThanOrEqual { Value = 1.0 };

        var result_equal = ((IValueConverter)converter).Convert(1.0, typeof(bool), null, CultureInfo.InvariantCulture);
        var result_slightly_more = ((IValueConverter)converter).Convert(1.0 + double.Epsilon, typeof(bool), null, CultureInfo.InvariantCulture);
        var result_slightly_less = ((IValueConverter)converter).Convert(1.0 - double.Epsilon, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result_equal).IsEqual(true);
        Assert.That.Value(result_slightly_more).IsEqual(true);
        Assert.That.Value(result_slightly_less).IsEqual(false);
    }

    /// <summary>Проверка что ConvertBack выбрасывает NotSupportedException</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        var converter = new GreaterThanOrEqual();

        Assert.ThrowsException<NotSupportedException>(() =>
            ((IValueConverter)converter).ConvertBack(true, typeof(double), null, CultureInfo.InvariantCulture),
            "ConvertBack не должен поддерживаться для конвертера сравнения");
    }
}
