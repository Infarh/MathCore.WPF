using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.Data;
using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер преобразования числового размера данных в строку с единицами измерения</summary>
/// <remarks>
/// Преобразует числовое значение байтов в читаемую строку с единицами (КБ, МБ, ГБ и т.д.).
/// Использует основание 1024 для вычисления единиц измерения.
/// <example>
/// <code>
/// &lt;TextBlock Text="{Binding FileSize, Converter={converters:DataLengthString}}" /&gt;
/// &lt;!-- Выведет: "1.5 МБ" для значения 1572864 --&gt;
/// </code>
/// </example>
/// </remarks>
[ValueConversion(typeof(double), typeof(DataLength))]
[MarkupExtensionReturnType(typeof(DataLengthString))]
// ReSharper disable once UnusedMember.Global
public sealed class DataLengthString : ValueConverter
{
    /// <summary>Преобразование числового размера в DataLength</summary>
    /// <param name="v">Размер в байтах</param>
    /// <param name="t">Целевой тип</param>
    /// <param name="p">Параметр конвертера (не используется)</param>
    /// <param name="c">Информация о культуре</param>
    /// <returns>Объект DataLength с форматированным представлением</returns>
    protected override object Convert(object? v, Type? t, object? p, CultureInfo? c) => 
        new DataLength(System.Convert.ToDouble(v), 1024d);
}