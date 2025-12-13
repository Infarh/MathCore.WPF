using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для логического конвертера И</summary>
[TestClass]
public class AndTests
{
    /// <summary>Конвертация всех true возвращает true</summary>
    [TestMethod]
    public void Convert_AllTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new And();
        var result = converter.Convert([true, true, true], typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result);
    }

    /// <summary>Конвертация с одним false возвращает false</summary>
    [TestMethod]
    public void Convert_OneFalse_ReturnsFalse()
    {
        IMultiValueConverter converter = new And();
        var result = converter.Convert([true, false, true], typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(false, (bool)result);
    }

    /// <summary>Конвертация null использует значение по умолчанию</summary>
    [TestMethod]
    public void Convert_Null_ReturnsDefaultValue()
    {
        IMultiValueConverter converter = new And { NullDefaultValue = true };
        var result = converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result);
    }
}
