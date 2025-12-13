using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера знака числа с порогом</summary>
[TestClass]
public class SignValueTests
{
    /// <summary>Тест положительного значения выше порога</summary>
    [TestMethod]
    public void Convert_PositiveValueAboveDelta_ReturnsOne()
    {
        IValueConverter converter = new SignValue { Delta = 0.5, W = 1 };
        var result = (double)converter.Convert(1, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(1);
    }

    /// <summary>Тест отрицательного значения выше порога</summary>
    [TestMethod]
    public void Convert_NegativeValueAboveDelta_ReturnsMinusOne()
    {
        IValueConverter converter = new SignValue { Delta = 0.5, W = 1 };
        var result = (double)converter.Convert(-1, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(-1);
    }

    /// <summary>Тест значения в пределах порога</summary>
    [TestMethod]
    public void Convert_ValueWithinDelta_ReturnsZero()
    {
        IValueConverter converter = new SignValue { Delta = 1 };
        var result = (double)converter.Convert(0.5, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(0);
    }

    /// <summary>Тест инверсии</summary>
    [TestMethod]
    public void Convert_WithInverse_ReturnsNegatedSign()
    {
        IValueConverter converter = new SignValue { Inverse = true, W = 1 };
        var result = (double)converter.Convert(5, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(-1);
    }

    /// <summary>Тест преобразования NaN возвращает NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNaN()
    {
        IValueConverter converter = new SignValue();
        var result = (double)converter.Convert(double.NaN, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(double.IsNaN(result), "Результат должен быть NaN");
    }
}
