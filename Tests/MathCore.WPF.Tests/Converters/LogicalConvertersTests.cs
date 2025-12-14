using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для логических конвертеров</summary>
[TestClass]
public class LogicalConvertersTests
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    #region And Tests

    /// <summary>Тест конвертера And - все значения true</summary>
    [TestMethod]
    public void And_AllTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new And();
        var result = (bool)converter.Convert([true, true, true], typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "And для всех true должен вернуть true");
    }

    /// <summary>Тест конвертера And - одно значение false</summary>
    [TestMethod]
    public void And_OneFalse_ReturnsFalse()
    {
        IMultiValueConverter converter = new And();
        var result = (bool)converter.Convert([true, false, true], typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "And с одним false должен вернуть false");
    }

    /// <summary>Тест конвертера And - все значения false</summary>
    [TestMethod]
    public void And_AllFalse_ReturnsFalse()
    {
        IMultiValueConverter converter = new And();
        var result = (bool)converter.Convert([false, false, false], typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "And для всех false должен вернуть false");
    }

    /// <summary>Тест конвертера And - пустой массив</summary>
    [TestMethod]
    public void And_EmptyArray_ReturnsTrue()
    {
        IMultiValueConverter converter = new And();
        var result = (bool)converter.Convert([], typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "And для пустого массива должен вернуть true");
    }

    /// <summary>Тест конвертера And - смешанные типы возвращают Binding.DoNothing</summary>
    [TestMethod]
    public void And_MixedTypes_ConvertsToBoolean()
    {
        IMultiValueConverter converter = new And();
        var result = converter.Convert([1, "text", true], typeof(bool), null, Culture);
        Assert.AreEqual(System.Windows.Data.Binding.DoNothing, result, "And для смешанных типов должен вернуть Binding.DoNothing");
    }

    #endregion

    #region Or Tests

    /// <summary>Тест конвертера Or - все значения false</summary>
    [TestMethod]
    public void Or_AllFalse_ReturnsFalse()
    {
        IMultiValueConverter converter = new Or();
        var result = (bool)converter.Convert([false, false, false], typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Or для всех false должен вернуть false");
    }

    /// <summary>Тест конвертера Or - одно значение true</summary>
    [TestMethod]
    public void Or_OneTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or();
        var result = (bool)converter.Convert([false, true, false], typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Or с одним true должен вернуть true");
    }

    /// <summary>Тест конвертера Or - все значения true</summary>
    [TestMethod]
    public void Or_AllTrue_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or();
        var result = (bool)converter.Convert([true, true, true], typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Or для всех true должен вернуть true");
    }

    /// <summary>Тест конвертера Or - пустой массив</summary>
    [TestMethod]
    public void Or_EmptyArray_ReturnsFalse()
    {
        IMultiValueConverter converter = new Or();
        var result = (bool)converter.Convert([], typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Or для пустого массива должен вернуть false");
    }

    #endregion

    #region Not Tests

    /// <summary>Тест конвертера Not - инверсия true</summary>
    [TestMethod]
    public void Not_True_ReturnsFalse()
    {
        IValueConverter converter = new Not();
        var result = (bool)converter.Convert(true, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Not для true должен вернуть false");
    }

    /// <summary>Тест конвертера Not - инверсия false</summary>
    [TestMethod]
    public void Not_False_ReturnsTrue()
    {
        IValueConverter converter = new Not();
        var result = (bool)converter.Convert(false, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Not для false должен вернуть true");
    }

    /// <summary>Тест конвертера Not - обратное преобразование true</summary>
    [TestMethod]
    public void Not_ConvertBack_True_ReturnsFalse()
    {
        IValueConverter converter = new Not();
        var result = (bool)converter.ConvertBack(true, typeof(bool), null, Culture)!;
        Assert.IsFalse(result, "Not ConvertBack для true должен вернуть false");
    }

    /// <summary>Тест конвертера Not - обратное преобразование false</summary>
    [TestMethod]
    public void Not_ConvertBack_False_ReturnsTrue()
    {
        IValueConverter converter = new Not();
        var result = (bool)converter.ConvertBack(false, typeof(bool), null, Culture)!;
        Assert.IsTrue(result, "Not ConvertBack для false должен вернуть true");
    }

    /// <summary>Тест конвертера Not - двойная инверсия</summary>
    [TestMethod]
    public void Not_DoubleInversion_ReturnsOriginal()
    {
        IValueConverter converter = new Not();
        var forward = (bool)converter.Convert(true, typeof(bool), null, Culture)!;
        var back = (bool)converter.ConvertBack(forward, typeof(bool), null, Culture)!;
        Assert.IsTrue(back, "Двойная инверсия должна вернуть исходное значение");
    }

    #endregion
}
