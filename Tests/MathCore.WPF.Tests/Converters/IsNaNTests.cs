using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера проверки на NaN IsNaN</summary>
[TestClass]
public class IsNaNTests
{
    /// <summary>Проверка NaN возвращает true в нормальном режиме</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsTrue()
    {
        IValueConverter converter = new IsNaN();
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result!, "NaN должен возвращать true");
    }

    /// <summary>Проверка обычного числа возвращает false</summary>
    [TestMethod]
    public void Convert_RegularNumber_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN();
        var result = converter.Convert(42.0, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, "Обычное число должно возвращать false");
    }

    /// <summary>Проверка различных числовых значений</summary>
    [DataTestMethod]
    [DataRow(0.0)]
    [DataRow(-1.0)]
    [DataRow(3.14159)]
    [DataRow(1e10)]
    [DataRow(-999.999)]
    public void Convert_VariousNumbers_ReturnsFalse(double value)
    {
        IValueConverter converter = new IsNaN();
        var result = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, $"Число {value} не является NaN, должно вернуть false");
    }

    /// <summary>Проверка положительной бесконечности</summary>
    [TestMethod]
    public void Convert_PositiveInfinity_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN();
        var result = converter.Convert(double.PositiveInfinity, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, "Положительная бесконечность не является NaN");
    }

    /// <summary>Проверка отрицательной бесконечности</summary>
    [TestMethod]
    public void Convert_NegativeInfinity_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN();
        var result = converter.Convert(double.NegativeInfinity, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, "Отрицательная бесконечность не является NaN");
    }

    /// <summary>Проверка NaN в инвертированном режиме возвращает false</summary>
    [TestMethod]
    public void Convert_NaN_WithInverted_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN(Inverted: true);
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, "NaN с Inverted=true должен возвращать false");
    }

    /// <summary>Проверка обычного числа в инвертированном режиме возвращает true</summary>
    [TestMethod]
    public void Convert_RegularNumber_WithInverted_ReturnsTrue()
    {
        IValueConverter converter = new IsNaN(Inverted: true);
        var result = converter.Convert(42.0, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result!, "Обычное число с Inverted=true должно возвращать true");
    }

    /// <summary>Проверка свойства Inverted через конструктор</summary>
    [TestMethod]
    public void Constructor_WithInvertedParameter_SetsProperty()
    {
        var converter = new IsNaN(Inverted: true);
        
        Assert.IsTrue(converter.Inverted, "Свойство Inverted должно быть установлено через конструктор");
    }

    /// <summary>Проверка изменения свойства Inverted после создания</summary>
    [TestMethod]
    public void InvertedProperty_CanBeChanged()
    {
        var converter = new IsNaN { Inverted = true };
        IValueConverter converter_interface = converter;
        
        var result = converter_interface.Convert(3.14, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result!, "Изменение Inverted должно влиять на результат");
    }

    /// <summary>Проверка нуля</summary>
    [TestMethod]
    public void Convert_Zero_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN();
        var result = converter.Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, "0.0 не является NaN");
    }

    /// <summary>Проверка минимального и максимального значений double</summary>
    [DataTestMethod]
    [DataRow(double.MinValue)]
    [DataRow(double.MaxValue)]
    [DataRow(double.Epsilon)]
    public void Convert_ExtremeDoubleValues_ReturnsFalse(double value)
    {
        IValueConverter converter = new IsNaN();
        var result = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, $"Экстремальное значение {value} не является NaN");
    }
}
