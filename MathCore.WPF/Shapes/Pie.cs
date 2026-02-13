using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

// ReSharper disable ArgumentsStyleAnonymousFunction

// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Shapes;

/// <summary>Определяет форму сектора круга или кольца</summary>
public class Pie : Shape
{
    private const FrameworkPropertyMetadataOptions __DependendPropertyMetadataOptions =
        FrameworkPropertyMetadataOptions.AffectsRender;

    private const double FullCircleDegrees = 360d;
    private const double MinArcDegrees = 1e-6; // минимальная длина дуги в градусах

    static Pie()
    {
        //StretchProperty.OverrideMetadata(typeof(Pie), new FrameworkPropertyMetadata(Stretch.None));
        StrokeProperty.OverrideMetadata(typeof(Pie), new FrameworkPropertyMetadata(Brushes.Gray));
        StrokeThicknessProperty.OverrideMetadata(typeof(Pie), new FrameworkPropertyMetadata(1d));
        FillProperty.OverrideMetadata(typeof(Pie), new FrameworkPropertyMetadata(Brushes.LightGray));
    }

    /// <summary>Определяет зависимое свойство для выравнивания сектора по меньшему размеру</summary>
    public static readonly DependencyProperty IsAlignedProperty =
        DependencyProperty.Register(nameof(IsAligned),
            typeof(bool),
            typeof(Pie),
            new FrameworkPropertyMetadata(false, __DependendPropertyMetadataOptions,
                propertyChangedCallback: null,
                coerceValueCallback: null,
                isAnimationProhibited: false,
                defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged));

    /// <summary>Получает или устанавливает значение, указывающее, выравнивается ли сектор по меньшему размеру</summary>
    public bool IsAligned { get => (bool)GetValue(IsAlignedProperty); set => SetValue(IsAlignedProperty, value); }

    /// <summary>Определяет зависимое свойство для внешнего радиуса сектора</summary>
    public static readonly DependencyProperty OuterRadiusProperty =
        DependencyProperty.Register(nameof(OuterRadius),
            typeof(double),
            typeof(Pie),
            new FrameworkPropertyMetadata(1d, __DependendPropertyMetadataOptions,
                propertyChangedCallback: null,
                coerceValueCallback: (o, R) => Math.Max((double)R, ((Pie)o).InnerRadius),
                isAnimationProhibited: false,
                defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged),
            ValidateRadius);

    /// <summary>Получает или устанавливает внешний радиус сектора (от 0 до 1)</summary>
    public double OuterRadius { get => (double)GetValue(OuterRadiusProperty); set => SetValue(OuterRadiusProperty, value); }

    /// <summary>Определяет зависимое свойство для внутреннего радиуса сектора</summary>
    public static readonly DependencyProperty InnerRadiusProperty =
        DependencyProperty.Register(nameof(InnerRadius),
            typeof(double),
            typeof(Pie),
            new FrameworkPropertyMetadata(0d, __DependendPropertyMetadataOptions,
                propertyChangedCallback: null,
                coerceValueCallback: (o, r) => Math.Min((double)r, ((Pie)o).OuterRadius),
                isAnimationProhibited: false,
                defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged),
            ValidateRadius);

    /// <summary>Проверяет, что радиус находится в диапазоне [0, 1]</summary>
    private static bool ValidateRadius(object r) => (double)r is >= 0 and <= 1;

    /// <summary>Получает или устанавливает внутренний радиус сектора (от 0 до 1)</summary>
    public double InnerRadius { get => (double)GetValue(InnerRadiusProperty); set => SetValue(InnerRadiusProperty, value); }

    /// <summary>Определяет зависимое свойство для начального угла сектора</summary>
    public static readonly DependencyProperty StartAngleProperty =
        DependencyProperty.Register(nameof(StartAngle),
            typeof(double),
            typeof(Pie),
            new FrameworkPropertyMetadata(0d, __DependendPropertyMetadataOptions,
                propertyChangedCallback: null,
                coerceValueCallback: null));

