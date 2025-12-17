using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертеров сравнения значений</summary>
[TestClass]
public class ComparisonConvertersTests
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    #region GreaterThan Tests

    /// <summary>Тест конвертера GreaterThan - значение больше порога</summary>
    [TestMethod]
    public void GreaterThan_ValueGreater_ReturnsTrue()
    {
        IValueConverter converter = new GreaterThan(3.14);
        var result = (bool)converter.Convert(5.0, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "5 > 3.14 должно вернуть true");
    }

    /// <summary>Тест конвертера GreaterThan - значение меньше порога</summary>
    [TestMethod]
    public void GreaterThan_ValueLess_ReturnsFalse()
    {
        IValueConverter converter = new GreaterThan(3.14);
        var result = (bool)converter.Convert(2.0, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "2 > 3.14 должно вернуть false");
    }

    /// <summary>Тест конвертера GreaterThan - значение равно порогу</summary>
    [TestMethod]
    public void GreaterThan_ValueEqual_ReturnsFalse()
    {
        IValueConverter converter = new GreaterThan(3.14);
        var result = (bool)converter.Convert(3.14, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "3.14 > 3.14 должно вернуть false");
    }

    #endregion

    #region GreaterThanMulti Tests

    /// <summary>Тест конвертера GreaterThanMulti - все последующие значения меньше первого</summary>
    [TestMethod]
    public void GreaterThanMulti_AllLess_ReturnsTrue()
    {
        IMultiValueConverter converter = new GreaterThanMulti();
        var result = (bool)converter.Convert([10.0, 5.0, 3.0, 8.0], typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Все значения < 10 должно вернуть true");
    }

    /// <summary>Тест конвертера GreaterThanMulti - одно значение больше или равно первому</summary>
    [TestMethod]
    public void GreaterThanMulti_OneGreaterOrEqual_ReturnsFalse()
    {
        IMultiValueConverter converter = new GreaterThanMulti();
        var result = (bool)converter.Convert([10.0, 5.0, 12.0, 8.0], typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Одно значение >= 10 должно вернуть false");
    }

    #endregion

    #region GreaterThanOrEqual Tests

    /// <summary>Тест конвертера GreaterThanOrEqual - значение больше порога</summary>
    [TestMethod]
    public void GreaterThanOrEqual_ValueGreater_ReturnsTrue()
    {
        IValueConverter converter = new GreaterThanOrEqual(5.0);
        var result = (bool)converter.Convert(10.0, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "10 >= 5 должно вернуть true");
    }

    /// <summary>Тест конвертера GreaterThanOrEqual - значение равно порогу</summary>
    [TestMethod]
    public void GreaterThanOrEqual_ValueEqual_ReturnsTrue()
    {
        IValueConverter converter = new GreaterThanOrEqual(5.0);
        var result = (bool)converter.Convert(5.0, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "5 >= 5 должно вернуть true");
    }

    /// <summary>Тест конвертера GreaterThanOrEqual - значение меньше порога</summary>
    [TestMethod]
    public void GreaterThanOrEqual_ValueLess_ReturnsFalse()
    {
        IValueConverter converter = new GreaterThanOrEqual(5.0);
        var result = (bool)converter.Convert(3.0, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "3 >= 5 должно вернуть false");
    }

    #endregion

    #region GreaterOrEqualThanMulti Tests

    /// <summary>Тест конвертера GreaterOrEqualThanMulti - все последующие значения не больше первого</summary>
    [TestMethod]
    public void GreaterOrEqualThanMulti_AllLessOrEqual_ReturnsTrue()
    {
        IMultiValueConverter converter = new GreaterOrEqualThanMulti();
        var result = (bool)converter.Convert([10.0, 5.0, 10.0, 8.0], typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Все значения <= 10 должно вернуть true");
    }

    /// <summary>Тест конвертера GreaterOrEqualThanMulti - одно значение больше первого</summary>
    [TestMethod]
    public void GreaterOrEqualThanMulti_OneGreater_ReturnsFalse()
    {
        IMultiValueConverter converter = new GreaterOrEqualThanMulti();
        var result = (bool)converter.Convert([10.0, 5.0, 11.0, 8.0], typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Одно значение > 10 должно вернуть false");
    }

    #endregion

    #region LessThan Tests

    /// <summary>Тест конвертера LessThan - значение меньше порога</summary>
    [TestMethod]
    public void LessThan_ValueLess_ReturnsTrue()
    {
        IValueConverter converter = new LessThan(100.0);
        var result = (bool)converter.Convert(50.0, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "50 < 100 должно вернуть true");
    }

    /// <summary>Тест конвертера LessThan - значение больше порога</summary>
    [TestMethod]
    public void LessThan_ValueGreater_ReturnsFalse()
    {
        IValueConverter converter = new LessThan(100.0);
        var result = (bool)converter.Convert(150.0, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "150 < 100 должно вернуть false");
    }

    /// <summary>Тест конвертера LessThan - значение равно порогу</summary>
    [TestMethod]
    public void LessThan_ValueEqual_ReturnsFalse()
    {
        IValueConverter converter = new LessThan(100.0);
        var result = (bool)converter.Convert(100.0, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "100 < 100 должно вернуть false");
    }

    #endregion

    #region LessThanMulti Tests

    /// <summary>Тест конвертера LessThanMulti - все последующие значения больше первого</summary>
    [TestMethod]
    public void LessThanMulti_AllGreater_ReturnsTrue()
    {
        IMultiValueConverter converter = new LessThanMulti();
        var result = (bool)converter.Convert([100.0, 150.0, 200.0, 125.0], typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Все значения > 100 должно вернуть true");
    }

    /// <summary>Тест конвертера LessThanMulti - одно значение меньше или равно первому</summary>
    [TestMethod]
    public void LessThanMulti_OneLessOrEqual_ReturnsFalse()
    {
        IMultiValueConverter converter = new LessThanMulti();
        var result = (bool)converter.Convert([100.0, 150.0, 50.0, 125.0], typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Одно значение <= 100 должно вернуть false");
    }

    #endregion

    #region LessThanOrEqual Tests

    /// <summary>Тест конвертера LessThanOrEqual - значение меньше порога</summary>
    [TestMethod]
    public void LessThanOrEqual_ValueLess_ReturnsTrue()
    {
        IValueConverter converter = new LessThanOrEqual(50.0);
        var result = (bool)converter.Convert(30.0, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "30 <= 50 должно вернуть true");
    }

    /// <summary>Тест конвертера LessThanOrEqual - значение равно порогу</summary>
    [TestMethod]
    public void LessThanOrEqual_ValueEqual_ReturnsTrue()
    {
        IValueConverter converter = new LessThanOrEqual(50.0);
        var result = (bool)converter.Convert(50.0, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "50 <= 50 должно вернуть true");
    }

    /// <summary>Тест конвертера LessThanOrEqual - значение больше порога</summary>
    [TestMethod]
    public void LessThanOrEqual_ValueGreater_ReturnsFalse()
    {
        IValueConverter converter = new LessThanOrEqual(50.0);
        var result = (bool)converter.Convert(75.0, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "75 <= 50 должно вернуть false");
    }

    #endregion

    #region LessOrEqualThanMulti Tests

    /// <summary>Тест конвертера LessOrEqualThanMulti - все последующие значения больше или равны первому</summary>
    [TestMethod]
    public void LessOrEqualThanMulti_AllGreaterOrEqual_ReturnsTrue()
    {
        IMultiValueConverter converter = new LessOrEqualThanMulti();
        var result = (bool)converter.Convert([100.0, 150.0, 100.0, 125.0], typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Все значения >= 100 должно вернуть true");
    }

    /// <summary>Тест конвертера LessOrEqualThanMulti - одно значение меньше первого</summary>
    [TestMethod]
    public void LessOrEqualThanMulti_OneLess_ReturnsFalse()
    {
        IMultiValueConverter converter = new LessOrEqualThanMulti();
        var result = (bool)converter.Convert([100.0, 150.0, 99.9, 125.0], typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Одно значение < 100 должно вернуть false");
    }

    #endregion
}
