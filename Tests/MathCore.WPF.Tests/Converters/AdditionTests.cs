using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class AdditionTests
{
    /// <summary>Проверка сложения с положительным числом</summary>
    [TestMethod]
    public void Convert_AddPositive_ReturnsCorrectSum()
    {
        var converter = new Addition { P = 10.0 };

        var result = ((IValueConverter)converter).Convert(5.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(15.0);
    }

    /// <summary>Проверка сложения с отрицательным числом</summary>
    [TestMethod]
    public void Convert_AddNegative_ReturnsCorrectSum()
    {
        var converter = new Addition { P = -10.0 };

        var result = ((IValueConverter)converter).Convert(20.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(10.0);
    }

    /// <summary>Проверка сложения с нулем</summary>
    [TestMethod]
    public void Convert_AddZero_ReturnsOriginalValue()
    {
        var converter = new Addition { P = 0.0 };

        var result = ((IValueConverter)converter).Convert(42.5, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(42.5);
    }

    /// <summary>Проверка обратного преобразования</summary>
    [TestMethod]
    public void ConvertBack_SubtractsCorrectly()
    {
        var converter = new Addition { P = 10.0 };

        var result = ((IValueConverter)converter).ConvertBack(15.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(5.0);
    }

    /// <summary>Проверка двустороннего преобразования</summary>
    [TestMethod]
    public void Convert_ThenConvertBack_ReturnsOriginal()
    {
        var converter = new Addition { P = 25.5 };
        var original = 100.0;

        var converted = (double)((IValueConverter)converter).Convert(original, typeof(double), null, CultureInfo.InvariantCulture)!;
        var back = ((IValueConverter)converter).ConvertBack(converted, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(back).IsEqual(original, 1e-10);
    }

    /// <summary>Проверка с очень большими числами</summary>
    [TestMethod]
    public void Convert_LargeNumbers_WorksCorrectly()
    {
        var converter = new Addition { P = 1e10 };

        var result = ((IValueConverter)converter).Convert(1e10, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(2e10, 1e-5);
    }

    /// <summary>Проверка с очень малыми числами</summary>
    [TestMethod]
    public void Convert_SmallNumbers_WorksCorrectly()
    {
        var converter = new Addition { P = double.Epsilon };

        var result = ((IValueConverter)converter).Convert(1.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(1.0 + double.Epsilon);
    }

    /// <summary>Проверка сложения с отрицательными значениями</summary>
    [TestMethod]
    public void Convert_NegativeValues_WorksCorrectly()
    {
        var converter = new Addition { P = -5.0 };

        var result = ((IValueConverter)converter).Convert(-10.0, typeof(double), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(-15.0);
    }
}
