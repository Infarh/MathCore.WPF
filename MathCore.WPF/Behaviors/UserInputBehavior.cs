using System.ComponentModel;
using System.Windows.Input;
using System.Windows;

using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors;

public class UserInputBehavior : Behavior<FrameworkElement>
{
    #region Position : Point - Положение мыши в координатах элемента

    /// <summary>Положение мыши в координатах элемента</summary>
    public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register(
            nameof(Position),
            typeof(Point),
            typeof(UserInputBehavior),
            new(default(Point)));

    /// <summary>Положение мыши в координатах элемента</summary>
    //[Category("")]
    [Description("Положение мыши в координатах элемента")]
    public Point Position { get => (Point)GetValue(PositionProperty); set => SetValue(PositionProperty, value); }

    #endregion

    #region LeftMouseDownCommand : ICommand - Команда, вызываемая в момент нажатия левой кнопки мыши

    /// <summary>Команда, вызываемая в момент нажатия левой кнопки мыши</summary>
    public static readonly DependencyProperty LeftMouseDownCommandProperty =
        DependencyProperty.Register(
            nameof(LeftMouseDownCommand),
            typeof(ICommand),
            typeof(UserInputBehavior),
            new(default(ICommand)));

    /// <summary>Команда, вызываемая в момент нажатия левой кнопки мыши</summary>
    //[Category("")]
    [Description("Команда, вызываемая в момент нажатия левой кнопки мыши")]
    public ICommand LeftMouseDownCommand { get => (ICommand)GetValue(LeftMouseDownCommandProperty); set => SetValue(LeftMouseDownCommandProperty, value); }

    #endregion

    #region LeftMouseUpCommand : ICommand - Команда, вызываемая в момент отпускания левой кнопки мыши

    /// <summary>Команда, вызываемая в момент отпускания левой кнопки мыши</summary>
    public static readonly DependencyProperty LeftMouseUpCommandProperty =
        DependencyProperty.Register(
            nameof(LeftMouseUpCommand),
            typeof(ICommand),
            typeof(UserInputBehavior),
            new(default(ICommand)));

    /// <summary>Команда, вызываемая в момент отпускания левой кнопки мыши</summary>
    //[Category("")]
    [Description("Команда, вызываемая в момент нажатия левой кнопки мыши")]
    public ICommand LeftMouseUpCommand { get => (ICommand)GetValue(LeftMouseUpCommandProperty); set => SetValue(LeftMouseUpCommandProperty, value); }

    #endregion

    #region MouseWheelCommand : ICommand - Команда, вызываемая при прокрутке колёсика мышки

    /// <summary>Команда, вызываемая при прокрутке колёсика мышки</summary>
    public static readonly DependencyProperty MouseWheelCommandProperty =
        DependencyProperty.Register(
            nameof(MouseWheelCommand),
            typeof(ICommand),
            typeof(UserInputBehavior),
            new(default(ICommand)));

    /// <summary>Команда, вызываемая при прокрутке колёсика мышки</summary>
    //[Category("")]
    [Description("Команда, вызываемая при прокрутке колёсика мышки")]
    public ICommand MouseWheelCommand { get => (ICommand)GetValue(MouseWheelCommandProperty); set => SetValue(MouseWheelCommandProperty, value); }

    #endregion

    #region KeyDownCommand : ICommand - Команда, вызываемая при нажатии кнопки на клавиатуре

    /// <summary>Команда, вызываемая при нажатии кнопки на клавиатуре</summary>
    public static readonly DependencyProperty KeyDownCommandProperty =
        DependencyProperty.Register(
            nameof(KeyDownCommand),
            typeof(ICommand),
            typeof(UserInputBehavior),
            new(default(ICommand)));

