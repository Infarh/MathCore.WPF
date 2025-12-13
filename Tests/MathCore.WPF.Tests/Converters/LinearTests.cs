using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Модульные тесты для конвертера Linear</summary>
[TestClass]
public class LinearTests
{
    /// <summary>Тест линейного преобразования y = 2x + 5</summary>
    [TestMethod]
    public void Convert_LinearTransform_ReturnsCorrectValue()
    {
        IValueConverter converter = new Linear { K = 2.0, B = 5.0 };
        var result = converter.Convert(10.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(25.0, "2 * 10 + 5 = 25");
    }

    /// <summary>Тест обратного преобразования</summary>
    [TestMethod]
    public void ConvertBack_InverseTransform_ReturnsOriginal()
    {
        IValueConverter converter = new Linear { K = 2.0, B = 5.0 };
        var result = converter.ConvertBack(25.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(10.0, "(25 - 5) / 2 = 10");
    }

    /// <summary>Тест двустороннего преобразования</summary>
    [TestMethod]
    public void ConvertAndConvertBack_ReturnsOriginal()
    {
        IValueConverter converter = new Linear { K = 3.14, B = 2.71 };
        var original = 25.0;
        
        var converted = (double)converter.Convert(original, typeof(double), null, CultureInfo.CurrentCulture)!;
        var back = (double)converter.ConvertBack(converted, typeof(double), null, CultureInfo.CurrentCulture)!;
        
        Assert.That.Value(back).IsEqual(original, 1e-13);
    }

    /// <summary>Тест с K=1 и B=0 (тождественное преобразование)</summary>
    [TestMethod]
    public void Convert_IdentityTransform_ReturnsOriginal()
    {
        IValueConverter converter = new Linear { K = 1.0, B = 0.0 };
        var result = converter.Convert(42.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(42.0);
    }

    /// <summary>Тест преобразования Цельсия в Кельвины (K = °C + 273.15)</summary>
    [TestMethod]
    public void Convert_CelsiusToKelvin_WorksCorrectly()
    {
        IValueConverter converter = new Linear { K = 1.0, B = 273.15 };
        var result = converter.Convert(0.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(273.15);
    }

    /// <summary>Тест инвертированного режима</summary>
    [TestMethod]
    public void Convert_InvertedMode_SwapsTransformations()
    {
        var normalConverter = new Linear { K = 2.0, B = 5.0, Inverted = false };
        var invertedConverter = new Linear { K = 2.0, B = 5.0, Inverted = true };
        
        var normalResult = (double)normalConverter.Convert(10.0, typeof(double), null, CultureInfo.CurrentCulture)!;
        var invertedResult = (double)invertedConverter.Convert(10.0, typeof(double), null, CultureInfo.CurrentCulture)!;
        
        // Инвертированный режим должен применять обратное преобразование
        Assert.That.Value(invertedResult).IsEqual((10.0 - 5.0) / 2.0);
        Assert.That.Value(normalResult).IsEqual(25.0);
    }

    /// <summary>Тест с K=0</summary>
    [TestMethod]
    public void ConvertBack_WithZeroK_ReturnsInfinity()
    {
        IValueConverter converter = new Linear { K = 0.0, B = 5.0 };
        var result = (double)converter.ConvertBack(10.0, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(double.IsInfinity(result), "Деление на 0 должно возвращать Infinity");
    }

    /// <summary>Тест с NaN значением</summary>
    [TestMethod]
    public void Convert_NaNValue_ReturnsNaN()
    {
        IValueConverter converter = new Linear { K = 2.0, B = 5.0 };
        var result = (double)converter.Convert(double.NaN, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(double.IsNaN(result));
    }

    /// <summary>Тест конструктора с двумя параметрами</summary>
    [TestMethod]
    public void Constructor_WithTwoParameters_SetsValues()
    {
        var converter = new Linear(3.0, 7.0);
        Assert.That.Value(converter.K).IsEqual(3.0);
        Assert.That.Value(converter.B).IsEqual(7.0);
    }

    /// <summary>Тест конструктора с одним параметром</summary>
    [TestMethod]
    public void Constructor_WithOneParameter_SetsBToZero()
    {
        var converter = new Linear(3.0);
        Assert.That.Value(converter.K).IsEqual(3.0);
        Assert.That.Value(converter.B).IsEqual(0.0);
    }

    /// <summary>Тест с отрицательным K</summary>
    [TestMethod]
    public void Convert_NegativeK_InvertsSign()
    {
        IValueConverter converter = new Linear { K = -1.0, B = 0.0 };
        var result = converter.Convert(5.0, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(-5.0);
    }

    /// <summary>Тест линейного масштабирования</summary>
    [TestMethod]
    public void Convert_ScalingOnly_WorksCorrectly()
    {
        IValueConverter converter = new Linear { K = 100.0, B = 0.0 };
        var result = converter.Convert(0.5, typeof(double), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(50.0);
    }
}
