using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразует NaN в Visibility</summary>
[MarkupExtensionReturnType(typeof(NaNtoVisibility))]
[ValueConversion(typeof(double), typeof(Visibility))]
public class NaNtoVisibility : ValueConverter
{
    /// <summary>Инвертировать результат преобразования</summary>
    public bool Inverted { get; set; }

    /// <summary>Использовать Visibility.Collapsed вместо Visibility.Hidden</summary>
    public bool Collapsed { get; set; }

    private Visibility Hidden => Collapsed ? Visibility.Collapsed : Visibility.Hidden;

    /// <summary>Преобразует NaN в Visibility</summary>
    /// <param name="v">Входное значение</param>
    /// <param name="t">Тип целевого значения</param>
    /// <param name="p">Параметр преобразования</param>
    /// <param name="c">Культура</param>
    /// <returns>Visibility.Visible если значение не NaN (с учётом флага Inverted), иначе Hidden; возвращает null для null входа</returns>
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) =>
        v is null
            ? null
            : v is double d
                ? Inverted
                    ? !double.IsNaN(d) ? Hidden : Visibility.Visible
                    : double.IsNaN(d) ? Hidden : Visibility.Visible
                : Binding.DoNothing;
}