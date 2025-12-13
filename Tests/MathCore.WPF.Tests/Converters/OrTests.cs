using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Модульные тесты для конвертера Or</summary>
[TestClass]
public class OrTests
{
    /// <summary>Тест логической операции OR для всех true значений</summary>
    [TestMethod]
    public void Convert_AllTrueValues_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or();
        var values = new object[] { true, true, true };

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест логической операции OR для смешанных значений</summary>
    [TestMethod]
    public void Convert_MixedValues_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or();
        var values = new object[] { true, false, true };

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест логической операции OR для всех false значений</summary>
    [TestMethod]
    public void Convert_AllFalseValues_ReturnsFalse()
    {
        IMultiValueConverter converter = new Or();
        var values = new object[] { false, false, false };

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест с одним значением true</summary>
    [TestMethod]
    public void Convert_SingleTrueValue_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or();
        var values = new object[] { true };

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест с одним значением false</summary>
    [TestMethod]
    public void Convert_SingleFalseValue_ReturnsFalse()
    {
        IMultiValueConverter converter = new Or();
        var values = new object[] { false };

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест с пустым массивом значений</summary>
    [TestMethod]
    public void Convert_EmptyArray_ReturnsFalse()
    {
        IMultiValueConverter converter = new Or();
        var values = Array.Empty<object>();

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false, "Пустой массив должен возвращать false (нет true значений)");
    }

    /// <summary>Тест с null массивом и значением по умолчанию false</summary>
    [TestMethod]
    public void Convert_NullArray_ReturnsNullDefaultValue()
    {
        var converter = new Or { NullDefaultValue = false };

        var result = ((IMultiValueConverter)converter).Convert(null, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест с null массивом и значением по умолчанию true</summary>
    [TestMethod]
    public void Convert_NullArrayWithTrueDefault_ReturnsTrue()
    {
        var converter = new Or { NullDefaultValue = true };

        var result = ((IMultiValueConverter)converter).Convert(null, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true);
    }

    /// <summary>Тест обратного преобразования - должно генерировать NotSupportedException</summary>
    [TestMethod]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        IMultiValueConverter converter = new Or();

        Assert.ThrowsException<NotSupportedException>(() =>
            converter.ConvertBack(true, new[] { typeof(bool), typeof(bool) }, null, CultureInfo.CurrentCulture),
            "ConvertBack не должен поддерживаться для Or конвертера");
    }

    /// <summary>Тест с большим количеством false значений</summary>
    [TestMethod]
    public void Convert_ManyFalseValues_ReturnsFalse()
    {
        IMultiValueConverter converter = new Or();
        var values = Enumerable.Repeat((object)false, 100).ToArray();

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(false);
    }

    /// <summary>Тест с одним true среди множества false</summary>
    [TestMethod]
    public void Convert_OneTrueAmongManyFalse_ReturnsTrue()
    {
        IMultiValueConverter converter = new Or();
        var values = Enumerable.Repeat((object)false, 50)
            .Append(true)
            .Concat(Enumerable.Repeat((object)false, 49))
            .ToArray();

        var result = converter.Convert(values, typeof(bool), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(true, "Один true должен сделать результат true");
    }
}
