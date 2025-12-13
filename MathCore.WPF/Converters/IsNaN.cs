using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки числового значения на NaN (Not-a-Number)</summary>
/// <remarks>
/// Проверяет является ли входное значение типа double специальным значением NaN
/// <para>Возвращаемые значения (в нормальном режиме):</para>
/// <list type="bullet">
/// <item><description>NaN → true</description></item>
/// <item><description>любое числовое значение (включая ±∞) → false</description></item>
/// </list>
/// <para>При установленном свойстве Inverted=true логика инвертируется:</para>
/// <list type="bullet">
/// <item><description>NaN → false</description></item>
/// <item><description>любое числовое значение → true</description></item>
/// </list>
/// <para>ConvertBack не поддерживается, так как невозможно однозначно восстановить исходное числовое значение</para>
/// </remarks>
/// <example>
/// <code language="xaml"><![CDATA[
/// <!-- Проверка на NaN -->
/// <TextBlock Text="Некорректное значение" 
///            Visibility="{Binding Value, Converter={converters:IsNaN}}" />
/// 
/// <!-- Инвертированная проверка (валидное число) -->
/// <Button IsEnabled="{Binding Coefficient, Converter={converters:IsNaN Inverted=True}}" />
/// ]]></code>
/// </example>
[MarkupExtensionReturnType(typeof(IsNaN))]
[ValueConversion(typeof(double), typeof(bool?))]
public class IsNaN(bool Inverted) : DoubleToBool
{
    public IsNaN() : this(false) { }

    /// <summary>Инвертировать результат проверки (true → не NaN, false → NaN)</summary>
    [ConstructorArgument(nameof(Inverted))]
    public bool Inverted { get; set; } = Inverted;

    /// <inheritdoc />
    protected override bool? Convert(double v) => Inverted ^ v.IsNaN();
}