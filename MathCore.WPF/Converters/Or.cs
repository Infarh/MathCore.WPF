using System.Globalization;
using System.Linq;
using System.Windows.Markup;
using System.Windows.Data;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Проверяет, что хотя бы одно из входных значений истинно</summary>
[MarkupExtensionReturnType(typeof(Or))]
public class Or : MultiValueValueConverter
{
    /// <summary>Значение по умолчанию при null входе</summary>
    public bool NullDefaultValue { get; set; }

    /// <summary>Возвращает true если хотя бы одно значение истинно, Binding.DoNothing при несоответствующих типах</summary>
    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c)
    {
        if (vv is null) return NullDefaultValue;

        var anyTrue = false;
        foreach (var item in vv)
        {
            if (item is not bool b) return Binding.DoNothing;
            if (b) { anyTrue = true; break; }
        }

        return anyTrue;
    }
}