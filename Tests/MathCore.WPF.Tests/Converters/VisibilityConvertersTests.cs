using System.Globalization;
using System.Windows;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертеров видимости элементов</summary>
[TestClass]
public class VisibilityConvertersTests
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    #region Bool2Visibility Tests

    /// <summary>Тест конвертера Bool2Visibility - true преобразуется в Visible</summary>
    [TestMethod]
    public void Bool2Visibility_True_ReturnsVisible()
    {
        IValueConverter converter = new Bool2Visibility();
        var result = (Visibility)converter.Convert(true, typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Visible, result, "true должен вернуть Visible");
    }

    /// <summary>Тест конвертера Bool2Visibility - false преобразуется в Hidden</summary>
    [TestMethod]
    public void Bool2Visibility_False_ReturnsHidden()
    {
        IValueConverter converter = new Bool2Visibility();
        var result = (Visibility)converter.Convert(false, typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Hidden, result, "false должен вернуть Hidden по умолчанию");
    }

    /// <summary>Тест конвертера Bool2Visibility - false с флагом Collapsed</summary>
    [TestMethod]
    public void Bool2Visibility_FalseCollapsed_ReturnsCollapsed()
    {
        IValueConverter converter = new Bool2Visibility { Collapsed = true };
        var result = (Visibility)converter.Convert(false, typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Collapsed, result, "false с Collapsed=true должен вернуть Collapsed");
    }

    /// <summary>Тест конвертера Bool2Visibility - инвертированный true</summary>
    [TestMethod]
    public void Bool2Visibility_InvertedTrue_ReturnsHidden()
    {
        IValueConverter converter = new Bool2Visibility { Inverted = true };
        var result = (Visibility)converter.Convert(true, typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Hidden, result, "true с Inverted=true должен вернуть Hidden");
    }

    /// <summary>Тест конвертера Bool2Visibility - инвертированный false</summary>
    [TestMethod]
    public void Bool2Visibility_InvertedFalse_ReturnsVisible()
    {
        IValueConverter converter = new Bool2Visibility { Inverted = true };
        var result = (Visibility)converter.Convert(false, typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Visible, result, "false с Inverted=true должен вернуть Visible");
    }

    /// <summary>Тест конвертера Bool2Visibility - обратное преобразование Visible</summary>
    [TestMethod]
    public void Bool2Visibility_ConvertBack_Visible_ReturnsTrue()
    {
        IValueConverter converter = new Bool2Visibility();
        var result = (bool)converter.ConvertBack(Visibility.Visible, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Visible должен преобразоваться в true");
    }

    /// <summary>Тест конвертера Bool2Visibility - обратное преобразование Hidden</summary>
    [TestMethod]
    public void Bool2Visibility_ConvertBack_Hidden_ReturnsFalse()
    {
        IValueConverter converter = new Bool2Visibility();
        var result = (bool)converter.ConvertBack(Visibility.Hidden, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Hidden должен преобразоваться в false");
    }

    /// <summary>Тест конвертера Bool2Visibility - обратное преобразование Collapsed</summary>
    [TestMethod]
    public void Bool2Visibility_ConvertBack_Collapsed_ReturnsFalse()
    {
        IValueConverter converter = new Bool2Visibility();
        var result = (bool)converter.ConvertBack(Visibility.Collapsed, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Collapsed должен преобразоваться в false");
    }

    #endregion

    #region Null2Visibility Tests

    /// <summary>Тест конвертера Null2Visibility - null с Inverted=false преобразуется в Visible</summary>
    [TestMethod]
    public void Null2Visibility_Null_ReturnsVisible()
    {
        IValueConverter converter = new Null2Visibility();
        var result = (Visibility)converter.Convert(null, typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Visible, result, "null должен вернуть Visible");
    }

    /// <summary>Тест конвертера Null2Visibility - не null с Inverted=false преобразуется в Visible</summary>
    [TestMethod]
    public void Null2Visibility_NotNull_ReturnsVisible()
    {
        IValueConverter converter = new Null2Visibility();
        var result = (Visibility)converter.Convert("test", typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Visible, result, "Не null должен вернуть Visible");
    }

    /// <summary>Тест конвертера Null2Visibility - null с флагом Collapsed и Inverted</summary>
    [TestMethod]
    public void Null2Visibility_NullCollapsed_ReturnsCollapsed()
    {
        IValueConverter converter = new Null2Visibility { Collapsed = true, Inverted = true };
        var result = (Visibility)converter.Convert(null, typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Collapsed, result, "null с Collapsed=true и Inverted=true должен вернуть Collapsed");
    }

    /// <summary>Тест конвертера Null2Visibility - инвертированный null</summary>
    [TestMethod]
    public void Null2Visibility_InvertedNull_ReturnsHidden()
    {
        IValueConverter converter = new Null2Visibility { Inverted = true };
        var result = (Visibility)converter.Convert(null, typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Hidden, result, "null с Inverted=true должен вернуть Hidden");
    }

    /// <summary>Тест конвертера Null2Visibility - инвертированный не null</summary>
    [TestMethod]
    public void Null2Visibility_InvertedNotNull_ReturnsHidden()
    {
        IValueConverter converter = new Null2Visibility { Inverted = true };
        var result = (Visibility)converter.Convert("test", typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Hidden, result, "Не null с Inverted=true должен вернуть Hidden");
    }

    #endregion

    #region NaNtoVisibility Tests

    /// <summary>Тест конвертера NaNtoVisibility - NaN преобразуется в Hidden</summary>
    [TestMethod]
    public void NaNtoVisibility_NaN_ReturnsHidden()
    {
        IValueConverter converter = new NaNtoVisibility();
        var result = (Visibility)converter.Convert(double.NaN, typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Hidden, result, "NaN должен вернуть Hidden");
    }

    /// <summary>Тест конвертера NaNtoVisibility - нормальное значение преобразуется в Visible</summary>
    [TestMethod]
    public void NaNtoVisibility_NormalValue_ReturnsVisible()
    {
        IValueConverter converter = new NaNtoVisibility();
        var result = (Visibility)converter.Convert(5.0, typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Visible, result, "Нормальное значение должно вернуть Visible");
    }

    /// <summary>Тест конвертера NaNtoVisibility - NaN с флагом Collapsed</summary>
    [TestMethod]
    public void NaNtoVisibility_NaNCollapsed_ReturnsCollapsed()
    {
        IValueConverter converter = new NaNtoVisibility { Collapsed = true };
        var result = (Visibility)converter.Convert(double.NaN, typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Collapsed, result, "NaN с Collapsed=true должен вернуть Collapsed");
    }

    /// <summary>Тест конвертера NaNtoVisibility - инвертированный NaN</summary>
    [TestMethod]
    public void NaNtoVisibility_InvertedNaN_ReturnsVisible()
    {
        IValueConverter converter = new NaNtoVisibility { Inverted = true };
        var result = (Visibility)converter.Convert(double.NaN, typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Visible, result, "NaN с Inverted=true должен вернуть Visible");
    }

    /// <summary>Тест конвертера NaNtoVisibility - инвертированное нормальное значение</summary>
    [TestMethod]
    public void NaNtoVisibility_InvertedNormalValue_ReturnsHidden()
    {
        IValueConverter converter = new NaNtoVisibility { Inverted = true };
        var result = (Visibility)converter.Convert(5.0, typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Hidden, result, "Нормальное значение с Inverted=true должно вернуть Hidden");
    }

    /// <summary>Тест конвертера NaNtoVisibility - бесконечность не является NaN</summary>
    [TestMethod]
    public void NaNtoVisibility_Infinity_ReturnsVisible()
    {
        IValueConverter converter = new NaNtoVisibility();
        var result = (Visibility)converter.Convert(double.PositiveInfinity, typeof(Visibility), null, Culture)!;
        Assert.AreEqual(Visibility.Visible, result, "Бесконечность должна вернуть Visible");
    }

    #endregion
}
