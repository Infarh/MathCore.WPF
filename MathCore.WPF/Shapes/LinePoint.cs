using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Shapes;

public static class LinePoint
{
    private static readonly List<WeakReference> __StartPointAttachedLinesList = [];
    private static readonly List<WeakReference> __EndPointAttachedLinesList = [];

    private static bool IsStartAttached(Line line)
    {
        lock (__StartPointAttachedLinesList)
        {
            var to_remove = new List<WeakReference>(__StartPointAttachedLinesList.Count);
            var exist     = false;
            for(var i = 0; i < __StartPointAttachedLinesList.Count; i++)
            {
                var obj = __StartPointAttachedLinesList[i].Target;
                if(obj is null)
                    to_remove.Add(__StartPointAttachedLinesList[i]);
                else if(ReferenceEquals(obj, line))
                {
                    exist = true;
                    i     = __StartPointAttachedLinesList.Count;
                }
            }
            for(var i = 0; i < to_remove.Count; i++)
                __StartPointAttachedLinesList.Remove(to_remove[i]);
            if(!exist) __StartPointAttachedLinesList.Add(new WeakReference(line));
            return exist;
        }
    }

    private static bool IsEndAttached(Line line)
    {
        lock (__EndPointAttachedLinesList)
        {
            var to_remove = new List<WeakReference>(__EndPointAttachedLinesList.Count);
            var exist     = false;
            for(var i = 0; i < __EndPointAttachedLinesList.Count; i++)
            {
                var obj = __EndPointAttachedLinesList[i].Target;
                if(obj is null)
                    to_remove.Add(__EndPointAttachedLinesList[i]);
                else if(ReferenceEquals(obj, line))
                {
                    exist = true;
                    i     = __EndPointAttachedLinesList.Count;
                }
            }
            for(var i = 0; i < to_remove.Count; i++)
                __EndPointAttachedLinesList.Remove(to_remove[i]);
            if(!exist) __EndPointAttachedLinesList.Add(new WeakReference(line));
            return exist;
        }
    }

    private const FrameworkPropertyMetadataOptions __Options = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender;
    public static readonly DependencyProperty StartProperty = DependencyProperty.RegisterAttached(
        "Start",
        typeof(Point),
        typeof(LinePoint),
        new FrameworkPropertyMetadata(new Point(0, 0), __Options, OnStartPointChanged));

    private static void OnStartPointChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        var line = (Line)D;
        if(IsStartAttached(line)) return;

        line.SetBinding(Line.X1Property, new Binding
        {
            Source              = line,
            Path                = new PropertyPath("(0)", StartProperty),
            Converter           = new Lambda<Point, double>(start => start.X, x1 => new Point(x1, line.Y1)),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            Mode                = BindingMode.TwoWay
        });

        line.SetBinding(Line.Y1Property, new Binding
        {
            Source              = line,
            Path                = new PropertyPath("(0)", StartProperty),
            Converter           = new Lambda<Point, double>(start => start.Y, y1 => new Point(line.X1, y1)),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            Mode                = BindingMode.TwoWay
        });
    }

    public static void SetStart(DependencyObject element, Point value) => element.SetValue(StartProperty, value);

    public static Point GetStart(DependencyObject element) => (Point)element.GetValue(StartProperty);

    public static readonly DependencyProperty EndProperty = DependencyProperty.RegisterAttached(
        "End",
        typeof(Point),
        typeof(LinePoint),
        new FrameworkPropertyMetadata(new Point(0, 0), __Options, OnEndPointChanged));

    private static void OnEndPointChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        var line = (Line)D;
        if(IsEndAttached(line)) return;
        line.SetBinding(Line.X2Property, new Binding
        {
            Source              = line,
            Path                = new PropertyPath("(0)", EndProperty),
            Converter           = new Lambda<Point, double>(end => end.X, x2 => new Point(x2, line.Y2)),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            Mode                = BindingMode.TwoWay
        });
        line.SetBinding(Line.Y2Property, new Binding
        {
            Source              = line,
            Path                = new PropertyPath("(0)", EndProperty),
            Converter           = new Lambda<Point, double>(end => end.Y, y2 => new Point(line.X2, y2)),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            Mode                = BindingMode.TwoWay
        });
    }

    public static void SetEnd(DependencyObject element, Point value) => element.SetValue(EndProperty, value);

    public static Point GetEnd(DependencyObject element) => (Point)element.GetValue(EndProperty);
}