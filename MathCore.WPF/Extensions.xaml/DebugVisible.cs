using System;
using System.Windows;
using System.Windows.Markup;
using MathCore.Annotations;
using MathCore.WPF.ViewModels;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF
{
    [MarkupExtensionReturnType(typeof(Visibility))]
    public class DebugVisible : MarkupExtension
    {
        public Visibility Visibility { get; set; } = Visibility.Collapsed;
        public Visibility DebugVisibility { get; set; } = Visibility.Visible;

        public DebugVisible() { }
        public DebugVisible(Visibility DebugVisibility) => this.DebugVisibility = DebugVisibility;

        [NotNull] public override object ProvideValue(IServiceProvider sp) => ViewModel.IsDesignMode ? DebugVisibility : Visibility;
    }
}
