using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертеров проверки значений</summary>
[TestClass]
public class ValidationConvertersTests
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    #region IsNaN Tests

    /// <summary>Тест конвертера IsNaN - значение NaN</summary>
    [TestMethod]
    public void IsNaN_NaNValue_ReturnsTrue()
    {
        IValueConverter converter = new IsNaN();
        var result = (bool)converter.Convert(double.NaN, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "NaN должен вернуть true");
    }

    /// <summary>Тест конвертера IsNaN - нормальное значение</summary>
    [TestMethod]
    public void IsNaN_NormalValue_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN();
        var result = (bool)converter.Convert(5.0, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Нормальное значение должно вернуть false");
    }

    /// <summary>Тест конвертера IsNaN - инвертированный режим с NaN</summary>
    [TestMethod]
    public void IsNaN_Inverted_NaNValue_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN { Inverted = true };
        var result = (bool)converter.Convert(double.NaN, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "NaN с инверсией должен вернуть false");
    }

    /// <summary>Тест конвертера IsNaN - инвертированный режим с нормальным значением</summary>
    [TestMethod]
    public void IsNaN_Inverted_NormalValue_ReturnsTrue()
    {
        IValueConverter converter = new IsNaN { Inverted = true };
        var result = (bool)converter.Convert(5.0, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Нормальное значение с инверсией должно вернуть true");
    }

    /// <summary>Тест конвертера IsNaN - бесконечность не является NaN</summary>
    [TestMethod]
    public void IsNaN_Infinity_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN();
        var result = (bool)converter.Convert(double.PositiveInfinity, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Бесконечность не является NaN");
    }

    #endregion

    #region IsNull Tests

    /// <summary>Тест конвертера IsNull - null значение</summary>
    [TestMethod]
    public void IsNull_NullValue_ReturnsTrue()
    {
        IValueConverter converter = new IsNull();
        var result = (bool)converter.Convert(null, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "null должен вернуть true");
    }

    /// <summary>Тест конвертера IsNull - не null значение</summary>
    [TestMethod]
    public void IsNull_NotNullValue_ReturnsFalse()
    {
        IValueConverter converter = new IsNull();
        var result = (bool)converter.Convert("test", typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Не null значение должно вернуть false");
    }

    /// <summary>Тест конвертера IsNull - инвертированный режим с null</summary>
    [TestMethod]
    public void IsNull_Inverted_NullValue_ReturnsFalse()
    {
        IValueConverter converter = new IsNull { Inverted = true };
        var result = (bool)converter.Convert(null, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "null с инверсией должен вернуть false");
    }

    /// <summary>Тест конвертера IsNull - инвертированный режим с не null</summary>
    [TestMethod]
    public void IsNull_Inverted_NotNullValue_ReturnsTrue()
    {
        IValueConverter converter = new IsNull { Inverted = true };
        var result = (bool)converter.Convert("test", typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Не null с инверсией должен вернуть true");
    }

    /// <summary>Тест конвертера IsNull - пустая строка не является null</summary>
    [TestMethod]
    public void IsNull_EmptyString_ReturnsFalse()
    {
        IValueConverter converter = new IsNull();
        var result = (bool)converter.Convert("", typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Пустая строка не является null");
    }

    #endregion

    #region IsPositive Tests

    /// <summary>Тест конвертера IsPositive - положительное значение</summary>
    [TestMethod]
    public void IsPositive_PositiveValue_ReturnsTrue()
    {
        IValueConverter converter = new IsPositive();
        var result = (bool)converter.Convert(5.0, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Положительное значение должно вернуть true");
    }

    /// <summary>Тест конвертера IsPositive - отрицательное значение</summary>
    [TestMethod]
    public void IsPositive_NegativeValue_ReturnsFalse()
    {
        IValueConverter converter = new IsPositive();
        var result = (bool)converter.Convert(-5.0, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Отрицательное значение должно вернуть false");
    }

    /// <summary>Тест конвертера IsPositive - нулевое значение</summary>
    [TestMethod]
    public void IsPositive_Zero_ReturnsFalse()
    {
        IValueConverter converter = new IsPositive();
        var result = (bool)converter.Convert(0.0, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Ноль должен вернуть false");
    }

    /// <summary>Тест конвертера IsPositive - малое положительное значение</summary>
    [TestMethod]
    public void IsPositive_SmallPositive_ReturnsTrue()
    {
        IValueConverter converter = new IsPositive();
        var result = (bool)converter.Convert(0.001, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Малое положительное значение должно вернуть true");
    }

    #endregion

    #region IsNegative Tests

    /// <summary>Тест конвертера IsNegative - отрицательное значение</summary>
    [TestMethod]
    public void IsNegative_NegativeValue_ReturnsTrue()
    {
        IValueConverter converter = new IsNegative();
        var result = (bool)converter.Convert(-5.0, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Отрицательное значение должно вернуть true");
    }

    /// <summary>Тест конвертера IsNegative - положительное значение</summary>
    [TestMethod]
    public void IsNegative_PositiveValue_ReturnsFalse()
    {
        IValueConverter converter = new IsNegative();
        var result = (bool)converter.Convert(5.0, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Положительное значение должно вернуть false");
    }

    /// <summary>Тест конвертера IsNegative - нулевое значение</summary>
    [TestMethod]
    public void IsNegative_Zero_ReturnsFalse()
    {
        IValueConverter converter = new IsNegative();
        var result = (bool)converter.Convert(0.0, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Ноль должен вернуть false");
    }

    /// <summary>Тест конвертера IsNegative - малое отрицательное значение</summary>
    [TestMethod]
    public void IsNegative_SmallNegative_ReturnsTrue()
    {
        IValueConverter converter = new IsNegative();
        var result = (bool)converter.Convert(-0.001, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Малое отрицательное значение должно вернуть true");
    }

    #endregion
}
