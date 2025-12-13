using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер преобразования null в Visibility</summary>
/// <remarks>
/// Преобразует значение в состояние видимости элемента в зависимости от того, является ли оно null.
/// По умолчанию: null → Visible, не-null → Hidden/Collapsed.
/// При Inverted=true: null → Hidden/Collapsed, не-null → Visible.
/// <example>
/// <code>
/// &lt;TextBlock Text="Нет данных" Visibility="{Binding Data, Converter={converters:Null2Visibility}}" /&gt;
/// &lt;ContentControl Visibility="{Binding Data, Converter={converters:Null2Visibility Inverted=True}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[ValueConversion(typeof(object), typeof(Visibility))]
[MarkupExtensionReturnType(typeof(Null2Visibility))]
public class Null2Visibility : ValueConverter
{
    /// <summary>Инвертировать логику преобразования</summary>
    public bool Inverted { get; set; }

    /// <summary>Использовать Collapsed вместо Hidden для скрытия элемента</summary>
    public bool Collapsed { get; set; }

    private Visibility Hidden => Collapsed ? Visibility.Collapsed : Visibility.Hidden;

    /// <summary>Преобразование значения в Visibility</summary>
    /// <param name="v">Значение для проверки на null</param>
    /// <param name="t">Целевой тип (не используется)</param>
    /// <param name="p">Параметр конвертера (не используется)</param>
    /// <param name="c">Информация о культуре (не используется)</param>
    /// <returns>Visible или Hidden/Collapsed в зависимости от значения и настроек</returns>
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => v is null 
        ? !Inverted ? Visibility.Visible : Hidden
        : Inverted ? Hidden : Visibility.Visible;

}