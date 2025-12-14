using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

using ConvertersRange = MathCore.WPF.Converters.Range;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертеров интервалов и диапазонов</summary>
[TestClass]
public class IntervalConvertersTests
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    #region InRange Tests

    /// <summary>Тест конвертера InRange - значение внутри диапазона</summary>
    [TestMethod]
    public void InRange_ValueInside_ReturnsTrue()
    {
        IValueConverter converter = new InRange(5.0, 10.0);
        var result = (bool)converter.Convert(7.0, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Значение 7 в диапазоне [5, 10] должно вернуть true");
    }

    /// <summary>Тест конвертера InRange - значение вне диапазона снизу</summary>
    [TestMethod]
    public void InRange_ValueBelowMin_ReturnsFalse()
    {
        IValueConverter converter = new InRange(5.0, 10.0);
        var result = (bool)converter.Convert(3.0, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Значение 3 вне диапазона [5, 10] должно вернуть false");
    }

    /// <summary>Тест конвертера InRange - значение вне диапазона сверху</summary>
    [TestMethod]
    public void InRange_ValueAboveMax_ReturnsFalse()
    {
        IValueConverter converter = new InRange(5.0, 10.0);
        var result = (bool)converter.Convert(12.0, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Значение 12 вне диапазона [5, 10] должно вернуть false");
    }

    /// <summary>Тест конвертера InRange - значение равно минимуму с включением границы</summary>
    [TestMethod]
    public void InRange_ValueEqualsMin_MinIncluded_ReturnsTrue()
    {
        var converter = new InRange(5.0, 10.0) { MinInclude = true };
        var result = (bool)((IValueConverter)converter).Convert(5.0, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Значение 5 с MinInclude=true должно вернуть true");
    }

    /// <summary>Тест конвертера InRange - значение равно максимуму с включением границы</summary>
    [TestMethod]
    public void InRange_ValueEqualsMax_MaxIncluded_ReturnsTrue()
    {
        var converter = new InRange(5.0, 10.0) { MaxInclude = true };
        var result = (bool)((IValueConverter)converter).Convert(10.0, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Значение 10 с MaxInclude=true должно вернуть true");
    }

    /// <summary>Тест конвертера InRange - значение равно минимуму без включения границы</summary>
    [TestMethod]
    public void InRange_ValueEqualsMin_MinExcluded_ReturnsFalse()
    {
        var converter = new InRange(5.0, 10.0) { MinInclude = false };
        var result = (bool)((IValueConverter)converter).Convert(5.0, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Значение 5 с MinInclude=false должно вернуть false");
    }

    #endregion

    #region OutRange Tests

    /// <summary>Тест конвертера OutRange - значение вне диапазона снизу</summary>
    [TestMethod]
    public void OutRange_ValueBelowMin_ReturnsTrue()
    {
        IValueConverter converter = new OutRange(5.0, 10.0);
        var result = (bool)converter.Convert(3.0, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Значение 3 вне диапазона [5, 10] должно вернуть true");
    }

    /// <summary>Тест конвертера OutRange - значение вне диапазона сверху</summary>
    [TestMethod]
    public void OutRange_ValueAboveMax_ReturnsTrue()
    {
        IValueConverter converter = new OutRange(5.0, 10.0);
        var result = (bool)converter.Convert(12.0, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Значение 12 вне диапазона [5, 10] должно вернуть true");
    }

    /// <summary>Тест конвертера OutRange - значение внутри диапазона</summary>
    [TestMethod]
    public void OutRange_ValueInside_ReturnsFalse()
    {
        IValueConverter converter = new OutRange(5.0, 10.0);
        var result = (bool)converter.Convert(7.0, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Значение 7 внутри диапазона [5, 10] должно вернуть false");
    }

    #endregion

    #region Range Tests

    /// <summary>Тест конвертера Range - значение внутри диапазона не изменяется</summary>
    [TestMethod]
    public void Range_ValueInside_RemainsUnchanged()
    {
        IValueConverter converter = new ConvertersRange(5.0, 10.0);
        var result = (double)converter.Convert(7.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(7.0);
    }

    /// <summary>Тест конвертера Range - значение ниже минимума ограничивается</summary>
    [TestMethod]
    public void Range_ValueBelowMin_ClampedToMin()
    {
        IValueConverter converter = new ConvertersRange(5.0, 10.0);
        var result = (double)converter.Convert(3.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(5.0);
    }

    /// <summary>Тест конвертера Range - значение выше максимума ограничивается</summary>
    [TestMethod]
    public void Range_ValueAboveMax_ClampedToMax()
    {
        IValueConverter converter = new ConvertersRange(5.0, 10.0);
        var result = (double)converter.Convert(15.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(10.0);
    }

    /// <summary>Тест конвертера Range - граничные значения</summary>
    [TestMethod]
    public void Range_BoundaryValues_RemainsUnchanged()
    {
        IValueConverter converter = new ConvertersRange(5.0, 10.0);
        
        var min_result = (double)converter.Convert(5.0, typeof(double), null, Culture)!;
        var max_result = (double)converter.Convert(10.0, typeof(double), null, Culture)!;
        
        Assert.That.Value(min_result).IsEqual(5.0);
        Assert.That.Value(max_result).IsEqual(10.0);
    }

    /// <summary>Тест конвертера Range - крайне малое значение</summary>
    [TestMethod]
    public void Range_VerySmallValue_ClampedToMin()
    {
        IValueConverter converter = new ConvertersRange(5.0, 10.0);
        var result = (double)converter.Convert(-1000.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(5.0);
    }

    /// <summary>Тест конвертера Range - крайне большое значение</summary>
    [TestMethod]
    public void Range_VeryLargeValue_ClampedToMax()
    {
        IValueConverter converter = new ConvertersRange(5.0, 10.0);
        var result = (double)converter.Convert(1000.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(10.0);
    }

    #endregion

    #region InIntervalValue Tests

    /// <summary>Тест конвертера InIntervalValue - ограничивает значение внутри интервала</summary>
    [TestMethod]
    public void InIntervalValue_ValueInside_ReturnsTrue()
    {
        IValueConverter converter = new InIntervalValue(5.0, 10.0);
        var result = (double)converter.Convert(7.0, typeof(double), null, Culture)!;
        
        Debug.WriteLine($"Нормализованное значение 7 в [5,10]: {result}");
        // Значение внутри интервала остается без изменений
        Assert.That.Value(result).IsEqual(7.0, 1e-10, "Значение 7 в интервале [5, 10] должно остаться 7");
    }

    /// <summary>Тест конвертера InIntervalValue - ограничивает значения вне интервала</summary>
    [TestMethod]
    public void InIntervalValue_ValueOutside_ReturnsFalse()
    {
        IValueConverter converter = new InIntervalValue(5.0, 10.0);
        var result_below = (double)converter.Convert(3.0, typeof(double), null, Culture)!;
        var result_above = (double)converter.Convert(12.0, typeof(double), null, Culture)!;
        
        Debug.WriteLine($"Нормализованное значение 3 (ниже [5,10]): {result_below}");
        Debug.WriteLine($"Нормализованное значение 12 (выше [5,10]): {result_above}");
        
        // Значения вне интервала ограничиваются границами
        Assert.That.Value(result_below).IsEqual(5.0, 1e-10, "Значение ниже минимума должно быть ограничено минимумом");
        Assert.That.Value(result_above).IsEqual(10.0, 1e-10, "Значение выше максимума должно быть ограничено максимумом");
    }

    #endregion
}
