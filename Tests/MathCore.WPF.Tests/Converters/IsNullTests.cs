using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера проверки на null</summary>
[TestClass]
public class IsNullTests
{
    /// <summary>Тест проверки null</summary>
    [TestMethod]
    public void Convert_Null_ReturnsTrue()
    {
        IValueConverter converter = new IsNull();
        var result = (bool)converter.Convert(null, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть true");
    }

    /// <summary>Тест проверки не-null</summary>
    [TestMethod]
    public void Convert_NotNull_ReturnsFalse()
    {
        IValueConverter converter = new IsNull();
        var result = (bool)converter.Convert("test", typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест инвертированной проверки null</summary>
    [TestMethod]
    public void Convert_NullInverted_ReturnsFalse()
    {
        IValueConverter converter = new IsNull(true);
        var result = (bool)converter.Convert(null, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест инвертированной проверки не-null</summary>
    [TestMethod]
    public void Convert_NotNullInverted_ReturnsTrue()
    {
        IValueConverter converter = new IsNull { Inverted = true };
        var result = (bool)converter.Convert("test", typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть true");
    }
}
