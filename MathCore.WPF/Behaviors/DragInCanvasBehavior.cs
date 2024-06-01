using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Behaviors;

public class DragInCanvasBehavior : Behavior<FrameworkElement>
{
    /// <summary>Ссылка на канву</summary>
    private Canvas? _Canvas;

    /// <summary>Запись точной позиции, в которой нажата кнопка</summary>
    private Point _StartPoint;

    #region IsDragging

    /// <summary>Отслеживание перетаскивания элемента</summary>
    private bool _IsDragging;

    private bool IsDragging
    {
        get => _IsDragging;
        set
        {
            if (_IsDragging)
            {
                if (value) return;
                _IsDragging = false;
                AssociatedObject.ReleaseMouseCapture();
            }
            else
            {
                if (!value) return;
                _IsDragging = true;
                AssociatedObject.CaptureMouse();
            }
        }
    }

    #endregion

    #region Xmin : double - Минимально допустимое значение координаты X

    /// <summary>Минимально допустимое значение координаты X</summary>
    //[Category("")]
    [Description("Минимально допустимое значение координаты X")]
    public double Xmin
    {
        get => (double)GetValue(XminProperty);
        set => SetValue(XminProperty, value);
    }

    /// <summary>Минимально допустимое значение координаты X</summary>
    public static readonly DependencyProperty XminProperty =
        DependencyProperty.Register(
            nameof(Xmin),
            typeof(double),
            typeof(DragInCanvasBehavior),
            new(double.NaN));

    #endregion

    #region Xmax : double - Максимально допустимое значение координаты X

    /// <summary>Максимально допустимое значение координаты X</summary>
    //[Category("")]
    [Description("Максимально допустимое значение координаты X")]
    public double Xmax
    {
        get => (double)GetValue(XmaxProperty);
        set => SetValue(XmaxProperty, value);
    }

    /// <summary>Максимально допустимое значение координаты X</summary>
    public static readonly DependencyProperty XmaxProperty =
        DependencyProperty.Register(
            nameof(Xmax),
            typeof(double),
            typeof(DragInCanvasBehavior),
            new(double.NaN));

    #endregion

    #region Ymin : double - Минимально допустимое значение координаты Y

    /// <summary>Минимально допустимое значение координаты Y</summary>
    //[Category("")]
    [Description("Минимально допустимое значение координаты Y")]
    public double Ymin
    {
        get => (double)GetValue(YminProperty);
        set => SetValue(YminProperty, value);
    }

    /// <summary>Минимально допустимое значение координаты Y</summary>
    public static readonly DependencyProperty YminProperty =
        DependencyProperty.Register(
            nameof(Ymin),
            typeof(double),
            typeof(DragInCanvasBehavior),
            new(double.NaN));

    #endregion

    #region Ymax : double - Максимально допустимое значение координаты Y

    /// <summary>Максимально допустимое значение координаты Y</summary>
    //[Category("")]
    [Description("Максимально допустимое значение координаты Y")]
    public double Ymax
    {
        get => (double)GetValue(YmaxProperty);
        set => SetValue(YmaxProperty, value);
    }

    /// <summary>Максимально допустимое значение координаты Y</summary>
    public static readonly DependencyProperty YmaxProperty =
        DependencyProperty.Register(
            nameof(Ymax),
            typeof(double),
            typeof(DragInCanvasBehavior),
            new(double.NaN));

    #endregion

    #region AllowX : bool - Разрешено перемещение по оси X

    /// <summary>Разрешено перемещение по оси X</summary>
    public static readonly DependencyProperty AllowXProperty =
        DependencyProperty.Register(
            nameof(AllowX),
            typeof(bool),
            typeof(DragInCanvasBehavior),
            new(true));

    /// <summary>Разрешено перемещение по оси X</summary>
    public bool AllowX
    {
        get => (bool)GetValue(AllowXProperty);
        set => SetValue(AllowXProperty, value);
    }

    #endregion

    #region AllowY : bool - Разрешено перетаскивание по оси Y

