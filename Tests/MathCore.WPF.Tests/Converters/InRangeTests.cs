using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера проверки вхождения в диапазон</summary>
[TestClass]
public class InRangeTests
{
    /// <summary>Тест значения внутри диапазона</summary>
    [TestMethod]
    public void Convert_ValueInRange_ReturnsTrue()
    {
        IValueConverter converter = new InRange(0, 10);
        var result = (bool)converter.Convert(5.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть true");
    }

    /// <summary>Тест значения вне диапазона</summary>
    [TestMethod]
    public void Convert_ValueOutOfRange_ReturnsFalse()
    {
        IValueConverter converter = new InRange(0, 10);
        var result = (bool)converter.Convert(15.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест значения на границе диапазона</summary>
    [TestMethod]
    public void Convert_ValueOnMinBoundary_DependsOnMinInclude()
    {
        IValueConverter converter = new InRange(0, 10) { MinInclude = false };
        var result = (bool)converter.Convert(0.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест значения на верхней границе диапазона</summary>
    [TestMethod]
    public void Convert_ValueOnMaxBoundary_DependsOnMaxInclude()
    {
        IValueConverter converter = new InRange(0, 10) { MaxInclude = true };
        var result = (bool)converter.Convert(10.0, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть true");
    }

    /// <summary>Тест проверки NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNull()
    {
        IValueConverter converter = new InRange(0, 10);
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.IsNull(result, "Результат должен быть null");
    }
}
