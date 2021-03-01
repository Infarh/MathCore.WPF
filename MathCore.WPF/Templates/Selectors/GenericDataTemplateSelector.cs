using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MathCore.Annotations;

namespace MathCore.WPF.Templates.Selectors
{
    public class GenericDataTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<Type, DataTemplate> _Templates;

        public GenericDataTemplateSelector([NotNull] IEnumerable<DataTemplate> Styles) =>
            _Templates = Styles.ToDictionary(t =>
            {
                var type = t.DataType as Type;
                if(type != null)
                    return type;
                throw new InvalidCastException(
                    $"Невозможно привести {t.DataType} типа {t.DataType.GetType()} к {typeof(Type)}");
            });

        public override DataTemplate? SelectTemplate(object item, DependencyObject container) => 
            item != null && _Templates.TryGetValue(item.GetType(), out var t) ? t : null;
    }
}