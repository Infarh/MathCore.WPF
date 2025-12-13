using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера проверки на NaN</summary>
[TestClass]
public class IsNaNTests
{
    /// <summary>Конвертация NaN возвращает true</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsTrue()
    {
        IValueConverter converter = new IsNaN();
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result!);
    }

    /// <summary>Конвертация числа возвращает false</summary>
    [TestMethod]
    public void Convert_Number_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN();
        var result = converter.Convert(5.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(false, (bool)result!);
    }

    /// <summary>Конвертация с инверсией возвращает противоположный результат</summary>
    [TestMethod]
    public void Convert_Inverted_ReturnsOpposite()
    {
        IValueConverter converter = new IsNaN { Inverted = true };
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(false, (bool)result!);
    }
}
