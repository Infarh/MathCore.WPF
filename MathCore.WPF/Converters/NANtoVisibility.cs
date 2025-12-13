using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер значения NaN в состояние видимости Visibility</summary>
/// <remarks>
/// Преобразует числовое значение типа double в значение Visibility на основе того, является ли оно NaN
/// <para>Возвращаемые значения (в нормальном режиме):</para>
/// <list type="bullet">
/// <item><description>NaN → Hidden/Collapsed (в зависимости от свойства Collapsed)</description></item>
/// <item><description>валидное число → Visible</description></item>
/// <item><description>null → null</description></item>
/// </list>
/// <para>При установленном свойстве Inverted=true логика инвертируется:</para>
/// <list type="bullet">
/// <item><description>NaN → Visible</description></item>
/// <item><description>валидное число → Hidden/Collapsed</description></item>
/// </list>
/// <para>Свойство Collapsed определяет тип скрытия: true → Collapsed (элемент не занимает места), false → Hidden (место сохраняется)</para>
/// <para>ConvertBack не поддерживается, так как из состояния видимости невозможно восстановить исходное числовое значение</para>
/// </remarks>
/// <example>
/// <code language="xaml"><![CDATA[
/// <!-- Скрыть элемент если значение NaN -->
/// <TextBlock Text="Значение корректно" 
///            Visibility="{Binding Value, Converter={converters:NaNtoVisibility}}" />
/// 
/// <!-- Показать предупреждение если значение NaN (инвертированный режим) -->
/// <TextBlock Text="Некорректное значение!" Foreground="Red"
///            Visibility="{Binding Value, Converter={converters:NaNtoVisibility Inverted=True}}" />
/// 
/// <!-- Использование Collapsed для освобождения места -->
/// <Border Visibility="{Binding Coefficient, 
///         Converter={converters:NaNtoVisibility Collapsed=True}}" />
/// ]]></code>
/// </example>
[MarkupExtensionReturnType(typeof(NaNtoVisibility))]
[ValueConversion(typeof(double), typeof(Visibility))]
public class NaNtoVisibility : ValueConverter
{
    /// <summary>Инвертировать логику преобразования (true → показывать для NaN, false → скрывать для NaN)</summary>
    public bool Inverted { get; set; }

    /// <summary>Использовать Collapsed вместо Hidden при скрытии элемента</summary>
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