using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using M = System.Windows.Media.Matrix;

namespace MathCore.WPF.Temp
{
    /// <summary>
    /// Фрактал?
    /// http://en.wikipedia.org/wiki/Barnsley_fern
    /// </summary>
    public static class BarnsleyFern
    {
        public static List<Point> Generate(int n = 1000, double width = 1.0, double height = 1.0)
        {
            // Transformations
            var a1 = new MatrixTransform(new M(0.85, -0.04, 0.04, 0.85, 0, 1.6));
            var a2 = new MatrixTransform(new M(0.20, 0.23, -0.26, 0.22, 0, 1.6));
            var a3 = new MatrixTransform(new M(-0.15, 0.26, 0.28, 0.24, 0, 0.44));
            var a4 = new MatrixTransform(new M(0, 0, 0, 0.16, 0, 0));
            var random = new Random(17);
            var point = new Point(0.5, 0.5);
            var points = new List<Point>();

            // Transformation for [-3,3,0,10] => output coordinates
            var T = new MatrixTransform(new M(width / 6.0, 0, 0, -height / 10.1, width / 2.0, height));

            for(var i = 0; i < n; i++)
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
}
