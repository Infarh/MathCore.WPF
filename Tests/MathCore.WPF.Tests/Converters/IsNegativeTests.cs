using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера проверки на отрицательное значение</summary>
[TestClass]
public class IsNegativeTests
{
    /// <summary>Конвертация отрицательного числа возвращает true</summary>
    [TestMethod]
    public void Convert_NegativeNumber_ReturnsTrue()
    {
        IValueConverter converter = new IsNegative();
        var result = converter.Convert(-5.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result!);
    }

    /// <summary>Конвертация положительного числа возвращает false</summary>
    [TestMethod]
    public void Convert_PositiveNumber_ReturnsFalse()
    {
        IValueConverter converter = new IsNegative();
        var result = converter.Convert(5.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(false, (bool)result!);
    }

    /// <summary>Конвертация нуля возвращает false</summary>
    [TestMethod]
    public void Convert_Zero_ReturnsFalse()
    {
        IValueConverter converter = new IsNegative();
        var result = converter.Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(false, (bool)result!);
    }

    /// <summary>Конвертация NaN возвращает null</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNull()
    {
        IValueConverter converter = new IsNegative();
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.IsNull(result);
    }
}
