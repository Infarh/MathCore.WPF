using System.Windows;
using System.Windows.Media;

namespace MathCore.WPF.Temp;

/// <summary>Генератор набора точек фрактала Папоротник Барнсли методом итеративной функции</summary>
/// <remarks>
/// Класс предоставляет статический метод Generate который строит заданное количество точек фрактала 
/// с использованием четырёх афинных преобразований и вероятностного выбора преобразований 
/// с последующим масштабированием в координаты вывода<br/>
/// http://en.wikipedia.org/wiki/Barnsley_fern
/// </remarks>
/// <example>
/// <code>
/// // Пример получения 10000 точек и привязки к размерам холста
/// var points = BarnsleyFern.Generate(10000, canvas.ActualWidth, canvas.ActualHeight);
/// // далее можно отрисовать points на WPF холсте например через DrawingContext или Polyline
/// </code>
/// </example>
public static class BarnsleyFern
{
    public static List<Point> Generate(int n = 1000, double width = 1.0, double height = 1.0)
    {
        // Transformations
        var a1 = new MatrixTransform(new(0.85, -0.04, 0.04, 0.85, 0, 1.6));
        var a2 = new MatrixTransform(new(0.20, 0.23, -0.26, 0.22, 0, 1.6));
        var a3 = new MatrixTransform(new(-0.15, 0.26, 0.28, 0.24, 0, 0.44));
        var a4 = new MatrixTransform(new(0, 0, 0, 0.16, 0, 0));
        var random = new Random(17);
        var point = new Point(0.5, 0.5);
        var points = new List<Point>();

        // Transformation for [-3,3,0,10] => output coordinates
        var T = new MatrixTransform(new(width / 6.0, 0, 0, -height / 10.1, width / 2.0, height));

        for (var i = 0; i < n; i++)
            points.Add(T.Transform(random.NextDouble() switch
            {
                < 0.85 => a1.Transform(point),
                < .92 => a2.Transform(point),
                < .99 => a3.Transform(point),
                _ => a4.Transform(point)
            }));

        return points;
    }
}