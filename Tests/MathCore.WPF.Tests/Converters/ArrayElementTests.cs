using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class ArrayElementTests
{
    /// <summary>Проверка извлечения элемента из массива по индексу</summary>
    [TestMethod]
    public void Convert_Array_ReturnsElementAtIndex()
    {
        var converter = new ArrayElement { Index = 2 };
        var array = new[] { "first", "second", "third", "fourth" };

        var result = ((IValueConverter)converter).Convert(array, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual("third");
    }

    /// <summary>Проверка извлечения элемента из списка</summary>
    [TestMethod]
    public void Convert_List_ReturnsElementAtIndex()
    {
        var converter = new ArrayElement { Index = 1 };
        var list = new List<int> { 10, 20, 30, 40 };

        var result = ((IValueConverter)converter).Convert(list, typeof(int), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(20);
    }

    /// <summary>Проверка извлечения первого элемента</summary>
    [TestMethod]
    public void Convert_IndexZero_ReturnsFirstElement()
    {
        var converter = new ArrayElement { Index = 0 };
        var array = new[] { 100, 200, 300 };

        var result = ((IValueConverter)converter).Convert(array, typeof(int), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(100);
    }

    /// <summary>Проверка с индексом из параметра</summary>
    [TestMethod]
    public void Convert_IndexFromParameter_ReturnsCorrectElement()
    {
        var converter = new ArrayElement();
        var array = new[] { "A", "B", "C", "D" };

        var result = ((IValueConverter)converter).Convert(array, typeof(string), 3, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual("D");
    }

    /// <summary>Проверка с индексом вне диапазона</summary>
    [TestMethod]
    public void Convert_IndexOutOfRange_ReturnsNull()
    {
        var converter = new ArrayElement { Index = 10 };
        var array = new[] { 1, 2, 3 };

        var result = ((IValueConverter)converter).Convert(array, typeof(int), null, CultureInfo.InvariantCulture);

        Assert.IsNull(result, "Индекс вне диапазона должен вернуть null");
    }

    /// <summary>Проверка с пустым массивом</summary>
    [TestMethod]
    public void Convert_EmptyArray_ReturnsNull()
    {
        var converter = new ArrayElement { Index = 0 };
        var array = Array.Empty<string>();

        var result = ((IValueConverter)converter).Convert(array, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.IsNull(result, "Пустой массив должен вернуть null");
    }

    /// <summary>Проверка с IEnumerable</summary>
    [TestMethod]
    public void Convert_IEnumerable_ReturnsElementAtIndex()
    {
        var converter = new ArrayElement { Index = 2 };
        var enumerable = Enumerable.Range(1, 5);

        var result = ((IValueConverter)converter).Convert(enumerable, typeof(int), null, CultureInfo.InvariantCulture);

        Assert.That.Value(result).IsEqual(3);
    }
}
