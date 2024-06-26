﻿using System.Windows;
using System.Windows.Media;
// ReSharper disable UnusedType.Global
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Shapes;

public class Arc : ShapeBase
{
    static Arc()
    {
        //StretchProperty.OverrideMetadata(typeof(Arc), new FrameworkPropertyMetadata(Stretch.None));
    }

    public double R { get => (double)GetValue(RProperty); set => SetValue(RProperty, value); }

    public static readonly DependencyProperty RProperty =
        DependencyProperty.Register(nameof(R),
            typeof(double),
            typeof(Arc),
            new FrameworkPropertyMetadata(1D,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public double StartAngle { get => (double)GetValue(StartAngleProperty); set => SetValue(StartAngleProperty, value); }

    public static readonly DependencyProperty StartAngleProperty =
        DependencyProperty.Register(nameof(StartAngle),
            typeof(double),
            typeof(Arc),
            new FrameworkPropertyMetadata(0D,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public double StopAngle { get => (double)GetValue(StopAngleProperty); set => SetValue(StopAngleProperty, value); }

    public static readonly DependencyProperty StopAngleProperty =
        DependencyProperty.Register(nameof(StopAngle),
            typeof(double),
            typeof(Arc),
            new FrameworkPropertyMetadata(360d,
                FrameworkPropertyMetadataOptions.AffectsRender));

    protected override Geometry DefiningGeometry =>
        _VisibleRect is { IsEmpty: false, Width: > 0, Height: > 0 }
            ? GetGeometry(_VisibleRect, StartAngle, StopAngle, R)
            : Geometry.Empty;

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

    private static Geometry GetGeometry(Rect rect, double Start, double End, double Radius)
    {
        var w = rect.Width;
        var h = rect.Height;
        if(w == 0 || h == 0) return Geometry.Empty;

        var d = Math.Abs(End - Start);
        if(d >= 360) return new EllipseGeometry(rect);

        var p1 = GetPoint(Math.Min(Start, End), Radius, rect);
        var p2 = GetPoint(Math.Max(Start, End), Radius, rect);

        /* To draw the arc in perfect way instead of seeing it as Big arc */
        var y   = w / 2 * Radius;
        var y1  = h / 2 * Radius;
        var arc = new Size(Math.Max(0, y), Math.Max(0, y1));

        var is_large = d > 180;

        var       geometry = new StreamGeometry();
        using var context  = geometry.Open();
        context.BeginFigure(p1, isFilled: false, isClosed: false);
        context.ArcTo(p2, arc, 0, is_large, SweepDirection.Clockwise, isStroked: true, isSmoothJoin: false);

        return geometry;
    }
}