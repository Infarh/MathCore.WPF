using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для адаптивного округления</summary>
[TestClass]
public class RoundAdaptiveTests
{
    /// <summary>Конвертация округляет значение</summary>
    [TestMethod]
    public void Convert_RoundsValue()
    {
        IValueConverter converter = new RoundAdaptive { Digits = 2 };
        var result = converter.Convert(3.14159, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(3.14, (double)result, 0.001);
    }

    /// <summary>Конвертация с нулевыми разрядами округляет до целого</summary>
    [TestMethod]
    public void Convert_ZeroDigits_RoundsToInteger()
    {
        IValueConverter converter = new RoundAdaptive();
        var result = converter.Convert(3.7, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(4.0, (double)result);
    }
}
