using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MathCore.Annotations;

namespace MathCore.WPF
{
    public class ResizingAdorner : Adorner
    {
        private readonly Thumb _TopLeft;
        private readonly Thumb _TopRight;
        private readonly Thumb _BottomLeft;
        private readonly Thumb _BottomRight;
        private readonly VisualCollection _VisualChildren;

        protected override int VisualChildrenCount => _VisualChildren.Count;

        /// <inheritdoc />
        public ResizingAdorner(UIElement AdornedElement) : base(AdornedElement)
        {
            _VisualChildren = new VisualCollection(this);
            BuildAdornerCorner(ref _TopLeft, Cursors.SizeNWSE);
            BuildAdornerCorner(ref _TopRight, Cursors.SizeNESW);
            BuildAdornerCorner(ref _BottomLeft, Cursors.SizeNESW);
            BuildAdornerCorner(ref _BottomRight, Cursors.SizeNWSE);

            _BottomLeft.DragDelta += HandleBottomLeft;
            _BottomRight.DragDelta += HandleBottomRight;
            _TopLeft.DragDelta += HandleTopLeft;
            _TopRight.DragDelta += HandleTopRight;
        }

        // Handler for resizing from the bottom-right.
        private void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            if (AdornedElement is not FrameworkElement element || sender is not Thumb thumb) return;
            //var parentElement = adorned_element.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(element);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            element.Width = Math.Max(element.Width + args.HorizontalChange, thumb.DesiredSize.Width);
            element.Height = Math.Max(args.VerticalChange + element.Height, thumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-right.

        private void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            if (AdornedElement is not FrameworkElement element || sender is not Thumb thumb) return;
            //var parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(element);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            element.Width = Math.Max(element.Width + args.HorizontalChange, thumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);

            var height_old = element.Height;
            var height_new = Math.Max(element.Height - args.VerticalChange, thumb.DesiredSize.Height);
            var top_old = Canvas.GetTop(element);
            element.Height = height_new;
            Canvas.SetTop(element, top_old - (height_new - height_old));
        }

        // Handler for resizing from the top-left.

        private void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {
            if (AdornedElement is not FrameworkElement element || sender is not Thumb thumb) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(element);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            //adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);

            var width_old = element.Width;
            var width_new = Math.Max(element.Width - args.HorizontalChange, thumb.DesiredSize.Width);
            var left_old = Canvas.GetLeft(element);
            element.Width = width_new;
            Canvas.SetLeft(element, left_old - (width_new - width_old));

            var height_old = element.Height;
            var height_new = Math.Max(element.Height - args.VerticalChange, thumb.DesiredSize.Height);
            var top_old = Canvas.GetTop(element);
            element.Height = height_new;
            Canvas.SetTop(element, top_old - (height_new - height_old));
        }

        // Handler for resizing from the bottom-left.

        private void HandleBottomLeft(object sender, DragDeltaEventArgs args)
        {
            if (AdornedElement is not FrameworkElement element || sender is not Thumb thumb) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(element);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            //adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            element.Height = Math.Max(args.VerticalChange + element.Height, thumb.DesiredSize.Height);

            var width_old = element.Width;
            var width_new = Math.Max(element.Width - args.HorizontalChange, thumb.DesiredSize.Width);
            var left_old = Canvas.GetLeft(element);
            element.Width = width_new;
            Canvas.SetLeft(element, left_old - (width_new - width_old));
        }

        // Arrange the Adorners.

        protected override Size ArrangeOverride(Size FinalSize)
        {
            // desiredWidth and desiredHeight are the width and height of the element that's being adorned.  
            // These will be used to place the ResizingAdorner at the corners of the adorned element.  
            var size_width = AdornedElement.DesiredSize.Width;
            var desired_height = AdornedElement.DesiredSize.Height;
            // adornerWidth & adornerHeight are used for placement as well.
            var adorner_width = DesiredSize.Width;
            var adorner_height = DesiredSize.Height;

            _TopLeft.Arrange(new Rect(-adorner_width / 2, -adorner_height / 2, adorner_width, adorner_height));
            _TopRight.Arrange(new Rect(size_width - adorner_width / 2, -adorner_height / 2, adorner_width, adorner_height));
            _BottomLeft.Arrange(new Rect(-adorner_width / 2, desired_height - adorner_height / 2, adorner_width, adorner_height));
            _BottomRight.Arrange(new Rect(size_width - adorner_width / 2, desired_height - adorner_height / 2, adorner_width, adorner_height));

            // Return the final size.
            return FinalSize;
        }

        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        private void BuildAdornerCorner(ref Thumb thumb, Cursor cursor)
        {
            if (thumb != null) return;
            // Set some arbitrary visual characteristics.
            _VisualChildren.Add(thumb = new Thumb
            {
                Cursor = cursor,
                Height = 10,
                Width = 10,
                Opacity = 0.40,
                Background = new SolidColorBrush(Colors.MediumBlue)
            });
        }

        // This method ensures that the Widths and Heights are initialized.  Sizing to content produces
        // Width and Height values of Double.NaN.  Because this Adorner explicitly resizes, the Width and Height
        // need to be set first.  It also sets the maximum size of the adorned element.
        private static void EnforceSize(FrameworkElement element)
        {
            if (element.Width.Equals(double.NaN))
                element.Width = element.DesiredSize.Width;
            if (element.Height.Equals(double.NaN))
                element.Height = element.DesiredSize.Height;

            if (element.Parent is not FrameworkElement parent) return;
            element.MaxHeight = parent.ActualHeight;
            element.MaxWidth = parent.ActualWidth;
        }

        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override Visual GetVisualChild(int index) => _VisualChildren[index];

        ///// <inheritdoc />
        //protected override void OnRender(DrawingContext drawing)
        //{
        //    var element_rect = new Rect(this.AdornedElement.DesiredSize);

        //    var brush = new SolidColorBrush(Colors.Green) { Opacity = 0.2 };
        //    var pen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
        //    var radius = 5.0;

        //    drawing.DrawEllipse(brush, pen, element_rect.TopLeft, radius, radius);
        //    drawing.DrawEllipse(brush, pen, element_rect.TopRight, radius, radius);
        //    drawing.DrawEllipse(brush, pen, element_rect.BottomLeft, radius, radius);
        //    drawing.DrawEllipse(brush, pen, element_rect.BottomRight, radius, radius);
        //}
    }
}
