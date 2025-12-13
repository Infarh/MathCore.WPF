using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Модульные тесты для конвертера Multiply</summary>
[TestClass]
public class MultiplyTests
{
    /// <summary>Тест умножения на положительное число</summary>
    [TestMethod]
    public void Convert_MultiplyByPositive_ReturnsProduct()
    {
        IValueConverter converter = new Multiply { K = 2.0 };
        var result = converter.Convert(5.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(10.0);
    }

    /// <summary>Тест умножения на отрицательное число</summary>
    [TestMethod]
    public void Convert_MultiplyByNegative_ReturnsNegativeProduct()
    {
        IValueConverter converter = new Multiply { K = -2.0 };
        var result = converter.Convert(5.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(-10.0);
    }

    /// <summary>Тест умножения на ноль</summary>
    [TestMethod]
    public void Convert_MultiplyByZero_ReturnsZero()
    {
        IValueConverter converter = new Multiply { K = 0.0 };
        var result = converter.Convert(42.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(0.0);
    }

    /// <summary>Тест обратного преобразования</summary>
    [TestMethod]
    public void ConvertBack_DividesValue_ReturnsOriginal()
    {
        IValueConverter converter = new Multiply { K = 2.0 };
        var result = converter.ConvertBack(10.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(5.0);
    }

    /// <summary>Тест двустороннего преобразования</summary>
    [TestMethod]
    public void ConvertAndConvertBack_ReturnsOriginal()
    {
        IValueConverter converter = new Multiply { K = 3.14 };
        var original = 25.0;
        
        var converted = (double)converter.Convert(original, typeof(double), null, CultureInfo.CurrentCulture)!;
        var back = (double)converter.ConvertBack(converted, typeof(double), null, CultureInfo.CurrentCulture)!;
        
        Assert.That.Value(back).IsEqual(original, 1e-14);
    }

    /// <summary>Тест обратного преобразования при K=0</summary>
    [TestMethod]
    public void ConvertBack_WithZeroK_ReturnsInfinity()
    {
        IValueConverter converter = new Multiply { K = 0.0 };
        var result = (double)converter.ConvertBack(10.0, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(double.IsInfinity(result), "Деление на 0 должно возвращать Infinity");
    }

    /// <summary>Тест умножения на 1</summary>
    [TestMethod]
    public void Convert_MultiplyByOne_ReturnsOriginal()
    {
        IValueConverter converter = new Multiply { K = 1.0 };
        var result = converter.Convert(42.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(42.0);
    }

    /// <summary>Тест с NaN значением</summary>
    [TestMethod]
    public void Convert_NaNValue_ReturnsNaN()
    {
        IValueConverter converter = new Multiply { K = 2.0 };
        var result = (double)converter.Convert(double.NaN, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(double.IsNaN(result));
    }

    /// <summary>Тест конструктора с параметром</summary>
    [TestMethod]
    public void Constructor_WithParameter_SetsValue()
    {
        var converter = new Multiply(42.0);
        Assert.That.Value(converter.K).IsEqual(42.0);
    }

    /// <summary>Тест умножения бесконечности</summary>
    [TestMethod]
    public void Convert_InfinityMultiply_ReturnsInfinity()
    {
        IValueConverter converter = new Multiply { K = 2.0 };
        var result = converter.Convert(double.PositiveInfinity, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(double.PositiveInfinity);
    }
}
