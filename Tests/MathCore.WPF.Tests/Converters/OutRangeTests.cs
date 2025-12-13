using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера проверки выхода за пределы диапазона</summary>
[TestClass]
public class OutRangeTests
{
    /// <summary>Тест значения вне диапазона</summary>
    [TestMethod]
    public void Convert_ValueOutOfRange_ReturnsTrue()
    {
        IValueConverter converter = new OutRange(0, 10);
        var result = (bool)converter.Convert(15.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть true");
    }

    /// <summary>Тест значения внутри диапазона</summary>
    [TestMethod]
    public void Convert_ValueInRange_ReturnsFalse()
    {
        IValueConverter converter = new OutRange(0, 10);
        var result = (bool)converter.Convert(5.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест значения на границе диапазона</summary>
    [TestMethod]
    public void Convert_ValueOnMinBoundary_DependsOnMinInclude()
    {
        IValueConverter converter = new OutRange(0, 10) { MinInclude = false };
        var result = (bool)converter.Convert(0.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть true");
    }

    /// <summary>Тест значения на верхней границе диапазона</summary>
    [TestMethod]
    public void Convert_ValueOnMaxBoundary_DependsOnMaxInclude()
    {
        IValueConverter converter = new OutRange(0, 10) { MaxInclude = true };
        var result = (bool)converter.Convert(10.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест проверки NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNull()
    {
        IValueConverter converter = new OutRange(0, 10);
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.IsNull(result, "Результат должен быть null");
    }
}
