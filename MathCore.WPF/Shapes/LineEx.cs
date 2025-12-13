using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;

namespace MathCore.WPF.Shapes;

/// <summary>Расширение для класса Line с присоединяемыми свойствами-зависимостями для работы с точками</summary>
/// <remarks>
/// Предоставляет присоединяемые свойства P1 и P2 для привязки к моделям-представлениям.
/// Обеспечивает автоматическую синхронизацию между точками (P1, P2) и координатами (X1, Y1, X2, Y2).
/// При использовании в XAML достаточно указать P1 и P2, координаты будут обновляться автоматически.
/// При изменении координат (X1, Y1, X2, Y2) прямо в коде Point'ы обновляться не будут;
/// для синхронизации в обратном направлении используйте привязку координат.
/// 
/// Использует ConditionalWeakTable для хранения вспомогательных объектов, что предотвращает утечку памяти
/// при использовании в шаблонах элементов (ItemsControl, ListBox и т.д.).
/// </remarks>
/// <example>
/// Привязка через точки (рекомендуется):
/// <code language="xaml">
/// &lt;shapes:Line
///     local:LineEx.P1="{Binding StartPoint}"
///     local:LineEx.P2="{Binding EndPoint}"
///     Stroke="Blue"
///     StrokeThickness="2" /&gt;
/// </code>
/// 
/// Привязка отдельных координат (альтернатива):
/// <code language="xaml">
/// &lt;shapes:Line
///     X1="{Binding StartPoint.X}"
///     Y1="{Binding StartPoint.Y}"
///     X2="{Binding EndPoint.X}"
///     Y2="{Binding EndPoint.Y}"
///     Stroke="Blue"
///     StrokeThickness="2" /&gt;
/// </code>
/// </example>
public static class LineEx
{
    // ConditionalWeakTable автоматически удаляет записи, когда Line больше не используется
    private static readonly ConditionalWeakTable<Line, LineBindingHelper> __HelperCache = new();
    private static bool __IsUpdatingP1 = false;
    private static bool __IsUpdatingP2 = false;

    #region P1 (начальная точка)

    /// <summary>Определяет присоединяемое свойство-зависимость для начальной точки линии</summary>
    public static readonly DependencyProperty P1Property =
        DependencyProperty.RegisterAttached(
            "P1",
            typeof(Point),
            typeof(LineEx),
            new FrameworkPropertyMetadata(
                new Point(0, 0),
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnP1Changed,
                coerceValueCallback: null,
                isAnimationProhibited: false,
                defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged));

    /// <summary>Получает начальную точку линии</summary>
    /// <param name="obj">Элемент Line</param>
    /// <returns>Начальная точка</returns>
    public static Point GetP1(DependencyObject obj) => (Point)obj.GetValue(P1Property);

    /// <summary>Устанавливает начальную точку линии</summary>
    /// <param name="obj">Элемент Line</param>
    /// <param name="value">Начальная точка</param>
    public static void SetP1(DependencyObject obj, Point value) => obj.SetValue(P1Property, value);

