using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MathCore.WPF.Shapes
{
    public abstract class ShapeBase : Shape
    {
        protected static double min(double x, double y) => Math.Min(x, y);
        protected static double max(double x, double y) => Math.Max(x, y);

        protected Rect _VisibleRect;

        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);
            var t = StrokeThickness;
            var m = t / 2;
            _VisibleRect = size.IsEmpty || size.Width.Equals(0d) || size.Height.Equals(0d)
                ? Rect.Empty
                : new Rect(m, m, max(0, size.Width - t), max(0, size.Height - t));

            switch(Stretch)
            {
                case Stretch.None:
                    //_Rect.Width = _Rect.Height = 0;
                    break;
                case Stretch.Fill:
                    break;
                case Stretch.Uniform:
                    if(_VisibleRect.Width > _VisibleRect.Height)
                        _VisibleRect.Width = _VisibleRect.Height;
                    else
                        _VisibleRect.Height = _VisibleRect.Width;
                    break;
                case Stretch.UniformToFill:
                    if(_VisibleRect.Width < _VisibleRect.Height)
                        _VisibleRect.Width = _VisibleRect.Height;
                    else
                        _VisibleRect.Height = _VisibleRect.Width;
                    break;
            }
            return size;
        }

        protected override Size MeasureOverride(Size ConstraintSize)
        {
            _VisibleRect = Rect.Empty;
            return base.MeasureOverride(ConstraintSize);
        }
    }
}