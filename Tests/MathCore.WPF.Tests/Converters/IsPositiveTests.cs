using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class IsPositiveTests
{
    /// <summary>Проверка что положительное число возвращает true</summary>
    [TestMethod]
    public void Convert_PositiveNumber_ReturnsTrue()
    {
        var converter = new IsPositive();

        var result = ((IValueConverter)converter).Convert(42.5, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка что отрицательное число возвращает false</summary>
    [TestMethod]
    public void Convert_NegativeNumber_ReturnsFalse()
    {
        var converter = new IsPositive();

        var result = ((IValueConverter)converter).Convert(-42.5, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка что ноль возвращает false</summary>
    [TestMethod]
    public void Convert_Zero_ReturnsFalse()
    {
        var converter = new IsPositive();

        var result = ((IValueConverter)converter).Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка что NaN возвращает null</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNull()
    {
        var converter = new IsPositive();

        var result = ((IValueConverter)converter).Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.IsNull(result, "NaN должен возвращать null");
    }

    /// <summary>Проверка что положительная бесконечность возвращает true</summary>
    [TestMethod]
    public void Convert_PositiveInfinity_ReturnsTrue()
    {
        var converter = new IsPositive();

        var result = ((IValueConverter)converter).Convert(double.PositiveInfinity, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка что отрицательная бесконечность возвращает false</summary>
    [TestMethod]
    public void Convert_NegativeInfinity_ReturnsFalse()
    {
        var converter = new IsPositive();

        var result = ((IValueConverter)converter).Convert(double.NegativeInfinity, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка что очень маленькое положительное число возвращает true</summary>
    [TestMethod]
    public void Convert_VerySmallPositive_ReturnsTrue()
    {
        var converter = new IsPositive();

        var result = ((IValueConverter)converter).Convert(double.Epsilon, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка что очень маленькое отрицательное число возвращает false</summary>
    [TestMethod]
    public void Convert_VerySmallNegative_ReturnsFalse()
    {
        var converter = new IsPositive();

        var result = ((IValueConverter)converter).Convert(-double.Epsilon, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка что большое положительное число возвращает true</summary>
    [TestMethod]
    public void Convert_LargePositive_ReturnsTrue()
    {
        var converter = new IsPositive();

        var result = ((IValueConverter)converter).Convert(double.MaxValue, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка что большое отрицательное число возвращает false</summary>
    [TestMethod]
    public void Convert_LargeNegative_ReturnsFalse()
    {
        var converter = new IsPositive();

        var result = ((IValueConverter)converter).Convert(double.MinValue, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка что ConvertBack выбрасывает NotSupportedException</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        var converter = new IsPositive();

        Assert.ThrowsException<NotSupportedException>(() =>
            ((IValueConverter)converter).ConvertBack(true, typeof(double), null, CultureInfo.InvariantCulture),
            "ConvertBack не должен поддерживаться для проверки на положительность");
    }
}
