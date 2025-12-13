using System.Globalization;
using System.Windows;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class NANtoVisibilityTests
{
    /// <summary>Проверка что NaN возвращает Hidden по умолчанию</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsHidden()
    {
        var converter = new NaNtoVisibility();

        var result = ((IValueConverter)converter).Convert(double.NaN, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Hidden);
    }

    /// <summary>Проверка что обычное число возвращает Visible</summary>
    [TestMethod]
    public void Convert_RegularNumber_ReturnsVisible()
    {
        var converter = new NaNtoVisibility();

        var result = ((IValueConverter)converter).Convert(42.5, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Visible);
    }

    /// <summary>Проверка что NaN возвращает Collapsed с флагом Collapsed</summary>
    [TestMethod]
    public void Convert_NaN_Collapsed_ReturnsCollapsed()
    {
        var converter = new NaNtoVisibility { Collapsed = true };

        var result = ((IValueConverter)converter).Convert(double.NaN, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Collapsed);
    }

    /// <summary>Проверка инвертированного режима для NaN</summary>
    [TestMethod]
    public void Convert_NaN_Inverted_ReturnsVisible()
    {
        var converter = new NaNtoVisibility { Inverted = true };

        var result = ((IValueConverter)converter).Convert(double.NaN, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Visible);
    }

    /// <summary>Проверка инвертированного режима для обычного числа</summary>
    [TestMethod]
    public void Convert_RegularNumber_Inverted_ReturnsHidden()
    {
        var converter = new NaNtoVisibility { Inverted = true };

        var result = ((IValueConverter)converter).Convert(123.456, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Hidden);
    }

    /// <summary>Проверка инвертированного режима с Collapsed</summary>
    [TestMethod]
    public void Convert_RegularNumber_InvertedCollapsed_ReturnsCollapsed()
    {
        var converter = new NaNtoVisibility { Inverted = true, Collapsed = true };

        var result = ((IValueConverter)converter).Convert(999.0, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Collapsed);
    }

    /// <summary>Проверка что ноль возвращает Visible</summary>
    [TestMethod]
    public void Convert_Zero_ReturnsVisible()
    {
        var converter = new NaNtoVisibility();

        var result = ((IValueConverter)converter).Convert(0.0, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Visible);
    }

    /// <summary>Проверка что бесконечность возвращает Visible</summary>
    [TestMethod]
    public void Convert_Infinity_ReturnsVisible()
    {
        var converter = new NaNtoVisibility();

        var result = ((IValueConverter)converter).Convert(double.PositiveInfinity, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Visible);
    }

    /// <summary>Проверка что null возвращает null</summary>
    [TestMethod]
    public void Convert_Null_ReturnsNull()
    {
        var converter = new NaNtoVisibility();

        var result = ((IValueConverter)converter).Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.IsNull(result, "null должен возвращать null");
    }

    /// <summary>Проверка что ConvertBack выбрасывает NotSupportedException</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        var converter = new NaNtoVisibility();

        Assert.ThrowsException<NotSupportedException>(() =>
            ((IValueConverter)converter).ConvertBack(Visibility.Visible, typeof(double), null, CultureInfo.InvariantCulture),
            "ConvertBack не должен поддерживаться");
    }
}
