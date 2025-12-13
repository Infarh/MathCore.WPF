using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера логической инверсии Not</summary>
[TestClass]
public class NotTests
{
    /// <summary>Проверка инверсии значения true</summary>
    [TestMethod]
    public void Convert_True_ReturnsFalse()
    {
        IValueConverter converter = new Not();
        var result = converter.Convert(true, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, "Инверсия true должна возвращать false");
    }

    /// <summary>Проверка инверсии значения false</summary>
    [TestMethod]
    public void Convert_False_ReturnsTrue()
    {
        IValueConverter converter = new Not();
        var result = converter.Convert(false, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result!, "Инверсия false должна возвращать true");
    }

    /// <summary>Проверка обработки null значения</summary>
    [TestMethod]
    public void Convert_Null_ReturnsNull()
    {
        IValueConverter converter = new Not();
        var result = converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsNull(result, "Инверсия null должна возвращать null");
    }

    /// <summary>Проверка обратного преобразования true</summary>
    [TestMethod]
    public void ConvertBack_True_ReturnsFalse()
    {
        IValueConverter converter = new Not();
        var result = converter.ConvertBack(true, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, "Обратная инверсия true должна возвращать false");
    }

    /// <summary>Проверка обратного преобразования false</summary>
    [TestMethod]
    public void ConvertBack_False_ReturnsTrue()
    {
        IValueConverter converter = new Not();
        var result = converter.ConvertBack(false, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result!, "Обратная инверсия false должна возвращать true");
    }

    /// <summary>Проверка обратного преобразования null</summary>
    [TestMethod]
    public void ConvertBack_Null_ReturnsNull()
    {
        IValueConverter converter = new Not();
        var result = converter.ConvertBack(null, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsNull(result, "Обратная инверсия null должна возвращать null");
    }

    /// <summary>Проверка двойной инверсии (идемпотентность)</summary>
    [TestMethod]
    public void DoubleInversion_ReturnsOriginalValue()
    {
        IValueConverter converter = new Not();
        const bool original_value = true;
        
        var inverted = converter.Convert(original_value, typeof(bool), null, CultureInfo.InvariantCulture);
        var restored = converter.Convert(inverted, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(original_value, restored, "Двойная инверсия должна вернуть исходное значение");
    }
}
