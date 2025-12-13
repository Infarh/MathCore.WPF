using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера проверки выхода за пределы диапазона</summary>
[TestClass]
public class OutRangeTests
{
    /// <summary>Конвертация значения вне диапазона возвращает true</summary>
    [TestMethod]
    public void Convert_ValueOutOfRange_ReturnsTrue()
    {
        IValueConverter converter = new OutRange { Min = 0.0, Max = 10.0 };
        var result = converter.Convert(15.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result!);
    }

    /// <summary>Конвертация значения в диапазоне возвращает false</summary>
    [TestMethod]
    public void Convert_ValueInRange_ReturnsFalse()
    {
        IValueConverter converter = new OutRange { Min = 0.0, Max = 10.0 };
        var result = converter.Convert(5.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(false, (bool)result!);
    }

    /// <summary>Конвертация граничного значения с исключенной границей возвращает true</summary>
    [TestMethod]
    public void Convert_BoundaryValueExcluded_ReturnsTrue()
    {
        IValueConverter converter = new OutRange { Min = 0.0, Max = 10.0, MinInclude = false };
        var result = converter.Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result!);
    }

    /// <summary>Конвертация NaN возвращает null</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNull()
    {
        IValueConverter converter = new OutRange();
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.IsNull(result);
    }
}
