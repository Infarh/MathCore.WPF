using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер значения NaN в видимость элемента</summary>
/// <remarks>
/// Преобразует числовое значение в значение видимости в зависимости от того, является ли оно NaN.
/// По умолчанию скрывает элемент, если значение НЕ является NaN, и показывает, если является.
/// <para><b>Логика по умолчанию:</b> NaN → Visible, не NaN → Hidden/Collapsed</para>
/// <para><b>Логика с Inverted=true:</b> NaN → Hidden/Collapsed, не NaN → Visible</para>
/// <para><b>ConvertBack:</b> Не поддерживается, так как невозможно восстановить числовое значение из видимости.
/// Значение Visible не позволяет определить, какое именно число было (NaN или нет).</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Показать сообщение об ошибке для NaN --&gt;
/// &lt;TextBlock Text="Некорректное значение" Visibility="{Binding Value, Converter={converters:NANtoVisibility}}" /&gt;
/// 
/// &lt;!-- Показать элемент только для валидных значений (не NaN) --&gt;
/// &lt;TextBlock Text="{Binding Value}" Visibility="{Binding Value, Converter={converters:NANtoVisibility Inverted=True}}" /&gt;
/// 
/// &lt;!-- Использовать Collapsed вместо Hidden --&gt;
/// &lt;TextBlock Visibility="{Binding Value, Converter={converters:NANtoVisibility Collapsed=True}}" /&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(NaNtoVisibility))]
[ValueConversion(typeof(double), typeof(Visibility))]
public class NaNtoVisibility : ValueConverter
{
    /// <summary>Инвертировать логику преобразования</summary>
    public bool Inverted { get; set; }

    /// <summary>Использовать Collapsed вместо Hidden для скрытия</summary>
    public bool Collapsed { get; set; }

    private Visibility Hidden => Collapsed ? Visibility.Collapsed : Visibility.Hidden;

    /// <inheritdoc />
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => 
        v is null 
            ? null 
            : Inverted
                ? !double.IsNaN((double)v) ? Hidden : Visibility.Visible
                : double.IsNaN((double)v) ? Hidden : Visibility.Visible;
}