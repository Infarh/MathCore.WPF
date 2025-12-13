using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера проверки на положительность</summary>
[TestClass]
public class IsPositiveTests
{
    /// <summary>Тест проверки положительного числа</summary>
    [TestMethod]
    public void Convert_PositiveValue_ReturnsTrue()
    {
        IValueConverter converter = new IsPositive();
        var result = (bool)converter.Convert(5.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть true");
    }

    /// <summary>Тест проверки отрицательного числа</summary>
    [TestMethod]
    public void Convert_NegativeValue_ReturnsFalse()
    {
        IValueConverter converter = new IsPositive();
        var result = (bool)converter.Convert(-5.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест проверки нуля</summary>
    [TestMethod]
    public void Convert_Zero_ReturnsFalse()
    {
        IValueConverter converter = new IsPositive();
        var result = (bool)converter.Convert(0.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест проверки NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNull()
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.IsNull(result, "Результат должен быть null");
    }
}
