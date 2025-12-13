using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер преобразования NaN в Visibility</summary>
/// <remarks>
/// Преобразует числовое значение в состояние видимости элемента в зависимости от того, является ли оно NaN.
/// По умолчанию: NaN → Hidden/Collapsed, число → Visible.
/// При Inverted=true: число → Hidden/Collapsed, NaN → Visible.
/// <example>
/// <code>
/// &lt;TextBlock Visibility="{Binding Value, Converter={converters:NaNtoVisibility}}" /&gt;
/// &lt;TextBlock Visibility="{Binding Value, Converter={converters:NaNtoVisibility Inverted=True, Collapsed=True}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(NaNtoVisibility))]
[ValueConversion(typeof(double), typeof(Visibility))]
public class NaNtoVisibility : ValueConverter
{
    /// <summary>Инвертировать логику преобразования</summary>
    public bool Inverted { get; set; }

    /// <summary>Использовать Collapsed вместо Hidden для скрытия элемента</summary>
    public bool Collapsed { get; set; }

    private Visibility Hidden => Collapsed ? Visibility.Collapsed : Visibility.Hidden;

    /// <summary>Преобразование значения в Visibility</summary>
    /// <param name="v">Числовое значение для проверки</param>
    /// <param name="t">Целевой тип (не используется)</param>
    /// <param name="p">Параметр конвертера (не используется)</param>
    /// <param name="c">Информация о культуре (не используется)</param>
    /// <returns>Visible или Hidden/Collapsed в зависимости от значения и настроек; null если v равно null</returns>
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => 
        v is null 
            ? null 
            : Inverted
                ? !double.IsNaN((double)v) ? Hidden : Visibility.Visible
                : double.IsNaN((double)v) ? Hidden : Visibility.Visible;
}