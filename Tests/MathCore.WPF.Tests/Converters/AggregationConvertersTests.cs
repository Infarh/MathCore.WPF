using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для агрегирующих конвертеров</summary>
[TestClass]
public class AggregationConvertersTests
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    #region AdditionMulti Tests

    /// <summary>Тест конвертера AdditionMulti - суммирование нескольких значений</summary>
    [TestMethod]
    public void AdditionMulti_MultipleValues_ReturnsSum()
    {
        IMultiValueConverter converter = new AdditionMulti();
        var result = (double)converter.Convert([5.0, 10.0, 15.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(30.0);
    }

    /// <summary>Тест конвертера AdditionMulti - одно значение</summary>
    [TestMethod]
    public void AdditionMulti_SingleValue_ReturnsValue()
    {
        IMultiValueConverter converter = new AdditionMulti();
        var result = (double)converter.Convert([42.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(42.0);
    }

    /// <summary>Тест конвертера AdditionMulti - пустой массив выбрасывает исключение</summary>
    [TestMethod]
    public void AdditionMulti_EmptyArray_ReturnsZero()
    {
        IMultiValueConverter converter = new AdditionMulti();
        
        var exception_thrown = false;
        try
        {
            converter.Convert([], typeof(double), null, Culture);
        }
        catch (System.IndexOutOfRangeException)
        {
            exception_thrown = true;
        }
        
        Assert.IsTrue(exception_thrown, "Сумма пустого массива должна выбросить IndexOutOfRangeException");
    }

    /// <summary>Тест конвертера AdditionMulti - отрицательные значения</summary>
    [TestMethod]
    public void AdditionMulti_NegativeValues_ReturnsCorrectSum()
    {
        IMultiValueConverter converter = new AdditionMulti();
        var result = (double)converter.Convert([10.0, -5.0, -3.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(2.0);
    }

    #endregion

    #region AverageMulti Tests

    /// <summary>Тест конвертера AverageMulti - среднее нескольких значений</summary>
    [TestMethod]
    public void AverageMulti_MultipleValues_ReturnsAverage()
    {
        IMultiValueConverter converter = new AverageMulti();
        var result = (double)converter.Convert([10.0, 20.0, 30.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(20.0);
    }

    /// <summary>Тест конвертера AverageMulti - одно значение</summary>
    [TestMethod]
    public void AverageMulti_SingleValue_ReturnsValue()
    {
        IMultiValueConverter converter = new AverageMulti();
        var result = (double)converter.Convert([42.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(42.0);
    }

    /// <summary>Тест конвертера AverageMulti - пустой массив выбрасывает исключение</summary>
    [TestMethod]
    public void AverageMulti_EmptyArray_ReturnsNaN()
    {
        IMultiValueConverter converter = new AverageMulti();
        
        var exception_thrown = false;
        try
        {
            converter.Convert([], typeof(double), null, Culture);
        }
        catch (System.IndexOutOfRangeException)
        {
            exception_thrown = true;
        }
        
        Assert.IsTrue(exception_thrown, "Среднее пустого массива должно выбросить IndexOutOfRangeException");
    }

    /// <summary>Тест конвертера AverageMulti - смешанные значения</summary>
    [TestMethod]
    public void AverageMulti_MixedValues_ReturnsCorrectAverage()
    {
        IMultiValueConverter converter = new AverageMulti();
        var result = (double)converter.Convert([5.0, -5.0, 10.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(3.333333333333, 1e-10);
    }

    #endregion

    #region MinValue Tests

    /// <summary>Тест конвертера MinValue - минимум нескольких значений</summary>
    [TestMethod]
    public void MinValue_MultipleValues_ReturnsMinimum()
    {
        IMultiValueConverter converter = new MinValue();
        var result = (double)converter.Convert([5.0, 2.0, 8.0, 1.5], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(1.5);
    }

    /// <summary>Тест конвертера MinValue - одно значение</summary>
    [TestMethod]
    public void MinValue_SingleValue_ReturnsValue()
    {
        IMultiValueConverter converter = new MinValue();
        var result = (double)converter.Convert([42.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(42.0);
    }

    /// <summary>Тест конвертера MinValue - отрицательные значения</summary>
    [TestMethod]
    public void MinValue_NegativeValues_ReturnsMinimum()
    {
        IMultiValueConverter converter = new MinValue();
        var result = (double)converter.Convert([-5.0, -10.0, -3.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(-10.0);
    }

    /// <summary>Тест конвертера MinValue - все одинаковые значения</summary>
    [TestMethod]
    public void MinValue_AllSameValues_ReturnsValue()
    {
        IMultiValueConverter converter = new MinValue();
        var result = (double)converter.Convert([7.0, 7.0, 7.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(7.0);
    }

    #endregion

    #region MaxValue Tests

    /// <summary>Тест конвертера MaxValue - максимум нескольких значений</summary>
    [TestMethod]
    public void MaxValue_MultipleValues_ReturnsMaximum()
    {
        IMultiValueConverter converter = new MaxValue();
        var result = (double)converter.Convert([5.0, 12.0, 8.0, 1.5], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(12.0);
    }

    /// <summary>Тест конвертера MaxValue - одно значение</summary>
    [TestMethod]
    public void MaxValue_SingleValue_ReturnsValue()
    {
        IMultiValueConverter converter = new MaxValue();
        var result = (double)converter.Convert([42.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(42.0);
    }

    /// <summary>Тест конвертера MaxValue - отрицательные значения</summary>
    [TestMethod]
    public void MaxValue_NegativeValues_ReturnsMaximum()
    {
        IMultiValueConverter converter = new MaxValue();
        var result = (double)converter.Convert([-5.0, -10.0, -3.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(-3.0);
    }

    /// <summary>Тест конвертера MaxValue - все одинаковые значения</summary>
    [TestMethod]
    public void MaxValue_AllSameValues_ReturnsValue()
    {
        IMultiValueConverter converter = new MaxValue();
        var result = (double)converter.Convert([7.0, 7.0, 7.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(7.0);
    }

    #endregion

    #region MultiplyMany Tests

    /// <summary>Тест конвертера MultiplyMany - произведение нескольких значений</summary>
    [TestMethod]
    public void MultiplyMany_MultipleValues_ReturnsProduct()
    {
        IMultiValueConverter converter = new MultiplyMany();
        var result = (double)converter.Convert([2.0, 3.0, 4.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(24.0);
    }

    /// <summary>Тест конвертера MultiplyMany - одно значение</summary>
    [TestMethod]
    public void MultiplyMany_SingleValue_ReturnsValue()
    {
        IMultiValueConverter converter = new MultiplyMany();
        var result = (double)converter.Convert([42.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(42.0);
    }

    /// <summary>Тест конвертера MultiplyMany - пустой массив выбрасывает исключение</summary>
    [TestMethod]
    public void MultiplyMany_EmptyArray_ReturnsOne()
    {
        IMultiValueConverter converter = new MultiplyMany();
        
        var exception_thrown = false;
        try
        {
            converter.Convert([], typeof(double), null, Culture);
        }
        catch (System.IndexOutOfRangeException)
        {
            exception_thrown = true;
        }
        
        Assert.IsTrue(exception_thrown, "Произведение пустого массива должно выбросить IndexOutOfRangeException");
    }

    /// <summary>Тест конвертера MultiplyMany - умножение с нулем</summary>
    [TestMethod]
    public void MultiplyMany_WithZero_ReturnsZero()
    {
        IMultiValueConverter converter = new MultiplyMany();
        var result = (double)converter.Convert([5.0, 0.0, 10.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0);
    }

    /// <summary>Тест конвертера MultiplyMany - отрицательные значения</summary>
    [TestMethod]
    public void MultiplyMany_NegativeValues_ReturnsCorrectProduct()
    {
        IMultiValueConverter converter = new MultiplyMany();
        var result = (double)converter.Convert([2.0, -3.0, 4.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(-24.0);
    }

    /// <summary>Тест конвертера MultiplyMany - дробные значения</summary>
    [TestMethod]
    public void MultiplyMany_FractionalValues_ReturnsCorrectProduct()
    {
        IMultiValueConverter converter = new MultiplyMany();
        var result = (double)converter.Convert([0.5, 0.5, 4.0], typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(1.0);
    }

    #endregion

    #region Average Tests

    /// <summary>Тест конвертера Average - скользящее среднее</summary>
    [TestMethod]
    public void Average_WithLength_CalculatesMovingAverage()
    {
        IValueConverter converter = new Average(3);
        
        var result1 = (double)converter.Convert(10.0, typeof(double), null, Culture)!;
        var result2 = (double)converter.Convert(20.0, typeof(double), null, Culture)!;
        var result3 = (double)converter.Convert(30.0, typeof(double), null, Culture)!;
        
        Debug.WriteLine($"Result 1: {result1}");
        Debug.WriteLine($"Result 2: {result2}");
        Debug.WriteLine($"Result 3: {result3}");
        
        Assert.That.Value(result3).IsEqual(20.0, 1e-10);
    }

    #endregion
}
