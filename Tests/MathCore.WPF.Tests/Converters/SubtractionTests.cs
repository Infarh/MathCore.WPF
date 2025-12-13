using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Модульные тесты для конвертера Subtraction</summary>
[TestClass]
public class SubtractionTests
{
    /// <summary>Тест вычитания положительного числа</summary>
    [TestMethod]
    public void Convert_SubtractPositiveValue_ReturnsDifference()
    {
        IValueConverter converter = new Subtraction { P = 10.0 };
        var result = converter.Convert(15.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(5.0);
    }

    /// <summary>Тест вычитания отрицательного числа</summary>
    [TestMethod]
    public void Convert_SubtractNegativeValue_ReturnsSum()
    {
        IValueConverter converter = new Subtraction { P = -10.0 };
        var result = converter.Convert(5.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(15.0);
    }

    /// <summary>Тест обратного преобразования</summary>
    [TestMethod]
    public void ConvertBack_AddsValue_ReturnsOriginal()
    {
        IValueConverter converter = new Subtraction { P = 10.0 };
        var result = converter.ConvertBack(5.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(15.0);
    }

    /// <summary>Тест двустороннего преобразования</summary>
    [TestMethod]
    public void ConvertAndConvertBack_ReturnsOriginal()
    {
        IValueConverter converter = new Subtraction { P = 2000.0 };
        var original = 2024.0;
        
        var converted = (double)converter.Convert(original, typeof(double), null, CultureInfo.CurrentCulture)!;
        var back = (double)converter.ConvertBack(converted, typeof(double), null, CultureInfo.CurrentCulture)!;
        
        Assert.That.Value(back).IsEqual(original, 1e-14);
    }

    /// <summary>Тест вычитания нуля</summary>
    [TestMethod]
    public void Convert_SubtractZero_ReturnsOriginal()
    {
        IValueConverter converter = new Subtraction { P = 0.0 };
        var result = converter.Convert(42.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(42.0);
    }

    /// <summary>Тест с NaN значением</summary>
    [TestMethod]
    public void Convert_NaNValue_ReturnsNaN()
    {
        IValueConverter converter = new Subtraction { P = 10.0 };
        var result = (double)converter.Convert(double.NaN, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(double.IsNaN(result));
    }

    /// <summary>Тест конструктора с параметром</summary>
    [TestMethod]
    public void Constructor_WithParameter_SetsValue()
    {
        var converter = new Subtraction(42.0);
        Assert.That.Value(converter.P).IsEqual(42.0);
    }
}
