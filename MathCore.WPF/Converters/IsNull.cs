using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки значения на null</summary>
/// <remarks>
/// Возвращает булево значение, указывающее является ли входное значение null
/// <para>Возвращаемые значения (в нормальном режиме):</para>
/// <list type="bullet">
/// <item><description>null → true</description></item>
/// <item><description>любое не-null значение → false</description></item>
/// </list>
/// <para>При установленном свойстве Inverted=true логика инвертируется:</para>
/// <list type="bullet">
/// <item><description>null → false</description></item>
/// <item><description>любое не-null значение → true</description></item>
/// </list>
/// <para>ConvertBack не поддерживается, так как из булева значения невозможно однозначно восстановить исходный объект</para>
/// </remarks>
/// <example>
/// <code language="xaml"><![CDATA[
/// <!-- Проверка на null -->
/// <TextBlock Visibility="{Binding User, Converter={converters:IsNull}}" />
/// 
/// <!-- Инвертированная проверка (не null) -->
/// <Button IsEnabled="{Binding Data, Converter={converters:IsNull Inverted=True}}" />
/// ]]></code>
/// </example>
[MarkupExtensionReturnType(typeof(IsNull))]
public class IsNull(bool Inverted) : ValueConverter
{
    public IsNull() : this(false) { }

    /// <summary>Инвертировать результат проверки (true → не null, false → null)</summary>
    [ConstructorArgument(nameof(Inverted))]
    public bool Inverted { get; set; } = Inverted;

    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) => Inverted ^ (v is null);
}