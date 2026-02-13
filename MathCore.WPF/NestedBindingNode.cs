namespace MathCore.WPF;

/// <summary>Узел дерева вложенных привязок, представляющий ссылку на значение в массиве привязок</summary>
/// <remarks>
/// Этот класс используется внутри <see cref="NestedBinding"/> для построения дерева вложенных привязок.
/// Каждый узел хранит индекс значения в массиве, передаваемом в конвертер.
/// Класс создаётся автоматически и не предназначен для прямого использования.
/// </remarks>
/// <example>
/// Класс создаётся автоматически при обработке <see cref="NestedBinding"/>.
/// Например, при следующей структуре привязок:
/// <code><![CDATA[
/// <wpf:NestedBinding Converter="{StaticResource OuterConverter}">
///     <Binding Path="Property1"/>              <!-- NestedBindingNode с Index=0 -->
///     <Binding Path="Property2"/>              <!-- NestedBindingNode с Index=1 -->
///     <wpf:NestedBinding Converter="{StaticResource InnerConverter}">
///         <Binding Path="Property3"/>          <!-- NestedBindingNode с Index=2 -->
///         <Binding Path="Property4"/>          <!-- NestedBindingNode с Index=3 -->
///     </wpf:NestedBinding>
/// </wpf:NestedBinding>
/// ]]></code>
/// Будет создано дерево, где для каждой обычной привязки создаётся <see cref="NestedBindingNode"/> с соответствующим индексом.
/// </example>
public class NestedBindingNode(int index)
{
    /// <summary>Индекс значения в массиве значений привязок</summary>
    public int Index => index;

    /// <summary>Возвращает строковое представление узла</summary>
    /// <returns>Строковое представление индекса узла</returns>
    public override string ToString() => index.ToString();
}