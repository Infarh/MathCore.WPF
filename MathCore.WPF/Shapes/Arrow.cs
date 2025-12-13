using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

// ReSharper disable ArgumentsStyleAnonymousFunction
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Shapes;

/// <summary>Визуальный элемент стрелки с настраиваемыми параметрами линии и головы</summary>
/// <remarks>
/// Стрелка состоит из линии и головы в виде треугольника
/// Поддерживает настройку координат начала и конца, размеров головы, стиля линии и заливки
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;shapes:Arrow
///     X1="10" Y1="10"
///     X2="100" Y2="100"
///     ArrowHeadWidth="10"
///     ArrowHeadLength="15"
///     IsArrowHeadClosed="True"
///     Stroke="Blue"
///     StrokeThickness="2"
///     Fill="LightBlue" /&gt;
/// </code>
/// </example>
public class Arrow : Shape
{
    private const FrameworkPropertyMetadataOptions __DependencyPropertyMetadataOptions =
        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure;

    static Arrow()
    {
        StrokeProperty.OverrideMetadata(typeof(Arrow), new FrameworkPropertyMetadata(Brushes.Black));
        StrokeThicknessProperty.OverrideMetadata(typeof(Arrow), new FrameworkPropertyMetadata(1d));
        FillProperty.OverrideMetadata(typeof(Arrow), new FrameworkPropertyMetadata(Brushes.Black));
    }

    #region Координаты линии стрелки

    /// <summary>Определяет зависимое свойство для X-координаты начальной точки стрелки</summary>
    public static readonly DependencyProperty X1Property =
        DependencyProperty.Register(nameof(X1),
            typeof(double),
            typeof(Arrow),
            new FrameworkPropertyMetadata(0d, __DependencyPropertyMetadataOptions,
                propertyChangedCallback: null,
                coerceValueCallback: null,
                isAnimationProhibited: false,
                defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged));

    /// <summary>Получает или устанавливает X-координату начальной точки стрелки</summary>
    public double X1 { get => (double)GetValue(X1Property); set => SetValue(X1Property, value); }

    /// <summary>Определяет зависимое свойство для Y-координаты начальной точки стрелки</summary>
    public static readonly DependencyProperty Y1Property =
        DependencyProperty.Register(nameof(Y1),
            typeof(double),
            typeof(Arrow),
            new FrameworkPropertyMetadata(0d, __DependencyPropertyMetadataOptions,
                propertyChangedCallback: null,
                coerceValueCallback: null,
                isAnimationProhibited: false,
                defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged));

    /// <summary>Получает или устанавливает Y-координату начальной точки стрелки</summary>
    public double Y1 { get => (double)GetValue(Y1Property); set => SetValue(Y1Property, value); }

    /// <summary>Определяет зависимое свойство для X-координаты конечной точки стрелки</summary>
    public static readonly DependencyProperty X2Property =
        DependencyProperty.Register(nameof(X2),
            typeof(double),
            typeof(Arrow),
            new FrameworkPropertyMetadata(0d, __DependencyPropertyMetadataOptions,
                propertyChangedCallback: null,
                coerceValueCallback: null,
                isAnimationProhibited: false,
                defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged));

    /// <summary>Получает или устанавливает X-координату конечной точки стрелки</summary>
    public double X2 { get => (double)GetValue(X2Property); set => SetValue(X2Property, value); }

    /// <summary>Определяет зависимое свойство для Y-координаты конечной точки стрелки</summary>
    public static readonly DependencyProperty Y2Property =
        DependencyProperty.Register(nameof(Y2),
            typeof(double),
            typeof(Arrow),
            new FrameworkPropertyMetadata(0d, __DependencyPropertyMetadataOptions,
                propertyChangedCallback: null,
                coerceValueCallback: null,
                isAnimationProhibited: false,
                defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged));

    /// <summary>Получает или устанавливает Y-координату конечной точки стрелки</summary>
    public double Y2 { get => (double)GetValue(Y2Property); set => SetValue(Y2Property, value); }

    #endregion

    #region Параметры головы стрелки

    /// <summary>Определяет зависимое свойство для ширины головы стрелки</summary>
    public static readonly DependencyProperty ArrowHeadWidthProperty =
        DependencyProperty.Register(nameof(ArrowHeadWidth),
            typeof(double),
            typeof(Arrow),
            new FrameworkPropertyMetadata(10d, __DependencyPropertyMetadataOptions,
                propertyChangedCallback: null,
                coerceValueCallback: (o, value) => Math.Max(0d, (double)value),
                isAnimationProhibited: false,
                defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged));

    /// <summary>Получает или устанавливает ширину головы стрелки</summary>
    public double ArrowHeadWidth { get => (double)GetValue(ArrowHeadWidthProperty); set => SetValue(ArrowHeadWidthProperty, value); }

