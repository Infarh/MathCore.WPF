using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для комбинирующих конвертеров</summary>
[TestClass]
public class CombinationConvertersTests
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    #region Combine Tests

    /// <summary>Тест конвертера Combine - последовательное применение двух конвертеров</summary>
    [TestMethod]
    public void Combine_TwoConverters_AppliesSequentially()
    {
        var addition = new Addition(5.0);    // +5
        var multiply = new Multiply(2.0);    // *2
        IValueConverter converter = new Combine(addition, multiply); // (x + 5) * 2
        
        var result = (double)converter.Convert(10.0, typeof(double), null, Culture)!;
        
        Assert.That.Value(result).IsEqual(30.0); // (10 + 5) * 2 = 30
    }

    /// <summary>Тест конвертера Combine - обратное преобразование</summary>
    [TestMethod]
    public void Combine_ConvertBack_ReverseOrder()
    {
        var addition = new Addition(5.0);    // +5
        var multiply = new Multiply(2.0);    // *2
        IValueConverter converter = new Combine(addition, multiply); // (x + 5) * 2
        
        var result = (double)converter.ConvertBack(30.0, typeof(double), null, Culture)!;
        
        Assert.That.Value(result).IsEqual(10.0); // Обратно: (30 / 2) - 5 = 10
    }

    /// <summary>Тест конвертера Combine - комбинация арифметических операций</summary>
    [TestMethod]
    public void Combine_ArithmeticOperations_ChainCorrectly()
    {
        var linear = new Linear(3.0, 2.0);   // 3x + 2
        var divide = new Divide(2.0);        // /2
        IValueConverter converter = new Combine(linear, divide); // (3x + 2) / 2
        
        var result = (double)converter.Convert(4.0, typeof(double), null, Culture)!;
        
        Assert.That.Value(result).IsEqual(7.0); // (3*4 + 2) / 2 = 14 / 2 = 7
    }

    /// <summary>Тест конвертера Combine - комбинация с логическим конвертером</summary>
    [TestMethod]
    public void Combine_WithComparison_WorksCorrectly()
    {
        var addition = new Addition(10.0);
        var greaterThan = new GreaterThan(20.0);
        IValueConverter converter = new Combine(addition, greaterThan); // (x + 10) > 20
        
        var result_true = (bool)converter.Convert(15.0, typeof(bool), null, Culture)!;  // 15 + 10 = 25 > 20
        var result_false = (bool)converter.Convert(5.0, typeof(bool), null, Culture)!;  // 5 + 10 = 15 < 20
        
        Assert.IsTrue(result_true, "25 > 20 должно вернуть true");
        Assert.IsFalse(result_false, "15 > 20 должно вернуть false");
    }

    /// <summary>Тест конвертера Combine - тройная комбинация</summary>
    [TestMethod]
    public void Combine_ThreeConverters_ChainsCorrectly()
    {
        var add = new Addition(5.0);         // +5
        var multiply = new Multiply(2.0);    // *2
        var combine1 = new Combine(add, multiply);       // (x + 5) * 2
        var subtract = new Subtraction(10.0);            // -10
        IValueConverter combine2 = new Combine(combine1, subtract);  // ((x + 5) * 2) - 10
        
        var result = (double)combine2.Convert(10.0, typeof(double), null, Culture)!;
        
        Assert.That.Value(result).IsEqual(20.0); // ((10 + 5) * 2) - 10 = 30 - 10 = 20
    }

    #endregion

    #region CombineMulti Tests

    /// <summary>Тест конвертера CombineMulti - комбинация multi-конвертеров</summary>
    [TestMethod]
    public void CombineMulti_TwoMultiConverters_WorksCorrectly()
    {
        IMultiValueConverter additionMulti = new AdditionMulti();     // Сумма
        IValueConverter greaterThan = new GreaterThan { Value = 50.0 }; // > 50
        IMultiValueConverter converter = new CombineMulti(additionMulti, greaterThan);
        
        var result_true = (bool)converter.Convert([30.0, 25.0], typeof(bool), null, Culture)!;  // 30 + 25 = 55 > 50
        var result_false = (bool)converter.Convert([20.0, 25.0], typeof(bool), null, Culture)!; // 20 + 25 = 45 < 50
        
        Assert.IsTrue(result_true, "55 > 50 должно вернуть true");
        Assert.IsFalse(result_false, "45 > 50 должно вернуть false");
    }

    /// <summary>Тест конвертера CombineMulti - агрегация и сравнение</summary>
    [TestMethod]
    public void CombineMulti_AggregationAndComparison_ChainsCorrectly()
    {
        IMultiValueConverter average = new AverageMulti();
        IValueConverter inRange = new InRange(10.0, 30.0) { MinInclude = true, MaxInclude = true };
        IMultiValueConverter converter = new CombineMulti(average, inRange);
        
        var result_true = (bool)converter.Convert([10.0, 20.0, 30.0], typeof(bool), null, Culture)!;  // Среднее 20 в [10, 30]
        var result_false = (bool)converter.Convert([5.0, 10.0, 15.0], typeof(bool), null, Culture)!;  // Среднее 10 в [10, 30]
        
        Assert.IsTrue(result_true, "Среднее 20 в диапазоне [10, 30] должно вернуть true");
        Assert.IsTrue(result_false, "Среднее 10 на границе диапазона должно вернуть true");
    }

    #endregion

    #region Composite Tests

    /// <summary>Тест конвертера Composite - композитный конвертер с несколькими преобразованиями</summary>
    [TestMethod]
    public void Composite_MultipleSteps_AppliesAll()
    {
        var composite = new Composite();
        composite.Converters.Add(new Addition(10.0));
        composite.Converters.Add(new Multiply(2.0));
        composite.Converters.Add(new Subtraction(5.0));
        
        IValueConverter converter = composite;
        var result = (double)converter.Convert(5.0, typeof(double), null, Culture)!;
        
        Debug.WriteLine($"Composite результат: {result}");
        // (5 + 10) * 2 - 5 = 15 * 2 - 5 = 30 - 5 = 25
        Assert.That.Value(result).IsEqual(25.0);
    }

    #endregion
}
