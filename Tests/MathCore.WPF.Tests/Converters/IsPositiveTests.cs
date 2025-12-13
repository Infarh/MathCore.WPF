using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Модульные тесты для конвертера IsPositive</summary>
[TestClass]
public class IsPositiveTests
{
    /// <summary>Тест проверки положительного значения</summary>
    [TestMethod]
    public void Convert_PositiveValue_ReturnsTrue()
    {
        IValueConverter converter = new IsPositive();

        var result = converter.Convert(42.0, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест проверки отрицательного значения</summary>
    [TestMethod]
    public void Convert_NegativeValue_ReturnsFalse()
    {
        IValueConverter converter = new IsPositive();

        var result = converter.Convert(-42.0, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест проверки нуля</summary>
    [TestMethod]
    public void Convert_ZeroValue_ReturnsFalse()
    {
        IValueConverter converter = new IsPositive();

        var result = converter.Convert(0.0, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false, "Ноль не является положительным числом");
    }

    /// <summary>Тест проверки NaN значения</summary>
    [TestMethod]
    public void Convert_NaNValue_ReturnsNull()
    {
        IValueConverter converter = new IsPositive();

        var result = converter.Convert(double.NaN, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.IsNull(result, "NaN должен возвращать null");
    }

    /// <summary>Тест проверки положительной бесконечности</summary>
    [TestMethod]
    public void Convert_PositiveInfinity_ReturnsTrue()
    {
        IValueConverter converter = new IsPositive();

        var result = converter.Convert(double.PositiveInfinity, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест проверки отрицательной бесконечности</summary>
    [TestMethod]
    public void Convert_NegativeInfinity_ReturnsFalse()
    {
        IValueConverter converter = new IsPositive();

        var result = converter.Convert(double.NegativeInfinity, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест проверки очень маленького положительного числа</summary>
    [TestMethod]
    public void Convert_VerySmallPositiveValue_ReturnsTrue()
    {
        IValueConverter converter = new IsPositive();

        var result = converter.Convert(double.Epsilon, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест проверки очень маленького отрицательного числа</summary>
    [TestMethod]
    public void Convert_VerySmallNegativeValue_ReturnsFalse()
    {
        IValueConverter converter = new IsPositive();

        var result = converter.Convert(-double.Epsilon, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест проверки максимального значения</summary>
    [TestMethod]
    public void Convert_MaxValue_ReturnsTrue()
    {
        IValueConverter converter = new IsPositive();

        var result = converter.Convert(double.MaxValue, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест проверки минимального значения</summary>
    [TestMethod]
    public void Convert_MinValue_ReturnsFalse()
    {
        IValueConverter converter = new IsPositive();

        var result = converter.Convert(double.MinValue, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест обратного преобразования - должно генерировать NotSupportedException</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        IValueConverter converter = new IsPositive();

        Assert.ThrowsException<NotSupportedException>(() =>
            converter.ConvertBack(true, typeof(double), null, CultureInfo.CurrentCulture),
            "ConvertBack не должен поддерживаться для IsPositive конвертера");
    }

    /// <summary>Тест конвертера как MarkupExtension</summary>
    [TestMethod]
    public void ProvideValue_ReturnsSelf()
    {
        var converter = new IsPositive();

        var result = converter.ProvideValue(null!);

        Assert.That.Value(result).IsReferenceSame(converter);
    }
}
