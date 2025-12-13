using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер логического ИЛИ для нескольких булевых значений</summary>
/// <remarks>
/// Возвращает true если хотя бы одно из входных значений равно true.
/// Используется в XAML для объединения нескольких условий через логическое ИЛИ.
/// <example>
/// <code>
/// &lt;MultiBinding Converter="{converters:Or}"&gt;
///     &lt;Binding Path="HasErrors"/&gt;
///     &lt;Binding Path="HasWarnings"/&gt;
///     &lt;Binding Path="IsModified"/&gt;
/// &lt;/MultiBinding&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(Or))]
public class Or : MultiValueValueConverter
{
    /// <summary>Значение по умолчанию, возвращаемое если массив значений равен null</summary>
    public bool NullDefaultValue { get; set; }

    /// <summary>Преобразование массива булевых значений в результат логического ИЛИ</summary>
    /// <param name="vv">Массив булевых значений</param>
    /// <param name="t">Целевой тип (не используется)</param>
    /// <param name="p">Параметр конвертера (не используется)</param>
    /// <param name="c">Информация о культуре (не используется)</param>
    /// <returns>true если хотя бы одно значение true, иначе false; при null возвращает NullDefaultValue</returns>
    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => vv?.Cast<bool>().Any(v => v) ?? NullDefaultValue;
}