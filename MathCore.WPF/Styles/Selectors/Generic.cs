using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MathCore.WPF.Styles.Selectors
{
    [ContentProperty("Styles")]
    [MarkupExtensionReturnType(typeof(GenericStyleSelector))]
    public class Generic : MarkupExtension
    {
        private readonly List<Style> _Items = new List<Style>();

        public IList Styles => _Items;

        public override object ProvideValue(IServiceProvider serviceProvider) => new GenericStyleSelector(_Items);
    }

    public class GenericStyleSelector : StyleSelector
    {
        private readonly Dictionary<Type, Style> _Styles;

        public GenericStyleSelector(IEnumerable<Style> Styles) { _Styles = Styles.ToDictionary(s => s.TargetType); }

        public override Style SelectStyle(object item, DependencyObject container) => item != null && _Styles.TryGetValue(item.GetType(), out var s) ? s : null;
    }
}
