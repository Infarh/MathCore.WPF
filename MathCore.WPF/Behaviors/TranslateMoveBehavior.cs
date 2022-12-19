using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors;

public class TranslateMoveBehavior : Behavior<UIElement>
{
    private TranslateTransform _Transform;

    protected override void OnAttached()
    {
        base.OnAttached();

        var obj = AssociatedObject;
        obj.MouseDown += OnMouseDown;

        var transform = new TranslateTransform();
        _Transform = transform;

        switch (obj.RenderTransform)
        {
            case null:
                obj.RenderTransform = transform;
                break;

            case TransformGroup group:
                group.Children.Add(transform);
                break;

            default:
                var other_transform = obj.RenderTransform;
                obj.RenderTransform = null;
                obj.RenderTransform = new TransformGroup { Children = { other_transform, transform } };
                break;
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        var obj = AssociatedObject;
        obj.MouseDown -= OnMouseDown;

        if (ReferenceEquals(obj.RenderTransform, _Transform))
            obj.RenderTransform = null;
        else if (obj.RenderTransform is TransformGroup group)
        {
            group.Children.Remove(_Transform);
            switch (group.Children.Count)
            {
                case 0: obj.RenderTransform = null; break;
                case 1: 
                    var transform = group.Children[0];
                    group.Children.Clear();
                    obj.RenderTransform = transform;
                    break;
            }
        }

        _Transform = null;
    }

    private Point _StartMousePosition;
    private IInputElement _Parent;
    private void OnMouseDown(object Sender, MouseButtonEventArgs E)
    {
        var element = (UIElement)Sender;

        _Parent = element.FindLogicalParent<IInputElement>();
        _StartMousePosition = E.GetPosition(_Parent);

        element.MouseUp   += OnMouseUp;
        element.MouseMove += OnMouseMove;
    }

    private void OnMouseUp(object s, MouseButtonEventArgs _)
    {
        var e = (UIElement)s;
        e.MouseUp   -= OnMouseUp;
        e.MouseMove -= OnMouseMove;
    }

    private void OnMouseMove(object Sender, MouseEventArgs E) => (_Transform.X, _Transform.Y) = _StartMousePosition.Substrate(E.GetPosition(_Parent));
}

public class RotateMoveBehavior : Behavior<UIElement>
{
    protected override void OnAttached()
    {
        throw new NotImplementedException();
        AssociatedObject.MouseDown += OnMouseDown;
        //base.OnAttached();
    }

    protected override void OnDetaching()
    {
        AssociatedObject.MouseDown -= OnMouseDown;


        base.OnDetaching();
    }

    private void OnMouseDown(object Sender, MouseButtonEventArgs E)
    {
        throw new NotImplementedException();
    }
}
