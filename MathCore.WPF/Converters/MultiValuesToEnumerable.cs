using System.Collections;
using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер объединения множества значений в перечисление</summary>
/// <remarks>
/// Преобразует массив значений из MultiBinding в перечисление и обратно.
/// Поддерживает двустороннее связывание с автоматическим преобразованием типов.
/// <example>
/// <code>
/// &lt;ItemsControl ItemsSource="{MultiBinding Converter={converters:MultiValuesToEnumerable}}"&gt;
///     &lt;Binding Path="Value1"/&gt;
///     &lt;Binding Path="Value2"/&gt;
///     &lt;Binding Path="Value3"/&gt;
/// &lt;/ItemsControl&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(MultiValuesToEnumerable))]
public class MultiValuesToEnumerable : MultiValueValueConverter
{
    /// <summary>Преобразование массива значений в перечисление</summary>
    /// <param name="vv">Массив значений из MultiBinding</param>
    /// <param name="t">Целевой тип (не используется)</param>
    /// <param name="p">Параметр конвертера (не используется)</param>
    /// <param name="c">Информация о культуре (не используется)</param>
    /// <returns>Массив значений как перечисление</returns>
    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => vv;

    /// <summary>Обратное преобразование перечисления в массив значений</summary>
    /// <param name="v">Перечисление значений</param>
    /// <param name="tt">Массив целевых типов для каждого значения</param>
    /// <param name="p">Параметр конвертера (не используется)</param>
    /// <param name="c">Информация о культуре (не используется)</param>
    /// <returns>Массив значений с преобразованными типами</returns>
    protected override object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) => 
        (v as IEnumerable)?.Cast<object>().Zip(tt!, System.Convert.ChangeType).ToArray()!;
}