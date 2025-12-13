using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters;

/// <summary>Логический конвертер AND (логическое И) для множества значений</summary>
/// <remarks>
/// Выполняет логическую операцию AND для массива булевых значений.
/// Возвращает <c>true</c> только если все входные значения равны <c>true</c>.
/// <para><b>Формула:</b> result = value[0] AND value[1] AND ... AND value[n]</para>
/// <para><b>ConvertBack:</b> Не поддерживается, так как невозможно однозначно восстановить множество исходных булевых значений из одного результата.
/// Например, результат <c>true</c> означает, что все значения были <c>true</c>, но результат <c>false</c> не позволяет определить,
/// какие именно значения были <c>false</c>.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;TextBlock Visibility="{Binding Converter={converters:And}}"&gt;
///     &lt;TextBlock.Visibility&gt;
///         &lt;MultiBinding Converter="{converters:And}"&gt;
///             &lt;Binding Path="IsEnabled"/&gt;
///             &lt;Binding Path="IsVisible"/&gt;
///             &lt;Binding Path="HasPermission"/&gt;
///         &lt;/MultiBinding&gt;
///     &lt;/TextBlock.Visibility&gt;
/// &lt;/TextBlock&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(And))]
public class And : MultiValueValueConverter
{
    /// <summary>Значение по умолчанию, возвращаемое при null</summary>
    public bool NullDefaultValue { get; set; }

    /// <inheritdoc />
    protected override object Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => vv?.Cast<bool>().All(v => v) ?? NullDefaultValue;
}