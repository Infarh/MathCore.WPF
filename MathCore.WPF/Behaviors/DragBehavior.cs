using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Behaviors;

public class DragBehavior : Behavior<FrameworkElement>
{
    private static (double min, double max) CheckMinMax(double min, double max)
    {
        if (min is double.NaN) min = double.NegativeInfinity;
        if (max is double.NaN) max = double.PositiveInfinity;
        return (Math.Min(min, max), Math.Max(min, max));
    }

    private ((double Min, double Max) X, (double Min, double Max) Y) GetInterval() => ((CheckMinMax(Xmin, Xmax)), CheckMinMax(Ymin, Ymax));

    private abstract class ObjectMover : IDisposable
    {
        private readonly Point _StartMousePos;
        private readonly IInputElement _ParentElement;
        private readonly UIElement _MovingElement;
        protected readonly DragBehavior _Behavior;

        private double _MinX;
        private double _MaxX;
        private double _MinY;
        private double _MaxY;

        protected ObjectMover(UIElement element, DragBehavior behavior)
        {
            _MovingElement = element;
            _Behavior      = behavior;
            _ParentElement = element.FindLogicalParent<IInputElement>() ?? throw new InvalidOperationException("Не найден родительский элемент");
            _StartMousePos = Mouse.GetPosition(_ParentElement);


            ((_MinX, _MaxX), (_MinY, _MaxY)) = behavior.GetInterval();

            _MinX = 0;
            _MaxX = 400;

            Mouse.Capture(element, CaptureMode.SubTree);
            element.MouseMove         += OnMouseMove;
            element.MouseLeftButtonUp += OnLeftMouseUp;
        }

        private void OnLeftMouseUp(object? Sender, MouseButtonEventArgs? E) => Dispose();

        private void OnMouseMove(object Sender, MouseEventArgs E)
        {
            var element = (FrameworkElement)Sender;
            if (Equals(Mouse.Captured, element))
            {
                var parent_point = E.GetPosition(_ParentElement);

                var (x0, y0) = parent_point.Substrate(E.GetPosition(_MovingElement));
                var (dx, dy) = parent_point.Substrate(_StartMousePos);

                var behavior = _Behavior;

                if (!behavior.AllowX) dx = 0;
                if (!behavior.AllowY) dy = 0;

                Debug.WriteLine(((_MinX, _MaxX), (x0, y0), (dx, dy)));

                if (!OnMouseMove(element, dx, dy)) return;

                behavior.dx     = dx;
                behavior.dy     = dy;
                behavior.Radius = Math.Sqrt(dx * dx + dy * dy);
                behavior.Angle  = Math.Atan2(dy, dx);
                return;
            }
            Dispose();
        }

        protected abstract bool OnMouseMove(FrameworkElement element, double dx, double dy);

        public void Dispose()
        {
            _MovingElement.MouseMove         -= OnMouseMove;
            _MovingElement.MouseLeftButtonUp -= OnLeftMouseUp;
            _MovingElement.ReleaseMouseCapture();
        }
    }

    private class ThicknessObjectMover : ObjectMover
    {
        private readonly Thickness _StartThickness;

        public ThicknessObjectMover(FrameworkElement element, DragBehavior behavior) 
            : base(element, behavior) => _StartThickness = element.Margin;

        protected override bool OnMouseMove(FrameworkElement element, double dx, double dy)
        {
            element.Margin = new Thickness(
                _StartThickness.Left + dx,
                _StartThickness.Top + dy,
                _StartThickness.Right - dx,
                _StartThickness.Bottom - dy
            );

            return true;
        }
    }

    private class CanvasObjectMover : ObjectMover
    {
        private readonly double _StartLeft;
        private readonly double _StartRight;
        private readonly double _StartTop;
        private readonly double _StartBottom;

        public CanvasObjectMover(FrameworkElement element, DragBehavior behavior) 
            : base(element, behavior)
        {
            _StartLeft   = Canvas.GetLeft(element);
            _StartRight  = Canvas.GetRight(element);
            _StartTop    = Canvas.GetTop(element);
            _StartBottom = Canvas.GetBottom(element);
        }

        protected override bool OnMouseMove(FrameworkElement element, double dx, double dy)
        {
            var (x_min, x_max) = CheckMinMax(_Behavior.Xmin, _Behavior.Xmax);
            var (y_min, y_max) = CheckMinMax(_Behavior.Ymin, _Behavior.Ymax);

            var moved = false;
            if (!double.IsNaN(_StartLeft))
            {
                var x = _StartLeft + dx;
                if (x >= x_min && x <= x_max)
                {
                    Canvas.SetLeft(element, x);
                    moved = true;
                }
            }

            if (!double.IsNaN(_StartRight))
            {
                var x = _StartRight - dx;
                if (x >= x_min && x <= x_max)
                {
                    Canvas.SetLeft(element, x);
                    moved = true;
                }
            }

            if (!double.IsNaN(_StartTop))
            {
                var y = _StartTop + dy;
                if (y >= y_min && y <= y_max)
                {
                    Canvas.SetLeft(element, y);
                    moved = true;
                }
            }

            if (!double.IsNaN(_StartBottom))
            {
                var y = _StartBottom - dy;
                if (y >= y_min && y <= y_max)
                {
                    Canvas.SetLeft(element, y);
                    moved = true;
                }
            }

            return moved;
        }
    }

