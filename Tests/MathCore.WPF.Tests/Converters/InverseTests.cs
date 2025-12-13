using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера обратного значения</summary>
[TestClass]
public class InverseTests
{
    /// <summary>Тест обратного значения</summary>
    [TestMethod]
    public void Convert_Value_ReturnsInverse()
    {
        IValueConverter converter = new Inverse();
        var result = (double)converter.Convert(2, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(0.5);
    }

    /// <summary>Тест обратного значения с параметром</summary>
    [TestMethod]
    public void Convert_WithParameter_ReturnsParameterDividedByValue()
    {
        IValueConverter converter = new Inverse();
        var result = (double)converter.Convert(2, typeof(double), 10.0, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(5);
    }

    /// <summary>Тест обратного значения единицы</summary>
    [TestMethod]
    public void Convert_One_ReturnsOne()
    {
        IValueConverter converter = new Inverse();
        var result = (double)converter.Convert(1, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(1);
    }

    /// <summary>Тест обратного значения отрицательного числа</summary>
    [TestMethod]
    public void Convert_NegativeValue_ReturnsNegativeInverse()
    {
        IValueConverter converter = new Inverse();
        var result = (double)converter.Convert(-2, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(-0.5);
    }
}
