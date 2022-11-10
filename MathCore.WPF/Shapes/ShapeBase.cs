using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MathCore.WPF.Shapes;

public abstract class ShapeBase : Shape
{
    protected Rect _VisibleRect;

    protected override Size ArrangeOverride(Size FinalSize)
    {
        var size = base.ArrangeOverride(FinalSize);
        var t    = StrokeThickness;
        var m    = t / 2;
        var y    = size.Width - t;
        var y1   = size.Height - t;
        _VisibleRect = size.IsEmpty || size.Width.Equals(0d) || size.Height.Equals(0d)
            ? Rect.Empty
            : new Rect(m, m, Math.Max(0, y), Math.Max(0, y1));

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