using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class DivideTests
{
    /// <summary>Проверка деления на положительное число</summary>
    [TestMethod]
    public void Convert_DivideByPositive_ReturnsCorrectQuotient()
    {
        var converter = new Divide { K = 5.0 };

        var result = ((IValueConverter)converter).Convert(50.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(10.0);
    }

    /// <summary>Проверка деления на отрицательное число</summary>
    [TestMethod]
    public void Convert_DivideByNegative_ReturnsCorrectQuotient()
    {
        var converter = new Divide { K = -2.0 };

        var result = ((IValueConverter)converter).Convert(10.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(-5.0);
    }

    /// <summary>Проверка деления на единицу</summary>
    [TestMethod]
    public void Convert_DivideByOne_ReturnsOriginalValue()
    {
        var converter = new Divide { K = 1.0 };

        var result = ((IValueConverter)converter).Convert(42.5, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(42.5);
    }

    /// <summary>Проверка обратного преобразования</summary>
    [TestMethod]
    public void ConvertBack_MultipliesCorrectly()
    {
        var converter = new Divide { K = 5.0 };

        var result = ((IValueConverter)converter).ConvertBack(10.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(50.0);
    }

    /// <summary>Проверка двустороннего преобразования</summary>
    [TestMethod]
    public void Convert_ThenConvertBack_ReturnsOriginal()
    {
        var converter = new Divide { K = 3.5 };
        var original = 100.0;

        var converted = (double)((IValueConverter)converter).Convert(original, typeof(double), null, CultureInfo.InvariantCulture)!;
        var back = ((IValueConverter)converter).ConvertBack(converted, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(back).IsEqual(original, 1e-10);
    }

    /// <summary>Проверка деления нуля</summary>
    [TestMethod]
    public void Convert_DivideZero_ReturnsZero()
    {
        var converter = new Divide { K = 5.0 };

        var result = ((IValueConverter)converter).Convert(0.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(0.0);
    }

    /// <summary>Проверка деления на дробное число</summary>
    [TestMethod]
    public void Convert_DivideByFraction_WorksCorrectly()
    {
        var converter = new Divide { K = 0.5 };

        var result = ((IValueConverter)converter).Convert(50.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(100.0);
    }

    /// <summary>Проверка с очень малыми делителями</summary>
    [TestMethod]
    public void Convert_SmallDivisor_WorksCorrectly()
    {
        var converter = new Divide { K = 0.01 };

        var result = ((IValueConverter)converter).Convert(1.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(100.0, 1e-10);
    }

    /// <summary>Проверка деления отрицательных чисел</summary>
    [TestMethod]
    public void Convert_NegativeValues_WorksCorrectly()
    {
        var converter = new Divide { K = -5.0 };

        var result = ((IValueConverter)converter).Convert(-50.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(10.0);
    }
}
