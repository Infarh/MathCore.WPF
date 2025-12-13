using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Модульные тесты для конвертера GreaterThan</summary>
[TestClass]
public class GreaterThanTests
{
    /// <summary>Тест сравнения когда значение больше порога</summary>
    [TestMethod]
    public void Convert_ValueGreaterThanThreshold_ReturnsTrue()
    {
        IValueConverter converter = new GreaterThan { Value = 10.0 };

        var result = converter.Convert(15.0, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест сравнения когда значение меньше порога</summary>
    [TestMethod]
    public void Convert_ValueLessThanThreshold_ReturnsFalse()
    {
        IValueConverter converter = new GreaterThan { Value = 10.0 };

        var result = converter.Convert(5.0, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест сравнения когда значение равно порогу</summary>
    [TestMethod]
    public void Convert_ValueEqualToThreshold_ReturnsFalse()
    {
        IValueConverter converter = new GreaterThan { Value = 10.0 };

        var result = converter.Convert(10.0, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false, "Равные значения должны возвращать false для 'больше чем'");
    }

    /// <summary>Тест с NaN значением</summary>
    [TestMethod]
    public void Convert_NaNValue_ReturnsNull()
    {
        IValueConverter converter = new GreaterThan { Value = 10.0 };

        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.IsNull(result, "NaN должен возвращать null");
    }

    /// <summary>Тест с нулевым порогом и положительным значением</summary>
    [TestMethod]
    public void Convert_PositiveValueWithZeroThreshold_ReturnsTrue()
    {
        IValueConverter converter = new GreaterThan { Value = 0.0 };

        var result = converter.Convert(1.0, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест с нулевым порогом и отрицательным значением</summary>
    [TestMethod]
    public void Convert_NegativeValueWithZeroThreshold_ReturnsFalse()
    {
        IValueConverter converter = new GreaterThan { Value = 0.0 };

        var result = converter.Convert(-1.0, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест с положительной бесконечностью</summary>
    [TestMethod]
    public void Convert_PositiveInfinity_ReturnsTrue()
    {
        IValueConverter converter = new GreaterThan { Value = 100.0 };

        var result = converter.Convert(double.PositiveInfinity, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест с отрицательной бесконечностью</summary>
    [TestMethod]
    public void Convert_NegativeInfinity_ReturnsFalse()
    {
        IValueConverter converter = new GreaterThan { Value = -100.0 };

        var result = converter.Convert(double.NegativeInfinity, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест с очень маленькой разницей</summary>
    [TestMethod]
    public void Convert_TinyDifference_ReturnsTrue()
    {
        IValueConverter converter = new GreaterThan { Value = 1.0 };

        var result = converter.Convert(1.0 + double.Epsilon, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест конструктора с параметром</summary>
    [TestMethod]
    public void Constructor_WithValue_SetsThreshold()
    {
        var converter = new GreaterThan(42.0);

        Assert.That.Value(converter.Value).IsEqual(42.0);
    }

    /// <summary>Тест обратного преобразования - должно генерировать NotSupportedException</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        IValueConverter converter = new GreaterThan { Value = 10.0 };

        Assert.ThrowsException<NotSupportedException>(() =>
            converter.ConvertBack(true, typeof(double), null, CultureInfo.CurrentCulture),
            "ConvertBack не должен поддерживаться для GreaterThan конвертера");
    }

    /// <summary>Тест конвертера как MarkupExtension</summary>
    [TestMethod]
    public void ProvideValue_ReturnsSelf()
    {
        var converter = new GreaterThan();

        var result = converter.ProvideValue(null!);

        Assert.That.Value(result).IsReferenceSame(converter);
    }

    /// <summary>Тест с отрицательными значениями</summary>
    [TestMethod]
    public void Convert_NegativeValues_WorksCorrectly()
    {
        IValueConverter converter = new GreaterThan { Value = -10.0 };

        var result1 = converter.Convert(-5.0, typeof(bool), null, CultureInfo.CurrentCulture);
        var result2 = converter.Convert(-15.0, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result1).IsEqual(true, "-5 > -10 должно быть true");
        Assert.That.Value(result2).IsEqual(false, "-15 > -10 должно быть false");
    }
}
