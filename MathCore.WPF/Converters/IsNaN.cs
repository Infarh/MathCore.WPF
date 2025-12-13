using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки числа на NaN (Not a Number)</summary>
/// <remarks>
/// Проверяет, является ли входное значение NaN.
/// Поддерживает инверсию результата через свойство Inverted.
/// <example>
/// <code>
/// &lt;TextBox Visibility="{Binding Value, Converter={converters:IsNaN}}" /&gt;
/// &lt;TextBox Visibility="{Binding Value, Converter={converters:IsNaN Inverted=True}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(IsNaN))]
[ValueConversion(typeof(double), typeof(bool?))]
public class IsNaN(bool Inverted) : DoubleToBool
{
    /// <summary>Инициализирует конвертер со значением инверсии по умолчанию (false)</summary>
    public IsNaN() : this(false) { }

    /// <summary>Инвертировать результат проверки</summary>
    [ConstructorArgument(nameof(Inverted))]
    public bool Inverted { get; set; } = Inverted;

    /// <summary>Проверка значения на NaN</summary>
    /// <param name="v">Проверяемое значение</param>
    /// <returns>true если значение NaN (или false если Inverted=true), иначе false (или true если Inverted=true)</returns>
    protected override bool? Convert(double v) => Inverted ^ v.IsNaN();
}