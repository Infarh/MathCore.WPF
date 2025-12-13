using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера логической операции ИЛИ (Or)</summary>
[TestClass]
public class OrTests
{
    /// <summary>Проверка что хотя бы одно true возвращает true</summary>
    [TestMethod]
    public void Convert_OneTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or();
        object[] values = [false, true, false];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result, "Хотя бы одно true должно вернуть true");
    }

    /// <summary>Проверка что все false возвращает false</summary>
    [TestMethod]
    public void Convert_AllFalse_ReturnsFalse()
    {
        IMultiValueConverter converter = new Or();
        object[] values = [false, false, false];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result, "Все false должны вернуть false");
    }

    /// <summary>Проверка что все true возвращает true</summary>
    [TestMethod]
    public void Convert_AllTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or();
        object[] values = [true, true, true];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result, "Все true должны вернуть true");
    }

    /// <summary>Проверка пустого массива с NullDefaultValue=false</summary>
    [TestMethod]
    public void Convert_EmptyArray_ReturnsNullDefaultValue()
    {
        IMultiValueConverter converter = new Or { NullDefaultValue = false };
        object[] values = [];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result, "Пустой массив должен вернуть NullDefaultValue");
    }

    /// <summary>Проверка пустого массива с NullDefaultValue=true</summary>
    [TestMethod]
    public void Convert_EmptyArray_WithNullDefaultTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or { NullDefaultValue = true };
        object[] values = [];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result, "Пустой массив должен вернуть NullDefaultValue=true");
    }

    /// <summary>Проверка null массива с NullDefaultValue=false</summary>
    [TestMethod]
    public void Convert_NullArray_ReturnsNullDefaultValue()
    {
        IMultiValueConverter converter = new Or { NullDefaultValue = false };
        
        var result = converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result, "null массив должен вернуть NullDefaultValue");
    }

    /// <summary>Проверка одного значения true</summary>
    [TestMethod]
    public void Convert_SingleTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or();
        object[] values = [true];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result, "Одно значение true должно вернуть true");
    }

    /// <summary>Проверка одного значения false</summary>
    [TestMethod]
    public void Convert_SingleFalse_ReturnsFalse()
    {
        IMultiValueConverter converter = new Or();
        object[] values = [false];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result, "Одно значение false должно вернуть false");
    }

    /// <summary>Проверка большого количества false значений</summary>
    [TestMethod]
    public void Convert_ManyFalseValues_ReturnsFalse()
    {
        IMultiValueConverter converter = new Or();
        object[] values = Enumerable.Repeat((object)false, 100).ToArray();
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result, "Много false значений должны вернуть false");
    }

    /// <summary>Проверка что true в конце массива возвращает true</summary>
    [TestMethod]
    public void Convert_TrueAtEnd_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or();
        object[] values = [false, false, false, true];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result, "true в конце должно вернуть true");
    }

    /// <summary>Проверка что true в начале массива возвращает true</summary>
    [TestMethod]
    public void Convert_TrueAtStart_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or();
        object[] values = [true, false, false, false];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result, "true в начале должно вернуть true");
    }

    /// <summary>Проверка двух значений false</summary>
    [TestMethod]
    public void Convert_TwoFalse_ReturnsFalse()
    {
        IMultiValueConverter converter = new Or();
        object[] values = [false, false];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result, "Два false должны вернуть false");
    }

    /// <summary>Проверка двух значений: одно true, одно false</summary>
    [TestMethod]
    public void Convert_OneTrueOneFalse_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or();
        object[] values = [true, false];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result, "true и false должны вернуть true");
    }

    /// <summary>Проверка свойства NullDefaultValue по умолчанию</summary>
    [TestMethod]
    public void DefaultNullDefaultValue_IsFalse()
    {
        var converter = new Or();
        
        Assert.IsFalse(converter.NullDefaultValue, "NullDefaultValue по умолчанию должно быть false");
    }
}
