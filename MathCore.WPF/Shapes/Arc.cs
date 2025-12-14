using System.Windows;
using System.Windows.Media;
// ReSharper disable UnusedType.Global
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Shapes;

/// <summary>Фигура WPF для рисования дуги окружности</summary>
/// <remarks>
/// Используется в XAML как обычная фигура, например внутри элемента <see cref="System.Windows.Controls.Canvas"/>
/// Свойства <see cref="StartAngle"/>, <see cref="StopAngle"/> и <see cref="R"/> управляют положением и размером дуги
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;Window
///     x:Class="DemoApp.MainWindow"
///     xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
///     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
///     xmlns:shapes="clr-namespace:MathCore.WPF.Shapes;assembly=MathCore.WPF"&gt;
///     &lt;Grid&gt;
///         &lt;shapes:Arc
///             Stroke="Red"
///             StrokeThickness="2"
///             R="1"
///             StartAngle="0"
///             StopAngle="180" /&gt;
///     &lt;/Grid&gt;
/// &lt;/Window&gt;
/// </code>
/// </example>
public class Arc : ShapeBase
{
    private const double FullCircleDegrees = 360d;
    private const double MinArcDegrees = 1e-6; // минимальная длина дуги в градусах, ниже считаем дугу нулевой

    static Arc()
    {
        //StretchProperty.OverrideMetadata(typeof(Arc), new FrameworkPropertyMetadata(Stretch.None));
    }

    /// <summary>Радиус дуги в относительных единицах от 0 до 1</summary>
    /// <remarks>
    /// Значение интерпретируется относительно размеров контейнера
    /// При значении 1 дуга строится по максимальному доступному радиусу в пределах прямоугольника
    /// Значения вне диапазона [0;1] автоматически ограничиваются
    /// </remarks>
    public double R { get => (double)GetValue(RProperty); set => SetValue(RProperty, value); }

    /// <summary>Радиус дуги</summary>
    public static readonly DependencyProperty RProperty =
        DependencyProperty.Register(nameof(R),
            typeof(double),
            typeof(Arc),
            new FrameworkPropertyMetadata(1D,
                FrameworkPropertyMetadataOptions.AffectsRender,
                null,
                CoerceR));

    /// <summary>Начальный угол дуги в градусах</summary>
    /// <remarks>
    /// Отсчёт ведётся по часовой стрелке, 0 градусов направлен вправо, 90 градусов вниз
    /// Дуга рисуется от <see cref="StartAngle"/> к <see cref="StopAngle"/>, знак разности задаёт направление обхода
    /// </remarks>
    public double StartAngle { get => (double)GetValue(StartAngleProperty); set => SetValue(StartAngleProperty, value); }