    #region Enabled

    /// <summary></summary>
    public static readonly DependencyProperty EnabledProperty =
        DependencyProperty.Register(
            nameof(Enabled),
            typeof(bool),
            typeof(DragBehavior),
            new PropertyMetadata(default(bool), (s, e) => { if (!(bool)e.NewValue) ((DragBehavior)s)?._ObjectMover?.Dispose(); }));

    /// <summary></summary>
    public bool Enabled
    {
        get => (bool)GetValue(EnabledProperty);
        set => SetValue(EnabledProperty, value);
    }

    #endregion

    #region dx : double - Величина смещения по горизонтали

    /// <summary>Величина смещения по горизонтали</summary>
    private static readonly DependencyPropertyKey dxPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(dx),
            typeof(double),
            typeof(DragBehavior),
            new PropertyMetadata(default(double)));

    /// <summary>Величина смещения по горизонтали</summary>
    public static readonly DependencyProperty dxProperty = dxPropertyKey.DependencyProperty;

    /// <summary>Величина смещения по горизонтали</summary>
    public double dx
    {
        get => (double)GetValue(dxProperty);
        private set => SetValue(dxPropertyKey, value);
    }

    #endregion

    #region dy : double - Величина смещения по вертикали

    /// <summary>Величина смещения по вертикали</summary>
    private static readonly DependencyPropertyKey dyPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(dy),
            typeof(double),
            typeof(DragBehavior),
            new PropertyMetadata(default(double)));

    /// <summary>Величина смещения по вертикали</summary>
    public static readonly DependencyProperty dyProperty = dyPropertyKey.DependencyProperty;

    /// <summary>Величина смещения по вертикали</summary>
    public double dy
    {
        get => (double)GetValue(dyProperty);
        private set => SetValue(dyPropertyKey, value);
    }

    #endregion

    #region Radius

    private static readonly DependencyPropertyKey RadiusPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(Radius),
            typeof(double),
            typeof(DragBehavior),
            new PropertyMetadata(default(double)));

    public static readonly DependencyProperty RadiusProperty = RadiusPropertyKey.DependencyProperty;

    public double Radius
    {
        get => (double)GetValue(RadiusProperty);
        private set => SetValue(RadiusPropertyKey, value);
    }

    #endregion

    #region Angle

    private static readonly DependencyPropertyKey AnglePropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(Angle),
            typeof(double),
            typeof(DragBehavior),
            new PropertyMetadata(default(double)));

    public static readonly DependencyProperty AngleProperty = AnglePropertyKey.DependencyProperty;

    public double Angle
    {
        get => (double)GetValue(AngleProperty);
        private set => SetValue(AnglePropertyKey, value);
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

    #region CurrentX : double - Текущее положение по OX

    /// <summary>Текущее положение по OX</summary>
    //[Category("")]
    [Description("Текущее положение по OX")]
    public double CurrentX
    {
        get => (double)GetValue(CurrentXProperty);
        set => SetValue(CurrentXProperty, value);
    }

    /// <summary>Текущее положение по OX</summary>
    public static readonly DependencyProperty CurrentXProperty =
        DependencyProperty.Register(
            nameof(CurrentX),
            typeof(double),
            typeof(DragBehavior),
            new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    #endregion

    #region CurrentY : double - Текущее положение по OY

    /// <summary>Текущее положение по OY</summary>
    //[Category("")]
    [Description("Текущее положение по OY")]
    public double CurrentY
    {
        get => (double)GetValue(CurrentYProperty);
        set => SetValue(CurrentYProperty, value);
    }

    /// <summary>Текущее положение по OY</summary>
    public static readonly DependencyProperty CurrentYProperty =
        DependencyProperty.Register(
            nameof(CurrentY),
            typeof(double),
            typeof(DragBehavior),
            new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    #endregion

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        _ObjectMover?.Dispose();
        _ObjectMover = null;
    }

    private ObjectMover? _ObjectMover;

    private void OnMouseLeftButtonDown(object? Sender, MouseButtonEventArgs? E)
    {
        if (Sender is not FrameworkElement element) return;
        var parent = element.FindLogicalParent<IInputElement>();
        if (parent is null) return;

        _ObjectMover?.Dispose();

        _ObjectMover = parent switch
        {
            Canvas         => new CanvasObjectMover(element, this),
            Panel          => new ThicknessObjectMover(element, this),
            ContentControl => new ThicknessObjectMover(element, this),
            _              => _ObjectMover
        };
    }
}