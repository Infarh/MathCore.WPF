using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class SubtractionTests
{
    /// <summary>Проверка вычитания положительного числа</summary>
    [TestMethod]
    public void Convert_SubtractPositive_ReturnsCorrectDifference()
    {
        var converter = new Subtraction { P = 10.0 };

        var result = ((IValueConverter)converter).Convert(20.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(10.0);
    }

    /// <summary>Проверка вычитания отрицательного числа</summary>
    [TestMethod]
    public void Convert_SubtractNegative_ReturnsCorrectDifference()
    {
        var converter = new Subtraction { P = -10.0 };

        var result = ((IValueConverter)converter).Convert(5.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(15.0);
    }

    /// <summary>Проверка вычитания нуля</summary>
    [TestMethod]
    public void Convert_SubtractZero_ReturnsOriginalValue()
    {
        var converter = new Subtraction { P = 0.0 };

        var result = ((IValueConverter)converter).Convert(42.5, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(42.5);
    }

    /// <summary>Проверка обратного преобразования</summary>
    [TestMethod]
    public void ConvertBack_AddsCorrectly()
    {
        var converter = new Subtraction { P = 10.0 };

        var result = ((IValueConverter)converter).ConvertBack(5.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(15.0);
    }

    /// <summary>Проверка двустороннего преобразования</summary>
    [TestMethod]
    public void Convert_ThenConvertBack_ReturnsOriginal()
    {
        var converter = new Subtraction { P = 25.5 };
        var original = 100.0;

        var converted = (double)((IValueConverter)converter).Convert(original, typeof(double), null, CultureInfo.InvariantCulture)!;
        var back = ((IValueConverter)converter).ConvertBack(converted, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(back).IsEqual(original, 1e-10);
    }

    /// <summary>Проверка вычитания с получением отрицательного результата</summary>
    [TestMethod]
    public void Convert_ResultNegative_WorksCorrectly()
    {
        var converter = new Subtraction { P = 20.0 };

        var result = ((IValueConverter)converter).Convert(10.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(-10.0);
    }

    /// <summary>Проверка с отрицательными значениями</summary>
    [TestMethod]
    public void Convert_NegativeValues_WorksCorrectly()
    {
        var converter = new Subtraction { P = -5.0 };

        var result = ((IValueConverter)converter).Convert(-10.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(-5.0);
    }

    /// <summary>Проверка с большими числами</summary>
    [TestMethod]
    public void Convert_LargeNumbers_WorksCorrectly()
    {
        var converter = new Subtraction { P = 1e10 };

        var result = ((IValueConverter)converter).Convert(2e10, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(1e10, 1e-5);
    }
}
