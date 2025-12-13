using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Модульные тесты для конвертера LessThan</summary>
[TestClass]
public class LessThanTests
{
    /// <summary>Тест сравнения когда значение меньше порога</summary>
    [TestMethod]
    public void Convert_ValueLessThanThreshold_ReturnsTrue()
    {
        IValueConverter converter = new LessThan { Value = 10.0 };
        var result = converter.Convert(5.0, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест сравнения когда значение больше порога</summary>
    [TestMethod]
    public void Convert_ValueGreaterThanThreshold_ReturnsFalse()
    {
        IValueConverter converter = new LessThan { Value = 10.0 };
        var result = converter.Convert(15.0, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест сравнения когда значение равно порогу</summary>
    [TestMethod]
    public void Convert_ValueEqualToThreshold_ReturnsFalse()
    {
        IValueConverter converter = new LessThan { Value = 10.0 };
        var result = converter.Convert(10.0, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест с NaN значением</summary>
    [TestMethod]
    public void Convert_NaNValue_ReturnsNull()
    {
        IValueConverter converter = new LessThan { Value = 10.0 };
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.IsNull(result);
    }

    /// <summary>Тест с отрицательной бесконечностью</summary>
    [TestMethod]
    public void Convert_NegativeInfinity_ReturnsTrue()
    {
        IValueConverter converter = new LessThan { Value = -100.0 };
        var result = converter.Convert(double.NegativeInfinity, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест с положительной бесконечностью</summary>
    [TestMethod]
    public void Convert_PositiveInfinity_ReturnsFalse()
    {
        IValueConverter converter = new LessThan { Value = 100.0 };
        var result = converter.Convert(double.PositiveInfinity, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест обратного преобразования</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        IValueConverter converter = new LessThan { Value = 10.0 };
        Assert.ThrowsException<NotSupportedException>(() =>
            converter.ConvertBack(true, typeof(double), null, CultureInfo.CurrentCulture));
    }

    /// <summary>Тест конструктора с параметром</summary>
    [TestMethod]
    public void Constructor_WithValue_SetsThreshold()
    {
        var converter = new LessThan(42.0);
        Assert.That.Value(converter.Value).IsEqual(42.0);
    }
}
