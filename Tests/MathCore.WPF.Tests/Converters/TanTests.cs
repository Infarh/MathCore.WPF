using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера тангенса</summary>
[TestClass]
public class TanTests
{
    /// <summary>Тест прямого преобразования</summary>
    [TestMethod]
    public void Convert_ReturnsCorrectTangentValue()
    {
        IValueConverter converter = new Tan();
        var result = (double)converter.Convert(0, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(0, 1e-14);
    }

    /// <summary>Тест преобразования с параметром K</summary>
    [TestMethod]
    public void Convert_WithK_ScalesResult()
    {
        IValueConverter converter = new Tan { K = 2, W = 1 };
        var result = (double)converter.Convert(0, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(0, 1e-14);
    }

    /// <summary>Тест преобразования с параметром B</summary>
    [TestMethod]
    public void Convert_WithB_OffsetsResult()
    {
        IValueConverter converter = new Tan { B = 1 };
        var result = (double)converter.Convert(0, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(1, 1e-14);
    }

    /// <summary>Тест преобразования NaN возвращает NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNaN()
    {
        IValueConverter converter = new Tan();
        var result = (double)converter.Convert(double.NaN, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(double.IsNaN(result), "Результат должен быть NaN");
    }

    /// <summary>Тест обратного преобразования должен возвращать значение без изменений</summary>
    [TestMethod]
    public void ConvertBack_ReturnsValueUnchanged()
    {
        IValueConverter converter = new Tan();
        var result = (double)converter.ConvertBack(5, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(5);
    }
}
