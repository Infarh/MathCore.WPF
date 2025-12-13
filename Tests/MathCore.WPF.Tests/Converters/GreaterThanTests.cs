using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера сравнения больше</summary>
[TestClass]
public class GreaterThanTests
{
    /// <summary>Тест значения больше порога</summary>
    [TestMethod]
    public void Convert_ValueGreater_ReturnsTrue()
    {
        IValueConverter converter = new GreaterThan(5);
        var result = (bool)converter.Convert(10.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть true");
    }

    /// <summary>Тест значения меньше порога</summary>
    [TestMethod]
    public void Convert_ValueLess_ReturnsFalse()
    {
        IValueConverter converter = new GreaterThan(5);
        var result = (bool)converter.Convert(3.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест значения равного порогу</summary>
    [TestMethod]
    public void Convert_ValueEqual_ReturnsFalse()
    {
        IValueConverter converter = new GreaterThan(5);
        var result = (bool)converter.Convert(5.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест проверки NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNull()
    {
        IValueConverter converter = new GreaterThan(5);
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.IsNull(result, "Результат должен быть null");
    }
}
