using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера сравнения больше или равно</summary>
[TestClass]
public class GreaterThanOrEqualTests
{
    /// <summary>Конвертация большего значения возвращает true</summary>
    [TestMethod]
    public void Convert_GreaterValue_ReturnsTrue()
    {
        IValueConverter converter = new GreaterThanOrEqual { Value = 5.0 };
        var result = converter.Convert(10.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result!);
    }

    /// <summary>Конвертация равного значения возвращает true</summary>
    [TestMethod]
    public void Convert_EqualValue_ReturnsTrue()
    {
        IValueConverter converter = new GreaterThanOrEqual { Value = 5.0 };
        var result = converter.Convert(5.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result!);
    }

    /// <summary>Конвертация меньшего значения возвращает false</summary>
    [TestMethod]
    public void Convert_LesserValue_ReturnsFalse()
    {
        IValueConverter converter = new GreaterThanOrEqual { Value = 5.0 };
        var result = converter.Convert(3.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(false, (bool)result!);
    }

    /// <summary>Конвертация NaN возвращает null</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNull()
    {
        IValueConverter converter = new GreaterThanOrEqual();
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.IsNull(result);
    }
}
