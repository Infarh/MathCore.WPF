using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(Point[]), typeof(PathGeometry))]
    public class Points2PathGeometry : ValueConverter
    {
        #region IValueConverter Members

        protected override object? Convert(object? v, Type? t, object? p, System.Globalization.CultureInfo? c)
        {
            if(v is not Point[] points || points.Length <= 0) return null;
            var start = points[0];
            var segments = new List<LineSegment>();
            for(var i = 1; i < points.Length; i++)
                segments.Add(new LineSegment(points[i], true));
            var figure = new PathFigure(start, segments, false); //true if closed
            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            return geometry;
        }
        
        #endregion
    }
}