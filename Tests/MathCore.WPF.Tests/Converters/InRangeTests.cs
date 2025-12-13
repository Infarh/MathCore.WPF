using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Модульные тесты для конвертера InRange</summary>
[TestClass]
public class InRangeTests
{
    /// <summary>Тест значения внутри диапазона</summary>
    [TestMethod]
    public void Convert_ValueInRange_ReturnsTrue()
    {
        IValueConverter converter = new InRange { Min = 0.0, Max = 10.0 };
        var result = converter.Convert(5.0, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест значения вне диапазона (меньше минимума)</summary>
    [TestMethod]
    public void Convert_ValueBelowRange_ReturnsFalse()
    {
        IValueConverter converter = new InRange { Min = 0.0, Max = 10.0 };
        var result = converter.Convert(-5.0, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест значения вне диапазона (больше максимума)</summary>
    [TestMethod]
    public void Convert_ValueAboveRange_ReturnsFalse()
    {
        IValueConverter converter = new InRange { Min = 0.0, Max = 10.0 };
        var result = converter.Convert(15.0, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест значения на минимальной границе</summary>
    [TestMethod]
    public void Convert_ValueAtMinBoundary_DependsOnMinInclude()
    {
        var converter1 = new InRange { Min = 0.0, Max = 10.0, MinInclude = true };
        var converter2 = new InRange { Min = 0.0, Max = 10.0, MinInclude = false };
        
        var result1 = ((IValueConverter)converter1).Convert(0.0, typeof(bool), null, CultureInfo.CurrentCulture);
        var result2 = ((IValueConverter)converter2).Convert(0.0, typeof(bool), null, CultureInfo.CurrentCulture);
        
        Assert.That.Value(result1).IsEqual(true, "С MinInclude=true граница должна включаться");
        Assert.That.Value(result2).IsEqual(false, "С MinInclude=false граница не должна включаться");
    }

    /// <summary>Тест значения на максимальной границе</summary>
    [TestMethod]
    public void Convert_ValueAtMaxBoundary_DependsOnMaxInclude()
    {
        var converter1 = new InRange { Min = 0.0, Max = 10.0, MaxInclude = true };
        var converter2 = new InRange { Min = 0.0, Max = 10.0, MaxInclude = false };
        
        var result1 = ((IValueConverter)converter1).Convert(10.0, typeof(bool), null, CultureInfo.CurrentCulture);
        var result2 = ((IValueConverter)converter2).Convert(10.0, typeof(bool), null, CultureInfo.CurrentCulture);
        
        Assert.That.Value(result1).IsEqual(true, "С MaxInclude=true граница должна включаться");
        Assert.That.Value(result2).IsEqual(false, "С MaxInclude=false граница не должна включаться");
    }

    /// <summary>Тест с NaN значением</summary>
    [TestMethod]
    public void Convert_NaNValue_ReturnsNull()
    {
        IValueConverter converter = new InRange { Min = 0.0, Max = 10.0 };
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.IsNull(result);
    }

    /// <summary>Тест симметричного диапазона через конструктор</summary>
    [TestMethod]
    public void Constructor_SymmetricRange_CreatesCorrectRange()
    {
        var converter = new InRange(5.0);
        Assert.That.Value(converter.Min).IsEqual(-5.0);
        Assert.That.Value(converter.Max).IsEqual(5.0);
    }

    /// <summary>Тест конструктора с минимумом и максимумом</summary>
    [TestMethod]
    public void Constructor_MinMax_SetsValues()
    {
        var converter = new InRange(10.0, 20.0);
        Assert.That.Value(converter.Min).IsEqual(10.0);
        Assert.That.Value(converter.Max).IsEqual(20.0);
    }

    /// <summary>Тест обратного преобразования</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        IValueConverter converter = new InRange { Min = 0.0, Max = 10.0 };
        Assert.ThrowsException<NotSupportedException>(() =>
            converter.ConvertBack(true, typeof(double), null, CultureInfo.CurrentCulture));
    }
}
