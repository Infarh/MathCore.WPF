using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Модульные тесты для конвертера Addition</summary>
[TestClass]
public class AdditionTests
{
    /// <summary>Тест сложения с положительным числом</summary>
    [TestMethod]
    public void Convert_AddPositiveValue_ReturnsSum()
    {
        IValueConverter converter = new Addition { P = 10.0 };
        var result = converter.Convert(5.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(15.0);
    }

    /// <summary>Тест сложения с отрицательным числом</summary>
    [TestMethod]
    public void Convert_AddNegativeValue_ReturnsSum()
    {
        IValueConverter converter = new Addition { P = -10.0 };
        var result = converter.Convert(5.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(-5.0);
    }

    /// <summary>Тест сложения с нулём</summary>
    [TestMethod]
    public void Convert_AddZero_ReturnsOriginal()
    {
        IValueConverter converter = new Addition { P = 0.0 };
        var result = converter.Convert(42.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(42.0);
    }

    /// <summary>Тест обратного преобразования</summary>
    [TestMethod]
    public void ConvertBack_SubtractsValue_ReturnsOriginal()
    {
        IValueConverter converter = new Addition { P = 10.0 };
        var result = converter.ConvertBack(15.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(5.0);
    }

    /// <summary>Тест двустороннего преобразования</summary>
    [TestMethod]
    public void ConvertAndConvertBack_ReturnsOriginal()
    {
        IValueConverter converter = new Addition { P = 273.15 };
        var original = 25.0;
        
        var converted = (double)converter.Convert(original, typeof(double), null, CultureInfo.CurrentCulture)!;
        var back = (double)converter.ConvertBack(converted, typeof(double), null, CultureInfo.CurrentCulture)!;
        
        Assert.That.Value(back).IsEqual(original, 1e-14);
    }

    /// <summary>Тест с NaN значением</summary>
    [TestMethod]
    public void Convert_NaNValue_ReturnsNaN()
    {
        IValueConverter converter = new Addition { P = 10.0 };
        var result = (double)converter.Convert(double.NaN, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(double.IsNaN(result));
    }

    /// <summary>Тест с бесконечностью</summary>
    [TestMethod]
    public void Convert_Infinity_ReturnsInfinity()
    {
        IValueConverter converter = new Addition { P = 10.0 };
        var result = converter.Convert(double.PositiveInfinity, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(double.PositiveInfinity);
    }

    /// <summary>Тест конструктора с параметром</summary>
    [TestMethod]
    public void Constructor_WithParameter_SetsValue()
    {
        var converter = new Addition(42.0);
        Assert.That.Value(converter.P).IsEqual(42.0);
    }

    /// <summary>Тест с очень большими числами</summary>
    [TestMethod]
    public void Convert_LargeNumbers_WorksCorrectly()
    {
        IValueConverter converter = new Addition { P = 1e100 };
        var result = converter.Convert(1e100, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(2e100);
    }
}
