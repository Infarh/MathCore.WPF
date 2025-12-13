using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Модульные тесты для конвертера OutRange</summary>
[TestClass]
public class OutRangeTests
{
    /// <summary>Тест значения вне диапазона (меньше минимума)</summary>
    [TestMethod]
    public void Convert_ValueBelowRange_ReturnsTrue()
    {
        IValueConverter converter = new OutRange { Min = 0.0, Max = 10.0 };
        var result = converter.Convert(-5.0, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест значения вне диапазона (больше максимума)</summary>
    [TestMethod]
    public void Convert_ValueAboveRange_ReturnsTrue()
    {
        IValueConverter converter = new OutRange { Min = 0.0, Max = 10.0 };
        var result = converter.Convert(15.0, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест значения внутри диапазона</summary>
    [TestMethod]
    public void Convert_ValueInRange_ReturnsFalse()
    {
        IValueConverter converter = new OutRange { Min = 0.0, Max = 10.0 };
        var result = converter.Convert(5.0, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест значения на минимальной границе</summary>
    [TestMethod]
    public void Convert_ValueAtMinBoundary_DependsOnMinInclude()
    {
        var converter1 = new OutRange { Min = 0.0, Max = 10.0, MinInclude = true };
        var converter2 = new OutRange { Min = 0.0, Max = 10.0, MinInclude = false };
        
        var result1 = ((IValueConverter)converter1).Convert(0.0, typeof(bool), null, CultureInfo.CurrentCulture);
        var result2 = ((IValueConverter)converter2).Convert(0.0, typeof(bool), null, CultureInfo.CurrentCulture);
        
        Assert.That.Value(result1).IsEqual(false, "С MinInclude=true граница внутри диапазона");
        Assert.That.Value(result2).IsEqual(true, "С MinInclude=false граница вне диапазона");
    }

    /// <summary>Тест значения на максимальной границе</summary>
    [TestMethod]
    public void Convert_ValueAtMaxBoundary_DependsOnMaxInclude()
    {
        var converter1 = new OutRange { Min = 0.0, Max = 10.0, MaxInclude = true };
        var converter2 = new OutRange { Min = 0.0, Max = 10.0, MaxInclude = false };
        
        var result1 = ((IValueConverter)converter1).Convert(10.0, typeof(bool), null, CultureInfo.CurrentCulture);
        var result2 = ((IValueConverter)converter2).Convert(10.0, typeof(bool), null, CultureInfo.CurrentCulture);
        
        Assert.That.Value(result1).IsEqual(false, "С MaxInclude=true граница внутри диапазона");
        Assert.That.Value(result2).IsEqual(true, "С MaxInclude=false граница вне диапазона");
    }

    /// <summary>Тест с NaN значением</summary>
    [TestMethod]
    public void Convert_NaNValue_ReturnsNull()
    {
        IValueConverter converter = new OutRange { Min = 0.0, Max = 10.0 };
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.CurrentCulture);
        Assert.IsNull(result);
    }

    /// <summary>Тест свойства IncludeLimits с true</summary>
    [TestMethod]
    public void IncludeLimits_SetToTrue_SetsBothIncludes()
    {
        var converter = new OutRange { IncludeLimits = true };
        Assert.That.Value(converter.MinInclude).IsEqual(true);
        Assert.That.Value(converter.MaxInclude).IsEqual(true);
    }

    /// <summary>Тест свойства IncludeLimits с false</summary>
    [TestMethod]
    public void IncludeLimits_SetToFalse_SetsBothExcludes()
    {
        var converter = new OutRange { IncludeLimits = false };
        Assert.That.Value(converter.MinInclude).IsEqual(false);
        Assert.That.Value(converter.MaxInclude).IsEqual(false);
    }

    /// <summary>Тест обратного преобразования</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        IValueConverter converter = new OutRange { Min = 0.0, Max = 10.0 };
        Assert.ThrowsException<NotSupportedException>(() =>
            converter.ConvertBack(true, typeof(double), null, CultureInfo.CurrentCulture));
    }
}