    private static void OnP1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Line line || __IsUpdatingP1)
            return;

        __IsUpdatingP1 = true;
        try
        {
            var point = (Point)e.NewValue;
            line.X1 = point.X;
            line.Y1 = point.Y;

            EnsureHelper(line).NotifyP1Changed();
        }
        finally
        {
            __IsUpdatingP1 = false;
        }
    }

    #endregion

    #region P2 (конечная точка)

    /// <summary>Определяет присоединяемое свойство-зависимость для конечной точки линии</summary>
    public static readonly DependencyProperty P2Property =
        DependencyProperty.RegisterAttached(
            "P2",
            typeof(Point),
            typeof(LineEx),
            new FrameworkPropertyMetadata(
                new Point(0, 0),
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnP2Changed,
                coerceValueCallback: null,
                isAnimationProhibited: false,
                defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged));

    /// <summary>Получает конечную точку линии</summary>
    /// <param name="obj">Элемент Line</param>
    /// <returns>Конечная точка</returns>
    public static Point GetP2(DependencyObject obj) => (Point)obj.GetValue(P2Property);

    /// <summary>Устанавливает конечную точку линии</summary>
    /// <param name="obj">Элемент Line</param>
    /// <param name="value">Конечная точка</param>
    public static void SetP2(DependencyObject obj, Point value) => obj.SetValue(P2Property, value);

    private static void OnP2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Line line || __IsUpdatingP2)
            return;

        __IsUpdatingP2 = true;
        try
        {
            var point = (Point)e.NewValue;
            line.X2 = point.X;
            line.Y2 = point.Y;

            EnsureHelper(line).NotifyP2Changed();
        }
        finally
        {
            __IsUpdatingP2 = false;
        }
    }

    #endregion

    private static LineBindingHelper EnsureHelper(Line line)
    {
        if (!__HelperCache.TryGetValue(line, out var helper))
        {
            helper = new LineBindingHelper(line);
            __HelperCache.Add(line, helper);
        }
        return helper;
    }

    /// <summary>Вспомогательный класс для отслеживания изменений координат линии</summary>
    private class LineBindingHelper : DependencyObject
    {
        private readonly Line _Line;
        private Point _LastP1;
        private Point _LastP2;

        public LineBindingHelper(Line line)
        {
            _Line = line;
            _LastP1 = new Point(line.X1, line.Y1);
            _LastP2 = new Point(line.X2, line.Y2);

            // Подписываемся на изменения X1
            var x1_binding = new Binding { Source = line, Path = new PropertyPath(Line.X1Property), Mode = BindingMode.OneWay };
            BindingOperations.SetBinding(this, X1Property, x1_binding);

            // Подписываемся на изменения Y1
            var y1_binding = new Binding { Source = line, Path = new PropertyPath(Line.Y1Property), Mode = BindingMode.OneWay };
            BindingOperations.SetBinding(this, Y1Property, y1_binding);

            // Подписываемся на изменения X2
            var x2_binding = new Binding { Source = line, Path = new PropertyPath(Line.X2Property), Mode = BindingMode.OneWay };
            BindingOperations.SetBinding(this, X2Property, x2_binding);

            // Подписываемся на изменения Y2
            var y2_binding = new Binding { Source = line, Path = new PropertyPath(Line.Y2Property), Mode = BindingMode.OneWay };
            BindingOperations.SetBinding(this, Y2Property, y2_binding);
        }

        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register(nameof(X1), typeof(double), typeof(LineBindingHelper),
                new PropertyMetadata(0.0, OnX1Changed));

        public double X1 { get => (double)GetValue(X1Property); set => SetValue(X1Property, value); }

        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register(nameof(Y1), typeof(double), typeof(LineBindingHelper),
                new PropertyMetadata(0.0, OnY1Changed));

        public double Y1 { get => (double)GetValue(Y1Property); set => SetValue(Y1Property, value); }

        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register(nameof(X2), typeof(double), typeof(LineBindingHelper),
                new PropertyMetadata(0.0, OnX2Changed));

        public double X2 { get => (double)GetValue(X2Property); set => SetValue(X2Property, value); }

        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register(nameof(Y2), typeof(double), typeof(LineBindingHelper),
                new PropertyMetadata(0.0, OnY2Changed));

        public double Y2 { get => (double)GetValue(Y2Property); set => SetValue(Y2Property, value); }

        private static void OnX1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LineBindingHelper helper)
                helper.OnCoordinatesChanged();
        }

        private static void OnY1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LineBindingHelper helper)
                helper.OnCoordinatesChanged();
        }

        private static void OnX2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LineBindingHelper helper)
                helper.OnCoordinatesChanged();
        }

        private static void OnY2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LineBindingHelper helper)
                helper.OnCoordinatesChanged();
        }

        private void OnCoordinatesChanged()
        {
            var current_p1 = new Point(X1, Y1);
            var current_p2 = new Point(X2, Y2);

            if (current_p1 != _LastP1)
            {
                _LastP1 = current_p1;
                SetP1(_Line, current_p1);
            }

            if (current_p2 != _LastP2)
            {
                _LastP2 = current_p2;
                SetP2(_Line, current_p2);
            }
        }

        public void NotifyP1Changed() => _LastP1 = new Point(X1, Y1);
        public void NotifyP2Changed() => _LastP2 = new Point(X2, Y2);
    }
}
