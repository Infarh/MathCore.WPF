using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера проверки на null</summary>
[TestClass]
public class IsNullTests
{
    /// <summary>Конвертация null возвращает true</summary>
    [TestMethod]
    public void Convert_Null_ReturnsTrue()
    {
        IValueConverter converter = new IsNull();
        var result = converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(true, (bool)result!);
    }

    /// <summary>Конвертация объекта возвращает false</summary>
    [TestMethod]
    public void Convert_Object_ReturnsFalse()
    {
        IValueConverter converter = new IsNull();
        var result = converter.Convert("test", typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(false, (bool)result!);
    }

    /// <summary>Конвертация с инверсией возвращает противоположный результат</summary>
    [TestMethod]
    public void Convert_Inverted_ReturnsOpposite()
    {
        IValueConverter converter = new IsNull { Inverted = true };
        var result = converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(false, (bool)result!);
    }
}
