using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

namespace MathCore.WPF.Templates.Selectors
{
    [ContentProperty("Templates")]
    [MarkupExtensionReturnType(typeof(GenericDataTemplateSelector))]
    public class Generic : MarkupExtension
    {
        private readonly List<DataTemplate> _Items = new List<DataTemplate>();

        public IList Templates => _Items;

        public override object ProvideValue(IServiceProvider serviceProvider) => new GenericDataTemplateSelector(_Items);
    }
}
