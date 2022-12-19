using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Behaviors;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Качество кода", "IDE0052:Удалить непрочитанные закрытые члены", Justification = "<Ожидание>")]
public class DragInCanvasBehavior : Behavior<UIElement>
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
            new PropertyMetadata(double.NaN));

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
            new PropertyMetadata(double.NaN));

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
            new PropertyMetadata(double.NaN));

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
            new PropertyMetadata(double.NaN));

    #endregion

    #region AllowX : bool - Разрешено перемещение по оси X

    /// <summary>Разрешено перемещение по оси X</summary>
    public static readonly DependencyProperty AllowXProperty =
        DependencyProperty.Register(
            nameof(AllowX),
            typeof(bool),
            typeof(DragInCanvasBehavior),
            new PropertyMetadata(true));

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
            new PropertyMetadata(true));

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
            new PropertyMetadata(default(double), OnCurrentXChanged));

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
            new PropertyMetadata(default(double), OnCurrentYChanged));

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
            new PropertyMetadata(true, (d, e) => ((DragInCanvasBehavior)d).IsDragging &= (bool)e.NewValue));

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

        AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
        AssociatedObject.MouseMove           += OnMouseMove;
        AssociatedObject.MouseLeftButtonUp   += OnMouseLeftButtonUp;
    }

    /// <summary>Отсоединение поведения от объекта</summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();

        AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        AssociatedObject.MouseMove           -= OnMouseMove;
        AssociatedObject.MouseLeftButtonUp   -= OnMouseLeftButtonUp;
    }

    /// <summary>При нажатии левой кнопки мыши</summary><param name="sender">Источник события</param><param name="e">Аргумент события</param>
    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Если канва не определена, то её надо найти вверх по визуальному дереву
        // ReSharper disable once ArrangeRedundantParentheses
        if ((_Canvas ??= VisualTreeHelper.GetParent(AssociatedObject) as Canvas) is null) return;

        // Фиксируем точку нажатия левой кнопки мыши относительно элемента
        _StartPoint = e.GetPosition(AssociatedObject);
        IsDragging  = true;
    }

    private bool _InMove;

    /// <summary>При перемещении мыши</summary>
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        // Если режим перетаскивания не активирован, то возврат
        if (!_IsDragging) return;

        // Иначе определяем положение указателя относительно канвы
        MoveTo(e.GetPosition(_Canvas));
    }

    private static (double min, double max) CheckMinMax(double min, double max)
    {
        if (min is double.NaN) min = double.NegativeInfinity;
        if (max is double.NaN) max = double.PositiveInfinity;
        return (Math.Min(min, max), Math.Max(min, max));
    }

    private void MoveTo(Point point)
    {
        _InMove = true;

        var (x_min, x_max) = CheckMinMax(Xmin, Xmax);
        var (y_min, y_max) = CheckMinMax(Ymin, Ymax);

        // Изменяем присоединённые к элементу свойства канвы, отвечающие за положение элемента на ней
        var obj = AssociatedObject;
        if (AllowX)
            if (obj.ReadLocalValue(Canvas.RightProperty) == DependencyProperty.UnsetValue)
            {
                var x = point.X - _StartPoint.X;
                if ((x >= x_min) && x <= x_max)
                {
                    obj.SetValue(Canvas.LeftProperty, x);
                    CurrentX = x;
                }
            }
            else
            {
                var x = point.X + _StartPoint.X;
                if (x >= x_min && x <= x_max)
                {
                    obj.SetValue(Canvas.RightProperty, x);
                    CurrentX = x;
                }
            }

        if (AllowY)
            if (obj.ReadLocalValue(Canvas.BottomProperty) == DependencyProperty.UnsetValue)
            {
                var y = point.Y - _StartPoint.Y;
                if (y >= y_min && y <= y_max)
                {
                    obj.SetValue(Canvas.TopProperty, y);
                    CurrentY = y;
                }
            }
            else
            {
                var y = point.Y + _StartPoint.Y;
                if (y >= y_min && y <= y_max)
                {
                    obj.SetValue(Canvas.BottomProperty, y);
                    CurrentY = y;
                }
            }

        _InMove = false;
    }

    /// <summary>При отпускании левой кнопки мыши</summary>
    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => IsDragging = false;
}