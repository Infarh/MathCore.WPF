using MathCore.WPF.Converters;
using System.Windows.Data;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера Sin</summary>
[TestClass]
public class SinTests
{
    [TestMethod]
    public void Convert_WithZero_ReturnsZero()
    {
        // Arrange
        var converter = new Sin() as IValueConverter;

        // Act
        var result = converter.Convert(0.0, typeof(double), null, null);

        // Assert
        Assert.AreEqual(0.0, (double)result, 1e-10, "sin(0) должен быть 0");
    }

    [TestMethod]
    public void Convert_WithPiOver2_ReturnsOne()
    {
        // Arrange
        var sin = new Sin { W = 1.0 };
        var converter = sin as IValueConverter;

        // Act
        var result = converter.Convert(Math.PI / 2, typeof(double), null, null);

        // Assert
        Assert.AreEqual(1.0, (double)result, 1e-10, "sin(π/2) должен быть 1");
    }

    [TestMethod]
    public void Convert_WithKParameter_ScalesResult()
    {
        // Arrange
        var sin = new Sin { K = 2.0, W = 1.0 };
        var converter = sin as IValueConverter;

        // Act
        var result = converter.Convert(Math.PI / 2, typeof(double), null, null);

        // Assert
        Assert.AreEqual(2.0, (double)result, 1e-10, "K * sin(x) должен масштабировать результат");
    }

    [TestMethod]
    public void Convert_WithBParameter_OffsetsResult()
    {
        // Arrange
        var sin = new Sin { B = 3.0, W = 1.0 };
        var converter = sin as IValueConverter;

        // Act
        var result = converter.Convert(0.0, typeof(double), null, null);

        // Assert
        Assert.AreEqual(3.0, (double)result, 1e-10, "B должен смещать результат");
    }

    [TestMethod]
    public void Convert_WithNaN_ReturnsNaN()
    {
        // Arrange
        var converter = new Sin() as IValueConverter;

        // Act
        var result = converter.Convert(double.NaN, typeof(double), null, null);

        // Assert
        Assert.IsTrue(double.IsNaN((double)result), "sin(NaN) должен возвращать NaN");
    }

    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        // Arrange
        var converter = new Sin() as IValueConverter;

        // Act & Assert
        // ConvertBack должен бросать NotSupportedException, так как синус не имеет однозначного обратного преобразования
        try
        {
            converter.ConvertBack(0.5, typeof(double), null, null);
            Assert.Fail("Ожидалось исключение NotSupportedException");
        }
        catch (NotSupportedException)
        {
            // Ожидаемое поведение
        }
    }
}
