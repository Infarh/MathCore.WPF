using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для логического конвертера НЕ</summary>
[TestClass]
public class NotTests
{
    /// <summary>Конвертация true возвращает false</summary>
    [TestMethod]
    public void Convert_True_ReturnsFalse()
    {
        IValueConverter converter = new Not();
        var result = converter.Convert(true, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(false, (bool)result!);
    }

    /// <summary>Конвертация false возвращает true</summary>
    [TestMethod]
    public void Convert_False_ReturnsTrue()
    {
        IValueConverter converter = new Not();
        var result = converter.Convert(false, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result!);
    }

    /// <summary>Обратное преобразование инвертирует значение</summary>
    [TestMethod]
    public void ConvertBack_InvertsValue()
    {
        IValueConverter converter = new Not();
        var result = converter.ConvertBack(false, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result!);
    }
}
