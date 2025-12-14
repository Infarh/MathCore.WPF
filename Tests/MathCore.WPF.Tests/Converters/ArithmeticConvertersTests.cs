using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для арифметических конвертеров</summary>
[TestClass]
public class ArithmeticConvertersTests
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    #region Abs Tests

    /// <summary>Тест конвертера Abs - абсолютное значение положительного числа</summary>
    [TestMethod]
    public void Abs_PositiveValue_ReturnsValue()
    {
        IValueConverter converter = new Abs();
        var result = (double)converter.Convert(5.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(5.0);
    }

    /// <summary>Тест конвертера Abs - абсолютное значение отрицательного числа</summary>
    [TestMethod]
    public void Abs_NegativeValue_ReturnsAbsoluteValue()
    {
        IValueConverter converter = new Abs();
        var result = (double)converter.Convert(-5.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(5.0);
    }

    /// <summary>Тест конвертера Abs - нулевое значение</summary>
    [TestMethod]
    public void Abs_Zero_ReturnsZero()
    {
        IValueConverter converter = new Abs();
        var result = (double)converter.Convert(0.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0);
    }

    #endregion

    #region Addition Tests

    /// <summary>Тест конвертера Addition - прибавление значения</summary>
    [TestMethod]
    public void Addition_WithParameter_AddsValue()
    {
        IValueConverter converter = new Addition(5.0);
        var result = (double)converter.Convert(10.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(15.0);
    }

    /// <summary>Тест конвертера Addition - обратное преобразование</summary>
    [TestMethod]
    public void Addition_ConvertBack_SubtractsValue()
    {
        IValueConverter converter = new Addition(5.0);
        var result = (double)converter.ConvertBack(15.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(10.0);
    }

    /// <summary>Тест конвертера Addition - значение по умолчанию</summary>
    [TestMethod]
    public void Addition_DefaultConstructor_AddsZero()
    {
        IValueConverter converter = new Addition();
        var result = (double)converter.Convert(10.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(10.0);
    }

    #endregion

    #region Subtraction Tests

    /// <summary>Тест конвертера Subtraction - вычитание значения</summary>
    [TestMethod]
    public void Subtraction_WithParameter_SubtractsValue()
    {
        IValueConverter converter = new Subtraction(3.0);
        var result = (double)converter.Convert(10.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(7.0);
    }

    /// <summary>Тест конвертера Subtraction - обратное преобразование</summary>
    [TestMethod]
    public void Subtraction_ConvertBack_AddsValue()
    {
        IValueConverter converter = new Subtraction(3.0);
        var result = (double)converter.ConvertBack(7.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(10.0);
    }

    #endregion

    #region Multiply Tests

    /// <summary>Тест конвертера Multiply - умножение на значение</summary>
    [TestMethod]
    public void Multiply_WithParameter_MultipliesValue()
    {
        IValueConverter converter = new Multiply(2.0);
        var result = (double)converter.Convert(5.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(10.0);
    }

    /// <summary>Тест конвертера Multiply - обратное преобразование</summary>
    [TestMethod]
    public void Multiply_ConvertBack_DividesValue()
    {
        IValueConverter converter = new Multiply(2.0);
        var result = (double)converter.ConvertBack(10.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(5.0);
    }

    /// <summary>Тест конвертера Multiply - умножение на ноль</summary>
    [TestMethod]
    public void Multiply_ByZero_ReturnsZero()
    {
        IValueConverter converter = new Multiply(0.0);
        var result = (double)converter.Convert(5.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0);
    }

    #endregion

    #region Divide Tests

    /// <summary>Тест конвертера Divide - деление на значение</summary>
    [TestMethod]
    public void Divide_WithParameter_DividesValue()
    {
        IValueConverter converter = new Divide(10.0);
        var result = (double)converter.Convert(20.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(2.0);
    }

    /// <summary>Тест конвертера Divide - обратное преобразование</summary>
    [TestMethod]
    public void Divide_ConvertBack_MultipliesValue()
    {
        IValueConverter converter = new Divide(10.0);
        var result = (double)converter.ConvertBack(2.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(20.0);
    }

    /// <summary>Тест конвертера Divide - деление на ноль возвращает бесконечность</summary>
    [TestMethod]
    public void Divide_ByZero_ReturnsInfinity()
    {
        IValueConverter converter = new Divide(0.0);
        var result = (double)converter.Convert(10.0, typeof(double), null, Culture)!;
        Assert.IsTrue(double.IsInfinity(result), "Деление на ноль должно вернуть бесконечность");
    }

    #endregion

    #region Mod Tests

    /// <summary>Тест конвертера Mod - остаток от деления</summary>
    [TestMethod]
    public void Mod_WithParameter_ReturnsRemainder()
    {
        IValueConverter converter = new Mod(5.0);
        var result = (double)converter.Convert(13.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(3.0);
    }

    /// <summary>Тест конвертера Mod - остаток равен нулю</summary>
    [TestMethod]
    public void Mod_EvenDivision_ReturnsZero()
    {
        IValueConverter converter = new Mod(5.0);
        var result = (double)converter.Convert(15.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0);
    }

    #endregion

    #region Linear Tests

    /// <summary>Тест конвертера Linear - линейное преобразование</summary>
    [TestMethod]
    public void Linear_WithKAndB_CalculatesCorrectly()
    {
        IValueConverter converter = new Linear(3.0, 5.0); // f(x) = 3x + 5
        var result = (double)converter.Convert(2.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(11.0); // 3*2 + 5 = 11
    }

    /// <summary>Тест конвертера Linear - обратное преобразование</summary>
    [TestMethod]
    public void Linear_ConvertBack_ReversesCalculation()
    {
        IValueConverter converter = new Linear(3.0, 5.0); // f(x) = 3x + 5
        var result = (double)converter.ConvertBack(11.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(2.0); // (11 - 5) / 3 = 2
    }

    /// <summary>Тест конвертера Linear - только масштабирование</summary>
    [TestMethod]
    public void Linear_OnlyK_ScalesValue()
    {
        IValueConverter converter = new Linear(2.0, 0.0);
        var result = (double)converter.ConvertBack(10.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(5.0);
    }

    #endregion

    #region Inverse Tests

    /// <summary>Тест конвертера Inverse - обратное значение по умолчанию</summary>
    [TestMethod]
    public void Inverse_DefaultParameter_ReturnsReciprocal()
    {
        IValueConverter converter = new Inverse();
        var result = (double)converter.Convert(2.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.5);
    }

    /// <summary>Тест конвертера Inverse - с параметром</summary>
    [TestMethod]
    public void Inverse_WithParameter_CalculatesCorrectly()
    {
        IValueConverter converter = new Inverse();
        var result = (double)converter.Convert(4.0, typeof(double), 5.0, Culture)!;  // Передаем параметр 5 через Convert
        Assert.That.Value(result).IsEqual(1.25); // 5 / 4 = 1.25
    }

    #endregion

    #region Round Tests

    /// <summary>Тест конвертера Round - округление по умолчанию</summary>
    [TestMethod]
    public void Round_DefaultDigits_RoundsToInteger()
    {
        IValueConverter converter = new Round();
        var result = (double)converter.Convert(3.7, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(4.0);
    }

    /// <summary>Тест конвертера Round - округление до указанного количества знаков</summary>
    [TestMethod]
    public void Round_WithDigits_RoundsCorrectly()
    {
        IValueConverter converter = new Round(2);
        var result = (double)converter.Convert(3.14159, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(3.14);
    }

    /// <summary>Тест конвертера Round - округление вниз</summary>
    [TestMethod]
    public void Round_RoundsDown_WhenNeeded()
    {
        IValueConverter converter = new Round(1);
        var result = (double)converter.Convert(3.14, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(3.1);
    }

    #endregion

    #region Truncate Tests

    /// <summary>Тест конвертера Truncate - отбрасывание дробной части</summary>
    [TestMethod]
    public void Truncate_PositiveValue_RemovesDecimal()
    {
        IValueConverter converter = new Truncate();
        var result = (double)converter.Convert(3.99, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(3.0);
    }

    /// <summary>Тест конвертера Truncate - отрицательное значение</summary>
    [TestMethod]
    public void Truncate_NegativeValue_RemovesDecimal()
    {
        IValueConverter converter = new Truncate();
        var result = (double)converter.Convert(-3.99, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(-3.0);
    }

    /// <summary>Тест конвертера Truncate - целое число остается неизменным</summary>
    [TestMethod]
    public void Truncate_IntegerValue_RemainsUnchanged()
    {
        IValueConverter converter = new Truncate();
        var result = (double)converter.Convert(5.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(5.0);
    }

    #endregion
}
