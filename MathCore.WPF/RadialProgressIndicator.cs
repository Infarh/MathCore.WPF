using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

using static System.Math;

namespace MathCore.WPF;

/// <summary>Индикатор радиального прогресса</summary>
public class RadialProgressIndicator : FrameworkElement
{
    #region Fields

    private Brush _ActiveForegourndBrush;

    private Pen _ActivePen;

    private Geometry? _BorderGeometry;

    private Point _Center;

    private Geometry? _CurrentGeometry;

    private Pen _ForegourndPen;

    private bool _IsListening;
    private int _LastTick;
    private int _TickCount;
    private Brush _ForegroundBrush;
    private Geometry? _ProgressBorderGeometry;
    private Geometry? _ProgressGeometry;
    private double _Radius;
    private double _RotationAngle;

    #endregion

    #region Constructors

    /// <summary>Регистрация метаданных зависимых свойств</summary>
    static RadialProgressIndicator() =>
        IsEnabledProperty.OverrideMetadata(
            typeof(RadialProgressIndicator),
            new UIPropertyMetadata(
                false,
                (o, e) => (o as RadialProgressIndicator)?.OnIsEnabledChanged((bool)e.OldValue, (bool)e.NewValue)));

    /// <summary>Инициализирует новый экземпляр <see cref="RadialProgressIndicator"/></summary>
    public RadialProgressIndicator()
    {
        _IsListening = false;
        _Radius = 0;
        _Center = new();
        _RotationAngle = 0;
        Unloaded += OnUnloaded;
    }

    #endregion

    #region Foreground

    /// <summary>Зависимое свойство для Foreground</summary>
    public static readonly DependencyProperty ForegroundProperty =
        TextElement.ForegroundProperty.AddOwner(
            typeof(RadialProgressIndicator),
            new FrameworkPropertyMetadata(
                SystemColors.ControlTextBrush,
                FrameworkPropertyMetadataOptions.Inherits,
                (o, e) => (o as RadialProgressIndicator)?.OnForegroundChanged((Brush)e.NewValue)));

