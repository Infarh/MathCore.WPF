using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class OutRangeTests
{
    /// <summary>Проверка значения внутри диапазона</summary>
    [TestMethod]
    public void Convert_ValueInRange_ReturnsFalse()
    {
        var converter = new OutRange { Min = 0.0, Max = 100.0 };

        var result = ((IValueConverter)converter).Convert(50.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка значения вне диапазона (меньше минимума)</summary>
    [TestMethod]
    public void Convert_ValueBelowRange_ReturnsTrue()
    {
        var converter = new OutRange { Min = 0.0, Max = 100.0 };

        var result = ((IValueConverter)converter).Convert(-10.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка значения вне диапазона (больше максимума)</summary>
    [TestMethod]
    public void Convert_ValueAboveRange_ReturnsTrue()
    {
        var converter = new OutRange { Min = 0.0, Max = 100.0 };

        var result = ((IValueConverter)converter).Convert(150.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка значения на минимальной границе с MinInclude=true</summary>
    [TestMethod]
    public void Convert_ValueAtMinBoundary_MinIncluded_ReturnsFalse()
    {
        var converter = new OutRange { Min = 0.0, Max = 100.0, MinInclude = true };

        var result = ((IValueConverter)converter).Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка значения на минимальной границе с MinInclude=false</summary>
    [TestMethod]
    public void Convert_ValueAtMinBoundary_MinExcluded_ReturnsTrue()
    {
        var converter = new OutRange { Min = 0.0, Max = 100.0, MinInclude = false };

        var result = ((IValueConverter)converter).Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка значения на максимальной границе с MaxInclude=true</summary>
    [TestMethod]
    public void Convert_ValueAtMaxBoundary_MaxIncluded_ReturnsFalse()
    {
        var converter = new OutRange { Min = 0.0, Max = 100.0, MaxInclude = true };

        var result = ((IValueConverter)converter).Convert(100.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка значения на максимальной границе с MaxInclude=false</summary>
    [TestMethod]
    public void Convert_ValueAtMaxBoundary_MaxExcluded_ReturnsTrue()
    {
        var converter = new OutRange { Min = 0.0, Max = 100.0, MaxInclude = false };

        var result = ((IValueConverter)converter).Convert(100.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка что NaN возвращает null</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNull()
    {
        var converter = new OutRange { Min = 0.0, Max = 100.0 };

        var result = ((IValueConverter)converter).Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.IsNull(result, "NaN должен возвращать null");
    }

    /// <summary>Проверка с отрицательным диапазоном</summary>
    [TestMethod]
    public void Convert_NegativeRange_WorksCorrectly()
    {
        var converter = new OutRange { Min = -100.0, Max = -10.0 };

        var result_in = ((IValueConverter)converter).Convert(-50.0, typeof(bool), null, CultureInfo.InvariantCulture);
        var result_out = ((IValueConverter)converter).Convert(-5.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result_in).IsEqual(false);
        Assert.That.Value(result_out).IsEqual(true);
    }

    /// <summary>Проверка свойства IncludeLimits</summary>
    [TestMethod]
    public void IncludeLimits_Property_WorksCorrectly()
    {
        var converter = new OutRange { Min = 0.0, Max = 100.0 };
        
        converter.IncludeLimits = true;
        Assert.That.Value(converter.MinInclude).IsEqual(true);
        Assert.That.Value(converter.MaxInclude).IsEqual(true);

        converter.IncludeLimits = false;
        Assert.That.Value(converter.MinInclude).IsEqual(false);
        Assert.That.Value(converter.MaxInclude).IsEqual(false);
    }

    /// <summary>Проверка что ConvertBack выбрасывает NotSupportedException</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        var converter = new OutRange();

        Assert.ThrowsException<NotSupportedException>(() =>
            ((IValueConverter)converter).ConvertBack(true, typeof(double), null, CultureInfo.InvariantCulture),
            "ConvertBack не должен поддерживаться для конвертера диапазона");
    }
}
