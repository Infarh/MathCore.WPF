using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки значения на NaN (Not a Number)</summary>
/// <remarks>
/// Проверяет, является ли входное значение NaN (неопределённое число).
/// Возвращает <c>true</c>, если значение равно <see cref="double.NaN"/>, иначе <c>false</c>.
/// <para><b>Формула:</b> result = IsNaN(value) [или !IsNaN(value) если Inverted=true]</para>
/// <para><b>ConvertBack:</b> Не поддерживается, так как невозможно восстановить исходное число из булевого значения.
/// Результат <c>true</c> означает NaN, но <c>false</c> может означать любое валидное число.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Проверка на NaN --&gt;
/// &lt;TextBlock Visibility="{Binding Value, Converter={converters:IsNaN}}" /&gt;
/// 
/// &lt;!-- Инвертированная проверка (видим если НЕ NaN) --&gt;
/// &lt;TextBlock Visibility="{Binding Value, Converter={converters:IsNaN Inverted=True}}" /&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(IsNaN))]
[ValueConversion(typeof(double), typeof(bool?))]
public class IsNaN(bool Inverted) : DoubleToBool
{
    public IsNaN() : this(false) { }

    /// <summary>Инвертировать результат проверки</summary>
    [ConstructorArgument(nameof(Inverted))]
    public bool Inverted { get; set; } = Inverted;

    /// <inheritdoc />
    protected override bool? Convert(double v) => Inverted ^ v.IsNaN();
}