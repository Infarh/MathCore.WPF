using System.Windows;
using System.Windows.Media;

namespace MathCore.WPF.TeX;

/// <summary>
/// Рендерер для отрисовки иерархии боксов математической формулы в WPF DrawingContext
/// </summary>
/// <remarks>
/// Класс служит мостом между миром боксов (логическое представление) и WPF отрисовкой.
/// Хранит исходный бокс и коэффициент масштабирования, а также предоставляет методы
/// для отрисовки и получения размеров формулы.
/// </remarks>
/// <example>
/// <code><![CDATA[
/// var formula = new TexFormula();
/// // ... построить формулу ...
/// 
/// var renderer = formula.GetRenderer(TexStyle.Display, 20.0);
/// 
/// // Получить размер для разметки
/// var size = renderer.RenderSize;
/// var baseline = renderer.Baseline;
/// 
/// // Отрисовать формулу
/// using (var drawingGroup = new DrawingGroup())
/// {
///     var dc = drawingGroup.Open();
///     renderer.Render(dc, 100, 50);
///     dc.Close();
/// }
/// ]]></code>
/// </example>
public class TexRenderer
{
    /// <summary>Главный бокс, содержащий всю иерархию боксов формулы</summary>
    public Box Box { get; set; }

    /// <summary>Коэффициент масштабирования, применяемый при рендеринге</summary>
    public double Scale { get; }

    /// <summary>
    /// Размер отрисованной формулы в пиксельных точках (с учётом масштабирования)
    /// </summary>
    public Size RenderSize => new(Box.Width * Scale, Box.TotalHeight * Scale);

    /// <summary>
    /// Расстояние от верхнего края до базовой линии формулы (используется для выравнивания)
    /// </summary>
    public double Baseline => Box.Height / Box.TotalHeight * Scale;

    /// <summary>
    /// Инициализирует новый экземпляр рендерера (обычно создаётся через TexFormula.GetRenderer)
    /// </summary>
    /// <param name="box">Главный бокс со всей иерархией формулы</param>
    /// <param name="scale">Коэффициент масштабирования</param>
    internal TexRenderer(Box box, double scale) 
    { 
        Box = box; 
        Scale = scale; 
    }

    /// <summary>
    /// Отрисовывает формулу в указанный DrawingContext на заданные координаты
    /// </summary>
    /// <param name="DrawingContext">WPF контекст отрисовки</param>
    /// <param name="x">Координата X в пиксельных точках</param>
    /// <param name="y">Координата Y в пиксельных точках</param>
    /// <remarks>
    /// Метод преобразует координаты, учитывая масштаб, и вызывает методы отрисовки боксов.
    /// Координата Y интерпретируется как верхний край формулы.
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// using (var drawingGroup = new DrawingGroup())
    /// {
    ///     var dc = drawingGroup.Open();
    ///     // Отрисовать формулу в позицию (10, 10)
    ///     renderer.Render(dc, 10, 10);
    ///     dc.Close();
    ///     
    ///     // Использовать результат в визуальном элементе
    ///     var drawingVisual = new DrawingVisual();
    ///     drawingVisual.Drawing = drawingGroup;
    /// }
    /// ]]></code>
    /// </example>
    public void Render(DrawingContext DrawingContext, double x, double y)
    {
        var box   = Box;
        var scale = Scale;
        box.Draw(DrawingContext, scale, x / scale, y / scale + box.Height);
    }
}