using System.Globalization;
using System.Windows.Data;

namespace MathCore.WPF;

/// <summary>Преобразователь значений для вложенных привязок</summary>
/// <remarks>
/// Этот класс используется внутри <see cref="NestedBinding"/> для обработки дерева вложенных привязок.
/// Преобразователь рекурсивно обходит дерево и применяет соответствующие конвертеры к значениям.
/// </remarks>
/// <example>
/// Этот класс создаётся автоматически при использовании <see cref="NestedBinding"/> и не требует ручного создания.
/// Пример пользовательских конвертеров для использования с вложенными привязками:
/// <code><![CDATA[
/// // Конвертер для объединения строк
/// public class StringConcatConverter : IMultiValueConverter
/// {
///     public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
///     {
///         return string.Join(" ", values.Select(v => v?.ToString() ?? ""));
///     }
///     
///     public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
///     {
///         throw new NotSupportedException();
///     }
/// }
/// 
/// // Конвертер для математических операций
/// public class MathConverter : IMultiValueConverter
/// {
///     public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
///     {
///         if (values.Length != 2) return 0;
///         var a = System.Convert.ToDouble(values[0]);
///         var b = System.Convert.ToDouble(values[1]);
///         return a + b;
///     }
///     
///     public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
///     {
///         throw new NotSupportedException();
///     }
/// }
/// ]]></code>
/// Использование в XAML:
/// <code><![CDATA[
/// <TextBlock>
///     <TextBlock.Text>
///         <wpf:NestedBinding Converter="{StaticResource StringConcatConverter}">
///             <Binding Path="FirstName"/>
///             <wpf:NestedBinding Converter="{StaticResource MathConverter}">
///                 <Binding Path="Value1"/>
///                 <Binding Path="Value2"/>
///             </wpf:NestedBinding>
///         </wpf:NestedBinding>
///     </TextBlock.Text>
/// </TextBlock>
/// ]]></code>
/// В этом примере MathConverter сначала суммирует Value1 и Value2, затем StringConcatConverter объединяет FirstName с результатом суммы.
/// </example>
public class NestedBindingConverter(NestedBindingsTree tree) : IMultiValueConverter
{
    /// <summary>Дерево вложенных привязок</summary>
    private NestedBindingsTree Tree { get; } = tree;

    /// <summary>Преобразует значения привязок в значение целевого типа</summary>
    /// <param name="values">Массив значений, полученных от привязок</param>
    /// <param name="TargetType">Целевой тип преобразования</param>
    /// <param name="parameter">Параметр преобразователя</param>
    /// <param name="culture">Культура для преобразования</param>
    /// <returns>Преобразованное значение</returns>
    public object? Convert(object[]? values, Type TargetType, object? parameter, CultureInfo culture)
    {
        var value = GetTreeValue(Tree, values, TargetType, culture);
        return value;
    }

    /// <summary>Рекурсивно вычисляет значение для узла дерева вложенных привязок</summary>
    /// <param name="tree">Узел дерева вложенных привязок</param>
    /// <param name="values">Массив значений, полученных от привязок</param>
    /// <param name="TargetType">Целевой тип преобразования</param>
    /// <param name="culture">Культура для преобразования</param>
    /// <returns>Преобразованное значение для данного узла дерева</returns>
    private static object? GetTreeValue(NestedBindingsTree tree, object[]? values, Type TargetType, CultureInfo culture)
    {
        var objects = new object[tree.Nodes.Count];
        for (var i = 0; i < objects.Length; i++)
        {
            var element = tree.Nodes[i];
            objects[i] = element is NestedBindingsTree bindings_tree
                ? GetTreeValue(bindings_tree, values, TargetType, culture)
                : values[element.Index];
        }

        var value = tree.Converter.Convert(objects, TargetType, tree.ConverterParameter, tree.ConverterCulture ?? culture);
        return value;
    }

    /// <summary>Обратное преобразование не поддерживается</summary>
    /// <param name="value">Значение для обратного преобразования</param>
    /// <param name="TargetTypes">Целевые типы</param>
    /// <param name="parameter">Параметр преобразователя</param>
    /// <param name="culture">Культура для преобразования</param>
    /// <returns>Массив преобразованных значений</returns>
    /// <exception cref="NotSupportedException">Обратное преобразование не поддерживается</exception>
    public object?[]? ConvertBack(object? value, Type[] TargetTypes, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}