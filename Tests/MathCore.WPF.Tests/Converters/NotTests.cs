using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера логического отрицания</summary>
[TestClass]
public class NotTests
{
    /// <summary>Тест отрицания true</summary>
    [TestMethod]
    public void Convert_True_ReturnsFalse()
    {
        IValueConverter converter = new Not();
        var result = (bool)converter.Convert(true, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест отрицания false</summary>
    [TestMethod]
    public void Convert_False_ReturnsTrue()
    {
        IValueConverter converter = new Not();
        var result = (bool)converter.Convert(false, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть true");
    }

    /// <summary>Тест обратного преобразования true</summary>
    [TestMethod]
    public void ConvertBack_True_ReturnsFalse()
    {
        IValueConverter converter = new Not();
        var result = (bool)converter.ConvertBack(true, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест обратного преобразования false</summary>
    [TestMethod]
    public void ConvertBack_False_ReturnsTrue()
    {
        IValueConverter converter = new Not();
        var result = (bool)converter.ConvertBack(false, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть true");
    }
}
