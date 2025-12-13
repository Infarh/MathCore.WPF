using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера остатка от деления</summary>
[TestClass]
public class ModTests
{
    /// <summary>Конвертация вычисляет остаток от деления</summary>
    [TestMethod]
    public void Convert_CalculatesModulo()
    {
        IValueConverter converter = new Mod { M = 3 };
        var result = converter.Convert(10.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(1.0, (double)result);
    }

    /// <summary>Конвертация с NaN возвращает NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNaN()
    {
        IValueConverter converter = new Mod { M = 3 };
        var result = converter.Convert(double.NaN, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.IsTrue(double.IsNaN((double)result));
    }
}
