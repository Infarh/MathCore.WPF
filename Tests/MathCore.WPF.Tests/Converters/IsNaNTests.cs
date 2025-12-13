using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Модульные тесты для конвертера IsNaN</summary>
[TestClass]
public class IsNaNTests
{
    /// <summary>Тест проверки NaN значения</summary>
    [TestMethod]
    public void Convert_NaNValue_ReturnsTrue()
    {
        IValueConverter converter = new IsNaN();

        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест проверки нормального числа</summary>
    [TestMethod]
    public void Convert_NormalValue_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN();

        var result = converter.Convert(42.0, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест проверки нуля</summary>
    [TestMethod]
    public void Convert_ZeroValue_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN();

        var result = converter.Convert(0.0, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест проверки положительной бесконечности</summary>
    [TestMethod]
    public void Convert_PositiveInfinity_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN();

        var result = converter.Convert(double.PositiveInfinity, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false, "PositiveInfinity не является NaN");
    }

    /// <summary>Тест проверки отрицательной бесконечности</summary>
    [TestMethod]
    public void Convert_NegativeInfinity_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN();

        var result = converter.Convert(double.NegativeInfinity, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false, "NegativeInfinity не является NaN");
    }

    /// <summary>Тест инвертированного режима с NaN</summary>
    [TestMethod]
    public void Convert_NaNValueInverted_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN { Inverted = true };

        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест инвертированного режима с нормальным числом</summary>
    [TestMethod]
    public void Convert_NormalValueInverted_ReturnsTrue()
    {
        IValueConverter converter = new IsNaN { Inverted = true };

        var result = converter.Convert(42.0, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест проверки очень большого числа</summary>
    [TestMethod]
    public void Convert_VeryLargeValue_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN();

        var result = converter.Convert(double.MaxValue, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест проверки очень маленького числа</summary>
    [TestMethod]
    public void Convert_VerySmallValue_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN();

        var result = converter.Convert(double.MinValue, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест проверки отрицательного нуля</summary>
    [TestMethod]
    public void Convert_NegativeZero_ReturnsFalse()
    {
        IValueConverter converter = new IsNaN();

        var result = converter.Convert(-0.0, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест обратного преобразования - должно генерировать NotSupportedException</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        IValueConverter converter = new IsNaN();

        Assert.ThrowsException<NotSupportedException>(() =>
            converter.ConvertBack(true, typeof(double), null, CultureInfo.CurrentCulture),
            "ConvertBack не должен поддерживаться для IsNaN конвертера");
    }

    /// <summary>Тест конвертера как MarkupExtension</summary>
    [TestMethod]
    public void ProvideValue_ReturnsSelf()
    {
        var converter = new IsNaN();

        var result = converter.ProvideValue(null!);

        Assert.That.Value(result).IsReferenceSame(converter);
    }
}
