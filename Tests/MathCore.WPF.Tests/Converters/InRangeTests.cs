using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class InRangeTests
{
    /// <summary>Проверка значения внутри диапазона</summary>
    [TestMethod]
    public void Convert_ValueInRange_ReturnsTrue()
    {
        var converter = new InRange { Min = 0.0, Max = 100.0 };

        var result = ((IValueConverter)converter).Convert(50.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка значения вне диапазона (меньше минимума)</summary>
    [TestMethod]
    public void Convert_ValueBelowRange_ReturnsFalse()
    {
        var converter = new InRange { Min = 0.0, Max = 100.0 };

        var result = ((IValueConverter)converter).Convert(-10.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка значения вне диапазона (больше максимума)</summary>
    [TestMethod]
    public void Convert_ValueAboveRange_ReturnsFalse()
    {
        var converter = new InRange { Min = 0.0, Max = 100.0 };

        var result = ((IValueConverter)converter).Convert(150.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка значения на минимальной границе с MinInclude=true</summary>
    [TestMethod]
    public void Convert_ValueAtMinBoundary_MinIncluded_ReturnsTrue()
    {
        var converter = new InRange { Min = 0.0, Max = 100.0, MinInclude = true };

        var result = ((IValueConverter)converter).Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка значения на минимальной границе с MinInclude=false</summary>
    [TestMethod]
    public void Convert_ValueAtMinBoundary_MinExcluded_ReturnsFalse()
    {
        var converter = new InRange { Min = 0.0, Max = 100.0, MinInclude = false };

        var result = ((IValueConverter)converter).Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка значения на максимальной границе с MaxInclude=true</summary>
    [TestMethod]
    public void Convert_ValueAtMaxBoundary_MaxIncluded_ReturnsTrue()
    {
        var converter = new InRange { Min = 0.0, Max = 100.0, MaxInclude = true };

        var result = ((IValueConverter)converter).Convert(100.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка значения на максимальной границе с MaxInclude=false</summary>
    [TestMethod]
    public void Convert_ValueAtMaxBoundary_MaxExcluded_ReturnsFalse()
    {
        var converter = new InRange { Min = 0.0, Max = 100.0, MaxInclude = false };

        var result = ((IValueConverter)converter).Convert(100.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка что NaN возвращает null</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNull()
    {
        var converter = new InRange { Min = 0.0, Max = 100.0 };

        var result = ((IValueConverter)converter).Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.IsNull(result, "NaN должен возвращать null");
    }

    /// <summary>Проверка с отрицательным диапазоном</summary>
    [TestMethod]
    public void Convert_NegativeRange_WorksCorrectly()
    {
        var converter = new InRange { Min = -100.0, Max = -10.0 };

        var result_in = ((IValueConverter)converter).Convert(-50.0, typeof(bool), null, CultureInfo.InvariantCulture);
        var result_out = ((IValueConverter)converter).Convert(-5.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result_in).IsEqual(true);
        Assert.That.Value(result_out).IsEqual(false);
    }

    /// <summary>Проверка симметричного диапазона</summary>
    [TestMethod]
    public void Convert_SymmetricRange_WorksCorrectly()
    {
        var converter = new InRange(50.0); // создает диапазон [-50, 50]

        var result_positive = ((IValueConverter)converter).Convert(25.0, typeof(bool), null, CultureInfo.InvariantCulture);
        var result_negative = ((IValueConverter)converter).Convert(-25.0, typeof(bool), null, CultureInfo.InvariantCulture);
        var result_out = ((IValueConverter)converter).Convert(75.0, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result_positive).IsEqual(true);
        Assert.That.Value(result_negative).IsEqual(true);
        Assert.That.Value(result_out).IsEqual(false);
    }

    /// <summary>Проверка что ConvertBack выбрасывает NotSupportedException</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        var converter = new InRange();

        Assert.ThrowsException<NotSupportedException>(() =>
            ((IValueConverter)converter).ConvertBack(true, typeof(double), null, CultureInfo.InvariantCulture),
            "ConvertBack не должен поддерживаться для конвертера диапазона");
    }
}
