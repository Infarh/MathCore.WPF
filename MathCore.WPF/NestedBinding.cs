using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF;

/// <summary>Расширение разметки для создания вложенных привязок.</summary>
[Copyright("adeptuss", url = "https://habr.com/ru/post/277157/")]
[ContentProperty(nameof(Bindings))]
[MarkupExtensionReturnType(typeof(object))]
public class NestedBinding : MarkupExtension
{
    /// <summary>Коллекция базовых привязок.</summary>
    public Collection<BindingBase> Bindings { get; } = [];

    /// <summary>Преобразователь, используемый для преобразования значений привязок.</summary>
    public IMultiValueConverter Converter { get; set; }

    /// <summary>Параметр, передаваемый преобразователю.</summary>
    public object ConverterParameter { get; set; }

    /// <summary>Культура, используемая для преобразования значений.</summary>
    public CultureInfo ConverterCulture { get; set; }

    /// <summary>Возвращает значение расширения разметки.</summary>
    /// <param name="Services">Провайдер служб.</param>
    /// <returns>Значение расширения разметки.</returns>
    public override object ProvideValue(IServiceProvider Services)
    {
        // Проверяем, что коллекция привязок не пуста.
        if (Bindings.NotNull().Count == 0)
            throw new ArgumentException(nameof(Bindings));

        // Проверяем, что преобразователь задан.
        if (Converter is null)
            throw new InvalidOperationException("Не заданы Converter");

        // Получаем целевой объект и свойство.
        var target = (IProvideValueTarget)Services.GetService(typeof(IProvideValueTarget));

        // Если целевым объектом является коллекция привязок, возвращаем привязку к текущему объекту.
        if (target.TargetObject is Collection<BindingBase>)
        {
            var binding = new Binding { Source = this };
            return binding;
        }

        // Создаём многоуровневую привязку.
        var multi_binding = new MultiBinding { Mode = BindingMode.OneWay };

        // Строим дерево вложенных привязок.
        var tree = GetNestedBindingsTree(this, multi_binding);

        // Создаём преобразователь для многоуровневой привязки.
        var converter = new NestedBindingConverter(tree);
        multi_binding.Converter = converter;

        // Возвращаем значение многоуровневой привязки.
        return multi_binding.ProvideValue(Services);
    }

    /// <summary>Строит дерево вложенных привязок.</summary>
    /// <param name="NestedBinding">Текущий объект вложенной привязки.</param>
    /// <param name="MultiBinding">Многоуровневая привязка.</param>
    /// <returns>Дерево вложенных привязок.</returns>
    private static NestedBindingsTree GetNestedBindingsTree(NestedBinding NestedBinding, MultiBinding MultiBinding)
    {
        // Создаём корень дерева.
        var tree = new NestedBindingsTree
        {
            Converter = NestedBinding.Converter,
            ConverterParameter = NestedBinding.ConverterParameter,
            ConverterCulture = NestedBinding.ConverterCulture
        };

        // Обходим коллекцию привязок.
        foreach (var binding_base in NestedBinding.Bindings)
        {
            // Если привязка является вложенной, рекурсивно строим дерево.
            if (binding_base is Binding { Source: NestedBinding child_nested_binding, Converter: null })
            {
                tree.Nodes.Add(GetNestedBindingsTree(child_nested_binding, MultiBinding));
                continue;
            }

            // Добавляем узел в дерево и привязку в многоуровневую привязку.
            tree.Nodes.Add(new(MultiBinding.Bindings.Count));
            MultiBinding.Bindings.Add(binding_base);
        }

        // Возвращаем дерево вложенных привязок.
        return tree;
    }
}