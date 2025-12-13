using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class AndTests
{
    /// <summary>Проверка логического И для всех true значений</summary>
    [TestMethod]
    public void Convert_AllTrue_ReturnsTrue()
    {
        var converter = new And();
        var values = new object[] { true, true, true };

        var result = ((IMultiValueConverter)converter).Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка логического И с одним false значением</summary>
    [TestMethod]
    public void Convert_OneFalse_ReturnsFalse()
    {
        var converter = new And();
        var values = new object[] { true, false, true };

        var result = ((IMultiValueConverter)converter).Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка логического И для всех false значений</summary>
    [TestMethod]
    public void Convert_AllFalse_ReturnsFalse()
    {
        var converter = new And();
        var values = new object[] { false, false, false };

        var result = ((IMultiValueConverter)converter).Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка логического И для одного true значения</summary>
    [TestMethod]
    public void Convert_SingleTrue_ReturnsTrue()
    {
        var converter = new And();
        var values = new object[] { true };

        var result = ((IMultiValueConverter)converter).Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка логического И для одного false значения</summary>
    [TestMethod]
    public void Convert_SingleFalse_ReturnsFalse()
    {
        var converter = new And();
        var values = new object[] { false };

        var result = ((IMultiValueConverter)converter).Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка поведения при null массиве с NullDefaultValue = false</summary>
    [TestMethod]
    public void Convert_NullArray_ReturnsNullDefaultValue_False()
    {
        var converter = new And { NullDefaultValue = false };

        var result = ((IMultiValueConverter)converter).Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка поведения при null массиве с NullDefaultValue = true</summary>
    [TestMethod]
    public void Convert_NullArray_ReturnsNullDefaultValue_True()
    {
        var converter = new And { NullDefaultValue = true };

        var result = ((IMultiValueConverter)converter).Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка логического И для пустого массива</summary>
    [TestMethod]
    public void Convert_EmptyArray_ReturnsTrue()
    {
        var converter = new And();
        var values = Array.Empty<object>();

        var result = ((IMultiValueConverter)converter).Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка логического И для большого количества true значений</summary>
    [TestMethod]
    public void Convert_ManyTrueValues_ReturnsTrue()
    {
        var converter = new And();
        var values = Enumerable.Repeat((object)true, 100).ToArray();

        var result = ((IMultiValueConverter)converter).Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка что ConvertBack выбрасывает NotSupportedException</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        var converter = new And();

        Assert.ThrowsException<NotSupportedException>(() =>
            ((IMultiValueConverter)converter).ConvertBack(true, [typeof(bool)], null, CultureInfo.InvariantCulture),
            "ConvertBack для MultiValueConverter не должен поддерживаться");
    }
}
