using System.Windows;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Behaviors;

public class MouseControlBehavior : Behavior<FrameworkElement>
{
    #region MousePosition : Point - Положение указателя мыши

    /// <summary>Положение указателя мыши</summary>
    public static readonly DependencyProperty MousePositionProperty =
        DependencyProperty.Register(
            nameof(MousePosition),
            typeof(Point),
            typeof(MouseControlBehavior),
            new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnMousePositionChanged));

    private static void OnMousePositionChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        var behavior = (MouseControlBehavior)D;
        var pos      = (Point)E.NewValue;
        var size     = behavior.AssociatedObject.RenderSize;
        behavior.MousePositionRelative = new(pos.X / size.Width, pos.Y / size.Height);
    }

    /// <summary>Положение указателя мыши</summary>
    public Point MousePosition
    {
        get => (Point)GetValue(MousePositionProperty);
        set => SetValue(MousePositionProperty, value);
    }

    #endregion MousePosition : Point - Положение указателя мыши

    #region MousePositionRelative : Point - Относительное положение указателя мыши

    /// <summary>Относительное положение указателя мыши</summary>
    public static readonly DependencyProperty MousePositionRelativeProperty =
        DependencyProperty.Register(
            nameof(MousePositionRelative),
            typeof(Point),
            typeof(MouseControlBehavior),
            new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>Относительное положение указателя мыши</summary>
    public Point MousePositionRelative
    {
        get => (Point)GetValue(MousePositionRelativeProperty);
        set => SetValue(MousePositionRelativeProperty, value);
    }

    #endregion MousePositionRelative : Point - Относительное положение указателя мыши

    #region ElementSize : Size - Размер элемента управления

    /// <summary>Размер элемента управления</summary>
    public static readonly DependencyProperty ElementSizeProperty =
        DependencyProperty.Register(
            nameof(ElementSize),
            typeof(Size),
            typeof(MouseControlBehavior),
            new FrameworkPropertyMetadata(default(Size), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSizePropertyChanged));

    private static void OnSizePropertyChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        var behavior = (MouseControlBehavior)D;
        var pos      = behavior.MousePosition;
        var size     = (Size)E.NewValue;
        behavior.MousePositionRelative = new(pos.X / size.Width, pos.Y / size.Height);
    }

    /// <summary>Размер элемента управления</summary>
    public Size ElementSize
    {
        get => (Size)GetValue(ElementSizeProperty);
        set => SetValue(ElementSizeProperty, value);
    }

    #endregion ElementSize : Size - Размер элемента управления

    #region IsLeftMouseDown : bool - Нажатие левой клавиши мыши

    /// <summary>Нажатие левой клавиши мыши</summary>
    public static readonly DependencyProperty IsLeftMouseDownProperty =
        DependencyProperty.Register(
            nameof(IsLeftMouseDown),
            typeof(bool),
            typeof(MouseControlBehavior),
            new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>Нажатие левой клавиши мыши</summary>
    public bool IsLeftMouseDown
    {
        get => (bool)GetValue(IsLeftMouseDownProperty);
        set => SetValue(IsLeftMouseDownProperty, value);
    }

    #endregion

    #region LeftMouseClick : ICommand - Команда, выполняемая при щелчке мышью

    /// <summary>Команда, выполняемая при щелчке мышью</summary>
    public static readonly DependencyProperty LeftMouseClickProperty =
        DependencyProperty.Register(
            nameof(LeftMouseClick),
            typeof(ICommand),
            typeof(MouseControlBehavior),
            new(default(ICommand)));

    /// <summary>Команда, выполняемая при щелчке мышью</summary>
    public ICommand LeftMouseClick
    {
        get => (ICommand)GetValue(LeftMouseClickProperty);
        set => SetValue(LeftMouseClickProperty, value);
    }

    #endregion

    #region Behavior<FrameworkElement>

    protected override void OnAttached()
    {
        var element = AssociatedObject;
        element.MouseMove   += OnMouseMove;
        element.SizeChanged += OnSizeChanged;
        element.MouseDown   += OnMouseDown;
        element.MouseUp     += OnMouseUp;
    }

    protected override void OnDetaching()
    {
        var element = AssociatedObject;
        element.MouseMove   -= OnMouseMove;
        element.SizeChanged -= OnSizeChanged;
    }

    private void OnMouseDown(object Sender, MouseButtonEventArgs E)
    {
        IsLeftMouseDown = true;
        Mouse.Capture((IInputElement)Sender, CaptureMode.SubTree);
    }

    private void OnMouseUp(object Sender, MouseButtonEventArgs E)
    {
        (Sender as UIElement)?.ReleaseMouseCapture();
        IsLeftMouseDown = false;
        var command = LeftMouseClick;
        if (command?.CanExecute(E) != true) return;
        command.Execute(E);
    }

    #endregion

    #region EventHandlers

    private void OnMouseMove(object Sender, MouseEventArgs E) => MousePosition = E.GetPosition((FrameworkElement)Sender);

    private void OnSizeChanged(object Sender, SizeChangedEventArgs E) => ElementSize = E.NewSize;

    #endregion
}