    /// <summary>Свойство зависимости начального угла дуги</summary>
    public static readonly DependencyProperty StartAngleProperty =
        DependencyProperty.Register(nameof(StartAngle),
            typeof(double),
            typeof(Arc),
            new FrameworkPropertyMetadata(0D,
                FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Конечный угол дуги в градусах</summary>
    /// <remarks>
    /// Если разница между <see cref="StartAngle"/> и <see cref="StopAngle"/> по модулю близка к 360 градусам,
    /// будет отрисована полная окружность вместо дуги
    /// </remarks>
    public double StopAngle { get => (double)GetValue(StopAngleProperty); set => SetValue(StopAngleProperty, value); }

    /// <summary>Свойство зависимости конечного угла дуги</summary>
    public static readonly DependencyProperty StopAngleProperty =
        DependencyProperty.Register(nameof(StopAngle),
            typeof(double),
            typeof(Arc),
            new FrameworkPropertyMetadata(360d,
                FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Геометрия, определяющая отображаемую дугу</summary>
    protected override Geometry DefiningGeometry =>
        _VisibleRect is { IsEmpty: false, Width: > 0, Height: > 0 }
            ? GetGeometry(_VisibleRect, StartAngle, StopAngle, R)
            : Geometry.Empty;

    private static object CoerceR(DependencyObject d, object base_value)
    {
        var r = (double)base_value;
        if (r < 0) return 0d;
        if (r > 1) return 1d;
        return r;
    }

    /// <summary>Нормализует угол к диапазону [0;360)</summary>
    private static double NormalizeAngle(double angle)
    {
        angle %= FullCircleDegrees;
        return angle < 0 ? angle + FullCircleDegrees : angle;
    }

    /// <summary>Вычисляет координаты точки дуги по углу и радиусу в пределах прямоугольника</summary>
    /// <param name="a">Угол в градусах</param>
    /// <param name="r">Радиус в относительных единицах</param>
    /// <param name="rect">Прямоугольник отрисовки</param>
    /// <returns>Координаты точки дуги</returns>
    private static Point GetPoint(double a, double r, Rect rect)
    {
        const double to_rad = Math.PI / 180;

        a = (a - 90) * to_rad;

        var half_width = rect.Width / 2;
        var half_height = rect.Height / 2;

        var center_x = rect.Left + half_width;
        var center_y = rect.Top + half_height;

        var radius_x = half_width * r;
        var radius_y = half_height * r;

        var x = center_x + radius_x * Math.Cos(a);
        var y = center_y + radius_y * Math.Sin(a);

        return new(x, y);
    }

    /// <summary>Создает геометрию дуги по заданным параметрам</summary>
    /// <param name="rect">Прямоугольник ограничивающей области</param>
    /// <param name="Start">Начальный угол дуги в градусах</param>
    /// <param name="End">Конечный угол дуги в градусах</param>
    /// <param name="Radius">Радиус дуги в относительных единицах</param>
    /// <returns>Геометрия дуги или пустая геометрия</returns>
    private static Geometry GetGeometry(Rect rect, double Start, double End, double Radius)
    {
        var w = rect.Width;
        var h = rect.Height;
        if (w == 0 || h == 0) return Geometry.Empty; // Если хотя бы одна из сторон прямоугольника равна нулю, возвращаем пустую геометрию

        var start_angle = NormalizeAngle(Start);
        var end_angle = NormalizeAngle(End);

        var d_raw = end_angle - start_angle;
        var d_abs = Math.Abs(d_raw);

        // Если длина дуги по модулю близка к полному кругу, рисуем полную окружность
        if (d_abs >= FullCircleDegrees - MinArcDegrees)
            return new EllipseGeometry(rect);

        // Слишком маленькая дуга считается нулевой
        if (d_abs < MinArcDegrees)
            return Geometry.Empty;

        var p1 = GetPoint(start_angle, Radius, rect); // Вычисляем координаты начальной точки дуги
        var p2 = GetPoint(end_angle, Radius, rect); // Вычисляем координаты конечной точки дуги

        var half_width = w / 2;
        var half_height = h / 2;

        var radius_x = Math.Max(0, half_width * Radius);
        var radius_y = Math.Max(0, half_height * Radius);
        var arc = new Size(radius_x, radius_y); // Размеры дуги (радиусы эллипса), гарантируем неотрицательность

        var is_large = d_abs > 180; // Определяем, является ли дуга большой (более 180 градусов)
        var sweep_direction = d_raw >= 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise; // Учитываем направление дуги

        var geometry = new StreamGeometry(); // Создаём потоковую геометрию для описания дуги
        using var context = geometry.Open(); // Открываем контекст для построения фигуры
        context.BeginFigure(p1, isFilled: false, isClosed: false); // Начинаем фигуру с первой точки, без заливки и без замыкания контура
        context.ArcTo(p2, arc, 0, is_large, sweep_direction, isStroked: true, isSmoothJoin: false); // Строим дугу от первой до второй точки

        geometry.Freeze(); // Оптимизация: делаем геометрию неизменяемой

        return geometry; // Возвращаем построенную геометрию дуги
    }
}