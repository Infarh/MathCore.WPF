using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер преобразования булева значения в Visibility</summary>
/// <remarks>
/// Преобразует булево значение в состояние видимости элемента.
/// По умолчанию: true → Visible, false → Hidden/Collapsed.
/// При Inverted=true: false → Visible, true → Hidden/Collapsed.
/// Поддерживает двустороннее связывание.
/// <example>
/// <code>
/// &lt;Button Visibility="{Binding IsEnabled, Converter={converters:Bool2Visibility}}" /&gt;
/// &lt;Border Visibility="{Binding IsHidden, Converter={converters:Bool2Visibility Inverted=True, Collapsed=True}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[ValueConversion(typeof(bool?), typeof(Visibility))]
[MarkupExtensionReturnType(typeof(Bool2Visibility))]
public class Bool2Visibility : ValueConverter
{
    /// <summary>Инвертировать логику преобразования</summary>
    public bool Inverted { get; set; }

    /// <summary>Использовать Collapsed вместо Hidden для скрытия элемента</summary>
    public bool Collapsed { get; set; }

    private Visibility Hidden => Collapsed ? Visibility.Collapsed : Visibility.Hidden;

    /// <summary>Преобразование булева значения в Visibility</summary>
    /// <param name="v">Булево значение или Visibility</param>
    /// <param name="t">Целевой тип</param>
    /// <param name="p">Параметр конвертера (не используется)</param>
    /// <param name="c">Информация о культуре (не используется)</param>
    /// <returns>Visible или Hidden/Collapsed в зависимости от значения и настроек</returns>
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) =>
        v switch
        {
            null => null,
            Visibility => v,
            true => !Inverted ? Visibility.Visible : Hidden,
            false => Inverted ? Visibility.Visible : Hidden,
            _ => throw new NotSupportedException()
        };

    /// <summary>Обратное преобразование Visibility в булево значение</summary>
    /// <param name="v">Значение Visibility или булево</param>
    /// <param name="t">Целевой тип</param>
    /// <param name="p">Параметр конвертера (не используется)</param>
    /// <param name="c">Информация о культуре (не используется)</param>
    /// <returns>true или false в зависимости от Visibility и настроек</returns>
    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) =>
        v switch
        {
            null => null,
            bool => v,
            Visibility.Visible => !Inverted,
            Visibility.Hidden => Inverted,
            Visibility.Collapsed => Inverted,
            _ => throw new NotSupportedException()
        };
}