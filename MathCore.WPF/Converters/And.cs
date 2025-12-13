using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер логического И для нескольких булевых значений</summary>
/// <remarks>
/// Возвращает true только если все входные значения равны true.
/// Используется в XAML для объединения нескольких условий через логическое И.
/// <example>
/// <code>
/// &lt;MultiBinding Converter="{converters:And}"&gt;
///     &lt;Binding Path="IsEnabled"/&gt;
///     &lt;Binding Path="IsValid"/&gt;
///     &lt;Binding Path="IsReady"/&gt;
/// &lt;/MultiBinding&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(And))]
public class And : MultiValueValueConverter
{
    /// <summary>Значение по умолчанию, возвращаемое если массив значений равен null</summary>
    public bool NullDefaultValue { get; set; }

    /// <summary>Преобразование массива булевых значений в результат логического И</summary>
    /// <param name="vv">Массив булевых значений</param>
    /// <param name="t">Целевой тип (не используется)</param>
    /// <param name="p">Параметр конвертера (не используется)</param>
    /// <param name="c">Информация о культуре (не используется)</param>
    /// <returns>true если все значения true, иначе false; при null возвращает NullDefaultValue</returns>
    protected override object Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => vv?.Cast<bool>().All(v => v) ?? NullDefaultValue;
}