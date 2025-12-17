using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для перемещения элемента с использованием TranslateTransform</summary>
public class TranslateMoveBehavior : Behavior<UIElement>
{
    /// <summary>Трансформация перемещения элемента</summary>
    private TranslateTransform _Transform;

    /// <summary>Вызывается при присоединении поведения к элементу</summary>
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

    /// <summary>Вызывается при отсоединении поведения от элемента</summary>
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

    /// <summary>Начальная позиция мыши</summary>
    private Point _StartMousePosition;
    /// <summary>Родительский элемент для определения координат</summary>
    private IInputElement _Parent;
    
    /// <summary>Обработчик нажатия кнопки мыши для начала перемещения</summary>
    /// <param name="Sender">Источник события</param>
    /// <param name="E">Аргументы события</param>
    private void OnMouseDown(object Sender, MouseButtonEventArgs E)
    {
        var element = (UIElement)Sender;

        _Parent = element.FindLogicalParent<IInputElement>();
        _StartMousePosition = E.GetPosition(_Parent);

        element.MouseUp   += OnMouseUp;
        element.MouseMove += OnMouseMove;
    }

    /// <summary>Обработчик отпускания кнопки мыши для завершения перемещения</summary>
    /// <param name="s">Источник события</param>
    /// <param name="_">Аргументы события</param>
    private void OnMouseUp(object s, MouseButtonEventArgs _)
    {
        var e = (UIElement)s;
        e.MouseUp   -= OnMouseUp;
        e.MouseMove -= OnMouseMove;
    }

    /// <summary>Обработчик перемещения мыши для обновления позиции элемента</summary>
    /// <param name="Sender">Источник события</param>
    /// <param name="E">Аргументы события</param>
    private void OnMouseMove(object Sender, MouseEventArgs E) => (_Transform.X, _Transform.Y) = _StartMousePosition.Substrate(E.GetPosition(_Parent));
}

//public class RotateMoveBehavior : Behavior<UIElement>
//{
//    protected override void OnAttached()
//    {
//        throw new NotImplementedException();
//        AssociatedObject.MouseDown += OnMouseDown;
//        //base.OnAttached;
//    }

//    protected override void OnDetaching()
//    {
//        AssociatedObject.MouseDown -= OnMouseDown;


//        base.OnDetaching();
//    }

//    private void OnMouseDown(object Sender, MouseButtonEventArgs E)
//    {
//        throw new NotImplementedException();
//    }
//}
