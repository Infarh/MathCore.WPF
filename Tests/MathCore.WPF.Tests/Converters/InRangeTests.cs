using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера проверки вхождения в диапазон</summary>
[TestClass]
public class InRangeTests
{
    /// <summary>Конвертация значения в диапазоне возвращает true</summary>
    [TestMethod]
    public void Convert_ValueInRange_ReturnsTrue()
    {
        IValueConverter converter = new InRange { Min = 0.0, Max = 10.0 };
        var result = converter.Convert(5.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result!);
    }

    /// <summary>Конвертация значения вне диапазона возвращает false</summary>
    [TestMethod]
    public void Convert_ValueOutOfRange_ReturnsFalse()
    {
        IValueConverter converter = new InRange { Min = 0.0, Max = 10.0 };
        var result = converter.Convert(15.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(false, (bool)result!);
    }

    /// <summary>Конвертация граничного значения с включенной границей возвращает true</summary>
    [TestMethod]
    public void Convert_BoundaryValueIncluded_ReturnsTrue()
    {
        IValueConverter converter = new InRange { Min = 0.0, Max = 10.0, MinInclude = true };
        var result = converter.Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result!);
    }

    /// <summary>Конвертация NaN возвращает null</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNull()
    {
        IValueConverter converter = new InRange();
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.IsNull(result);
    }
}
