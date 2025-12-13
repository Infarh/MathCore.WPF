using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера отсечения дробной части</summary>
[TestClass]
public class TruncateTests
{
    /// <summary>Тест отсечения положительного значения</summary>
    [TestMethod]
    public void Convert_PositiveValue_ReturnsTruncated()
    {
        IValueConverter converter = new Truncate();
        var result = (double)converter.Convert(3.7, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(3);
    }

    /// <summary>Тест отсечения отрицательного значения</summary>
    [TestMethod]
    public void Convert_NegativeValue_ReturnsTruncated()
    {
        IValueConverter converter = new Truncate();
        var result = (double)converter.Convert(-3.7, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(-3);
    }

    /// <summary>Тест отсечения целого числа</summary>
    [TestMethod]
    public void Convert_IntegerValue_ReturnsSameValue()
    {
        IValueConverter converter = new Truncate();
        var result = (double)converter.Convert(5, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(5);
    }

    /// <summary>Тест обратного преобразования должен возвращать значение без изменений</summary>
    [TestMethod]
    public void ConvertBack_ReturnsValueUnchanged()
    {
        IValueConverter converter = new Truncate();
        var result = (double)converter.ConvertBack(5, typeof(double), null, CultureInfo.CurrentCulture)!;
        Assert.That.Value(result).IsEqual(5);
    }
}