    /// <summary>summary</summary>
    public static readonly DependencyProperty AllowYProperty =
        DependencyProperty.Register(
            nameof(AllowY),
            typeof(bool),
            typeof(DragInCanvasBehavior),
            new(true));

    /// <summary>Разрешено перетаскивание по оси Y</summary>
    public bool AllowY
    {
        get => (bool)GetValue(AllowYProperty);
        set => SetValue(AllowYProperty, value);
    }

    #endregion

    #region CurrentX : double - Текущее горизонтальное положение

    /// <summary>Текущее горизонтальное положение</summary>
    //[Category("")]
    [Description("Текущее горизонтальное положение")]
    public double CurrentX
    {
        get => (double)GetValue(CurrentXProperty);
        set => SetValue(CurrentXProperty, value);
    }

    /// <summary>Текущее горизонтальное положение</summary>
    public static readonly DependencyProperty CurrentXProperty =
        DependencyProperty.Register(
            nameof(CurrentX),
            typeof(double),
            typeof(DragInCanvasBehavior),
            new(default(double), OnCurrentXChanged));

    private static void OnCurrentXChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        if (d is not DragInCanvasBehavior { _InMove: false, } drag) return;

        drag.MoveTo(new((double)args.NewValue, drag.CurrentY));
    }

    #endregion

    #region CurrentY : double - Текущее вертикальное положение

    /// <summary>Текущее горизонтальное положение</summary>
    //[Category("")]
    [Description("Текущее горизонтальное положение")]
    public double CurrentY
    {
        get => (double)GetValue(CurrentYProperty);
        set => SetValue(CurrentYProperty, value);
    }

    /// <summary>Текущее горизонтальное положение</summary>
    public static readonly DependencyProperty CurrentYProperty =
        DependencyProperty.Register(
            nameof(CurrentY),
            typeof(double),
            typeof(DragInCanvasBehavior),
            new(default(double), OnCurrentYChanged));

    private static void OnCurrentYChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        if (d is not DragInCanvasBehavior { _InMove: false, } drag) return;

        drag.MoveTo(new(drag.CurrentX, (double)args.NewValue));
    }

    #endregion

    #region Enabled : bool - Перетаскивание активно

    /// <summary>Перетаскивание активно</summary>
    public static readonly DependencyProperty EnabledProperty =
        DependencyProperty.Register(
            nameof(Enabled),
            typeof(bool),
            typeof(DragInCanvasBehavior),
            new(true, (d, e) => ((DragInCanvasBehavior)d).IsDragging &= (bool)e.NewValue));

    /// <summary>Перетаскивание активно</summary>
    public bool Enabled
    {
        get => (bool)GetValue(EnabledProperty);
        set => SetValue(EnabledProperty, value);
    }

    #endregion

    /// <summary>Присоединение поведения к объекту</summary>
    protected override void OnAttached()
    {
        base.OnAttached();

        var obj = AssociatedObject;
        if (VisualTreeHelper.GetParent(obj) is not Canvas canvas)
            return;

        _Canvas = canvas;

        obj.MouseLeftButtonDown += OnMouseLeftButtonDown;

        var x_value = obj.ReadLocalValue(Canvas.LeftProperty);
        if (x_value != DependencyProperty.UnsetValue && x_value is double x)
            CurrentX = x;
        else
            CurrentX = obj.GetValue(Canvas.RightProperty) as double? ?? double.NaN;

        var y_value = obj.ReadLocalValue(Canvas.TopProperty);
        if (y_value != DependencyProperty.UnsetValue && y_value is double y)
            CurrentY = y;
        else
            CurrentY = obj.GetValue(Canvas.BottomProperty) as double? ?? double.NaN;
    }

    /// <summary>Отсоединение поведения от объекта</summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();

        IsDragging  = false;
        _IsDragging = false;

        CurrentX = double.NaN;
        CurrentY = double.NaN;

        AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;

        if (_Canvas is not { } canvas) return;
        canvas.MouseMove -= OnMouseMove;
        canvas.MouseLeftButtonUp -= OnMouseLeftButtonUp;
    }

    /// <summary>При нажатии левой кнопки мыши</summary><param name="sender">Источник события</param><param name="e">Аргумент события</param>
    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var obj = AssociatedObject;
        var event_sender = e.OriginalSource;

        // Если канва не определена, то её надо найти вверх по визуальному дереву

        if (!ReferenceEquals(event_sender, obj)
            && VisualTreeHelper.GetParent((DependencyObject)event_sender) is Canvas source_canvas
            && !ReferenceEquals(source_canvas, _Canvas))
            return;

        // Фиксируем точку нажатия левой кнопки мыши относительно элемента
        _StartPoint = e.GetPosition(obj);
        IsDragging = true;

        Mouse.Capture(obj, CaptureMode.SubTree);

        _Canvas!.MouseMove += OnMouseMove;
        _Canvas.MouseLeftButtonUp += OnMouseLeftButtonUp;
    }

    /// <summary>При отпускании левой кнопки мыши</summary>
    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        Mouse.Capture(null);

        IsDragging = false;
        _InMove    = false;

        if (_Canvas is not { } canvas) return;
        canvas.MouseMove         -= OnMouseMove;
        canvas.MouseLeftButtonUp -= OnMouseLeftButtonUp;
    }

    private bool _InMove;

    /// <summary>При перемещении мыши</summary>
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var canvas = _Canvas;

        // Если режим перетаскивания не активирован, то возврат
        if (!_IsDragging || Mouse.LeftButton != MouseButtonState.Pressed)
        {
            Mouse.Capture(null);

            canvas!.MouseMove -= OnMouseMove;
            canvas.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            return;
        }

        // Иначе определяем положение указателя относительно канвы
        MoveTo(e.GetPosition(canvas));
    }

    private static (double min, double max) CheckMinMax(double min, double max)
    {
        if (min is double.NaN) min = double.NegativeInfinity;
        if (max is double.NaN) max = double.PositiveInfinity;
        return (min, max);
    }

    private void MoveTo(Point point)
    {
        _InMove = true;
        var obj = AssociatedObject;
        var (width, height) = (obj.ActualWidth, obj.ActualHeight);

        var (x_min, x_max) = CheckMinMax(Xmin, Xmax);
        var (y_min, y_max) = CheckMinMax(Ymin, Ymax);

        FrameworkElement? parent = null;
        if (x_max <= 0)
        {
            parent = obj.FindLogicalParent<FrameworkElement>();
            x_max = parent.ActualWidth + x_max;
        }

        if (y_max <= 0)
        {
            parent ??= obj.FindLogicalParent<FrameworkElement>();
            y_max = parent.ActualHeight + y_max;
        }

        // Изменяем присоединённые к элементу свойства канвы, отвечающие за положение элемента на ней
        if (AllowX)
            if (obj.ReadLocalValue(Canvas.RightProperty) == DependencyProperty.UnsetValue)
            {
                var x = point.X - _StartPoint.X;
                if ((x >= x_min) && x <= x_max - width)
                {
                    obj.SetValue(Canvas.LeftProperty, x);
                    CurrentX = x;
                }
            }
            else
            {
                var x = point.X + _StartPoint.X;
                if (x >= x_min && x <= x_max - width)
                {
                    obj.SetValue(Canvas.RightProperty, x);
                    CurrentX = x;
                }
            }

        if (AllowY)
            if (obj.ReadLocalValue(Canvas.BottomProperty) == DependencyProperty.UnsetValue)
            {
                var y = point.Y - _StartPoint.Y;
                if (y >= y_min && y <= y_max - height)
                {
                    obj.SetValue(Canvas.TopProperty, y);
                    CurrentY = y;
                }
            }
            else
            {
                var y = point.Y + _StartPoint.Y;
                if (y >= y_min && y <= y_max - height)
                {
                    obj.SetValue(Canvas.BottomProperty, y);
                    CurrentY = y;
                }
            }

        _InMove = false;
    }
}
