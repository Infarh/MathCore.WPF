using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера логического И</summary>
[TestClass]
public class AndTests
{
    /// <summary>Тест конъюнкции всех true</summary>
    [TestMethod]
    public void Convert_AllTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new And();
        var result = (bool)converter.Convert([true, true, true], typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть true");
    }

    /// <summary>Тест конъюнкции с одним false</summary>
    [TestMethod]
    public void Convert_OneFalse_ReturnsFalse()
    {
        IMultiValueConverter converter = new And();
        var result = (bool)converter.Convert([true, false, true], typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест конъюнкции всех false</summary>
    [TestMethod]
    public void Convert_AllFalse_ReturnsFalse()
    {
        IMultiValueConverter converter = new And();
        var result = (bool)converter.Convert([false, false, false], typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест с пустым массивом возвращает значение по умолчанию</summary>
    [TestMethod]
    public void Convert_Null_ReturnsNullDefaultValue()
    {
        IMultiValueConverter converter = new And { NullDefaultValue = true };
        var result = (bool)converter.Convert(null, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть NullDefaultValue");
    }
}
