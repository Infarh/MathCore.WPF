using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Модульные тесты для конвертера Divide</summary>
[TestClass]
public class DivideTests
{
    /// <summary>Тест деления на положительное число</summary>
    [TestMethod]
    public void Convert_DivideByPositive_ReturnsQuotient()
    {
        IValueConverter converter = new Divide { K = 2.0 };
        var result = converter.Convert(10.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(5.0);
    }

    /// <summary>Тест деления на отрицательное число</summary>
    [TestMethod]
    public void Convert_DivideByNegative_ReturnsNegativeQuotient()
    {
        IValueConverter converter = new Divide { K = -2.0 };
        var result = converter.Convert(10.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(-5.0);
    }

    /// <summary>Тест деления на ноль</summary>
    [TestMethod]
    public void Convert_DivideByZero_ReturnsInfinity()
    {
        IValueConverter converter = new Divide { K = 0.0 };
        var result = (double)converter.Convert(10.0, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(double.IsInfinity(result));
    }

    /// <summary>Тест обратного преобразования</summary>
    [TestMethod]
    public void ConvertBack_MultipliesValue_ReturnsOriginal()
    {
        IValueConverter converter = new Divide { K = 2.0 };
        var result = converter.ConvertBack(5.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(10.0);
    }

    /// <summary>Тест двустороннего преобразования</summary>
    [TestMethod]
    public void ConvertAndConvertBack_ReturnsOriginal()
    {
        IValueConverter converter = new Divide { K = 3.14 };
        var original = 100.0;
        
        var converted = (double)converter.Convert(original, typeof(double), null, CultureInfo.CurrentCulture)!;
        var back = (double)converter.ConvertBack(converted, typeof(double), null, CultureInfo.CurrentCulture)!;
        
        Assert.That.Value(back).IsEqual(original, 1e-13);
    }

    /// <summary>Тест деления на 1</summary>
    [TestMethod]
    public void Convert_DivideByOne_ReturnsOriginal()
    {
        IValueConverter converter = new Divide { K = 1.0 };
        var result = converter.Convert(42.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(42.0);
    }

    /// <summary>Тест с NaN значением</summary>
    [TestMethod]
    public void Convert_NaNValue_ReturnsNaN()
    {
        IValueConverter converter = new Divide { K = 2.0 };
        var result = (double)converter.Convert(double.NaN, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(double.IsNaN(result));
    }

    /// <summary>Тест конструктора с параметром</summary>
    [TestMethod]
    public void Constructor_WithParameter_SetsValue()
    {
        var converter = new Divide(42.0);
        Assert.That.Value(converter.K).IsEqual(42.0);
    }

    /// <summary>Тест преобразования процентов</summary>
    [TestMethod]
    public void Convert_Percent_ConvertsProperly()
    {
        IValueConverter converter = new Divide { K = 100.0 };
        var result = converter.Convert(50.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(0.5);
    }
}
