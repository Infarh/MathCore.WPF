using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MathCore.Annotations;

namespace MathCore.WPF.Styles.Selectors
{
    public class GenericStyleSelector : StyleSelector
    {
        private readonly Dictionary<Type, Style> _Styles;

        public GenericStyleSelector([NotNull] IEnumerable<Style> Styles) => _Styles = Styles.ToDictionary(s => s.TargetType);

        public override Style? SelectStyle(object? item, DependencyObject container) => 
            item != null && _Styles.TryGetValue(item.GetType(), out var s) 
                ? s 
                : null;
    }
}