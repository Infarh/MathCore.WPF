using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class OrTests
{
    /// <summary>Проверка логического ИЛИ для всех true значений</summary>
    [TestMethod]
    public void Convert_AllTrue_ReturnsTrue()
    {
        var converter = new Or();
        var values = new object[] { true, true, true };

        var result = ((IMultiValueConverter)converter).Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка логического ИЛИ с одним true значением</summary>
    [TestMethod]
    public void Convert_OneTrue_ReturnsTrue()
    {
        var converter = new Or();
        var values = new object[] { false, true, false };

        var result = ((IMultiValueConverter)converter).Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка логического ИЛИ для всех false значений</summary>
    [TestMethod]
    public void Convert_AllFalse_ReturnsFalse()
    {
        var converter = new Or();
        var values = new object[] { false, false, false };

        var result = ((IMultiValueConverter)converter).Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка логического ИЛИ для одного true значения</summary>
    [TestMethod]
    public void Convert_SingleTrue_ReturnsTrue()
    {
        var converter = new Or();
        var values = new object[] { true };

        var result = ((IMultiValueConverter)converter).Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка логического ИЛИ для одного false значения</summary>
    [TestMethod]
    public void Convert_SingleFalse_ReturnsFalse()
    {
        var converter = new Or();
        var values = new object[] { false };

        var result = ((IMultiValueConverter)converter).Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка поведения при null массиве с NullDefaultValue = false</summary>
    [TestMethod]
    public void Convert_NullArray_ReturnsNullDefaultValue_False()
    {
        var converter = new Or { NullDefaultValue = false };

        var result = ((IMultiValueConverter)converter).Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка поведения при null массиве с NullDefaultValue = true</summary>
    [TestMethod]
    public void Convert_NullArray_ReturnsNullDefaultValue_True()
    {
        var converter = new Or { NullDefaultValue = true };

        var result = ((IMultiValueConverter)converter).Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка логического ИЛИ для пустого массива</summary>
    [TestMethod]
    public void Convert_EmptyArray_ReturnsFalse()
    {
        var converter = new Or();
        var values = Array.Empty<object>();

        var result = ((IMultiValueConverter)converter).Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Проверка логического ИЛИ с одним true среди многих false</summary>
    [TestMethod]
    public void Convert_OneTrueAmongManyFalse_ReturnsTrue()
    {
        var converter = new Or();
        var values = new object[] { false, false, false, true, false, false };

        var result = ((IMultiValueConverter)converter).Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Проверка что ConvertBack выбрасывает NotSupportedException</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        var converter = new Or();

        Assert.ThrowsException<NotSupportedException>(() =>
            ((IMultiValueConverter)converter).ConvertBack(true, new Type[] { typeof(bool) }, null, CultureInfo.InvariantCulture),
            "ConvertBack для MultiValueConverter не должен поддерживаться");
    }
}
