using System.Windows;
using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Behaviors;

public class Focused : Behavior<FrameworkElement>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Initialized += (s, _) => (s as FrameworkElement)?.Focus();
        AssociatedObject.Focus();
    }
}