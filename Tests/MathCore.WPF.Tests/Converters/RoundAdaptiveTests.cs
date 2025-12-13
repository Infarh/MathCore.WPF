using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты адаптивного округления</summary>
[TestClass]
public class RoundAdaptiveTests
{
    /// <summary>Тест округления положительного значения</summary>
    [TestMethod]
    public void Convert_PositiveValue_ReturnsRounded()
    {
        IValueConverter converter = new RoundAdaptive(2);
        var result = (double)converter.Convert(3.14159, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(3.14, 1e-10);
    }

    /// <summary>Тест округления отрицательного значения</summary>
    [TestMethod]
    public void Convert_NegativeValue_ReturnsRounded()
    {
        IValueConverter converter = new RoundAdaptive(2);
        var result = (double)converter.Convert(-3.14159, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(-3.14, 1e-10);
    }

    /// <summary>Тест округления с нулевыми знаками</summary>
    [TestMethod]
    public void Convert_WithZeroDigits_ReturnsInteger()
    {
        IValueConverter converter = new RoundAdaptive(0);
        var result = (double)converter.Convert(3.7, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(4);
    }

    /// <summary>Тест обратного преобразования должен возвращать значение без изменений</summary>
    [TestMethod]
    public void ConvertBack_ReturnsValueUnchanged()
    {
        IValueConverter converter = new RoundAdaptive();
        var result = (double)converter.ConvertBack(5, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(5);
    }
}
