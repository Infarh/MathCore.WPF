using System.Globalization;
using System.Windows;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class Bool2VisibilityTests
{
    /// <summary>Проверка что true возвращает Visible</summary>
    [TestMethod]
    public void Convert_True_ReturnsVisible()
    {
        var converter = new Bool2Visibility();

        var result = ((IValueConverter)converter).Convert(true, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Visible);
    }

    /// <summary>Проверка что false возвращает Hidden</summary>
    [TestMethod]
    public void Convert_False_ReturnsHidden()
    {
        var converter = new Bool2Visibility();

        var result = ((IValueConverter)converter).Convert(false, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Hidden);
    }

    /// <summary>Проверка что false с Collapsed возвращает Collapsed</summary>
    [TestMethod]
    public void Convert_False_Collapsed_ReturnsCollapsed()
    {
        var converter = new Bool2Visibility { Collapsed = true };

        var result = ((IValueConverter)converter).Convert(false, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Collapsed);
    }

    /// <summary>Проверка инвертированного режима для true</summary>
    [TestMethod]
    public void Convert_True_Inverted_ReturnsHidden()
    {
        var converter = new Bool2Visibility { Inverted = true };

        var result = ((IValueConverter)converter).Convert(true, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Hidden);
    }

    /// <summary>Проверка инвертированного режима для false</summary>
    [TestMethod]
    public void Convert_False_Inverted_ReturnsVisible()
    {
        var converter = new Bool2Visibility { Inverted = true };

        var result = ((IValueConverter)converter).Convert(false, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Visible);
    }

    /// <summary>Проверка обратного преобразования Visible</summary>
    [TestMethod]
    public void ConvertBack_Visible_ReturnsTrue()
    {
        var converter = new Bool2Visibility();

        var result = ((IValueConverter)converter).ConvertBack(Visibility.Visible, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка обратного преобразования Hidden</summary>
    [TestMethod]
    public void ConvertBack_Hidden_ReturnsFalse()
    {
        var converter = new Bool2Visibility();

        var result = ((IValueConverter)converter).ConvertBack(Visibility.Hidden, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка обратного преобразования Collapsed</summary>
    [TestMethod]
    public void ConvertBack_Collapsed_ReturnsFalse()
    {
        var converter = new Bool2Visibility();

        var result = ((IValueConverter)converter).ConvertBack(Visibility.Collapsed, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка что null возвращает null</summary>
    [TestMethod]
    public void Convert_Null_ReturnsNull()
    {
        var converter = new Bool2Visibility();

        var result = ((IValueConverter)converter).Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.IsNull(result, "null должен возвращать null");
    }

    /// <summary>Проверка двустороннего преобразования</summary>
    [TestMethod]
    public void Convert_ThenConvertBack_ReturnsOriginal()
    {
        var converter = new Bool2Visibility();

        var converted = ((IValueConverter)converter).Convert(true, typeof(Visibility), null, CultureInfo.InvariantCulture);
        var back = ((IValueConverter)converter).ConvertBack(converted, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(back).IsEqual(true);
    }
}
