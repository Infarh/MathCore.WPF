using System.Globalization;
using System.Windows;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера NaN в Visibility NaNtoVisibility</summary>
[TestClass]
public class NaNtoVisibilityTests
{
    /// <summary>Проверка NaN возвращает Hidden в нормальном режиме</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsHidden()
    {
        IValueConverter converter = new NaNtoVisibility();
        var result = converter.Convert(double.NaN, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Hidden, result, "NaN должен возвращать Hidden");
    }

    /// <summary>Проверка обычного числа возвращает Visible</summary>
    [TestMethod]
    public void Convert_RegularNumber_ReturnsVisible()
    {
        IValueConverter converter = new NaNtoVisibility();
        var result = converter.Convert(42.0, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Visible, result, "Обычное число должно возвращать Visible");
    }

    /// <summary>Проверка NaN с Collapsed=true возвращает Collapsed</summary>
    [TestMethod]
    public void Convert_NaN_WithCollapsed_ReturnsCollapsed()
    {
        IValueConverter converter = new NaNtoVisibility { Collapsed = true };
        var result = converter.Convert(double.NaN, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Collapsed, result, "NaN с Collapsed=true должен возвращать Collapsed");
    }

    /// <summary>Проверка null возвращает null</summary>
    [TestMethod]
    public void Convert_Null_ReturnsNull()
    {
        IValueConverter converter = new NaNtoVisibility();
        var result = converter.Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.IsNull(result, "null должен возвращать null");
    }

    /// <summary>Проверка NaN в инвертированном режиме возвращает Visible</summary>
    [TestMethod]
    public void Convert_NaN_WithInverted_ReturnsVisible()
    {
        IValueConverter converter = new NaNtoVisibility { Inverted = true };
        var result = converter.Convert(double.NaN, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Visible, result, "NaN с Inverted=true должен возвращать Visible");
    }

    /// <summary>Проверка обычного числа в инвертированном режиме возвращает Hidden</summary>
    [TestMethod]
    public void Convert_RegularNumber_WithInverted_ReturnsHidden()
    {
        IValueConverter converter = new NaNtoVisibility { Inverted = true };
        var result = converter.Convert(42.0, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Hidden, result, "Обычное число с Inverted=true должно возвращать Hidden");
    }

    /// <summary>Проверка обычного числа с Inverted и Collapsed возвращает Collapsed</summary>
    [TestMethod]
    public void Convert_RegularNumber_WithInvertedAndCollapsed_ReturnsCollapsed()
    {
        IValueConverter converter = new NaNtoVisibility { Inverted = true, Collapsed = true };
        var result = converter.Convert(3.14, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Collapsed, result, "Обычное число с Inverted и Collapsed должно возвращать Collapsed");
    }

    /// <summary>Проверка различных валидных чисел возвращают Visible</summary>
    [DataTestMethod]
    [DataRow(0.0)]
    [DataRow(-1.0)]
    [DataRow(3.14159)]
    [DataRow(1e10)]
    public void Convert_VariousValidNumbers_ReturnsVisible(double value)
    {
        IValueConverter converter = new NaNtoVisibility();
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Visible, result, $"Валидное число {value} должно возвращать Visible");
    }

    /// <summary>Проверка положительной бесконечности возвращает Visible</summary>
    [TestMethod]
    public void Convert_PositiveInfinity_ReturnsVisible()
    {
        IValueConverter converter = new NaNtoVisibility();
        var result = converter.Convert(double.PositiveInfinity, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Visible, result, "Положительная бесконечность должна возвращать Visible");
    }

    /// <summary>Проверка отрицательной бесконечности возвращает Visible</summary>
    [TestMethod]
    public void Convert_NegativeInfinity_ReturnsVisible()
    {
        IValueConverter converter = new NaNtoVisibility();
        var result = converter.Convert(double.NegativeInfinity, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Visible, result, "Отрицательная бесконечность должна возвращать Visible");
    }

    /// <summary>Проверка комбинации всех свойств</summary>
    [TestMethod]
    public void Convert_NaN_WithInvertedAndCollapsed_ReturnsVisible()
    {
        IValueConverter converter = new NaNtoVisibility { Inverted = true, Collapsed = true };
        var result = converter.Convert(double.NaN, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Visible, result, "NaN с Inverted должен возвращать Visible независимо от Collapsed");
    }

    /// <summary>Проверка свойств по умолчанию</summary>
    [TestMethod]
    public void DefaultProperties_AreFalse()
    {
        var converter = new NaNtoVisibility();
        
        Assert.IsFalse(converter.Inverted, "Inverted по умолчанию должен быть false");
        Assert.IsFalse(converter.Collapsed, "Collapsed по умолчанию должен быть false");
    }

    /// <summary>Проверка нуля возвращает Visible</summary>
    [TestMethod]
    public void Convert_Zero_ReturnsVisible()
    {
        IValueConverter converter = new NaNtoVisibility();
        var result = converter.Convert(0.0, typeof(Visibility), null, CultureInfo.InvariantCulture);
        
        Assert.AreEqual(Visibility.Visible, result, "Ноль должен возвращать Visible");
    }
}
