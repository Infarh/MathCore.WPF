using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера знака числа</summary>
[TestClass]
public class SignTests
{
    /// <summary>Тест положительного значения</summary>
    [TestMethod]
    public void Convert_PositiveValue_ReturnsOne()
    {
        IValueConverter converter = new Sign { W = 1 };
        var result = (double)converter.Convert(5, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(1);
    }

    /// <summary>Тест отрицательного значения</summary>
    [TestMethod]
    public void Convert_NegativeValue_ReturnsMinusOne()
    {
        IValueConverter converter = new Sign { W = 1 };
        var result = (double)converter.Convert(-5, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(-1);
    }

    /// <summary>Тест нулевого значения</summary>
    [TestMethod]
    public void Convert_Zero_ReturnsZero()
    {
        IValueConverter converter = new Sign { W = 1 };
        var result = (double)converter.Convert(0, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(0);
    }

    /// <summary>Тест преобразования с параметром K</summary>
    [TestMethod]
    public void Convert_WithK_ScalesResult()
    {
        IValueConverter converter = new Sign { K = 2, W = 1 };
        var result = (double)converter.Convert(5, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(2);
    }

    /// <summary>Тест преобразования NaN возвращает NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNaN()
    {
        IValueConverter converter = new Sign();
        var result = (double)converter.Convert(double.NaN, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(double.IsNaN(result), "Результат должен быть NaN");
    }

    /// <summary>Тест обратного преобразования должен возвращать значение без изменений</summary>
    [TestMethod]
    public void ConvertBack_ReturnsValueUnchanged()
    {
        IValueConverter converter = new Sign();
        var result = (double)converter.ConvertBack(1, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(1);
    }
}
