using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера логической операции И (And)</summary>
[TestClass]
public class AndTests
{
    /// <summary>Проверка что все true возвращает true</summary>
    [TestMethod]
    public void Convert_AllTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new And();
        object[] values = [true, true, true];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result, "Все значения true должны вернуть true");
    }

    /// <summary>Проверка что одно false возвращает false</summary>
    [TestMethod]
    public void Convert_OneFalse_ReturnsFalse()
    {
        IMultiValueConverter converter = new And();
        object[] values = [true, false, true];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result, "Хотя бы одно false должно вернуть false");
    }

    /// <summary>Проверка что все false возвращает false</summary>
    [TestMethod]
    public void Convert_AllFalse_ReturnsFalse()
    {
        IMultiValueConverter converter = new And();
        object[] values = [false, false, false];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result, "Все false должны вернуть false");
    }

    /// <summary>Проверка пустого массива с NullDefaultValue=false</summary>
    [TestMethod]
    public void Convert_EmptyArray_ReturnsNullDefaultValue()
    {
        IMultiValueConverter converter = new And { NullDefaultValue = false };
        object[] values = [];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result, "Пустой массив должен вернуть NullDefaultValue");
    }

    /// <summary>Проверка пустого массива с NullDefaultValue=true</summary>
    [TestMethod]
    public void Convert_EmptyArray_WithNullDefaultTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new And { NullDefaultValue = true };
        object[] values = [];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result, "Пустой массив должен вернуть NullDefaultValue=true");
    }

    /// <summary>Проверка null массива с NullDefaultValue=false</summary>
    [TestMethod]
    public void Convert_NullArray_ReturnsNullDefaultValue()
    {
        IMultiValueConverter converter = new And { NullDefaultValue = false };
        
        var result = converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result, "null массив должен вернуть NullDefaultValue");
    }

    /// <summary>Проверка одного значения true</summary>
    [TestMethod]
    public void Convert_SingleTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new And();
        object[] values = [true];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result, "Одно значение true должно вернуть true");
    }

    /// <summary>Проверка одного значения false</summary>
    [TestMethod]
    public void Convert_SingleFalse_ReturnsFalse()
    {
        IMultiValueConverter converter = new And();
        object[] values = [false];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result, "Одно значение false должно вернуть false");
    }

    /// <summary>Проверка большого количества true значений</summary>
    [TestMethod]
    public void Convert_ManyTrueValues_ReturnsTrue()
    {
        IMultiValueConverter converter = new And();
        object[] values = Enumerable.Repeat((object)true, 100).ToArray();
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result, "Много true значений должны вернуть true");
    }

    /// <summary>Проверка что false в конце массива возвращает false</summary>
    [TestMethod]
    public void Convert_FalseAtEnd_ReturnsFalse()
    {
        IMultiValueConverter converter = new And();
        object[] values = [true, true, true, false];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result, "false в конце должно вернуть false");
    }

    /// <summary>Проверка что false в начале массива возвращает false</summary>
    [TestMethod]
    public void Convert_FalseAtStart_ReturnsFalse()
    {
        IMultiValueConverter converter = new And();
        object[] values = [false, true, true, true];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result, "false в начале должно вернуть false");
    }

    /// <summary>Проверка двух значений true</summary>
    [TestMethod]
    public void Convert_TwoTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new And();
        object[] values = [true, true];
        
        var result = converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result, "Два true должны вернуть true");
    }

    /// <summary>Проверка свойства NullDefaultValue по умолчанию</summary>
    [TestMethod]
    public void DefaultNullDefaultValue_IsFalse()
    {
        var converter = new And();
        
        Assert.IsFalse(converter.NullDefaultValue, "NullDefaultValue по умолчанию должно быть false");
    }
}
