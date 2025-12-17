using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters;

/// <summary>Проверяет, что все элементы входного массива являются true</summary>
[MarkupExtensionReturnType(typeof(And))]
public class And : MultiValueValueConverter
{
    /// <summary>Возвращаемое значение при null входе</summary>
    public bool NullDefaultValue { get; set; }

    /// <summary>Возвращает true если все элементы истинны, иначе false; при наличии несоответствующих типов возвращает Binding.DoNothing</summary>
    protected override object Convert(object[]? vv, Type? t, object? p, CultureInfo? c)
    {
        if (vv is null) return NullDefaultValue;

        foreach (var item in vv)
        {
            if (item is not bool b) return Binding.DoNothing;
            if (!b) return false;
        }

        return true;
    }
}