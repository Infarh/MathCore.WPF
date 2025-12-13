using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера децибелов</summary>
[TestClass]
public class dBTests
{
    /// <summary>Тест преобразования в децибелы по мощности</summary>
    [TestMethod]
    public void Convert_ByPower_ReturnsDecibels()
    {
        IValueConverter converter = new dB { ByPower = true };
        var result = (double)converter.Convert(100, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(20, 1e-10);
    }

    /// <summary>Тест преобразования в децибелы по амплитуде</summary>
    [TestMethod]
    public void Convert_ByAmplitude_ReturnsDecibels()
    {
        IValueConverter converter = new dB { ByPower = false };
        var result = (double)converter.Convert(10, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(20, 1e-10);
    }

    /// <summary>Тест обратного преобразования из децибелов по мощности</summary>
    [TestMethod]
    public void ConvertBack_ByPower_ReturnsLinearValue()
    {
        IValueConverter converter = new dB { ByPower = true };
        var result = (double)converter.ConvertBack(20, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(100, 1e-10);
    }

    /// <summary>Тест обратного преобразования из децибелов по амплитуде</summary>
    [TestMethod]
    public void ConvertBack_ByAmplitude_ReturnsLinearValue()
    {
        IValueConverter converter = new dB { ByPower = false };
        var result = (double)converter.ConvertBack(20, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(10, 1e-10);
    }

    /// <summary>Тест инвертированного преобразования</summary>
    [TestMethod]
    public void Convert_Inverted_ReturnsLinearValue()
    {
        IValueConverter converter = new dB { Invert = true, ByPower = true };
        var result = (double)converter.Convert(20, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(100, 1e-10);
    }

    /// <summary>Тест преобразования NaN возвращает NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNaN()
    {
        IValueConverter converter = new dB();
        var result = (double)converter.Convert(double.NaN, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(double.IsNaN(result), "Результат должен быть NaN");
    }
}
