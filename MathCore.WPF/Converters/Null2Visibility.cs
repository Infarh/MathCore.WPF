using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер значения null в состояние видимости Visibility</summary>
/// <remarks>
/// Преобразует объект в значение Visibility на основе того, является ли он null
/// <para>Возвращаемые значения (в нормальном режиме):</para>
/// <list type="bullet">
/// <item><description>null → Visible</description></item>
/// <item><description>не-null → Hidden/Collapsed (в зависимости от свойства Collapsed)</description></item>
/// </list>
/// <para>При установленном свойстве Inverted=true логика инвертируется:</para>
/// <list type="bullet">
/// <item><description>null → Hidden/Collapsed</description></item>
/// <item><description>не-null → Visible</description></item>
/// </list>
/// <para>Свойство Collapsed определяет тип скрытия: true → Collapsed (элемент не занимает места), false → Hidden (место сохраняется)</para>
/// <para>ConvertBack не поддерживается, так как из состояния видимости невозможно восстановить исходный объект</para>
/// </remarks>
/// <example>
/// <code language="xaml"><![CDATA[
/// <!-- Показать текст "Нет данных" когда значение null -->
/// <TextBlock Text="Нет данных" 
///            Visibility="{Binding Data, Converter={converters:Null2Visibility}}" />
/// 
/// <!-- Скрыть элемент когда значение null (инвертированный режим) -->
/// <StackPanel Visibility="{Binding User, 
///             Converter={converters:Null2Visibility Inverted=True}}" />
/// 
/// <!-- Использование Collapsed для освобождения места -->
/// <Border Visibility="{Binding Content, 
///         Converter={converters:Null2Visibility Inverted=True, Collapsed=True}}" />
/// ]]></code>
/// </example>
[ValueConversion(typeof(object), typeof(Visibility))]
[MarkupExtensionReturnType(typeof(Null2Visibility))]
public class Null2Visibility : ValueConverter
{
    /// <summary>Инвертировать логику преобразования (true → скрывать для null, false → показывать для null)</summary>
    public bool Inverted { get; set; }

    /// <summary>Использовать Collapsed вместо Hidden при скрытии элемента</summary>
    public bool Collapsed { get; set; }

    private Visibility Hidden => Collapsed ? Visibility.Collapsed : Visibility.Hidden;

    /// <inheritdoc />
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => v is null 
        ? !Inverted ? Visibility.Visible : Hidden
        : Inverted ? Visibility.Visible : Hidden;

}