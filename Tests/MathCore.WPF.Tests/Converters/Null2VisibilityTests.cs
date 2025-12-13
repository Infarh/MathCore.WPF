using System.Globalization;
using System.Windows;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class Null2VisibilityTests
{
    /// <summary>Проверка что null возвращает Visible по умолчанию</summary>
    [TestMethod]
    public void Convert_Null_ReturnsVisible()
    {
        var converter = new Null2Visibility();

        var result = ((IValueConverter)converter).Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Visible);
    }

    /// <summary>Проверка что не-null объект возвращает Hidden</summary>
    [TestMethod]
    public void Convert_NotNull_ReturnsHidden()
    {
        var converter = new Null2Visibility();

        var result = ((IValueConverter)converter).Convert("test", typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Hidden);
    }

    /// <summary>Проверка что не-null объект возвращает Collapsed с флагом Collapsed</summary>
    [TestMethod]
    public void Convert_NotNull_Collapsed_ReturnsCollapsed()
    {
        var converter = new Null2Visibility { Collapsed = true };

        var result = ((IValueConverter)converter).Convert(42, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Collapsed);
    }

    /// <summary>Проверка инвертированного режима для null</summary>
    [TestMethod]
    public void Convert_Null_Inverted_ReturnsHidden()
    {
        var converter = new Null2Visibility { Inverted = true };

        var result = ((IValueConverter)converter).Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Hidden);
    }

    /// <summary>Проверка инвертированного режима для не-null объекта</summary>
    [TestMethod]
    public void Convert_NotNull_Inverted_ReturnsVisible()
    {
        var converter = new Null2Visibility { Inverted = true };

        var result = ((IValueConverter)converter).Convert("data", typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Visible);
    }

    /// <summary>Проверка инвертированного режима с Collapsed для null</summary>
    [TestMethod]
    public void Convert_Null_InvertedCollapsed_ReturnsCollapsed()
    {
        var converter = new Null2Visibility { Inverted = true, Collapsed = true };

        var result = ((IValueConverter)converter).Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Collapsed);
    }

    /// <summary>Проверка что пустая строка считается не-null</summary>
    [TestMethod]
    public void Convert_EmptyString_ReturnsHidden()
    {
        var converter = new Null2Visibility();

        var result = ((IValueConverter)converter).Convert(string.Empty, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Hidden);
    }

    /// <summary>Проверка что ноль считается не-null</summary>
    [TestMethod]
    public void Convert_Zero_ReturnsHidden()
    {
        var converter = new Null2Visibility();

        var result = ((IValueConverter)converter).Convert(0, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Hidden);
    }

    /// <summary>Проверка что false считается не-null</summary>
    [TestMethod]
    public void Convert_False_ReturnsHidden()
    {
        var converter = new Null2Visibility();

        var result = ((IValueConverter)converter).Convert(false, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(Visibility.Hidden);
    }

    /// <summary>Проверка работы с различными типами объектов</summary>
    [TestMethod]
    public void Convert_DifferentTypes_WorksCorrectly()
    {
        var converter = new Null2Visibility();
        
        var result_list = ((IValueConverter)converter).Convert(new List<int>(), typeof(Visibility), null, CultureInfo.InvariantCulture);
        var result_date = ((IValueConverter)converter).Convert(DateTime.Now, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result_list).IsEqual(Visibility.Hidden);
        Assert.That.Value(result_date).IsEqual(Visibility.Hidden);
    }

    /// <summary>Проверка что ConvertBack выбрасывает NotSupportedException</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        var converter = new Null2Visibility();

        Assert.ThrowsException<NotSupportedException>(() =>
            ((IValueConverter)converter).ConvertBack(Visibility.Visible, typeof(object), null, CultureInfo.InvariantCulture),
            "ConvertBack не должен поддерживаться");
    }
}
