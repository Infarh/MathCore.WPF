using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера инверсии</summary>
[TestClass]
public class InverseTests
{
    /// <summary>Конвертация возвращает обратную величину</summary>
    [TestMethod]
    public void Convert_ReturnsInverse()
    {
        IValueConverter converter = new Inverse();
        var result = converter.Convert(2.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(0.5, (double)result, 0.0001);
    }

    /// <summary>Конвертация с параметром использует параметр как числитель</summary>
    [TestMethod]
    public void Convert_WithParameter_UsesParameterAsNumerator()
    {
        IValueConverter converter = new Inverse();
        var result = converter.Convert(2.0, typeof(double), 10.0, CultureInfo.InvariantCulture);
        Assert.AreEqual(5.0, (double)result, 0.0001);
    }
}
