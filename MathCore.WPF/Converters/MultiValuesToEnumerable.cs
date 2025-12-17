using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Возвращает входные значения как IEnumerable и восстанавливает массив при ConvertBack</summary>
[MarkupExtensionReturnType(typeof(MultiValuesToEnumerable))]
public class MultiValuesToEnumerable : MultiValueValueConverter
{
    /// <summary>Возвращает входной массив как есть</summary>
    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => vv;

    /// <summary>Пытается восстановить массив значений из IEnumerable, используя указанные типы</summary>
    protected override object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c)
    {
        if (v is not IEnumerable enumerable || tt is null) return null;

        var values = enumerable.Cast<object?>().ToArray();
        if (values.Length != tt.Length) return null;

        try
        {
            var result = values.Zip(tt, (val, type) => System.Convert.ChangeType(val, type)).ToArray();
            return result;
        }
        catch
        {
            return null;
        }
    }
}