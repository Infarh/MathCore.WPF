using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты конвертера логического ИЛИ</summary>
[TestClass]
public class OrTests
{
    /// <summary>Тест дизъюнкции всех true</summary>
    [TestMethod]
    public void Convert_AllTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or();
        var result = (bool)converter.Convert([true, true, true], typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть true");
    }

    /// <summary>Тест дизъюнкции с одним true</summary>
    [TestMethod]
    public void Convert_OneTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or();
        var result = (bool)converter.Convert([false, true, false], typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть true");
    }

    /// <summary>Тест дизъюнкции всех false</summary>
    [TestMethod]
    public void Convert_AllFalse_ReturnsFalse()
    {
        IMultiValueConverter converter = new Or();
        var result = (bool)converter.Convert([false, false, false], typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsFalse(result, "Результат должен быть false");
    }

    /// <summary>Тест с пустым массивом возвращает значение по умолчанию</summary>
    [TestMethod]
    public void Convert_Null_ReturnsNullDefaultValue()
    {
        IMultiValueConverter converter = new Or { NullDefaultValue = true };
        var result = (bool)converter.Convert(null, typeof(bool), null, CultureInfo.CurrentCulture)!;
        Assert.IsTrue(result, "Результат должен быть NullDefaultValue");
    }
}