    /// <summary>Свойство Foreground</summary>
    public Brush Foreground
    {
        get => (Brush)GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    #endregion Foreground

    #region ActiveForeground

    /// <summary>Зависимое свойство для ActiveForeground</summary>
    public static readonly DependencyProperty ActiveForegroundProperty =
        DependencyProperty.Register(
            nameof(ActiveForeground),
            typeof(Brush),
            typeof(RadialProgressIndicator),
            new FrameworkPropertyMetadata(
                SystemColors.ControlBrush,
                FrameworkPropertyMetadataOptions.AffectsRender,
                (o, e) => (o as RadialProgressIndicator)?.OnActiveForegroundChanged((Brush)e.NewValue)));

    /// <summary>Свойство ActiveForeground</summary>
    public Brush ActiveForeground
    {
        get => (Brush)GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    #endregion ActiveForeground

    #region CurrentValue

    /// <summary>Зависимое свойство для CurrentValue</summary>
    public static readonly DependencyProperty CurrentValueProperty =
        DependencyProperty.Register(
            nameof(CurrentValue),
            typeof(double),
            typeof(RadialProgressIndicator),
            new FrameworkPropertyMetadata(
                0d,
                FrameworkPropertyMetadataOptions.AffectsRender,
                (o, _) => (o as RadialProgressIndicator)?.OnCurrentValueChanged(),
                (_, e) => DoubleUtil.LessThan((double)e, 0) ? 0 : (DoubleUtil.GreaterThan((double)e, 100) ? 100 : (double)e)));

    /// <summary>Свойство CurrentValue</summary>
    public double CurrentValue
    {
        get => (double)GetValue(CurrentValueProperty);
        set => SetValue(CurrentValueProperty, value);
    }

    #endregion CurrentValue

    #region Overrides

    /// <summary>Участвует в операциях отрисовки, выполняемых системой компоновки, сохраняя инструкции для последующего асинхронного использования</summary>
    /// <param name="DrawingContext">Контекст рисования для элемента, предоставленный системой компоновки</param>
    protected override void OnRender(DrawingContext DrawingContext)
    {
        base.OnRender(DrawingContext);

        if (!DoubleUtil.IsZero(_Radius))
            DrawingContext.DrawEllipse(null, _ForegourndPen, _Center, _Radius, _Radius);

        if (!IsEnabled)
        {
            if (_BorderGeometry != null)
                DrawingContext.DrawGeometry(_ForegroundBrush, null, _BorderGeometry);
        }
        else
        {
            if (_ProgressBorderGeometry != null)
                DrawingContext.DrawGeometry(_ForegroundBrush, null, _ProgressBorderGeometry);

            if (_ProgressGeometry != null)
                DrawingContext.DrawGeometry(_ActiveForegourndBrush, null, _ProgressGeometry);
        }
    }

    // Детали изменения старого и нового размера // кратко по делу
    /// <summary>Вызывает событие System.Windows.FrameworkElement.SizeChanged, используя указанные данные</summary>
    /// <param name="SizeInfo">Данные об изменении размера</param>
    protected override void OnRenderSizeChanged(SizeChangedInfo SizeInfo)
    {
        base.OnRenderSizeChanged(SizeInfo);

        if (!DoubleUtil.IsZero(_Radius) && !SizeInfo.HeightChanged && !SizeInfo.WidthChanged)
            return;

        var new_radius = Min(ActualWidth, ActualHeight) / 2;
        var new_center = new Point(ActualWidth / 2, ActualHeight / 2);

        if (DoubleUtil.AreClose(_Radius, new_radius) && DoubleUtil.AreClose(_Center, new_center))
            return;

        _Radius = new_radius;
        _Center = new_center;
        CreateGeomerty();
    }

    #endregion Overrides

    #region Rendering

    private void OnRendering(object sender, EventArgs e)
    {
        if (!IsEnabled)
            return;

        var tick = Environment.TickCount;
        _TickCount = tick - _LastTick;
        _LastTick = tick;

        CreateProgressPath(_TickCount);
    }

    private void CreateProgressPath(int variation)
    {
        if (variation > 10)
            _RotationAngle = _RotationAngle.EaseAngle();

        if (_CurrentGeometry != null)
        {
            var path_geometry = (PathGeometry)_CurrentGeometry.Clone();
            path_geometry.Transform = new RotateTransform(_RotationAngle, _Center.X, _Center.Y);
            _ProgressGeometry = path_geometry.GetFlattenedPathGeometry();
        }

        if (_BorderGeometry != null)
        {
            var path_geometry = (PathGeometry)_BorderGeometry.Clone();
            path_geometry.Transform = new RotateTransform(-_RotationAngle, _Center.X, _Center.Y);
            _ProgressBorderGeometry = path_geometry.GetFlattenedPathGeometry();
        }

        InvalidateVisual();
    }

    private void CreateGeomerty()
    {
        if (DoubleUtil.IsZero(_Radius))
            return;

        _CurrentGeometry = _Center.CreatePath(CurrentValue.Angle(), _Radius - 14, _Radius - 20); // геометрия текущего значения // кратко по делу
        _CurrentGeometry.Freeze();

        _BorderGeometry = _Center.Create(4, 2, _Radius - 4, _Radius - 12); // геометрия границы // кратко по делу
        _BorderGeometry.Freeze();
    }

    private void StartListening()
    {
        VerifyAccess();

        if (_IsListening)
            return;

        _IsListening = true;
        CompositionTarget.Rendering += OnRendering;
    }

    private void StopListening()
    {
        VerifyAccess();

        if (!_IsListening)
            return;

        _IsListening = false;
        CompositionTarget.Rendering -= OnRendering;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_IsListening) StopListening();
    }

    #endregion Rendering

    #region Property Changes

    private void OnCurrentValueChanged() => CreateGeomerty();

    private void OnIsEnabledChanged(bool OldValue, bool NewValue)
    {
        if (NewValue)
        {
            CreateGeomerty();
            if (!_IsListening)
                StartListening();
        }
        else
        {
            if (_IsListening)
                StopListening();
            _RotationAngle = 0;
            _CurrentGeometry = null;
            _ProgressGeometry = null;
        }

        if (OldValue != NewValue)
            InvalidateVisual();
    }

    private void OnForegroundChanged(Brush NewValue)
    {
        _ForegroundBrush = NewValue.Clone();
        _ForegroundBrush.Freeze();

        _ForegourndPen = new(_ForegroundBrush, 2);
        _ForegourndPen.Freeze();
    }

    private void OnActiveForegroundChanged(Brush NewValue)
    {
        _ActiveForegourndBrush = NewValue.Clone();
        _ActiveForegourndBrush.Freeze();

        _ActivePen = new(_ActiveForegourndBrush, 2);
        _ActivePen.Freeze();
    }

    #endregion Property Changes
}

/// <summary>Методы расширения для построения геометрии</summary>
file static class GeometryExtensions
{
    #region Static

    private const double __FullCircleInDegrees = 360;

    #endregion

    #region Methods

    /// <summary>Плавно увеличивает угол, приближая его к 360 на 5 процентов</summary>
    /// <param name="angle">Начальный угол</param>
    /// <returns>Увеличенный угол</returns>
    public static double EaseAngle(this double angle)
    {
        var sign = Sign(angle);

        var normalized_angle = Abs(angle).Normalize();
        var percentage = normalized_angle / 360;

        normalized_angle = percentage.EaseInOut(normalized_angle, 5, 2);
        normalized_angle = Max(normalized_angle, 1);

        return sign < 0 ? sign * normalized_angle : normalized_angle;
    }

    /// <summary>Увеличивает угол на дельту и нормализует результат в диапазоне от -360 до 360</summary>
    /// <param name="angle">Угол в градусах для увеличения</param>
    /// <param name="delta">Дельта угла в градусах</param>
    /// <returns>Нормализованный угол после увеличения</returns>
    public static double Angle(this double angle, double delta) => (angle.Normalize() + delta).Normalize();

    /// <summary>Преобразует процент от 0 до 100 в пропорциональный угол от 0 до 360</summary>
    /// <param name="percent">Процент для преобразования</param>
    /// <returns>Угол от 0 до 360 пропорционально значению процента</returns>
    public static double Angle(this double percent)
    {
        if (DoubleUtil.LessThan(percent, 0) || DoubleUtil.GreaterThan(percent, 100))
            throw new ArgumentOutOfRangeException($"Percent '{percent}' must be between 0 to 100");

        return __FullCircleInDegrees / 100 * percent;
    }

    /// <summary>Создаёт дугу окружности для заданной точки, угла, радиуса и внутреннего радиуса</summary>
    /// <param name="location">Начальная точка</param>
    /// <param name="angle">Угол в градусах</param>
    /// <param name="radius">Радиус</param>
    /// <param name="InnerRadius">Внутренний радиус</param>
    /// <returns>Геометрия дуги окружности</returns>
    public static PathGeometry CreatePath(this Point location, double angle, double radius, double InnerRadius)
    {
        if (DoubleUtil.LessThan(radius, 0))
            throw new ArgumentOutOfRangeException($"Radius '{radius}' must be greater than zero.");
        if (DoubleUtil.LessThan(InnerRadius, 0))
            throw new ArgumentOutOfRangeException($"Inner radius '{InnerRadius}' must be greater than zero.");

        var is_large_arc = angle > __FullCircleInDegrees / 2;

        var arc_point = ConvertRadianToCartesian(angle, radius);
        var inner_arc_point = ConvertRadianToCartesian(angle, InnerRadius);

        var segments = new PathSegmentCollection
        {
            new LineSegment(location with { Y = location.Y - radius }, false),
            new ArcSegment(
                new(location.X + arc_point.X, location.Y + arc_point.Y),
                new(radius, radius),
                0,
                is_large_arc,
                SweepDirection.Clockwise,
                false),
            new LineSegment(new(location.X + inner_arc_point.X, location.Y + inner_arc_point.Y), false),
            new ArcSegment(
                location with { Y = location.Y - InnerRadius },
                new(InnerRadius, InnerRadius),
                0,
                is_large_arc,
                SweepDirection.Counterclockwise,
                false)
        };

        return new() { Figures = [new(location, segments, true)] };
    }

    /// <summary>Создаёт путь окружности, разделённой на заданное число сегментов</summary>
    /// <param name="point">Начальная точка</param>
    /// <param name="segments">Количество сегментов</param>
    /// <param name="margin">Зазор между сегментами в градусах</param>
    /// <param name="radius">Радиус</param>
    /// <param name="InnerRadius">Внутренний радиус</param>
    /// <returns>Суммарная геометрия окружности, разделённой на сегменты</returns>
    public static PathGeometry Create(
        this Point point,
        int segments,
        double margin,
        double radius,
        double InnerRadius)
    {
        if (segments <= 0) throw new ArgumentOutOfRangeException($"Segments '{segments}' must be greater than zero.");

        if (DoubleUtil.LessThan(margin, 0) || DoubleUtil.GreaterThan(margin, 360))
            throw new ArgumentOutOfRangeException($"Margin '{margin}' must be greater than zero and less than 360.");

        if (DoubleUtil.LessThan(radius, 0))
            throw new ArgumentOutOfRangeException($"Radius '{radius}' must be greater than zero.");

        if (DoubleUtil.LessThan(InnerRadius, 0))
            throw new ArgumentOutOfRangeException($"Inner radius '{InnerRadius}' must be greater than zero.");

        var angle_segment = 360d / segments - margin;
        var path_geometry = new PathGeometry();

        var angle = margin / 2;
        for (var i = 0; i < segments; i++)
        {
            var geometry = point.CreatePath(angle_segment, radius, InnerRadius);
            geometry.Transform = new RotateTransform(angle, point.X, point.Y);
            var segment_geometry = geometry.GetFlattenedPathGeometry();
            path_geometry.AddGeometry(segment_geometry);

            angle += margin + angle_segment;
        }

        return path_geometry;
    }

    /// <summary>Возвращает точку вектора для заданного угла и радиуса</summary>
    /// <param name="angle">Угол в градусах</param>
    /// <param name="radius">Радиус</param>
    /// <returns>Точка вектора</returns>
    public static Point ConvertRadianToCartesian(this double angle, double radius)
    {
        if (DoubleUtil.LessThan(radius, 0))
            throw new ArgumentOutOfRangeException($"{nameof(radius)} '{radius}' must be greater than zero.");

        var angle_radius = PI / (__FullCircleInDegrees / 2) * (angle - __FullCircleInDegrees / 4);
        var x = radius * Cos(angle_radius);
        var y = radius * Sin(angle_radius);
        return new(x, y);
    }

    /// <summary>Нормализует угол в градусах к диапазону от 0 до 360</summary>
    /// <param name="angle">Угол для нормализации</param>
    /// <returns>Нормализованный угол от 0 до 360</returns>
    public static double Normalize(this double angle)
    {
        var remainder = angle % __FullCircleInDegrees;

        if (DoubleUtil.GreaterThanOrClose(remainder, __FullCircleInDegrees))
            return remainder - __FullCircleInDegrees;

        if (DoubleUtil.LessThan(remainder, 0))
            remainder += __FullCircleInDegrees;

        return remainder;
    }

    /// <summary>Реализует EaseIn для экспоненциальной анимации роста</summary>
    /// <param name="TimeFraction">Доля времени от 0 до 1</param>
    /// <param name="start">Начальное значение</param>
    /// <param name="delta">Дельта между начальным и конечным значениями</param>
    /// <param name="power">Показатель экспоненциального роста</param>
    /// <returns>Результирующее значение</returns>
    public static double EaseIn(this double TimeFraction, double start, double delta, double power)
        => Pow(TimeFraction, power) * delta + start;

    /// <summary>Реализует EaseOut для экспоненциальной анимации затухания</summary>
    /// <param name="TimeFraction">Доля времени от 0 до 1</param>
    /// <param name="start">Начальное значение</param>
    /// <param name="delta">Дельта между начальным и конечным значениями</param>
    /// <param name="power">Показатель экспоненциального затухания</param>
    /// <returns>Результирующее значение</returns>
    public static double EaseOut(this double TimeFraction, double start, double delta, double power)
        => Pow(TimeFraction, 1 / power) * delta + start;

    /// <summary>Реализует EaseInOut для экспоненциальной анимации роста и затухания</summary>
    /// <param name="TimeFraction">Доля времени от 0 до 1</param>
    /// <param name="start">Начальное значение</param>
    /// <param name="delta">Дельта между начальным и конечным значениями</param>
    /// <param name="power">Показатель экспоненциального роста и затухания</param>
    /// <returns>Результирующее значение</returns>
    public static double EaseInOut(this double TimeFraction, double start, double delta, double power)
        => TimeFraction <= 0.5
            ? EaseOut(TimeFraction * 2, start, delta / 2, power)
            : EaseIn((TimeFraction - 0.5) * 2, start, delta / 2, power) + delta / 2;

    #endregion
}

/// <summary>Вспомогательные методы сравнения значений double</summary>
file static class DoubleUtil
{
    #region Types

    [StructLayout(LayoutKind.Explicit)]
    private struct NanUnion
    {
        [FieldOffset(0)]
        internal double DoubleValue;

        [FieldOffset(0)]
        internal readonly ulong UintValue;
    }

    #endregion

    #region Static

    // Константы взяты из sdk\inc\crt\float.h // кратко по делу
    private const double __DoubleEpsilon = 2.2204460492503131e-016; /* smallest such that 1.0+DoubleEpsilon != 1.0 */

    #endregion

    #region Methods

    /// <summary>Возвращает признак того, что два значения double близки друг к другу</summary>
    /// <returns>Результат сравнения</returns>
    /// <param name="value1">Первое значение для сравнения</param>
    /// <param name="value2">Второе значение для сравнения</param>
    public static bool AreClose(double value1, double value2)
    {
        // в случае бесконечностей проверка эпсилон не работает // кратко по делу
        // ReSharper disable CompareOfFloatsByEqualityOperator 
        if (value1 == value2) return true;
        // ReSharper restore CompareOfFloatsByEqualityOperator 

        // вычисляет (|value1-value2| / (|value1| + |value2| + 10.0)) < DoubleEpsilon // кратко по делу
        var eps = (Abs(value1) + Abs(value2) + 10.0) * __DoubleEpsilon;
        var delta = value1 - value2;
        return (-eps < delta) && (eps > delta);
    }

    /// <summary>Сравнивает два значения Size с учётом погрешности</summary>
    /// <param name='size1'>Первый размер для сравнения</param>
    /// <param name='size2'>Второй размер для сравнения</param>
    /// <returns>Признак равенства размеров</returns>
    public static bool AreClose(Size size1, Size size2) => AreClose(size1.Width, size2.Width) && AreClose(size1.Height, size2.Height);

    // Классы Point, Size, Rect и Matrix перемещены в WinCorLib // кратко по делу

    /// <summary>Сравнивает две точки с учётом погрешности</summary>
    /// <param name='point1'>Первая точка для сравнения</param>
    /// <param name='point2'>Вторая точка для сравнения</param>
    /// <returns>Признак равенства точек</returns>
    public static bool AreClose(Point point1, Point point2) => AreClose(point1.X, point2.X)
        && AreClose(point1.Y, point2.Y);

    /// <summary>Сравнивает два значения Vector с учётом погрешности</summary>
    /// <param name='vector1'>Первый вектор для сравнения</param>
    /// <param name='vector2'>Второй вектор для сравнения</param>
    /// <returns>Признак равенства векторов</returns>
    public static bool AreClose(Vector vector1, Vector vector2) => AreClose(vector1.X, vector2.X)
        && AreClose(vector1.Y, vector2.Y);

    /// <summary>Возвращает признак того, что первое значение меньше второго и не близко к нему</summary>
    /// <returns>Результат сравнения</returns>
    /// <param name="value1">Первое значение для сравнения</param>
    /// <param name="value2">Второе значение для сравнения</param>
    public static bool LessThan(double value1, double value2) => (value1 < value2) && !AreClose(value1, value2);

    /// <summary>Возвращает признак того, что первое значение больше второго и не близко к нему</summary>
    /// <returns>Результат сравнения</returns>
    /// <param name="value1">Первое значение для сравнения</param>
    /// <param name="value2">Второе значение для сравнения</param>
    public static bool GreaterThan(double value1, double value2) => (value1 > value2) && !AreClose(value1, value2);

    /// <summary>Возвращает признак того, что первое значение меньше второго или близко к нему</summary>
    /// <returns>Результат сравнения</returns>
    /// <param name="value1">Первое значение для сравнения</param>
    /// <param name="value2">Второе значение для сравнения</param>
    public static bool LessThanOrClose(double value1, double value2)
        => (value1 < value2) || AreClose(value1, value2);

    /// <summary>Возвращает признак того, что первое значение больше второго или близко к нему</summary>
    /// <returns>Результат сравнения</returns>
    /// <param name="value1">Первое значение для сравнения</param>
    /// <param name="value2">Второе значение для сравнения</param>
    public static bool GreaterThanOrClose(double value1, double value2)
        => (value1 > value2) || AreClose(value1, value2);

    /// <summary>Возвращает признак того, что значение близко к 1</summary>
    /// <returns>Результат сравнения</returns>
    /// <param name="value">Значение для сравнения с 1</param>
    public static bool IsOne(double value) => Abs(value - 1.0) < 10.0 * __DoubleEpsilon;

    /// <summary>Возвращает признак того, что значение близко к 0</summary>
    /// <returns>Результат сравнения</returns>
    /// <param name="value">Значение для сравнения с 0</param>
    public static bool IsZero(double value) => Abs(value) < 10.0 * __DoubleEpsilon;

    /// <summary>Проверяет, что значение является конечным числом</summary>
    /// <param name='value'>Значение для проверки</param>
    /// <returns>Признак конечного числа</returns>
    public static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);

    /// <summary>Проверяет, что значение допустимо для размера</summary>
    /// <param name='value'>Значение для проверки</param>
    /// <returns>Признак допустимого значения размера</returns>
    public static bool IsValidSize(double value) => IsFinite(value) && GreaterThanOrClose(value, 0);

    /// <summary>Проверяет, что значение является нечисловым</summary>
    /// <param name="value">Значение для проверки</param>
    /// <returns>True, если значение не является числом</returns>
    public static bool IsNaN(double value)
    {
        var t = new NanUnion { DoubleValue = value };

        var exp = t.UintValue & 0xfff0000000000000;
        var man = t.UintValue & 0x000fffffffffffff;

        return exp is 0x7ff0000000000000 or 0xfff0000000000000 && (man != 0);
    }

    #endregion
}