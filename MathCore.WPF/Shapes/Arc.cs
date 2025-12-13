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
    static Arc()
    {
        //StretchProperty.OverrideMetadata(typeof(Arc), new FrameworkPropertyMetadata(Stretch.None));
    }

    /// <summary>Радиус дуги в относительных единицах от 0 до 1</summary>
    /// <remarks>
    /// Значение интерпретируется относительно размеров контейнера
    /// При значении 1 дуга строится по максимальному доступному радиусу в пределах прямоугольника
    /// </remarks>
    public double R { get => (double)GetValue(RProperty); set => SetValue(RProperty, value); }

    /// <summary>Радиус дуги</summary>
    public static readonly DependencyProperty RProperty =
        DependencyProperty.Register(nameof(R),
            typeof(double),
            typeof(Arc),
            new FrameworkPropertyMetadata(1D,
                FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Начальный угол дуги в градусах</summary>
    /// <remarks>
    /// Отсчёт ведётся по часовой стрелке, 0 градусов направлен вправо, 90 градусов вниз
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
    /// Если разница между <see cref="StartAngle"/> и <see cref="StopAngle"/> больше либо равна 360,
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

    /// <summary>Вычисляет координаты точки дуги по углу и радиусу в пределах прямоугольника</summary>
    /// <param name="a">Угол в градусах</param>
    /// <param name="r">Радиус в относительных единицах</param>
    /// <param name="rect">Прямоугольник отрисовки</param>
    /// <returns>Координаты точки дуги</returns>
    private static Point GetPoint(double a, double r, Rect rect)
    {
        const double to_rad = Math.PI / 180;
        a -= 90;
        a *= to_rad;
        r /= 2;
        var x = (0.5 + r * Math.Cos(a)) * rect.Width + rect.Left;
        var y = (0.5 + r * Math.Sin(a)) * rect.Height + rect.Top;
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

        var d = Math.Abs(End - Start);
        if (d >= 360) return new EllipseGeometry(rect); // Если угол больше либо равен полному обороту, рисуем окружность целиком

        var p1 = GetPoint(Math.Min(Start, End), Radius, rect); // Вычисляем координаты начальной точки дуги
        var p2 = GetPoint(Math.Max(Start, End), Radius, rect); // Вычисляем координаты конечной точки дуги

        /* Рисуем дугу корректным образом, чтобы она не считалась большой дугой */
        var y = w / 2 * Radius; // Половина ширины прямоугольника, масштабированная радиусом, как горизонтальная полуось
        var y1 = h / 2 * Radius; // Половина высоты прямоугольника, масштабированная радиусом, как.vertical C# 7.3+
        var arc = new Size(Math.Max(0, y), Math.Max(0, y1)); // Размеры дуги (радиусы эллипса), гарантируем неотрицательность

        var is_large = d > 180; // Определяем, является ли дуга большой (более 180 градусов)

        var geometry = new StreamGeometry(); // Создаём потоковую геометрию для описания дуги
        using var context = geometry.Open(); // Открываем контекст для построения фигуры
        context.BeginFigure(p1, isFilled: false, isClosed: false); // Начинаем фигуру с первой точки, без заливки и без замыкания контура
        context.ArcTo(p2, arc, 0, is_large, SweepDirection.Clockwise, isStroked: true, isSmoothJoin: false); // Строим дугу от первой до второй точки по часовой стрелке

        return geometry; // Возвращаем построенную геометрию дуги
    }
}