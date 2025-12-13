using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер логической операции ИЛИ (OR) для множества булевых значений</summary>
/// <remarks>
/// Выполняет логическую операцию ИЛИ над всеми входными булевыми значениями
/// <para>Возвращаемые значения:</para>
/// <list type="bullet">
/// <item><description>Хотя бы одно значение true → true</description></item>
/// <item><description>Все значения false → false</description></item>
/// <item><description>Пустой массив или null → значение свойства NullDefaultValue</description></item>
/// </list>
/// <para>ConvertBack не поддерживается, так как невозможно однозначно восстановить исходный набор булевых значений:</para>
/// <list type="bullet">
/// <item><description>true → {true, false} или {false, true} и т.д. - множество вариантов</description></item>
/// <item><description>false → {false, false, ...} - бесконечное множество решений</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code language="xaml"><![CDATA[
/// <!-- Кнопка активна если выполнено хотя бы одно условие -->
/// <Button>
///     <Button.IsEnabled>
///         <MultiBinding Converter="{x:Static converters:Or}">
///             <Binding Path="CanEdit" />
///             <Binding Path="CanView" />
///             <Binding Path="IsAdmin" />
///         </MultiBinding>
///     </Button.IsEnabled>
/// </Button>
/// 
/// <!-- С использованием NullDefaultValue -->
/// <TextBlock>
///     <TextBlock.Visibility>
///         <MultiBinding Converter="{converters:Or NullDefaultValue=False}">
///             <Binding Path="ShowIfCondition1" />
///             <Binding Path="ShowIfCondition2" />
///         </MultiBinding>
///     </TextBlock.Visibility>
/// </TextBlock>
/// ]]></code>
/// </example>
[MarkupExtensionReturnType(typeof(Or))]
public class Or : MultiValueValueConverter
{
    /// <summary>Значение по умолчанию, возвращаемое если входной массив равен null или пуст</summary>
    public bool NullDefaultValue { get; set; }

    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => vv?.Cast<bool>().Any(v => v) ?? NullDefaultValue;
}