using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера проверки на null IsNull</summary>
[TestClass]
public class IsNullTests
{
    /// <summary>Проверка null возвращает true в нормальном режиме</summary>
    [TestMethod]
    public void Convert_Null_ReturnsTrue()
    {
        IValueConverter converter = new IsNull();
        var result = converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result!, "null должен возвращать true");
    }

    /// <summary>Проверка не-null значения возвращает false в нормальном режиме</summary>
    [TestMethod]
    public void Convert_NotNull_ReturnsFalse()
    {
        IValueConverter converter = new IsNull();
        var result = converter.Convert("test", typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, "не-null значение должно возвращать false");
    }

    /// <summary>Проверка различных не-null типов</summary>
    [DataTestMethod]
    [DataRow(42)]
    [DataRow("string")]
    [DataRow(3.14)]
    [DataRow(true)]
    public void Convert_VariousNotNullValues_ReturnsFalse(object value)
    {
        IValueConverter converter = new IsNull();
        var result = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, $"Значение {value} (не-null) должно возвращать false");
    }

    /// <summary>Проверка null в инвертированном режиме возвращает false</summary>
    [TestMethod]
    public void Convert_Null_WithInverted_ReturnsFalse()
    {
        IValueConverter converter = new IsNull(Inverted: true);
        var result = converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, "null с Inverted=true должен возвращать false");
    }

    /// <summary>Проверка не-null значения в инвертированном режиме возвращает true</summary>
    [TestMethod]
    public void Convert_NotNull_WithInverted_ReturnsTrue()
    {
        IValueConverter converter = new IsNull(Inverted: true);
        var result = converter.Convert("test", typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result!, "не-null значение с Inverted=true должно возвращать true");
    }

    /// <summary>Проверка свойства Inverted через конструктор</summary>
    [TestMethod]
    public void Constructor_WithInvertedParameter_SetsProperty()
    {
        var converter = new IsNull(Inverted: true);
        
        Assert.IsTrue(converter.Inverted, "Свойство Inverted должно быть установлено через конструктор");
    }

    /// <summary>Проверка изменения свойства Inverted после создания</summary>
    [TestMethod]
    public void InvertedProperty_CanBeChanged()
    {
        var converter = new IsNull { Inverted = true };
        IValueConverter converter_interface = converter;
        
        var result = converter_interface.Convert("test", typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result!, "Изменение Inverted должно влиять на результат");
    }

    /// <summary>Проверка пустой строки (не является null)</summary>
    [TestMethod]
    public void Convert_EmptyString_ReturnsFalse()
    {
        IValueConverter converter = new IsNull();
        var result = converter.Convert(string.Empty, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, "Пустая строка не является null, должно вернуть false");
    }

    /// <summary>Проверка значения по умолчанию для типов значений</summary>
    [TestMethod]
    public void Convert_DefaultInt_ReturnsFalse()
    {
        IValueConverter converter = new IsNull();
        var result = converter.Convert(0, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, "0 (default int) не является null, должно вернуть false");
    }
}
