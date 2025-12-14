using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Преобразует массив в строку</summary>
[MarkupExtensionReturnType(typeof(ArrayToStringConverter))]
public class ArrayToStringConverter : ValueConverter
{
    /// <summary>Преобразует массив в строку, элементы разделены запятой</summary>
    /// <param name="v">Массив для преобразования</param>
    /// <param name="t">Тип целевого значения</param>
    /// <param name="p">Параметр преобразования (не используется)</param>
    /// <param name="c">Культура</param>
    /// <returns>Строковое представление массива или Binding.DoNothing для неподдерживаемых входов</returns>
    /// <remarks>Использует запятую в качестве разделителя</remarks>
    protected override object? Convert(object? v, Type t, object? p, CultureInfo c)
    {
        if (v is not Array array)
            return Binding.DoNothing;

        var items = array.Cast<object?>().Select(x => x?.ToString() ?? string.Empty).ToArray();
        return string.Join(",", items);
    }
}
