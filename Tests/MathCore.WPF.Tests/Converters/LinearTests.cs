using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class LinearTests
{
    /// <summary>Проверка линейного преобразования с положительными коэффициентами</summary>
    [TestMethod]
    public void Convert_LinearTransform_ReturnsCorrectResult()
    {
        var converter = new Linear { K = 2.0, B = 10.0 };

        var result = ((IValueConverter)converter).Convert(5.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(20.0); // 2 * 5 + 10 = 20
    }

    /// <summary>Проверка с K=1, B=0 (без преобразования)</summary>
    [TestMethod]
    public void Convert_Identity_ReturnsOriginalValue()
    {
        var converter = new Linear { K = 1.0, B = 0.0 };

        var result = ((IValueConverter)converter).Convert(42.5, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(42.5);
    }

    /// <summary>Проверка обратного преобразования</summary>
    [TestMethod]
    public void ConvertBack_InvertsCorrectly()
    {
        var converter = new Linear { K = 2.0, B = 10.0 };

        var result = ((IValueConverter)converter).ConvertBack(20.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(5.0); // (20 - 10) / 2 = 5
    }

    /// <summary>Проверка двустороннего преобразования</summary>
    [TestMethod]
    public void Convert_ThenConvertBack_ReturnsOriginal()
    {
        var converter = new Linear { K = 3.5, B = 12.7 };
        var original = 100.0;

        var converted = (double)((IValueConverter)converter).Convert(original, typeof(double), null, CultureInfo.InvariantCulture)!;
        var back = ((IValueConverter)converter).ConvertBack(converted, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(back).IsEqual(original, 1e-10);
    }

    /// <summary>Проверка инвертированного режима</summary>
    [TestMethod]
    public void Convert_Inverted_SwapsTransform()
    {
        var converter = new Linear { K = 2.0, B = 10.0, Inverted = true };

        // В инвертированном режиме Convert выполняет обратное преобразование
        var result = ((IValueConverter)converter).Convert(20.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(5.0); // (20 - 10) / 2 = 5
    }

    /// <summary>Проверка ConvertBack в инвертированном режиме</summary>
    [TestMethod]
    public void ConvertBack_Inverted_SwapsTransform()
    {
        var converter = new Linear { K = 2.0, B = 10.0, Inverted = true };

        // В инвертированном режиме ConvertBack выполняет прямое преобразование
        var result = ((IValueConverter)converter).ConvertBack(5.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(20.0); // 2 * 5 + 10 = 20
    }

    /// <summary>Проверка с отрицательным коэффициентом K</summary>
    [TestMethod]
    public void Convert_NegativeK_WorksCorrectly()
    {
        var converter = new Linear { K = -2.0, B = 10.0 };

        var result = ((IValueConverter)converter).Convert(5.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(0.0); // -2 * 5 + 10 = 0
    }

    /// <summary>Проверка с отрицательным смещением B</summary>
    [TestMethod]
    public void Convert_NegativeB_WorksCorrectly()
    {
        var converter = new Linear { K = 2.0, B = -10.0 };

        var result = ((IValueConverter)converter).Convert(10.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(10.0); // 2 * 10 - 10 = 10
    }

    /// <summary>Проверка преобразования Цельсия в Фаренгейт</summary>
    [TestMethod]
    public void Convert_CelsiusToFahrenheit_WorksCorrectly()
    {
        // F = 1.8 * C + 32
        var converter = new Linear { K = 1.8, B = 32.0 };

        var celsius_0 = (double)((IValueConverter)converter).Convert(0.0, typeof(double), null, CultureInfo.InvariantCulture)!;
        var celsius_100 = (double)((IValueConverter)converter).Convert(100.0, typeof(double), null, CultureInfo.InvariantCulture)!;

        Assert.That.Value(celsius_0).IsEqual(32.0, 1e-10);
        Assert.That.Value(celsius_100).IsEqual(212.0, 1e-10);
    }

    /// <summary>Проверка обратного преобразования Фаренгейта в Цельсий</summary>
    [TestMethod]
    public void ConvertBack_FahrenheitToCelsius_WorksCorrectly()
    {
        // F = 1.8 * C + 32, следовательно C = (F - 32) / 1.8
        var converter = new Linear { K = 1.8, B = 32.0 };

        var fahrenheit_32 = (double)((IValueConverter)converter).ConvertBack(32.0, typeof(double), null, CultureInfo.InvariantCulture)!;
        var fahrenheit_212 = (double)((IValueConverter)converter).ConvertBack(212.0, typeof(double), null, CultureInfo.InvariantCulture)!;

        Assert.That.Value(fahrenheit_32).IsEqual(0.0, 1e-10);
        Assert.That.Value(fahrenheit_212).IsEqual(100.0, 1e-10);
    }

    /// <summary>Проверка с очень малым K</summary>
    [TestMethod]
    public void Convert_SmallK_WorksCorrectly()
    {
        var converter = new Linear { K = 0.001, B = 0.0 };

        var result = ((IValueConverter)converter).Convert(1000.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(1.0, 1e-10);
    }
}
