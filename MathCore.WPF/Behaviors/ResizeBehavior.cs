using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MathCore.Annotations;
using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Behaviors
{
    /// <summary>A simple Resizing Behavior that makes use of a ResizingAdorner</summary>
    public class ResizeBehavior : Behavior<UIElement>
    {
        private AdornerLayer? _AdornerLayer;
        private FrameworkElement? _Element;
        private UIElement? _AttachedElement;

        protected override void OnAttached()
        {
            _AttachedElement = AssociatedObject;
            _Element = (FrameworkElement) _AttachedElement;

            if (_Element?.Parent != null)
                ((FrameworkElement)_Element.Parent).Loaded += ResizeBehaviorParent_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            _AdornerLayer = null;
        }

        /// <summary>Create the AdornerLayer when Parent for current Element loads</summary>
        private void ResizeBehaviorParent_Loaded(object sender, RoutedEventArgs e)
        {
            if (_AdornerLayer is null)
                _AdornerLayer = AdornerLayer.GetAdornerLayer(sender as Visual ?? throw new InvalidOperationException());
            _AttachedElement!.MouseEnter += AttachedElement_MouseEnter;
        }

        /// <summary>When mouse enters, create a new Resizing Adorner</summary>
        private void AttachedElement_MouseEnter([NotNull] object sender, MouseEventArgs e)
        {
            var resizing_adorner = new ResizingAdorner(sender as UIElement ?? throw new InvalidOperationException());
            resizing_adorner.MouseLeave += ResizingAdorner_MouseLeave;
            _AdornerLayer!.Add(resizing_adorner);
        }

        /// <summary>On mouse leave for the Resizing Adorner, remove the Resizing Adorner from the AdornerLayer</summary>
        private void ResizingAdorner_MouseLeave([CanBeNull] object sender, MouseEventArgs e)
        {
            if (sender != null)
                _AdornerLayer!.Remove(sender as ResizingAdorner ?? throw new InvalidOperationException());
        }
    }
}