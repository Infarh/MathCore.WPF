using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class LastItemConverterTests
{
    /// <summary>Проверка извлечения последнего элемента из массива</summary>
    [TestMethod]
    public void Convert_Array_ReturnsLastElement()
    {
        var converter = new LastItemConverter();
        var array = new[] { "first", "second", "third", "last" };

        var result = ((IValueConverter)converter).Convert(array, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual("last");
    }

    /// <summary>Проверка извлечения последнего элемента из списка</summary>
    [TestMethod]
    public void Convert_List_ReturnsLastElement()
    {
        var converter = new LastItemConverter();
        var list = new List<int> { 10, 20, 30, 40, 50 };

        var result = ((IValueConverter)converter).Convert(list, typeof(int), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(50);
    }

    /// <summary>Проверка с массивом из одного элемента</summary>
    [TestMethod]
    public void Convert_SingleElement_ReturnsThatElement()
    {
        var converter = new LastItemConverter();
        var array = new[] { "only" };

        var result = ((IValueConverter)converter).Convert(array, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual("only");
    }

    /// <summary>Проверка с пустым массивом</summary>
    [TestMethod]
    public void Convert_EmptyArray_ReturnsNull()
    {
        var converter = new LastItemConverter();
        var array = Array.Empty<string>();

        var result = ((IValueConverter)converter).Convert(array, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.IsNull(result, "Пустой массив должен вернуть null");
    }

    /// <summary>Проверка с пустым списком</summary>
    [TestMethod]
    public void Convert_EmptyList_ReturnsNull()
    {
        var converter = new LastItemConverter();
        var list = new List<int>();

        var result = ((IValueConverter)converter).Convert(list, typeof(int), null, CultureInfo.InvariantCulture);

        Assert.IsNull(result, "Пустой список должен вернуть null");
    }

    /// <summary>Проверка с IEnumerable</summary>
    [TestMethod]
    public void Convert_IEnumerable_ReturnsLastElement()
    {
        var converter = new LastItemConverter();
        var enumerable = Enumerable.Range(1, 10);

        var result = ((IValueConverter)converter).Convert(enumerable, typeof(int), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(10);
    }

    /// <summary>Проверка с пустым IEnumerable</summary>
    [TestMethod]
    public void Convert_EmptyIEnumerable_ReturnsNull()
    {
        var converter = new LastItemConverter();
        var enumerable = Enumerable.Empty<string>();

        var result = ((IValueConverter)converter).Convert(enumerable, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.IsNull(result, "Пустое перечисление должно вернуть null");
    }
}