    /// <summary>Команда, вызываемая при нажатии кнопки на клавиатуре</summary>
    //[Category("")]
    [Description("Команда, вызываемая при нажатии кнопки на клавиатуре")]
    public ICommand KeyDownCommand { get => (ICommand)GetValue(MouseWheelCommandProperty); set => SetValue(MouseWheelCommandProperty, value); }

    #endregion

    #region KeyUpCommand : ICommand - Команда, вызываемая при нажатии кнопки на клавиатуре

    /// <summary>Команда, вызываемая при нажатии кнопки на клавиатуре</summary>
    public static readonly DependencyProperty KeyUpCommandProperty =
        DependencyProperty.Register(
            nameof(KeyUpCommand),
            typeof(ICommand),
            typeof(UserInputBehavior),
            new(default(ICommand)));

    /// <summary>Команда, вызываемая при нажатии кнопки на клавиатуре</summary>
    //[Category("")]
    [Description("Команда, вызываемая при нажатии кнопки на клавиатуре")]
    public ICommand KeyUpCommand { get => (ICommand)GetValue(MouseWheelCommandProperty); set => SetValue(MouseWheelCommandProperty, value); }

    #endregion

    protected override void OnAttached()
    {
        base.OnAttached();

        var element = AssociatedObject;
        element.MouseMove += OnMouseMove;
        //element.MouseLeave += OnMouseLeave;
        element.MouseLeftButtonDown += OnLeftMouseDown;
        element.MouseLeftButtonUp += OnLeftMouseUp;
        element.MouseWheel += OnMouseWheel;
        element.KeyDown += OnKeyDown;
        element.KeyUp += OnKeyUp;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        var element = AssociatedObject;
        element.ReleaseMouseCapture();

        element.MouseMove -= OnMouseMove;
        //element.MouseLeave -= OnMouseLeave;
        element.MouseLeftButtonDown -= OnLeftMouseDown;
        element.MouseLeftButtonUp -= OnLeftMouseUp;
        element.MouseWheel -= OnMouseWheel;
        element.KeyDown -= OnKeyDown;
        element.KeyUp -= OnKeyUp;
    }

    private void OnMouseMove(object Sender, MouseEventArgs E)
    {
        if (Sender is not FrameworkElement element) return;
        var position = E.GetPosition(element);
        var element_size = element.RenderSize;
        var (x, y) = position;
        if (x < 0 || y < 0 || x > element_size.Width || y > element_size.Height) return;

        Position = position;
    }

    //private void OnMouseLeave(object Sender, MouseEventArgs E)
    //{
    //    Debug.WriteLine("Leave");
    //    //Position = new(double.NaN, double.NaN);
    //}

    private void OnLeftMouseDown(object Sender, MouseButtonEventArgs E)
    {
        if (Sender is not IInputElement element) return;
        if (LeftMouseDownCommand is not { } command) return;
        var point = E.GetPosition(element);
        if (!command.CanExecute(point)) return;
        element.CaptureMouse();
        command.Execute(point);
    }

    private void OnLeftMouseUp(object Sender, MouseButtonEventArgs E)
    {
        if (Sender is not IInputElement element) return;
        if (LeftMouseUpCommand is not { } command) return;
        var point = E.GetPosition(element);
        if (command.CanExecute(point))
            command.Execute(point);
        element.ReleaseMouseCapture();
    }

    private void OnMouseWheel(object Sender, MouseWheelEventArgs E)
    {
        if (MouseWheelCommand is not { } command) return;
        object delta = E.Delta;
        if (command.CanExecute(delta))
            command.Execute(delta);
    }

    private void OnKeyDown(object Sender, KeyEventArgs E)
    {
        if (KeyDownCommand is not { } command) return;
        object key = E.Key;
        if (command.CanExecute(key))
            command.Execute(key);
    }

    private void OnKeyUp(object Sender, KeyEventArgs E)
    {
        if (KeyUpCommand is not { } command) return;
        object key = E.Key;
        if (command.CanExecute(key))
            command.Execute(key);
    }
}