using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера проверки на положительное значение</summary>
[TestClass]
public class IsPositiveTests
{
    /// <summary>Конвертация положительного числа возвращает true</summary>
    [TestMethod]
    public void Convert_PositiveNumber_ReturnsTrue()
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(5.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result!);
    }

    /// <summary>Конвертация отрицательного числа возвращает false</summary>
    [TestMethod]
    public void Convert_NegativeNumber_ReturnsFalse()
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(-5.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(false, (bool)result!);
    }

    /// <summary>Конвертация нуля возвращает false</summary>
    [TestMethod]
    public void Convert_Zero_ReturnsFalse()
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(false, (bool)result!);
    }

    /// <summary>Конвертация NaN возвращает null</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNull()
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.IsNull(result);
    }
}