    /// <summary>Получает или устанавливает начальный угол сектора в градусах</summary>
    /// <remarks>
    /// Отсчёт ведётся по часовой стрелке, 0 градусов направлен вправо, 90 градусов вниз
    /// Сектор всегда рисуется по часовой стрелке от нормализованного <see cref="StartAngle"/> к нормализованному <see cref="StopAngle"/>
    /// </remarks>
    public double StartAngle { get => (double)GetValue(StartAngleProperty); set => SetValue(StartAngleProperty, value); }

    /// <summary>Определяет зависимое свойство для конечного угла сектора</summary>
    public static readonly DependencyProperty StopAngleProperty =
        DependencyProperty.Register(nameof(StopAngle),
            typeof(double),
            typeof(Pie),
            new FrameworkPropertyMetadata(360d, __DependendPropertyMetadataOptions,
                propertyChangedCallback: OnStopAngleChanged,
                coerceValueCallback: null));

    /// <summary>Обработчик изменения конечного угла</summary>
    private static void OnStopAngleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) =>
        o.SetValue(AngleProperty, (double)e.NewValue - ((Pie)o).StartAngle);

    /// <summary>Получает или устанавливает конечный угол сектора в градусах</summary>
    /// <remarks>
    /// После нормализации обоих углов к диапазону [0;360) сектор рисуется по часовой стрелке
    /// Примеры:
    /// - StartAngle=270, StopAngle=60 → сектор 150° по часовой (270→360→60)
    /// - StartAngle=60, StopAngle=270 → сектор 210° по часовой (60→270)
    /// - StartAngle=0, StopAngle=360 → полный круг (разность исходных углов = 360°)
    /// </remarks>
    public double StopAngle { get => (double)GetValue(StopAngleProperty); set => SetValue(StopAngleProperty, value); }

    /// <summary>Определяет зависимое свойство для угла раствора сектора</summary>
    public static readonly DependencyProperty AngleProperty =
        DependencyProperty.Register(nameof(Angle),
            typeof(double),
            typeof(Pie),
            new FrameworkPropertyMetadata(360d, __DependendPropertyMetadataOptions,
                propertyChangedCallback: OnAngleChanged,
                coerceValueCallback: null,
                isAnimationProhibited: false,
                defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged));

    /// <summary>Обработчик изменения угла раствора</summary>
    private static void OnAngleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) =>
        o.SetValue(StopAngleProperty, (double)e.NewValue + ((Pie)o).StartAngle);

    /// <summary>Получает или устанавливает угол раствора сектора в градусах</summary>
    public double Angle { get => (double)GetValue(AngleProperty); set => SetValue(AngleProperty, value); }

    private readonly EllipseGeometry _OuterEllipse = new();
    private readonly EllipseGeometry _InnerEllipse = new();
    private readonly CombinedGeometry _Cycle;
    private readonly CombinedGeometry _Pie;
    private Rect _Rect;

    /// <summary>Получает геометрию, определяющую форму сектора</summary>
    protected override Geometry DefiningGeometry =>
        _Rect is { IsEmpty: false, Width: > 0, Height: > 0 }
            ? GetGeometry(_Rect, StartAngle, StopAngle, OuterRadius, InnerRadius, IsAligned)
            : Geometry.Empty;

    /// <summary>Инициализирует новый экземпляр класса Pie</summary>
    public Pie()
    {
        _Cycle = new(GeometryCombineMode.Exclude, _OuterEllipse, _InnerEllipse);
        _Pie = new(GeometryCombineMode.Exclude, _OuterEllipse, _InnerEllipse);
    }

    /// <summary>Измеряет размер сектора</summary>
    protected override Size MeasureOverride(Size ConstraintSize)
    {
        _Rect = Rect.Empty;
        return base.MeasureOverride(ConstraintSize);
    }

    /// <summary>Располагает сектор в пределах отведённого пространства</summary>
    protected override Size ArrangeOverride(Size FinalSize)
    {
        var size = base.ArrangeOverride(FinalSize);
        var t = StrokeThickness;
        var m = t / 2;
        _Rect = size.IsEmpty || size.Width.Equals(0d) || size.Height.Equals(0d)
            ? Rect.Empty
            : new(m, m, Math.Max(0, size.Width - t), Math.Max(0, size.Height - t));

        switch (Stretch)
        {
            case Stretch.None:
                //_Rect.Width = _Rect.Height = 0;
                break;
            case Stretch.Fill:
                break;
            case Stretch.Uniform:
                if (_Rect.Width > _Rect.Height)
                    _Rect.Width = _Rect.Height;
                else
                    _Rect.Height = _Rect.Width;
                break;
            case Stretch.UniformToFill:
                if (_Rect.Width < _Rect.Height)
                    _Rect.Width = _Rect.Height;
                else
                    _Rect.Height = _Rect.Width;
                break;
        }
        return size;
    }

    /// <summary>Вычисляет геометрию сектора на основе заданных параметров</summary>
    /// <param name="rect">Прямоугольник ограничивающей области</param>
    /// <param name="start">Начальный угол в градусах</param>
    /// <param name="stop">Конечный угол в градусах</param>
    /// <param name="R">Внешний радиус (от 0 до 1)</param>
    /// <param name="r">Внутренний радиус (от 0 до 1)</param>
    /// <param name="aligned">Флаг выравнивания по меньшему размеру</param>
    /// <returns>Геометрия сектора</returns>
    private Geometry GetGeometry(Rect rect, double start, double stop, double R, double r, bool aligned)
    {
        var a = Math.Abs(stop - start);
        if (a is 0d)
            return Geometry.Empty;
        ChangeGeometry(rect, R, r, aligned);
        if (a is 0d || Math.Abs(a) >= 360)
            return r is 0d
                ? _OuterEllipse
                : _Cycle;

        var geometry = new StreamGeometry();
        using (var geometry_context = geometry.Open())
            DrawGeometry(geometry_context, rect, R, r, start, stop, aligned);
        geometry.Freeze();
        if (r is 0d) return geometry;
        _Pie.Geometry1 = geometry;
        return _Pie;
    }

    /// <summary>Обновляет геометрию эллипсов внешнего и внутреннего радиусов</summary>
    /// <param name="rect">Прямоугольник ограничивающей области</param>
    /// <param name="R">Внешний радиус</param>
    /// <param name="r">Внутренний радиус</param>
    /// <param name="aligned">Флаг выравнивания по меньшему размеру</param>
    private void ChangeGeometry(Rect rect, double R, double r, bool aligned)
    {
        var center = new Point(rect.Width / 2 + rect.Left, rect.Height / 2 + rect.Left);
        var w = rect.Width;
        var h = rect.Height;
        if (aligned) w = h = Math.Min(w, h);

        w /= 2;
        h /= 2;

        _OuterEllipse.Center = _InnerEllipse.Center = center;
        _OuterEllipse.RadiusX = w * R;
        _OuterEllipse.RadiusY = h * R;
        _InnerEllipse.RadiusX = w * r;
        _InnerEllipse.RadiusY = h * r;
    }

    /// <summary>Вычисляет координата точки на эллипсе по углу и радиусу</summary>
    /// <param name="rect">Прямоугольник ограничивающей области</param>
    /// <param name="a">Угол в градусах</param>
    /// <param name="r">Радиус (от 0 до 1)</param>
    /// <returns>Координата точки на эллипсе</returns>
    private static Point GetPoint(Rect rect, double a, double r)
    {
        const double to_rad = Math.PI / 180;
        a -= 90;
        a *= to_rad;
        r /= 2;
        var x = (0.5 + r * Math.Cos(a)) * rect.Width + rect.Left;
        var y = (0.5 + r * Math.Sin(a)) * rect.Height + rect.Top;
        return new(x, y);
    }

    /// <summary>Нормализует угол к диапазону [0;360)</summary>
    private static double NormalizeAngle(double angle)
    {
        angle %= FullCircleDegrees;
        return angle < 0 ? angle + FullCircleDegrees : angle;
    }

    /// <summary>Рисует геометрию сектора в контекст потока</summary>
    /// <param name="g">Контекст потока геометрии</param>
    /// <param name="rect">Прямоугольник ограничивающей области</param>
    /// <param name="R">Внешний радиус</param>
    /// <param name="r">Внутренний радиус</param>
    /// <param name="start">Начальный угол</param>
    /// <param name="stop">Конечный угол</param>
    /// <param name="aligned">Флаг выравнивания</param>
    private static void DrawGeometry(StreamGeometryContext g, Rect rect, double R, double r, double start, double stop, bool aligned)
    {
        // Получаем ширину и высоту прямоугольника
        var w = rect.Width;
        var h = rect.Height;
        if (w <= 0 || h <= 0) return;

        // Вычисляем центральную точку прямоугольника
        var p0 = new Point(0.5 * rect.Width + rect.Left, 0.5 * rect.Height + rect.Top);

        // Нормализуем углы к диапазону [0;360)
        var start_angle = NormalizeAngle(start);
        var stop_angle = NormalizeAngle(stop);

        // Вычисляем угловое расстояние по часовой стрелке от start_angle до stop_angle
        var delta_clockwise = stop_angle - start_angle;
        if (delta_clockwise < 0) delta_clockwise += FullCircleDegrees; // Приводим к диапазону [0;360)

        // Слишком маленькая дуга считается нулевой
        if (delta_clockwise < MinArcDegrees) return;

        // Если включено выравнивание, приводим к квадрату по меньшей стороне
        if (aligned)
        {
            w = Math.Min(w, h);
            h = Math.Min(w, h);
        }

        // Вычисляем ключевые точки для построения сектора:
        var out_arc_start = GetPoint(rect, start_angle, R);  // начальная точка внешней дуги
        var out_arc_stop = GetPoint(rect, stop_angle, R);     // конечная точка внешней дуги
        var in_arc_start = GetPoint(rect, start_angle, r);   // начальная точка внутренней дуги
        var in_arc_stop = GetPoint(rect, stop_angle, r);      // конечная точка внутренней дуги

        // Определяем тип дуги (большая дуга если угол > 180°)
        var arc_isout = delta_clockwise > 180.0;

        // Вычисляем размеры эллипсов для внутренней и внешней дуг
        var in_arc_size = new Size(r * w / 2, r * h / 2);
        var out_arc_size = new Size(R * w / 2, R * h / 2);

        // Проверяем, является ли сектор просто линией (внутренний и внешний радиусы почти равны)
        var line_only = Math.Abs(R - r) < 0.001;

        // Начинаем построение фигуры
        if (line_only)
            g.BeginFigure(out_arc_start, false, true); // Только дуга, без заливки
        else
        {
            // Начинаем с центра (если r = 0) или с точки на внутренней дуге
            g.BeginFigure(r is 0d ? p0 : in_arc_start, true, true);
            g.LineTo(out_arc_start, true, true); // Линия к началу внешней дуги
        }

        // Рисуем внешнюю дугу от начального до конечного угла по часовой стрелке
        g.ArcTo(out_arc_stop, out_arc_size, 0, arc_isout, SweepDirection.Clockwise, true, true);

        if (r is 0d || line_only) return; // Если внутренний радиус 0 или это линия, завершаем

        // Рисуем линию к началу внутренней дуги
        g.LineTo(in_arc_stop, true, true);

        // Рисуем внутреннюю дугу от конечного до начального угла против часовой стрелки
        g.ArcTo(in_arc_start, in_arc_size, 0, arc_isout, SweepDirection.Counterclockwise, true, true);
    }
}