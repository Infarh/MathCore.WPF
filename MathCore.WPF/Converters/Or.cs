using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Логический конвертер OR (логическое ИЛИ) для множества значений</summary>
/// <remarks>
/// Выполняет логическую операцию OR для массива булевых значений.
/// Возвращает <c>true</c> если хотя бы одно входное значение равно <c>true</c>.
/// <para><b>Формула:</b> result = value[0] OR value[1] OR ... OR value[n]</para>
/// <para><b>ConvertBack:</b> Не поддерживается, так как невозможно однозначно восстановить множество исходных булевых значений из одного результата.
/// Например, результат <c>true</c> не позволяет определить, какие именно значения были <c>true</c>, а результат <c>false</c> означает,
/// что все значения были <c>false</c>, но не определяет их количество.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;Button IsEnabled="{Binding Converter={converters:Or}}"&gt;
///     &lt;Button.IsEnabled&gt;
///         &lt;MultiBinding Converter="{converters:Or}"&gt;
///             &lt;Binding Path="CanEdit"/&gt;
///             &lt;Binding Path="CanDelete"/&gt;
///             &lt;Binding Path="IsAdmin"/&gt;
///         &lt;/MultiBinding&gt;
///     &lt;/Button.IsEnabled&gt;
/// &lt;/Button&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(Or))]
public class Or : MultiValueValueConverter
{
    /// <summary>Значение по умолчанию, возвращаемое при null</summary>
    public bool NullDefaultValue { get; set; }

    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => vv?.Cast<bool>().Any(v => v) ?? NullDefaultValue;
}