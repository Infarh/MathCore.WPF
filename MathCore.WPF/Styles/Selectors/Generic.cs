using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using MathCore.Annotations;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Styles.Selectors
{
    [ContentProperty("Styles")]
    [MarkupExtensionReturnType(typeof(GenericStyleSelector))]
    public class Generic : MarkupExtension
    {
        private readonly List<Style> _Items = new List<Style>();

        public IList Styles => _Items;

        [NotNull] public override object ProvideValue(IServiceProvider sp) => new GenericStyleSelector(_Items);
    }
}
