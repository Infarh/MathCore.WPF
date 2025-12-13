using MathCore.WPF.Converters;
using System.Windows.Data;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера Round</summary>
[TestClass]
public class RoundTests
{
    [TestMethod]
    [DataRow(3.14159, 0, 3.0)]
    [DataRow(3.14159, 2, 3.14)]
    [DataRow(3.14159, 4, 3.1416)]
    [DataRow(2.5, 0, 2.0)] // MidpointRounding.ToEven (default)
    [DataRow(3.5, 0, 4.0)]
    public void Convert_RoundsToSpecifiedDigits(double input, int digits, double expected)
    {
        // Arrange
        var converter = new Round(digits) as IValueConverter;

        // Act
        var result = converter.Convert(input, typeof(double), null, null);

        // Assert
        Assert.AreEqual(expected, (double)result, 1e-10, $"Округление {input} до {digits} знаков должно давать {expected}");
    }

    [TestMethod]
    public void Convert_WithKParameter_ScalesBeforeRounding()
    {
        // Arrange
        var round = new Round(0) { K = 10.0 };
        var converter = round as IValueConverter;

        // Act
        var result = converter.Convert(3.14, typeof(double), null, null);

        // Assert
        // 3.14 * 10 = 31.4 -> Round(31.4) = 31 -> 31 / 10 = 3.1
        Assert.AreEqual(3.1, (double)result, 1e-10, "K должен масштабировать перед округлением");
    }

    [TestMethod]
    public void Convert_WithNegativeValue_RoundsCorrectly()
    {
        // Arrange
        var converter = new Round(2) as IValueConverter;

        // Act
        var result = converter.Convert(-3.14159, typeof(double), null, null);

        // Assert
        Assert.AreEqual(-3.14, (double)result, 1e-10, "Округление отрицательных чисел должно работать корректно");
    }

    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        // Arrange
        var converter = new Round(2) as IValueConverter;

        // Act & Assert
        // ConvertBack должен бросать NotSupportedException, так как информация о потерянных разрядах не восстанавливается
        try
        {
            converter.ConvertBack(3.14, typeof(double), null, null);
            Assert.Fail("Ожидалось исключение NotSupportedException");
        }
        catch (NotSupportedException)
        {
            // Ожидаемое поведение
        }
    }
}
