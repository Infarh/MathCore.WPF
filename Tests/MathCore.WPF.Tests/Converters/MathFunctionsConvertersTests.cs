using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертеров математических функций</summary>
[TestClass]
public class MathFunctionsConvertersTests
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    #region Sin Tests

    /// <summary>Тест конвертера Sin - синус нуля</summary>
    [TestMethod]
    public void Sin_Zero_ReturnsZero()
    {
        IValueConverter converter = new Sin();
        var result = (double)converter.Convert(0.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0, 1e-15);
    }

    /// <summary>Тест конвертера Sin - синус π/2 (ввод 0.25 т.к. W=2π)</summary>
    [TestMethod]
    public void Sin_PiOver2_ReturnsOne()
    {
        IValueConverter converter = new Sin();
        // W = 2π, sin(2π * 0.25) = sin(π/2) = 1
        var result = (double)converter.Convert(0.25, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(1.0, 1e-15);
    }

    /// <summary>Тест конвертера Sin - синус π (ввод 0.5 т.к. W=2π)</summary>
    [TestMethod]
    public void Sin_Pi_ReturnsZero()
    {
        IValueConverter converter = new Sin();
        // W = 2π, sin(2π * 0.5) = sin(π) = 0
        var result = (double)converter.Convert(0.5, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0, 1e-15);
    }

    /// <summary>Тест конвертера Sin - обратное преобразование просто возвращает значение</summary>
    [TestMethod]
    public void Sin_ConvertBack_Asin_ReturnsAngle()
    {
        IValueConverter converter = new Sin();
        // ConvertBack просто возвращает входное значение
        var result = (double)converter.ConvertBack(0.5, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.5, 1e-15);
    }

    #endregion

    #region Cos Tests

    /// <summary>Тест конвертера Cos - косинус нуля</summary>
    [TestMethod]
    public void Cos_Zero_ReturnsOne()
    {
        IValueConverter converter = new Cos();
        var result = (double)converter.Convert(0.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(1.0, 1e-15);
    }

    /// <summary>Тест конвертера Cos - косинус π/2 (ввод 0.25 т.к. W=2π)</summary>
    [TestMethod]
    public void Cos_PiOver2_ReturnsZero()
    {
        IValueConverter converter = new Cos();
        // W = 2π, cos(2π * 0.25) = cos(π/2) = 0
        var result = (double)converter.Convert(0.25, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0, 1e-15);
    }

    /// <summary>Тест конвертера Cos - косинус π (ввод 0.5 т.к. W=2π)</summary>
    [TestMethod]
    public void Cos_Pi_ReturnsMinusOne()
    {
        IValueConverter converter = new Cos();
        // W = 2π, cos(2π * 0.5) = cos(π) = -1
        var result = (double)converter.Convert(0.5, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(-1.0, 1e-14); // Немного больше допуск из-за погрешностей
    }

    /// <summary>Тест конвертера Cos - обратное преобразование просто возвращает значение</summary>
    [TestMethod]
    public void Cos_ConvertBack_Acos_ReturnsAngle()
    {
        IValueConverter converter = new Cos();
        // ConvertBack просто возвращает входное значение
        var result = (double)converter.ConvertBack(0.5, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.5, 1e-15);
    }

    #endregion

    #region Tan Tests

    /// <summary>Тест конвертера Tan - тангенс нуля</summary>
    [TestMethod]
    public void Tan_Zero_ReturnsZero()
    {
        IValueConverter converter = new Tan();
        var result = (double)converter.Convert(0.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0, 1e-15);
    }

    /// <summary>Тест конвертера Tan - тангенс π/4 (ввод 0.125 т.к. W=2π)</summary>
    [TestMethod]
    public void Tan_PiOver4_ReturnsOne()
    {
        IValueConverter converter = new Tan();
        // W = 2π, tan(2π * 0.125) = tan(π/4) = 1
        var result = (double)converter.Convert(0.125, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(1.0, 1e-15);
    }

    /// <summary>Тест конвертера Tan - обратное преобразование просто возвращает значение</summary>
    [TestMethod]
    public void Tan_ConvertBack_Atan_ReturnsAngle()
    {
        IValueConverter converter = new Tan();
        // ConvertBack просто возвращает входное значение
        var result = (double)converter.ConvertBack(1.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(1.0, 1e-15);
    }

    #endregion

    #region Ctg Tests

    /// <summary>Тест конвертера Ctg - котангенс π/4</summary>
    [TestMethod]
    public void Ctg_PiOver4_ReturnsOne()
    {
        IValueConverter converter = new Ctg();
        // W = 2π по умолчанию, значит ctg(2π * v)
        // Для π/4 нужно v = 1/8 (т.к. 2π * 1/8 = π/4)
        var result = (double)converter.Convert(0.125, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(1.0, 1e-14);
    }

    /// <summary>Тест конвертера Ctg - котангенс π/2</summary>
    [TestMethod]
    public void Ctg_PiOver2_ReturnsZero()
    {
        IValueConverter converter = new Ctg();
        // W = 2π по умолчанию, значит ctg(2π * v)
        // Для π/2 нужно v = 1/4 (т.к. 2π * 1/4 = π/2)
        var result = (double)converter.Convert(0.25, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0, 1e-15);
    }

    #endregion

    #region dB Tests

    /// <summary>Тест конвертера dB - преобразование напряжения в децибеллы</summary>
    [TestMethod]
    public void dB_VoltageMode_CalculatesCorrectly()
    {
        IValueConverter converter = new dB { ByPower = false };
        var result = (double)converter.Convert(10.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(20.0, 1e-10); // 20*log10(10) = 20
    }

    /// <summary>Тест конвертера dB - преобразование мощности в децибеллы</summary>
    [TestMethod]
    public void dB_PowerMode_CalculatesCorrectly()
    {
        IValueConverter converter = new dB { ByPower = true };
        var result = (double)converter.Convert(100.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(20.0, 1e-10); // 10*log10(100) = 20
    }

    /// <summary>Тест конвертера dB - обратное преобразование напряжения из децибелл</summary>
    [TestMethod]
    public void dB_InvertVoltageMode_CalculatesCorrectly()
    {
        IValueConverter converter = new dB { ByPower = false, Invert = true };
        var result = (double)converter.Convert(20.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(10.0, 1e-10); // 10^(20/20) = 10
    }

    /// <summary>Тест конвертера dB - обратное преобразование мощности из децибелл</summary>
    [TestMethod]
    public void dB_InvertPowerMode_CalculatesCorrectly()
    {
        IValueConverter converter = new dB { ByPower = true, Invert = true };
        var result = (double)converter.Convert(20.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(100.0, 1e-10); // 10^(20/10) = 100
    }

    /// <summary>Тест конвертера dB - нулевое значение возвращает отрицательную бесконечность</summary>
    [TestMethod]
    public void dB_ZeroValue_ReturnsNegativeInfinity()
    {
        IValueConverter converter = new dB { ByPower = false };
        var result = (double)converter.Convert(0.0, typeof(double), null, Culture)!;
        Assert.IsTrue(double.IsNegativeInfinity(result), "0 в дБ должен вернуть -∞");
    }

    /// <summary>Тест конвертера dB - единица в напряжении</summary>
    [TestMethod]
    public void dB_OneVoltage_ReturnsZero()
    {
        IValueConverter converter = new dB { ByPower = false };
        var result = (double)converter.Convert(1.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0, 1e-15);
    }

    /// <summary>Тест конвертера dB - единица в мощности</summary>
    [TestMethod]
    public void dB_OnePower_ReturnsZero()
    {
        IValueConverter converter = new dB { ByPower = true };
        var result = (double)converter.Convert(1.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0, 1e-15);
    }

    #endregion
}
