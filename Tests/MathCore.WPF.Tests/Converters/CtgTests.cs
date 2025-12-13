using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера котангенса</summary>
[TestClass]
public class CtgTests
{
    /// <summary>Тест прямого преобразования</summary>
    [TestMethod]
    public void Convert_ReturnsCorrectCotangentValue()
    {
        IValueConverter converter = new Ctg { W = 1 };
        var result = (double)converter.Convert(Math.PI / 4, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(1, 1e-14);
    }

    /// <summary>Тест преобразования с параметром K</summary>
    [TestMethod]
    public void Convert_WithK_ScalesResult()
    {
        IValueConverter converter = new Ctg { K = 2, W = 1 };
        var result = (double)converter.Convert(Math.PI / 4, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(2, 1e-14);
    }

    /// <summary>Тест преобразования с параметром B</summary>
    [TestMethod]
    public void Convert_WithB_OffsetsResult()
    {
        IValueConverter converter = new Ctg { B = 1, W = 1 };
        var result = (double)converter.Convert(Math.PI / 4, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(2, 1e-14);
    }

    /// <summary>Тест преобразования NaN возвращает NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNaN()
    {
        IValueConverter converter = new Ctg();
        var result = (double)converter.Convert(double.NaN, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(double.IsNaN(result), "Результат должен быть NaN");
    }

    /// <summary>Тест обратного преобразования должен возвращать значение без изменений</summary>
    [TestMethod]
    public void ConvertBack_ReturnsValueUnchanged()
    {
        IValueConverter converter = new Ctg();
        var result = (double)converter.ConvertBack(5, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(5);
    }
}
