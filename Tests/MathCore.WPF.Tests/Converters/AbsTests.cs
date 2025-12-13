using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера Abs</summary>
[TestClass]
public class AbsTests
{
    [TestMethod]
    [DataRow(5.0, 5.0)]
    [DataRow(-5.0, 5.0)]
    [DataRow(0.0, 0.0)]
    [DataRow(3.14, 3.14)]
    [DataRow(-3.14, 3.14)]
    public void Convert_ReturnsAbsoluteValue(double input, double expected)
    {
        // Arrange
        var converter = new Abs();

        // Act
        var result = (converter as System.Windows.Data.IValueConverter).Convert(input, typeof(double), null, null);

        // Assert
        Assert.AreEqual(expected, result, "Abs должен возвращать абсолютное значение");
    }

    [TestMethod]
    public void Convert_WithNaN_ReturnsNaN()
    {
        // Arrange
        var converter = new Abs();

        // Act
        var result = (converter as System.Windows.Data.IValueConverter).Convert(double.NaN, typeof(double), null, null);

        // Assert
        Assert.IsTrue(double.IsNaN((double)result), "Abs от NaN должен возвращать NaN");
    }

    [TestMethod]
    public void Convert_WithPositiveInfinity_ReturnsPositiveInfinity()
    {
        // Arrange
        var converter = new Abs();

        // Act
        var result = (converter as System.Windows.Data.IValueConverter).Convert(double.PositiveInfinity, typeof(double), null, null);

        // Assert
        Assert.AreEqual(double.PositiveInfinity, result, "Abs от +∞ должен возвращать +∞");
    }

    [TestMethod]
    public void Convert_WithNegativeInfinity_ReturnsPositiveInfinity()
    {
        // Arrange
        var converter = new Abs();

        // Act
        var result = (converter as System.Windows.Data.IValueConverter).Convert(double.NegativeInfinity, typeof(double), null, null);

        // Assert
        Assert.AreEqual(double.PositiveInfinity, result, "Abs от -∞ должен возвращать +∞");
    }

    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        // Arrange
        var converter = new Abs() as System.Windows.Data.IValueConverter;

        // Act & Assert
        // ConvertBack должен бросать NotSupportedException, так как обратное преобразование модуля неоднозначно
        try
        {
            converter.ConvertBack(5.0, typeof(double), null, null);
            Assert.Fail("Ожидалось исключение NotSupportedException");
        }
        catch (NotSupportedException)
        {
            // Ожидаемое поведение
        }
    }
}