    /// <summary>Определяет зависимое свойство для длины головы стрелки</summary>
    public static readonly DependencyProperty ArrowHeadLengthProperty =
        DependencyProperty.Register(nameof(ArrowHeadLength),
            typeof(double),
            typeof(Arrow),
            new FrameworkPropertyMetadata(15d, __DependencyPropertyMetadataOptions,
                propertyChangedCallback: null,
                coerceValueCallback: (o, value) => Math.Max(0d, (double)value),
                isAnimationProhibited: false,
                defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged));

    /// <summary>Получает или устанавливает длину головы стрелки</summary>
    public double ArrowHeadLength { get => (double)GetValue(ArrowHeadLengthProperty); set => SetValue(ArrowHeadLengthProperty, value); }

    /// <summary>Определяет зависимое свойство для замкнутости контура головы стрелки</summary>
    public static readonly DependencyProperty IsArrowHeadClosedProperty =
        DependencyProperty.Register(nameof(IsArrowHeadClosed),
            typeof(bool),
            typeof(Arrow),
            new FrameworkPropertyMetadata(true, __DependencyPropertyMetadataOptions,
                propertyChangedCallback: null,
                coerceValueCallback: null,
                isAnimationProhibited: false,
                defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged));

    /// <summary>Получает или устанавливает значение, указывающее, замкнут ли контур головы стрелки</summary>
    public bool IsArrowHeadClosed { get => (bool)GetValue(IsArrowHeadClosedProperty); set => SetValue(IsArrowHeadClosedProperty, value); }

    #endregion

    /// <summary>Получает геометрию, определяющую форму стрелки</summary>
    protected override Geometry DefiningGeometry => GetGeometry(X1, Y1, X2, Y2, ArrowHeadWidth, ArrowHeadLength, IsArrowHeadClosed);

    /// <summary>Вычисляет геометрию стрелки на основе заданных параметров</summary>
    /// <param name="x1">X-координата начальной точки</param>
    /// <param name="y1">Y-координата начальной точки</param>
    /// <param name="x2">X-координата конечной точки</param>
    /// <param name="y2">Y-координата конечной точки</param>
    /// <param name="head_width">Ширина головы стрелки</param>
    /// <param name="head_length">Длина головы стрелки</param>
    /// <param name="head_closed">Замкнут ли контур головы стрелки</param>
    /// <returns>Геометрия стрелки</returns>
    private static Geometry GetGeometry(double x1, double y1, double x2, double y2, double head_width, double head_length, bool head_closed)
    {
        // Вычисляем вектор направления стрелки
        var dx = x2 - x1;
        var dy = y2 - y1;
        var length = Math.Sqrt(dx * dx + dy * dy);

        // Если стрелка имеет нулевую длину, возвращаем пустую геометрию
        if (length < 1e-10)
            return Geometry.Empty;

        // Нормализуем вектор направления
        var dir_x = dx / length;
        var dir_y = dy / length;

        // Вычисляем перпендикулярный вектор для построения головы стрелки
        var perp_x = -dir_y;
        var perp_y = dir_x;

        // Создаём группу геометрий для линии и головы стрелки
        var geometry_group = new GeometryGroup();

        // Вычисляем точку основания головы стрелки (где заканчивается линия)
        var line_end_x = x2 - dir_x * head_length;
        var line_end_y = y2 - dir_y * head_length;

        // Создаём линию стрелки от начальной точки до основания головы
        var line = new LineGeometry(new Point(x1, y1), new Point(line_end_x, line_end_y));
        geometry_group.Children.Add(line);

        // Создаём голову стрелки в виде треугольника
        if (head_width > 0 && head_length > 0)
        {
            var arrow_head = new StreamGeometry();
            using (var context = arrow_head.Open())
            {
                // Вычисляем три вершины треугольника головы стрелки:
                // 1. Острие стрелки (конечная точка)
                var tip = new Point(x2, y2);

                // 2. Левая точка основания головы
                var left_base_x = line_end_x + perp_x * head_width / 2;
                var left_base_y = line_end_y + perp_y * head_width / 2;
                var left_base = new Point(left_base_x, left_base_y);

                // 3. Правая точка основания головы
                var right_base_x = line_end_x - perp_x * head_width / 2;
                var right_base_y = line_end_y - perp_y * head_width / 2;
                var right_base = new Point(right_base_x, right_base_y);

                // Рисуем треугольник головы стрелки
                context.BeginFigure(tip, head_closed, head_closed);
                context.LineTo(left_base, true, true);
                context.LineTo(right_base, true, true);
            }
            arrow_head.Freeze();
            geometry_group.Children.Add(arrow_head);
        }

        return geometry_group;
    }
}
