using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Shapes;

public class PointLine : Shape
{
    private const FrameworkPropertyMetadataOptions __Options = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender;

    static PointLine()
    {
        Line.X1Property.OverrideMetadata(typeof(PointLine),
            new FrameworkPropertyMetadata(0d, __Options,
                (s, e) => ((PointLine)s).Start = new Point((double)e.NewValue, (double)s.GetValue(Line.Y1Property))));
        Line.Y1Property.OverrideMetadata(typeof(PointLine),
            new FrameworkPropertyMetadata(0d, __Options,
                (s, e) => ((PointLine)s).Start = new Point((double)s.GetValue(Line.X1Property), (double)e.NewValue)));
        Line.X2Property.OverrideMetadata(typeof(PointLine),
            new FrameworkPropertyMetadata(0d, __Options,
                (s, e) => ((PointLine)s).Start = new Point((double)e.NewValue, (double)s.GetValue(Line.Y2Property))));
        Line.Y2Property.OverrideMetadata(typeof(PointLine),
            new FrameworkPropertyMetadata(0d, __Options,
                (s, e) => ((PointLine)s).Start = new Point((double)s.GetValue(Line.X2Property), (double)e.NewValue)));
    }

    public double X1 { get => (double)GetValue(Line.X1Property); set => SetValue(Line.X1Property, value); }
    public double X2 { get => (double)GetValue(Line.X2Property); set => SetValue(Line.X2Property, value); }
    public double Y1 { get => (double)GetValue(Line.Y1Property); set => SetValue(Line.Y1Property, value); }
    public double Y2 { get => (double)GetValue(Line.Y2Property); set => SetValue(Line.Y2Property, value); }


    public static readonly DependencyProperty StartProperty = DependencyProperty.Register(
        nameof(Start),
        typeof(Point),
        typeof(PointLine),
        new FrameworkPropertyMetadata(new Point(0, 0), __Options));

    public Point Start
    {
        get => (Point)GetValue(StartProperty);
        set => SetValue(StartProperty, value);
    }

    public static readonly DependencyProperty EndProperty = DependencyProperty.Register(
        nameof(End),
        typeof(Point),
        typeof(PointLine),
        new FrameworkPropertyMetadata(new Point(0, 0), __Options));

    public Point End
    {
        get => (Point)GetValue(EndProperty);
        set => SetValue(EndProperty, value);
    }

    protected override Geometry DefiningGeometry => throw new NotImplementedException();
}