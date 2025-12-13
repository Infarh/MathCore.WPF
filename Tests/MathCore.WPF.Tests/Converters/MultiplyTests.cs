using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class MultiplyTests
{
    /// <summary>Проверка умножения на положительное число</summary>
    [TestMethod]
    public void Convert_MultiplyPositive_ReturnsCorrectProduct()
    {
        var converter = new Multiply { K = 5.0 };

        var result = ((IValueConverter)converter).Convert(10.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(50.0);
    }

    /// <summary>Проверка умножения на отрицательное число</summary>
    [TestMethod]
    public void Convert_MultiplyNegative_ReturnsCorrectProduct()
    {
        var converter = new Multiply { K = -2.0 };

        var result = ((IValueConverter)converter).Convert(10.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(-20.0);
    }

    /// <summary>Проверка умножения на единицу</summary>
    [TestMethod]
    public void Convert_MultiplyByOne_ReturnsOriginalValue()
    {
        var converter = new Multiply { K = 1.0 };

        var result = ((IValueConverter)converter).Convert(42.5, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(42.5);
    }

    /// <summary>Проверка умножения на ноль</summary>
    [TestMethod]
    public void Convert_MultiplyByZero_ReturnsZero()
    {
        var converter = new Multiply { K = 0.0 };

        var result = ((IValueConverter)converter).Convert(42.5, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(0.0);
    }

    /// <summary>Проверка обратного преобразования</summary>
    [TestMethod]
    public void ConvertBack_DividesCorrectly()
    {
        var converter = new Multiply { K = 5.0 };

        var result = ((IValueConverter)converter).ConvertBack(50.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(10.0);
    }

    /// <summary>Проверка двустороннего преобразования</summary>
    [TestMethod]
    public void Convert_ThenConvertBack_ReturnsOriginal()
    {
        var converter = new Multiply { K = 3.5 };
        var original = 100.0;

        var converted = (double)((IValueConverter)converter).Convert(original, typeof(double), null, CultureInfo.InvariantCulture)!;
        var back = ((IValueConverter)converter).ConvertBack(converted, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(back).IsEqual(original, 1e-10);
    }

    /// <summary>Проверка умножения дробного числа</summary>
    [TestMethod]
    public void Convert_MultiplyFraction_WorksCorrectly()
    {
        var converter = new Multiply { K = 0.5 };

        var result = ((IValueConverter)converter).Convert(100.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(50.0);
    }

    /// <summary>Проверка с очень большими числами</summary>
    [TestMethod]
    public void Convert_LargeNumbers_WorksCorrectly()
    {
        var converter = new Multiply { K = 1e5 };

        var result = ((IValueConverter)converter).Convert(1e5, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(1e10, 1e-5);
    }

    /// <summary>Проверка умножения отрицательных чисел</summary>
    [TestMethod]
    public void Convert_NegativeValues_WorksCorrectly()
    {
        var converter = new Multiply { K = -3.0 };

        var result = ((IValueConverter)converter).Convert(-10.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(30.0);
    }
}
