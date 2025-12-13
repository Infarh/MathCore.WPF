using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер null-значения в видимость элемента</summary>
/// <remarks>
/// Преобразует значение в видимость элемента в зависимости от того, является ли оно null.
/// По умолчанию показывает элемент для null и скрывает для не-null значений.
/// <para><b>Логика по умолчанию:</b> null → Visible, не null → Hidden/Collapsed</para>
/// <para><b>Логика с Inverted=true:</b> null → Hidden/Collapsed, не null → Visible</para>
/// <para><b>ConvertBack:</b> Не поддерживается, так как невозможно восстановить исходный объект из видимости.
/// Значение Visible не позволяет определить, что именно было (null или какой-то объект).</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Показать сообщение "Нет данных" когда значение null --&gt;
/// &lt;TextBlock Text="Нет данных" Visibility="{Binding Data, Converter={converters:Null2Visibility}}" /&gt;
/// 
/// &lt;!-- Показать содержимое только когда есть данные (не null) --&gt;
/// &lt;ContentPresenter Content="{Binding Data}" Visibility="{Binding Data, Converter={converters:Null2Visibility Inverted=True}}" /&gt;
/// 
/// &lt;!-- Использовать Collapsed вместо Hidden --&gt;
/// &lt;TextBlock Visibility="{Binding Data, Converter={converters:Null2Visibility Collapsed=True}}" /&gt;
/// </code>
/// </example>
[ValueConversion(typeof(object), typeof(Visibility))]
[MarkupExtensionReturnType(typeof(Null2Visibility))]
public class Null2Visibility : ValueConverter
{
    /// <summary>Инвертировать логику преобразования</summary>
    public bool Inverted { get; set; }

    /// <summary>Использовать Collapsed вместо Hidden для скрытия</summary>
    public bool Collapsed { get; set; }

    private Visibility Hidden => Collapsed ? Visibility.Collapsed : Visibility.Hidden;

    /// <inheritdoc />
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => v is null 
        ? !Inverted ? Visibility.Visible : Hidden
        : Inverted ? Hidden : Visibility.Visible;

}