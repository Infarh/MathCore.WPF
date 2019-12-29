using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MathCore.WPF.Shapes
{
    public class Pie : Shape
    {
        private const FrameworkPropertyMetadataOptions c_DependendPropertyMetadataOptions =
            FrameworkPropertyMetadataOptions.AffectsRender;

        static Pie()
        {
            //StretchProperty.OverrideMetadata(typeof(Pie), new FrameworkPropertyMetadata(Stretch.None));
            StrokeProperty.OverrideMetadata(typeof(Pie), new FrameworkPropertyMetadata(Brushes.Gray));
            StrokeThicknessProperty.OverrideMetadata(typeof(Pie), new FrameworkPropertyMetadata(1d));
            FillProperty.OverrideMetadata(typeof(Pie), new FrameworkPropertyMetadata(Brushes.LightGray));
        }

        public static readonly DependencyProperty IsAlignedProperty =
            DependencyProperty.Register(nameof(IsAligned),
                typeof(bool),
                typeof(Pie),
                new FrameworkPropertyMetadata(false, c_DependendPropertyMetadataOptions,
                    (o, e) => { }, (o, a) => a,
                    isAnimationProhibited: false,
                    defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged));

        public bool IsAligned { get => (bool)GetValue(IsAlignedProperty); set => SetValue(IsAlignedProperty, value); }

        public static readonly DependencyProperty OuterRadiusProperty =
            DependencyProperty.Register(nameof(OuterRadius),
                typeof(double),
                typeof(Pie),
                new FrameworkPropertyMetadata(1d, c_DependendPropertyMetadataOptions,
                    (o, e) => { },
                    (o, R) => Math.Max((double)R, ((Pie)o).InnerRadius),
                    isAnimationProhibited: false,
                    defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged),
                ValidateRadius);

        public double OuterRadius { get => (double)GetValue(OuterRadiusProperty); set => SetValue(OuterRadiusProperty, value); }

        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register(nameof(InnerRadius),
                typeof(double),
                typeof(Pie),
                new FrameworkPropertyMetadata(0d, c_DependendPropertyMetadataOptions,
                    (o, e) => { },
                    (o, r) => Math.Min((double)r, ((Pie)o).OuterRadius),
                    isAnimationProhibited: false,
                    defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged),
                ValidateRadius);

        private static bool ValidateRadius(object r) => (double)r >= 0 && (double)r <= 1;

        public double InnerRadius { get => (double)GetValue(InnerRadiusProperty); set => SetValue(InnerRadiusProperty, value); }

        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register(nameof(StartAngle),
                typeof(double),
                typeof(Pie),
                new FrameworkPropertyMetadata(0d, c_DependendPropertyMetadataOptions,
                    (o, e) => { },
                    (o, v) => v));

        public double StartAngle { get => (double)GetValue(StartAngleProperty); set => SetValue(StartAngleProperty, value); }

        public static readonly DependencyProperty StopAngleProperty =
            DependencyProperty.Register(nameof(StopAngle),
                typeof(double),
                typeof(Pie),
                new FrameworkPropertyMetadata(360d, c_DependendPropertyMetadataOptions,
                    OnStopAngleChanged,
                    (o, v) => v));

        private static void OnStopAngleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) =>
            o.SetValue(AngleProperty, (double)e.NewValue - ((Pie)o).StartAngle);

        public double StopAngle { get => (double)GetValue(StopAngleProperty); set => SetValue(StopAngleProperty, value); }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register(nameof(Angle),
                typeof(double),
                typeof(Pie),
                new FrameworkPropertyMetadata(360d, c_DependendPropertyMetadataOptions,
                    OnAngleChanged,
                    (o, v) => v,
                    isAnimationProhibited: false,
                    defaultUpdateSourceTrigger: UpdateSourceTrigger.PropertyChanged));

        private static void OnAngleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) =>
            o.SetValue(StopAngleProperty, (double)e.NewValue + ((Pie)o).StartAngle);

        public double Angle { get => (double)GetValue(AngleProperty); set => SetValue(AngleProperty, value); }

        private readonly EllipseGeometry _OuterEllipse = new EllipseGeometry();
        private readonly EllipseGeometry _InnerEllipse = new EllipseGeometry();
        private readonly CombinedGeometry _Cycle;
        private readonly CombinedGeometry _Pie;
        private static readonly Func<double, double, double> min = Math.Min;
        private static readonly Func<double, double, double> max = Math.Max;
        private Rect _Rect;

        protected override Geometry DefiningGeometry
        {
            get
            {
                if(_Rect.IsEmpty || _Rect.Width.Equals(0d) || _Rect.Height.Equals(0d))
                    return Geometry.Empty;
                return GetGeometry(_Rect, StartAngle, StopAngle, OuterRadius, InnerRadius, IsAligned);
            }
        }

        public Pie()
        {
            _Cycle = new CombinedGeometry(GeometryCombineMode.Exclude, _OuterEllipse, _InnerEllipse);
            _Pie = new CombinedGeometry(GeometryCombineMode.Exclude, _OuterEllipse, _InnerEllipse);
        }

        protected override Size MeasureOverride(Size ConstraintSize)
        {
            _Rect = Rect.Empty;
            return base.MeasureOverride(ConstraintSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);
            var t = StrokeThickness;
            var m = t / 2;
            _Rect = size.IsEmpty || size.Width.Equals(0d) || size.Height.Equals(0d)
                ? Rect.Empty
                : new Rect(m, m, max(0, size.Width - t), max(0, size.Height - t));

            switch(Stretch)
            {
                case Stretch.None:
                    //_Rect.Width = _Rect.Height = 0;
                    break;
                case Stretch.Fill:
                    break;
                case Stretch.Uniform:
                    if(_Rect.Width > _Rect.Height)
                        _Rect.Width = _Rect.Height;
                    else
                        _Rect.Height = _Rect.Width;
                    break;
                case Stretch.UniformToFill:
                    if(_Rect.Width < _Rect.Height)
                        _Rect.Width = _Rect.Height;
                    else
                        _Rect.Height = _Rect.Width;
                    break;
            }
            return size;
        }

        private Geometry GetGeometry(Rect rect, double start, double stop, double R, double r, bool aligned)
        {
            var a = Math.Abs(stop - start);
            if(a.Equals(0d))
                return Geometry.Empty;
            ChangeGeometry(rect, R, r, aligned);
            if(a.Equals(0d) || Math.Abs(a) >= 360)
                if(r.Equals(0d))
                    return _OuterEllipse;
                else
                    return _Cycle;
            var geometry = new StreamGeometry();
            using(var geometry_context = geometry.Open())
                DrawGeometry(geometry_context, rect, R, r, start, stop, aligned);
            geometry.Freeze();
            if(r.Equals(0d)) return geometry;
            _Pie.Geometry1 = geometry;
            return _Pie;
        }

        private void ChangeGeometry(Rect rect, double R, double r, bool aligned)
        {
            var center = new Point(rect.Width / 2 + rect.Left, rect.Height / 2 + rect.Left);
            var w = rect.Width;
            var h = rect.Height;
            if(aligned)
            {
                w = min(w, h);
                h = min(w, h);
            }

            w /= 2;
            h /= 2;

            _OuterEllipse.Center = _InnerEllipse.Center = center;
            _OuterEllipse.RadiusX = w * R;
            _OuterEllipse.RadiusY = h * R;
            _InnerEllipse.RadiusX = w * r;
            _InnerEllipse.RadiusY = h * r;
        }

        //private static Point GetPoint(Point p0, Rect rect, double a, double r, double w, double h)
        //{
        //    const double to_rad = Math.PI / 180.0;
        //    a -= 90;
        //    a *= to_rad;
        //    return new Point(p0.X + r * Math.Cos(a) * w / 2, p0.Y + r * Math.Sin(a) * h / 2);
        //}

        private static Point GetPoint(Rect rect, double a, double r)
        {
            const double to_rad = Math.PI / 180;
            a -= 90;
            a *= to_rad;
            r /= 2;
            var x = (0.5 + r * Math.Cos(a)) * rect.Width + rect.Left;
            var y = (0.5 + r * Math.Sin(a)) * rect.Height + rect.Top;
            return new Point(x, y);
        }

        private static void DrawGeometry(StreamGeometryContext g, Rect rect, double R, double r, double start, double stop, bool aligned)
        {
            var w = rect.Width;
            var h = rect.Height;
            if(w <= 0 || h <= 0) return;
            var p0 = new Point(0.5 * rect.Width + rect.Left, 0.5 * rect.Height + rect.Top);

            var a = min(start, stop);
            var b = max(start, stop);
            var d = b - a;
            if(d.Equals(0d)) return;

            if(aligned)
            {
                w = min(w, h);
                h = min(w, h);
            }

            var in_arc_stop = GetPoint(rect, a, r);
            var out_arc_start = GetPoint(rect, a, R);
            var out_arc_stop = GetPoint(rect, b, R);
            var in_arc_start = GetPoint(rect, b, r);


            var arc_isout = d > 180.0;
            var in_arc_size = new Size(r * w / 2, r * h / 2);
            var out_arc_size = new Size(R * w / 2, R * h / 2);

            var line_only = Math.Abs(R - r) < 0.001;
            if(line_only)
                g.BeginFigure(out_arc_start, false, true);
            else
            {
                g.BeginFigure(r.Equals(0d) ? p0 : in_arc_stop, true, true);
                g.LineTo(out_arc_start, true, true);
            }
            g.ArcTo(out_arc_stop, out_arc_size, 0, arc_isout, SweepDirection.Clockwise, true, true);
            if(r.Equals(0d) || line_only) return;
            g.LineTo(in_arc_start, true, true);
            g.ArcTo(in_arc_stop, in_arc_size, 0, arc_isout, SweepDirection.Counterclockwise, true, true);
        }
    }
}
