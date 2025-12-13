using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class IsNaNTests
{
    /// <summary>Проверка что NaN распознается корректно</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsTrue()
    {
        var converter = new IsNaN();

        var result = ((IValueConverter)converter).Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка что обычное число возвращает false</summary>
    [TestMethod]
    public void Convert_RegularNumber_ReturnsFalse()
    {
        var converter = new IsNaN();

        var result = ((IValueConverter)converter).Convert(42.5, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка что ноль возвращает false</summary>
    [TestMethod]
    public void Convert_Zero_ReturnsFalse()
    {
        var converter = new IsNaN();

        var result = ((IValueConverter)converter).Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка что положительная бесконечность возвращает false</summary>
    [TestMethod]
    public void Convert_PositiveInfinity_ReturnsFalse()
    {
        var converter = new IsNaN();

        var result = ((IValueConverter)converter).Convert(double.PositiveInfinity, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка что отрицательная бесконечность возвращает false</summary>
    [TestMethod]
    public void Convert_NegativeInfinity_ReturnsFalse()
    {
        var converter = new IsNaN();

        var result = ((IValueConverter)converter).Convert(double.NegativeInfinity, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка инвертированного режима для NaN</summary>
    [TestMethod]
    public void Convert_NaN_Inverted_ReturnsFalse()
    {
        var converter = new IsNaN { Inverted = true };

        var result = ((IValueConverter)converter).Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка инвертированного режима для обычного числа</summary>
    [TestMethod]
    public void Convert_RegularNumber_Inverted_ReturnsTrue()
    {
        var converter = new IsNaN { Inverted = true };

        var result = ((IValueConverter)converter).Convert(123.456, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка что отрицательное число возвращает false</summary>
    [TestMethod]
    public void Convert_NegativeNumber_ReturnsFalse()
    {
        var converter = new IsNaN();

        var result = ((IValueConverter)converter).Convert(-999.99, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка что очень маленькое число возвращает false</summary>
    [TestMethod]
    public void Convert_VerySmallNumber_ReturnsFalse()
    {
        var converter = new IsNaN();

        var result = ((IValueConverter)converter).Convert(double.Epsilon, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка что ConvertBack выбрасывает NotSupportedException</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        var converter = new IsNaN();

        Assert.ThrowsException<NotSupportedException>(() =>
            ((IValueConverter)converter).ConvertBack(true, typeof(double), null, CultureInfo.InvariantCulture),
            "ConvertBack не должен поддерживаться для проверки на NaN");
    }
}
