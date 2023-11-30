using System.Windows;
using System.Windows.Markup;

using MathCore.WPF.ViewModels;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF;

[MarkupExtensionReturnType(typeof(Visibility))]
public class DebugVisible(Visibility DebugVisibility) : MarkupExtension
{
    public DebugVisible() : this(default) { }

    public Visibility Visibility { get; set; } = Visibility.Collapsed;
    public Visibility DebugVisibility { get; set; } = DebugVisibility;

    public override object ProvideValue(IServiceProvider sp) => ViewModel.IsDesignMode ? DebugVisibility : Visibility;
}