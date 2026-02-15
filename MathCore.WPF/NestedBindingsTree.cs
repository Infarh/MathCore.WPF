using System.Globalization;
using System.Windows.Data;

namespace MathCore.WPF;

/// <summary>Дерево вложенных привязок, представляющее иерархическую структуру конвертеров и привязок</summary>
/// <remarks>
/// Этот класс используется внутри <see cref="NestedBinding"/> для построения дерева вложенных привязок.
/// Каждый узел дерева хранит конвертер и коллекцию дочерних узлов, которые могут быть как простыми
/// ссылками на значения (<see cref="NestedBindingNode"/>), так и вложенными деревьями (<see cref="NestedBindingsTree"/>).
/// Класс создаётся автоматически и не предназначен для прямого использования.
/// </remarks>
/// <example>
/// Класс создаётся автоматически при обработке <see cref="NestedBinding"/>.
/// Например, при следующей XAML-разметке:
/// <code><![CDATA[
/// <wpf:NestedBinding Converter="{StaticResource OuterConverter}">
///     <Binding Path="FirstName"/>
///     <Binding Path="LastName"/>
///     <wpf:NestedBinding Converter="{StaticResource InnerConverter}">
///         <Binding Path="Age"/>
///         <Binding Path="Country"/>
///     </wpf:NestedBinding>
/// </wpf:NestedBinding>
/// ]]></code>
/// Будет создано следующее дерево:
/// - Корневое дерево (NestedBindingsTree) с OuterConverter
///   - Узел (NestedBindingNode) с Index=0 для FirstName
///   - Узел (NestedBindingNode) с Index=1 для LastName
///   - Вложенное дерево (NestedBindingsTree) с InnerConverter
///     - Узел (NestedBindingNode) с Index=2 для Age
///     - Узел (NestedBindingNode) с Index=3 для Country
/// </example>
public class NestedBindingsTree() : NestedBindingNode(-1)
{
    /// <summary>Конвертер, используемый для преобразования значений дочерних узлов</summary>
    public IMultiValueConverter Converter { get; set; }

    /// <summary>Параметр, передаваемый конвертеру</summary>
    public object ConverterParameter { get; set; }

    /// <summary>Культура, используемая для преобразования значений</summary>
    public CultureInfo ConverterCulture { get; set; }

    /// <summary>Коллекция дочерних узлов дерева</summary>
    /// <remarks>
    /// Каждый элемент может быть либо <see cref="NestedBindingNode"/> (ссылка на значение привязки),
    /// либо <see cref="NestedBindingsTree"/> (вложенное поддерево с собственным конвертером).
    /// </remarks>
    public List<NestedBindingNode> Nodes { get; } = [];
}