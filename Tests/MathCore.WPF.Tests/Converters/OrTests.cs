using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для логического конвертера ИЛИ</summary>
[TestClass]
public class OrTests
{
    /// <summary>Конвертация всех false возвращает false</summary>
    [TestMethod]
    public void Convert_AllFalse_ReturnsFalse()
    {
        IMultiValueConverter converter = new Or();
        var result = converter.Convert([false, false, false], typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(false, (bool)result);
    }

    /// <summary>Конвертация с одним true возвращает true</summary>
    [TestMethod]
    public void Convert_OneTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or();
        var result = converter.Convert([false, true, false], typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result);
    }

    /// <summary>Конвертация null использует значение по умолчанию</summary>
    [TestMethod]
    public void Convert_Null_ReturnsDefaultValue()
    {
        IMultiValueConverter converter = new Or { NullDefaultValue = true };
        var result = converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result);
    }
}
