using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера знака числа с порогом</summary>
[TestClass]
public class SignValueTests
{
    /// <summary>Конвертация положительного числа возвращает единицу</summary>
    [TestMethod]
    public void Convert_PositiveNumber_ReturnsOne()
    {
        IValueConverter converter = new SignValue();
        var result = converter.Convert(5.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(1.0, (double)result);
    }

    /// <summary>Конвертация отрицательного числа возвращает минус единицу</summary>
    [TestMethod]
    public void Convert_NegativeNumber_ReturnsMinusOne()
    {
        IValueConverter converter = new SignValue();
        var result = converter.Convert(-5.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(-1.0, (double)result);
    }

    /// <summary>Конвертация малого значения возвращает ноль</summary>
    [TestMethod]
    public void Convert_SmallValue_ReturnsZero()
    {
        IValueConverter converter = new SignValue { Delta = 0.1 };
        var result = converter.Convert(0.05, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(0.0, (double)result);
    }

    /// <summary>Конвертация с инверсией возвращает противоположный знак</summary>
    [TestMethod]
    public void Convert_InverseMode_ReturnsNegativeSign()
    {
        IValueConverter converter = new SignValue { Inverse = true };
        var result = converter.Convert(5.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(-1.0, (double)result);
    }

    /// <summary>Конвертация NaN возвращает NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNaN()
    {
        IValueConverter converter = new SignValue();
        var result = converter.Convert(double.NaN, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.IsTrue(double.IsNaN((double)result));
    }
}
