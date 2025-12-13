using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Модульные тесты для конвертера And</summary>
[TestClass]
public class AndTests
{
    /// <summary>Тест логической операции AND для всех true значений</summary>
    [TestMethod]
    public void Convert_AllTrueValues_ReturnsTrue()
    {
        IMultiValueConverter converter = new And();
        var values = new object[] { true, true, true };

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест логической операции AND для смешанных значений</summary>
    [TestMethod]
    public void Convert_MixedValues_ReturnsFalse()
    {
        IMultiValueConverter converter = new And();
        var values = new object[] { true, false, true };

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест логической операции AND для всех false значений</summary>
    [TestMethod]
    public void Convert_AllFalseValues_ReturnsFalse()
    {
        IMultiValueConverter converter = new And();
        var values = new object[] { false, false, false };

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест с одним значением true</summary>
    [TestMethod]
    public void Convert_SingleTrueValue_ReturnsTrue()
    {
        IMultiValueConverter converter = new And();
        var values = new object[] { true };

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест с одним значением false</summary>
    [TestMethod]
    public void Convert_SingleFalseValue_ReturnsFalse()
    {
        IMultiValueConverter converter = new And();
        var values = new object[] { false };

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест с пустым массивом значений</summary>
    [TestMethod]
    public void Convert_EmptyArray_ReturnsTrue()
    {
        IMultiValueConverter converter = new And();
        var values = Array.Empty<object>();

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true, "Пустой массив должен возвращать true (вакуумная истина)");
    }

    /// <summary>Тест с null массивом и значением по умолчанию false</summary>
    [TestMethod]
    public void Convert_NullArray_ReturnsNullDefaultValue()
    {
        var converter = new And { NullDefaultValue = false };

        var result = ((IMultiValueConverter)converter).Convert(null, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест с null массивом и значением по умолчанию true</summary>
    [TestMethod]
    public void Convert_NullArrayWithTrueDefault_ReturnsTrue()
    {
        var converter = new And { NullDefaultValue = true };

        var result = ((IMultiValueConverter)converter).Convert(null, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест обратного преобразования - должно генерировать NotSupportedException</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        IMultiValueConverter converter = new And();

        Assert.ThrowsException<NotSupportedException>(() =>
            converter.ConvertBack(true, new[] { typeof(bool), typeof(bool) }, null, CultureInfo.CurrentCulture),
            "ConvertBack не должен поддерживаться для And конвертера");
    }

    /// <summary>Тест с большим количеством true значений</summary>
    [TestMethod]
    public void Convert_ManyTrueValues_ReturnsTrue()
    {
        IMultiValueConverter converter = new And();
        var values = Enumerable.Repeat((object)true, 100).ToArray();

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест с одним false среди множества true</summary>
    [TestMethod]
    public void Convert_OneFalseAmongManyTrue_ReturnsFalse()
    {
        IMultiValueConverter converter = new And();
        var values = Enumerable.Repeat((object)true, 50)
            .Append(false)
            .Concat(Enumerable.Repeat((object)true, 49))
            .ToArray();

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false, "Один false должен сделать результат false");
    }
}
