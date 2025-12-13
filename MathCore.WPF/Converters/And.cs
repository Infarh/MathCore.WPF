using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер логической операции И (AND) для множества булевых значений</summary>
/// <remarks>
/// Выполняет логическую операцию И над всеми входными булевыми значениями
/// <para>Возвращаемые значения:</para>
/// <list type="bullet">
/// <item><description>Все значения true → true</description></item>
/// <item><description>Хотя бы одно значение false → false</description></item>
/// <item><description>Пустой массив или null → значение свойства NullDefaultValue</description></item>
/// </list>
/// <para>ConvertBack не поддерживается, так как невозможно однозначно восстановить исходный набор булевых значений:</para>
/// <list type="bullet">
/// <item><description>true → {true, true, ...} - бесконечное множество решений</description></item>
/// <item><description>false → {false, true} или {true, false} и т.д. - множество вариантов</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code language="xaml"><![CDATA[
/// <!-- Кнопка активна только если все условия выполнены -->
/// <Button>
///     <Button.IsEnabled>
///         <MultiBinding Converter="{x:Static converters:And}">
///             <Binding Path="IsConnected" />
///             <Binding Path="IsAuthenticated" />
///             <Binding Path="HasPermission" />
///         </MultiBinding>
///     </Button.IsEnabled>
/// </Button>
/// 
/// <!-- С использованием NullDefaultValue -->
/// <TextBlock>
///     <TextBlock.Visibility>
///         <MultiBinding Converter="{converters:And NullDefaultValue=True}">
///             <Binding Path="Condition1" />
///             <Binding Path="Condition2" />
///         </MultiBinding>
///     </TextBlock.Visibility>
/// </TextBlock>
/// ]]></code>
/// </example>
[MarkupExtensionReturnType(typeof(And))]
public class And : MultiValueValueConverter
{
    /// <summary>Значение по умолчанию, возвращаемое если входной массив равен null или пуст</summary>
    public bool NullDefaultValue { get; set; }

    /// <inheritdoc />
    protected override object Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => vv?.Cast<bool>().All(v => v) ?? NullDefaultValue;
}