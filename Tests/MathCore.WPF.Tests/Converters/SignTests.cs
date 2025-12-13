using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера знака числа</summary>
[TestClass]
public class SignTests
{
    /// <summary>Конвертация положительного числа возвращает единицу</summary>
    [TestMethod]
    public void Convert_PositiveNumber_ReturnsOne()
    {
        IValueConverter converter = new Sign();
        var result = converter.Convert(5.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(1.0, (double)result);
    }

    /// <summary>Конвертация отрицательного числа возвращает минус единицу</summary>
    [TestMethod]
    public void Convert_NegativeNumber_ReturnsMinusOne()
    {
        IValueConverter converter = new Sign();
        var result = converter.Convert(-5.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(-1.0, (double)result);
    }

    /// <summary>Конвертация нуля возвращает ноль</summary>
    [TestMethod]
    public void Convert_Zero_ReturnsZero()
    {
        IValueConverter converter = new Sign();
        var result = converter.Convert(0.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(0.0, (double)result);
    }

    /// <summary>Конвертация NaN возвращает NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNaN()
    {
        IValueConverter converter = new Sign();
        var result = converter.Convert(double.NaN, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.IsTrue(double.IsNaN((double)result));
    }
}
