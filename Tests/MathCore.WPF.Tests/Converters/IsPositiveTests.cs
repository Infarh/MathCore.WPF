using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера проверки на положительность IsPositive</summary>
[TestClass]
public class IsPositiveTests
{
    /// <summary>Проверка положительного числа возвращает true</summary>
    [TestMethod]
    public void Convert_PositiveNumber_ReturnsTrue()
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(42.0, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result!, "Положительное число должно возвращать true");
    }

    /// <summary>Проверка отрицательного числа возвращает false</summary>
    [TestMethod]
    public void Convert_NegativeNumber_ReturnsFalse()
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(-42.0, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, "Отрицательное число должно возвращать false");
    }

    /// <summary>Проверка нуля возвращает false</summary>
    [TestMethod]
    public void Convert_Zero_ReturnsFalse()
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, "Ноль должен возвращать false");
    }

    /// <summary>Проверка NaN возвращает null</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNull()
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsNull(result, "NaN должен возвращать null");
    }

    /// <summary>Проверка положительной бесконечности возвращает true</summary>
    [TestMethod]
    public void Convert_PositiveInfinity_ReturnsTrue()
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(double.PositiveInfinity, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result!, "Положительная бесконечность должна возвращать true");
    }

    /// <summary>Проверка отрицательной бесконечности возвращает false</summary>
    [TestMethod]
    public void Convert_NegativeInfinity_ReturnsFalse()
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(double.NegativeInfinity, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, "Отрицательная бесконечность должна возвращать false");
    }

    /// <summary>Проверка различных положительных значений</summary>
    [DataTestMethod]
    [DataRow(0.001)]
    [DataRow(1.0)]
    [DataRow(3.14159)]
    [DataRow(100.0)]
    [DataRow(1e10)]
    public void Convert_VariousPositiveValues_ReturnsTrue(double value)
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result!, $"Положительное значение {value} должно возвращать true");
    }

    /// <summary>Проверка различных отрицательных значений</summary>
    [DataTestMethod]
    [DataRow(-0.001)]
    [DataRow(-1.0)]
    [DataRow(-3.14159)]
    [DataRow(-100.0)]
    [DataRow(-1e10)]
    public void Convert_VariousNegativeValues_ReturnsFalse(double value)
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, $"Отрицательное значение {value} должно возвращать false");
    }

    /// <summary>Проверка очень малого положительного числа (Epsilon)</summary>
    [TestMethod]
    public void Convert_Epsilon_ReturnsTrue()
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(double.Epsilon, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result!, "Epsilon (минимальное положительное) должен возвращать true");
    }

    /// <summary>Проверка максимального значения double</summary>
    [TestMethod]
    public void Convert_MaxValue_ReturnsTrue()
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(double.MaxValue, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsTrue((bool)result!, "MaxValue должен возвращать true");
    }

    /// <summary>Проверка минимального значения double</summary>
    [TestMethod]
    public void Convert_MinValue_ReturnsFalse()
    {
        IValueConverter converter = new IsPositive();
        var result = converter.Convert(double.MinValue, typeof(bool), null, CultureInfo.InvariantCulture);
        
        Assert.IsFalse((bool)result!, "MinValue должен возвращать false");
    }
}
