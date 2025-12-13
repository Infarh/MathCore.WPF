using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера остатка от деления</summary>
[TestClass]
public class ModTests
{
    /// <summary>Тест остатка от деления</summary>
    [TestMethod]
    public void Convert_WithModulus_ReturnsRemainder()
    {
        IValueConverter converter = new Mod(3);
        var result = (double)converter.Convert(10, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(1);
    }

    /// <summary>Тест остатка от деления с параметром</summary>
    [TestMethod]
    public void Convert_WithParameter_UsesParameter()
    {
        IValueConverter converter = new Mod(5);
        var result = (double)converter.Convert(double.NaN, typeof(double), 7, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(2);
    }

    /// <summary>Тест с NaN в качестве модуля</summary>
    [TestMethod]
    public void Convert_WithNaNModulus_ReturnsParameter()
    {
        IValueConverter converter = new Mod(double.NaN);
        var result = (double)converter.Convert(double.NaN, typeof(double), 5, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(5);
    }

    /// <summary>Тест преобразования NaN возвращает NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNaN()
    {
        IValueConverter converter = new Mod(3);
        var result = (double)converter.Convert(double.NaN, typeof(double), double.NaN, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(double.IsNaN(result), "Результат должен быть NaN");
    }
}
