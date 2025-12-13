using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера проверки на NaN</summary>
[TestClass]
public class IsNaNTests
{
    /// <summary>Тест проверки NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsTrue()
    {
        IValueConverter converter = new IsNaN();
        var result = (bool)converter.Convert(double.NaN, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть true");
    }

    /// <summary>Тест проверки числа</summary>
    [TestMethod]
    public void Convert_Number_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN();
        var result = (bool)converter.Convert(5.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест инвертированной проверки NaN</summary>
    [TestMethod]
    public void Convert_NaNInverted_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN(true);
        var result = (bool)converter.Convert(double.NaN, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест инвертированной проверки числа</summary>
    [TestMethod]
    public void Convert_NumberInverted_ReturnsTrue()
    {
        IValueConverter converter = new IsNaN { Inverted = true };
        var result = (bool)converter.Convert(5.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть true");
    }
}
