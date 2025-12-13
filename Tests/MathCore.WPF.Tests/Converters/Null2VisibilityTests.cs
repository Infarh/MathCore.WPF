using System.Globalization;
using System.Windows;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера null в Visibility Null2Visibility</summary>
[TestClass]
public class Null2VisibilityTests
{
    /// <summary>Проверка null возвращает Visible в нормальном режиме</summary>
    [TestMethod]
    public void Convert_Null_ReturnsVisible()
    {
        IValueConverter converter = new Null2Visibility();
        var result = converter.Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Visible, result, "null должен возвращать Visible");
    }

    /// <summary>Проверка не-null возвращает Hidden</summary>
    [TestMethod]
    public void Convert_NotNull_ReturnsHidden()
    {
        IValueConverter converter = new Null2Visibility();
        var result = converter.Convert("test", typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Hidden, result, "не-null должен возвращать Hidden");
    }

    /// <summary>Проверка не-null с Collapsed=true возвращает Collapsed</summary>
    [TestMethod]
    public void Convert_NotNull_WithCollapsed_ReturnsCollapsed()
    {
        IValueConverter converter = new Null2Visibility { Collapsed = true };
        var result = converter.Convert("test", typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Collapsed, result, "не-null с Collapsed=true должен возвращать Collapsed");
    }

    /// <summary>Проверка null в инвертированном режиме возвращает Hidden</summary>
    [TestMethod]
    public void Convert_Null_WithInverted_ReturnsHidden()
    {
        IValueConverter converter = new Null2Visibility { Inverted = true };
        var result = converter.Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Hidden, result, "null с Inverted=true должен возвращать Hidden");
    }

    /// <summary>Проверка null в инвертированном режиме с Collapsed возвращает Collapsed</summary>
    [TestMethod]
    public void Convert_Null_WithInvertedAndCollapsed_ReturnsCollapsed()
    {
        IValueConverter converter = new Null2Visibility { Inverted = true, Collapsed = true };
        var result = converter.Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Collapsed, result, "null с Inverted и Collapsed должен возвращать Collapsed");
    }

    /// <summary>Проверка не-null в инвертированном режиме возвращает Visible</summary>
    [TestMethod]
    public void Convert_NotNull_WithInverted_ReturnsVisible()
    {
        IValueConverter converter = new Null2Visibility { Inverted = true };
        var result = converter.Convert(42, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Visible, result, "не-null с Inverted=true должен возвращать Visible");
    }

    /// <summary>Проверка различных не-null типов</summary>
    [DataTestMethod]
    [DataRow(42)]
    [DataRow("string")]
    [DataRow(3.14)]
    [DataRow(true)]
    public void Convert_VariousNotNullValues_ReturnsHidden(object value)
    {
        IValueConverter converter = new Null2Visibility();
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Hidden, result, $"не-null значение {value} должно возвращать Hidden");
    }

    /// <summary>Проверка пустой строки (не является null)</summary>
    [TestMethod]
    public void Convert_EmptyString_ReturnsHidden()
    {
        IValueConverter converter = new Null2Visibility();
        var result = converter.Convert(string.Empty, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Hidden, result, "Пустая строка не является null, должна вернуть Hidden");
    }

    /// <summary>Проверка нуля (не является null)</summary>
    [TestMethod]
    public void Convert_Zero_ReturnsHidden()
    {
        IValueConverter converter = new Null2Visibility();
        var result = converter.Convert(0, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Hidden, result, "0 не является null, должен вернуть Hidden");
    }

    /// <summary>Проверка пустого массива (не является null)</summary>
    [TestMethod]
    public void Convert_EmptyArray_ReturnsHidden()
    {
        IValueConverter converter = new Null2Visibility();
        var result = converter.Convert(Array.Empty<int>(), typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Hidden, result, "Пустой массив не является null, должен вернуть Hidden");
    }

    /// <summary>Проверка свойств по умолчанию</summary>
    [TestMethod]
    public void DefaultProperties_AreFalse()
    {
        var converter = new Null2Visibility();
        
        Assert.IsFalse(converter.Inverted, "Inverted по умолчанию должен быть false");
        Assert.IsFalse(converter.Collapsed, "Collapsed по умолчанию должен быть false");
    }

    /// <summary>Проверка комбинации всех свойств с не-null</summary>
    [TestMethod]
    public void Convert_NotNull_WithInvertedAndCollapsed_ReturnsVisible()
    {
        IValueConverter converter = new Null2Visibility { Inverted = true, Collapsed = true };
        var result = converter.Convert("data", typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Visible, result, "не-null с Inverted должен возвращать Visible независимо от Collapsed");
    }

    /// <summary>Проверка объекта с типом значения</summary>
    [TestMethod]
    public void Convert_ValueTypeObject_ReturnsHidden()
    {
        IValueConverter converter = new Null2Visibility();
        var result = converter.Convert(DateTime.Now, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Hidden, result, "Объект типа значения должен возвращать Hidden");
    }
}
