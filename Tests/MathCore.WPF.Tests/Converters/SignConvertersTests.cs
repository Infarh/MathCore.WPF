using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертеров определения знака</summary>
[TestClass]
public class SignConvertersTests
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    #region Sign Tests

    /// <summary>Тест конвертера Sign - положительное значение</summary>
    [TestMethod]
    public void Sign_PositiveValue_ReturnsOne()
    {
        IValueConverter converter = new Sign();
        var result = (double)converter.Convert(5.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(1.0);
    }

    /// <summary>Тест конвертера Sign - отрицательное значение</summary>
    [TestMethod]
    public void Sign_NegativeValue_ReturnsMinusOne()
    {
        IValueConverter converter = new Sign();
        var result = (double)converter.Convert(-5.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(-1.0);
    }

    /// <summary>Тест конвертера Sign - нулевое значение</summary>
    [TestMethod]
    public void Sign_Zero_ReturnsZero()
    {
        IValueConverter converter = new Sign();
        var result = (double)converter.Convert(0.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0);
    }

    /// <summary>Тест конвертера Sign - очень малое положительное значение</summary>
    [TestMethod]
    public void Sign_SmallPositive_ReturnsOne()
    {
        IValueConverter converter = new Sign();
        var result = (double)converter.Convert(0.0001, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(1.0);
    }

    /// <summary>Тест конвертера Sign - очень малое отрицательное значение</summary>
    [TestMethod]
    public void Sign_SmallNegative_ReturnsMinusOne()
    {
        IValueConverter converter = new Sign();
        var result = (double)converter.Convert(-0.0001, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(-1.0);
    }

    #endregion

    #region SignValue Tests

    /// <summary>Тест конвертера SignValue - значение больше дельты</summary>
    [TestMethod]
    public void SignValue_AboveDelta_ReturnsOne()
    {
        IValueConverter converter = new SignValue { Delta = 5.0 };
        var result = (double)converter.Convert(10.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(1.0);
    }

    /// <summary>Тест конвертера SignValue - значение меньше отрицательной дельты</summary>
    [TestMethod]
    public void SignValue_BelowNegativeDelta_ReturnsMinusOne()
    {
        IValueConverter converter = new SignValue { Delta = 5.0 };
        var result = (double)converter.Convert(-10.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(-1.0);
    }

    /// <summary>Тест конвертера SignValue - значение в мёртвой зоне</summary>
    [TestMethod]
    public void SignValue_WithinDeadZone_ReturnsZero()
    {
        IValueConverter converter = new SignValue { Delta = 5.0 };
        var result = (double)converter.Convert(3.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0);
    }

    /// <summary>Тест конвертера SignValue - отрицательное значение в мёртвой зоне</summary>
    [TestMethod]
    public void SignValue_NegativeWithinDeadZone_ReturnsZero()
    {
        IValueConverter converter = new SignValue { Delta = 5.0 };
        var result = (double)converter.Convert(-3.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0);
    }

    /// <summary>Тест конвертера SignValue - граница мёртвой зоны положительная</summary>
    [TestMethod]
    public void SignValue_ExactlyAtPositiveDelta_ReturnsZero()
    {
        IValueConverter converter = new SignValue { Delta = 5.0 };
        var result = (double)converter.Convert(5.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0);
    }

    /// <summary>Тест конвертера SignValue - граница мёртвой зоны отрицательная</summary>
    [TestMethod]
    public void SignValue_ExactlyAtNegativeDelta_ReturnsZero()
    {
        IValueConverter converter = new SignValue { Delta = 5.0 };
        var result = (double)converter.Convert(-5.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0);
    }

    /// <summary>Тест конвертера SignValue - инверсия положительного значения</summary>
    [TestMethod]
    public void SignValue_Inverse_PositiveValue_ReturnsMinusOne()
    {
        IValueConverter converter = new SignValue { Delta = 5.0, Inverse = true };
        var result = (double)converter.Convert(10.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(-1.0);
    }

    /// <summary>Тест конвертера SignValue - инверсия отрицательного значения</summary>
    [TestMethod]
    public void SignValue_Inverse_NegativeValue_ReturnsOne()
    {
        IValueConverter converter = new SignValue { Delta = 5.0, Inverse = true };
        var result = (double)converter.Convert(-10.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(1.0);
    }

    /// <summary>Тест конвертера SignValue - инверсия не влияет на мёртвую зону</summary>
    [TestMethod]
    public void SignValue_Inverse_DeadZone_ReturnsZero()
    {
        IValueConverter converter = new SignValue { Delta = 5.0, Inverse = true };
        var result = (double)converter.Convert(3.0, typeof(double), null, Culture)!;
        Assert.That.Value(result).IsEqual(0.0);
    }

    /// <summary>Тест конвертера SignValue - дельта равна нулю ведёт себя как Sign</summary>
    [TestMethod]
    public void SignValue_ZeroDelta_BehavesLikeSign()
    {
        IValueConverter converter = new SignValue { Delta = 0.0 };
        
        var positive = (double)converter.Convert(5.0, typeof(double), null, Culture)!;
        var negative = (double)converter.Convert(-5.0, typeof(double), null, Culture)!;
        var zero = (double)converter.Convert(0.0, typeof(double), null, Culture)!;
        
        Assert.That.Value(positive).IsEqual(1.0);
        Assert.That.Value(negative).IsEqual(-1.0);
        Assert.That.Value(zero).IsEqual(0.0);
    }

    #endregion
}
