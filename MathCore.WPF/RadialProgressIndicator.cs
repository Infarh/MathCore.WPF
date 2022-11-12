using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

using static System.Math;

namespace MathCore.WPF;

public class RadialProgressIndicator : FrameworkElement
{
    #region Fields

    private Brush _ActiveForegourndBrush;

    private Pen _ActivePen;

    private Geometry? _BorderGeometry;

    private Point _Center;

    private Geometry? _CurrentGeometry;

    private Pen _ForegourndPen;

    private bool _IsListening;
    private int _LastTick;
    private int _TickCount;
    private Brush _ForegroundBrush;
    private Geometry? _ProgressBorderGeometry;
    private Geometry? _ProgressGeometry;
    private double _Radius;
    private double _RotationAngle;

    #endregion

    #region Constructors

    /// <summary>Static meta data registrations</summary>
    static RadialProgressIndicator() =>
        IsEnabledProperty.OverrideMetadata(
            typeof(RadialProgressIndicator),
            new UIPropertyMetadata(
                false,
                (o, e) => (o as RadialProgressIndicator)?.OnIsEnabledChanged((bool)e.OldValue, (bool)e.NewValue)));

    /// <summary>Initializes a new instance of <see cref="RadialProgressIndicator" /></summary>
    public RadialProgressIndicator()
    {
        _IsListening   =  false;
        _Radius        =  0;
        _Center        =  new Point();
        _RotationAngle =  0;
        Unloaded       += OnUnloaded;
    }

    #endregion

    #region Foreground

    /// <summary>Dependency property for Foreground</summary>
    public static readonly DependencyProperty ForegroundProperty =
        TextElement.ForegroundProperty.AddOwner(
            typeof(RadialProgressIndicator),
            new FrameworkPropertyMetadata(
                SystemColors.ControlTextBrush,
                FrameworkPropertyMetadataOptions.Inherits,
                (o, e) => (o as RadialProgressIndicator)?.OnForegroundChanged((Brush)e.NewValue)));

    /// <summary>Foreground property</summary>
    public Brush Foreground
    {
        get => (Brush)GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    #endregion Foreground

    #region ActiveForeground

    /// <summary>Dependency property for ActiveForeground</summary>
    public static readonly DependencyProperty ActiveForegroundProperty =
        DependencyProperty.Register(
            nameof(ActiveForeground),
            typeof(Brush),
            typeof(RadialProgressIndicator),
            new FrameworkPropertyMetadata(
                SystemColors.ControlBrush,
                FrameworkPropertyMetadataOptions.AffectsRender,
                (o, e) => (o as RadialProgressIndicator)?.OnActiveForegroundChanged((Brush)e.NewValue)));

    /// <summary>    ActiveForeground property.</summary>
    public Brush ActiveForeground
    {
        get => (Brush)GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    #endregion ActiveForeground

    #region CurrentValue

    /// <summary>    Dependency property for CurrentValue.</summary>
    public static readonly DependencyProperty CurrentValueProperty =
        DependencyProperty.Register(
            nameof(CurrentValue),
            typeof(double),
            typeof(RadialProgressIndicator),
            new FrameworkPropertyMetadata(
                0d,
                FrameworkPropertyMetadataOptions.AffectsRender,
                (o, _) => (o as RadialProgressIndicator)?.OnCurrentValueChanged(),
                (_, e) => DoubleUtil.LessThan((double)e, 0) ? 0 : (DoubleUtil.GreaterThan((double)e, 100) ? 100 : (double)e)));

    /// <summary>Current value property.</summary>
    public double CurrentValue
    {
        get => (double)GetValue(CurrentValueProperty);
        set => SetValue(CurrentValueProperty, value);
    }

    #endregion CurrentValue

    #region Overrides

    /// <summary>
    ///     When overridden in a derived class, participates in rendering operations
    ///     that are directed by the layout system. The rendering instructions for this
    ///     element are not used directly when this method is invoked, and are instead
    ///     preserved for later asynchronous use by layout and drawing.
    /// </summary>
    /// <param name="DrawingContext">
    ///     The drawing instructions for a specific element. This context is provided
    ///     to the layout system.
    /// </param>
    protected override void OnRender(DrawingContext DrawingContext)
    {
        base.OnRender(DrawingContext);

        if (!DoubleUtil.IsZero(_Radius))
            DrawingContext.DrawEllipse(null, _ForegourndPen, _Center, _Radius, _Radius);

        if (!IsEnabled)
        {
            if (_BorderGeometry != null)
                DrawingContext.DrawGeometry(_ForegroundBrush, null, _BorderGeometry);
        }
        else
        {
            if (_ProgressBorderGeometry != null)
                DrawingContext.DrawGeometry(_ForegroundBrush, null, _ProgressBorderGeometry);

            if (_ProgressGeometry != null)
                DrawingContext.DrawGeometry(_ActiveForegourndBrush, null, _ProgressGeometry);
        }
    }

    //  Details of the old and new size involved in the change. 
    /// <summary>
    ///     Raises the System.Windows.FrameworkElement.SizeChanged event, using the specified
    ///     information as part of the eventual event data.
    /// </summary>
    /// <param name="SizeInfo">
    /// </param>
    protected override void OnRenderSizeChanged(SizeChangedInfo SizeInfo)
    {
        base.OnRenderSizeChanged(SizeInfo);

        if (!DoubleUtil.IsZero(_Radius) && !SizeInfo.HeightChanged && !SizeInfo.WidthChanged)
            return;

        var new_radius = Min(ActualWidth, ActualHeight) / 2;
        var new_center = new Point(ActualWidth / 2, ActualHeight / 2);

        if (DoubleUtil.AreClose(_Radius, new_radius) && DoubleUtil.AreClose(_Center, new_center))
            return;

        _Radius = new_radius;
        _Center = new_center;
        CreateGeomerty();
    }

    #endregion Overrides

    #region Rendering

    private void OnRendering(object sender, EventArgs e)
    {
        if (!IsEnabled)
            return;

        var tick = Environment.TickCount;
        _TickCount = tick - _LastTick;
        _LastTick  = tick;

        CreateProgressPath(_TickCount);
    }

    private void CreateProgressPath(int variation)
    {
        if (variation > 10)
            _RotationAngle = _RotationAngle.EaseAngle();

        if (_CurrentGeometry != null)
        {
            var path_geometry = (PathGeometry)_CurrentGeometry.Clone();
            path_geometry.Transform = new RotateTransform(_RotationAngle, _Center.X, _Center.Y);
            _ProgressGeometry       = path_geometry.GetFlattenedPathGeometry();
        }

        if (_BorderGeometry != null)
        {
            var path_geometry = (PathGeometry)_BorderGeometry.Clone();
            path_geometry.Transform = new RotateTransform(-_RotationAngle, _Center.X, _Center.Y);
            _ProgressBorderGeometry = path_geometry.GetFlattenedPathGeometry();
        }

        InvalidateVisual();
    }

    private void CreateGeomerty()
    {
        if (DoubleUtil.IsZero(_Radius))
            return;

        // Current value geometry 
        _CurrentGeometry = _Center.CreatePath(CurrentValue.Angle(), _Radius - 14, _Radius - 20);
        _CurrentGeometry.Freeze();

        // Border geometry      
        _BorderGeometry = _Center.Create(4, 2, _Radius - 4, _Radius - 12);
        _BorderGeometry.Freeze();
    }

    private void StartListening()
    {
        VerifyAccess();

        if (_IsListening)
            return;

        _IsListening                =  true;
        CompositionTarget.Rendering += OnRendering;
    }

    private void StopListening()
    {
        VerifyAccess();

        if (!_IsListening)
            return;

        _IsListening                =  false;
        CompositionTarget.Rendering -= OnRendering;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_IsListening) StopListening();
    }

    #endregion Rendering

    #region Property Changes

    private void OnCurrentValueChanged() => CreateGeomerty();

    private void OnIsEnabledChanged(bool OldValue, bool NewValue)
    {
        if (NewValue)
        {
            CreateGeomerty();
            if (!_IsListening)
                StartListening();
        }
        else
        {
            if (_IsListening)
                StopListening();
            _RotationAngle    = 0;
            _CurrentGeometry  = null;
            _ProgressGeometry = null;
        }

        if (OldValue != NewValue)
            InvalidateVisual();
    }

    private void OnForegroundChanged(Brush NewValue)
    {
        _ForegroundBrush = NewValue.Clone();
        _ForegroundBrush.Freeze();

        _ForegourndPen = new Pen(_ForegroundBrush, 2);
        _ForegourndPen.Freeze();
    }

    private void OnActiveForegroundChanged(Brush NewValue)
    {
        _ActiveForegourndBrush = NewValue.Clone();
        _ActiveForegourndBrush.Freeze();

        _ActivePen = new Pen(_ActiveForegourndBrush, 2);
        _ActivePen.Freeze();
    }

    #endregion Property Changes
}

internal static class GeometryExtensions
{
    #region Static

    private const double __FullCircleInDegrees = 360;

    #endregion

    #region Methods

    /// <summary>    Easing in angle by delta proportionally 5 percent towards 360.</summary>
    /// <param name="angle">
    ///     The angle to start.
    /// </param>
    /// <returns>    Increased angle eased in by delta proportionally 5 percent towards 360.</returns>
    public static double EaseAngle(this double angle)
    {
        var sign = Sign(angle);

        var normalized_angle = Abs(angle).Normalize();
        var percentage       = normalized_angle / 360;

        normalized_angle = percentage.EaseInOut(normalized_angle, 5, 2);
        normalized_angle = Max(normalized_angle, 1);

        return sign < 0 ? sign * normalized_angle : normalized_angle;
    }

    /// <summary>
    ///     Increases the angle by the delta and ensure the final result is in
    ///     -360 to 360 degrees.
    /// </summary>
    /// <param name="angle">
    ///     The angle in degrees to increase.
    /// </param>
    /// <param name="delta">
    ///     The delta angle in degree to increase by.
    /// </param>
    /// <returns>
    ///     The angle increased by delta and ensure the final result is in
    ///     -360 to 360 degrees.
    /// </returns>
    public static double Angle(this double angle, double delta) => (angle.Normalize() + delta).Normalize();

    /// <summary>    Converts the percent from 0 to 100 into proportional angle from 0 to 360.</summary>
    /// <param name="percent">
    ///     The percent to convert.
    /// </param>
    /// <returns>    The converted angle from 0 to 360 proportional to 0 to 100 percent.</returns>
    public static double Angle(this double percent)
    {
        if (DoubleUtil.LessThan(percent, 0) || DoubleUtil.GreaterThan(percent, 100))
            throw new ArgumentOutOfRangeException($"Percent '{percent}' must be between 0 to 100");

        return __FullCircleInDegrees / 100 * percent;
    }

    /// <summary>    Creates a circle path for the specified location, angle in degrees, circle radius and inner radius.</summary>
    /// <param name="location">
    ///     The start location.
    /// </param>
    /// <param name="angle">
    ///     The angle in degrees.
    /// </param>
    /// <param name="radius">
    ///     The radius.
    /// </param>
    /// <param name="InnerRadius">
    ///     Inner radius.
    /// </param>
    /// <returns>    The circle path for the specified location, angle in degrees, circle radius and inner radius.</returns>
    public static PathGeometry CreatePath(this Point location, double angle, double radius, double InnerRadius)
    {
        if (DoubleUtil.LessThan(radius, 0))
            throw new ArgumentOutOfRangeException($"Radius '{radius}' must be greater than zero.");
        if (DoubleUtil.LessThan(InnerRadius, 0))
            throw new ArgumentOutOfRangeException($"Inner radius '{InnerRadius}' must be greater than zero.");

        var is_large_arc = angle > __FullCircleInDegrees / 2;

        var arc_point       = ConvertRadianToCartesian(angle, radius);
        var inner_arc_point = ConvertRadianToCartesian(angle, InnerRadius);

        var segments = new PathSegmentCollection
        {
            new LineSegment(location with { Y = location.Y - radius }, false),
            new ArcSegment(
                new Point(location.X + arc_point.X, location.Y + arc_point.Y),
                new Size(radius, radius),
                0,
                is_large_arc,
                SweepDirection.Clockwise,
                false),
            new LineSegment(new Point(location.X + inner_arc_point.X, location.Y + inner_arc_point.Y), false),
            new ArcSegment(
                location with { Y = location.Y - InnerRadius },
                new Size(InnerRadius, InnerRadius),
                0,
                is_large_arc,
                SweepDirection.Counterclockwise,
                false)
        };

        return new PathGeometry { Figures = new PathFigureCollection { new(location, segments, true) } };
    }

    /// <summary>Creates a circle path spilits into the given number of sigments.</summary>
    /// <param name="point">The start location.</param>
    /// <param name="segments">Number of sigments.</param>
    /// <param name="margin">Sigment distance between each other in degrees.</param>
    /// <param name="radius">The radius.</param>
    /// <param name="InnerRadius">The inner radius.</param>
    /// <returns>The combined path geomerty of the circle spilits into the number of segments.</returns>
    public static PathGeometry Create(
        this Point point,
        int segments,
        double margin,
        double radius,
        double InnerRadius)
    {
        if (segments <= 0) throw new ArgumentOutOfRangeException($"Segments '{segments}' must be greater than zero.");

        if (DoubleUtil.LessThan(margin, 0) || DoubleUtil.GreaterThan(margin, 360))
            throw new ArgumentOutOfRangeException($"Margin '{margin}' must be greater than zero and less than 360.");

        if (DoubleUtil.LessThan(radius, 0))
            throw new ArgumentOutOfRangeException($"Radius '{radius}' must be greater than zero.");

        if (DoubleUtil.LessThan(InnerRadius, 0))
            throw new ArgumentOutOfRangeException($"Inner radius '{InnerRadius}' must be greater than zero.");

        var angle_segment = 360d / segments - margin;
        var path_geometry = new PathGeometry();

        var angle = margin / 2;
        for (var i = 0; i < segments; i++)
        {
            var geometry = point.CreatePath(angle_segment, radius, InnerRadius);
            geometry.Transform = new RotateTransform(angle, point.X, point.Y);
            var segment_geometry = geometry.GetFlattenedPathGeometry();
            path_geometry.AddGeometry(segment_geometry);

            angle += margin + angle_segment;
        }

        return path_geometry;
    }

    /// <summary>    Gets the vector point for the specified angle in degrees and radius.</summary>
    /// <param name="angle">
    ///     The angle in degrees.
    /// </param>
    /// <param name="radius">
    ///     The radius.
    /// </param>
    /// <returns>    The vector point for the specified angle in degrees and radius.</returns>
    public static Point ConvertRadianToCartesian(this double angle, double radius)
    {
        if (DoubleUtil.LessThan(radius, 0))
            throw new ArgumentOutOfRangeException($"{nameof(radius)} '{radius}' must be greater than zero.");

        var angle_radius = PI / (__FullCircleInDegrees / 2) * (angle - __FullCircleInDegrees / 4);
        var x            = radius * Cos(angle_radius);
        var y            = radius * Sin(angle_radius);
        return new Point(x, y);
    }

    /// <summary>    Normalizes the specified angle in degrees to angles between 0 to 360;</summary>
    /// <param name="angle">
    ///     The angle to normalize.
    /// </param>
    /// <returns>    Normalized angle in degrees from 0 to 360 for the specified <paramref name="angle" /></returns>
    public static double Normalize(this double angle)
    {
        var remainder = angle % __FullCircleInDegrees;

        if (DoubleUtil.GreaterThanOrClose(remainder, __FullCircleInDegrees))
            return remainder - __FullCircleInDegrees;

        if (DoubleUtil.LessThan(remainder, 0))
            remainder += __FullCircleInDegrees;

        return remainder;
    }

    /// <summary>    Impelement the EaseIn style of exponential animation which is one of exponential growth.</summary>
    /// <param name="TimeFraction">
    ///     Time we've been running from 0 to 1.
    /// </param>
    /// <param name="start">
    ///     Start value.
    /// </param>
    /// <param name="delta">
    ///     Delta between start value and the end value we want.
    /// </param>
    /// <param name="power">
    ///     The rate of exponental growth.
    /// </param>
    /// <returns>    The result value.</returns>
    public static double EaseIn(this double TimeFraction, double start, double delta, double power)
        => Pow(TimeFraction, power) * delta + start;

    /// <summary>    Impelement the EaseOut style of exponential animation which is one of exponential decay.</summary>
    /// <param name="TimeFraction">
    ///     Time we've been running from 0 to 1.
    /// </param>
    /// <param name="start">
    ///     Start value.
    /// </param>
    /// <param name="delta">
    ///     Delta between start value and the end value we want.
    /// </param>
    /// <param name="power">
    ///     The rate of exponental decay.
    /// </param>
    /// <returns>    The result value.</returns>
    public static double EaseOut(this double TimeFraction, double start, double delta, double power)
        => Pow(TimeFraction, 1 / power) * delta + start;

    /// <summary>
    ///     Impelement the EaseInOut style of exponential animation which is one of exponential growth
    ///     for the first half of the animation and one of exponential decay for the second half.
    /// </summary>
    /// <param name="TimeFraction">
    ///     Time we've been running from 0 to 1.
    /// </param>
    /// <param name="start">
    ///     Start value.
    /// </param>
    /// <param name="delta">
    ///     Delta between start value and the end value we want.
    /// </param>
    /// <param name="power">
    ///     The rate of exponental growth/decay.
    /// </param>
    /// <returns>    The result value.</returns>
    public static double EaseInOut(this double TimeFraction, double start, double delta, double power)
        => TimeFraction <= 0.5
            ? EaseOut(TimeFraction * 2, start, delta / 2, power)
            : EaseIn((TimeFraction - 0.5) * 2, start, delta / 2, power) + delta / 2;

    #endregion
}

internal static class DoubleUtil
{
    #region Types

    [StructLayout(LayoutKind.Explicit)]
    private struct NanUnion
    {
        [FieldOffset(0)]
        internal double DoubleValue;

        [FieldOffset(0)]
        internal readonly ulong UintValue;
    }

    #endregion

    #region Static

    // Const values come from sdk\inc\crt\float.h 
    private const double __DoubleEpsilon = 2.2204460492503131e-016; /* smallest such that 1.0+DoubleEpsilon != 1.0 */

    #endregion

    #region Methods

    /// <summary>
    ///     AreClose - Returns whether or not two doubles are "close".  That is, whether or
    ///     not they are within epsilon of each other.  Note that this epsilon is proportional
    ///     to the numbers themselves to that AreClose survives scalar multiplication.
    ///     There are plenty of ways for this to return false even for numbers which
    ///     are theoretically identical, so no code calling this should fail to work if this
    ///     returns false.  This is important enough to repeat:
    ///     NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
    ///     used for optimizations *only*.
    /// </summary>
    /// <returns>    bool - the result of the AreClose comparision.</returns>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    public static bool AreClose(double value1, double value2)
    {
        //in case they are Infinities (then epsilon check does not work) 
        // ReSharper disable CompareOfFloatsByEqualityOperator 
        if (value1 == value2) return true;
        // ReSharper restore CompareOfFloatsByEqualityOperator 

        // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DoubleEpsilon  
        var eps   = (Abs(value1) + Abs(value2) + 10.0) * __DoubleEpsilon;
        var delta = value1 - value2;
        return (-eps < delta) && (eps > delta);
    }

    /// <summary>
    ///     Compares two Size instances for fuzzy equality.  This function
    ///     helps compensate for the fact that double values can
    ///     acquire error when operated upon
    /// </summary>
    /// <param name='size1'>The first size to compare</param>
    /// <param name='size2'>The second size to compare</param>
    /// <returns>Whether or not the two Size instances are equal</returns>
    public static bool AreClose(Size size1, Size size2) => AreClose(size1.Width, size2.Width) && AreClose(size1.Height, size2.Height);

    // The Point, Size, Rect and Matrix class have moved to WinCorLib.  However, we provide  
    // internal AreClose methods for our own use here. 

    /// <summary>
    ///     Compares two points for fuzzy equality.  This function
    ///     helps compensate for the fact that double values can
    ///     acquire error when operated upon
    /// </summary>
    /// <param name='point1'>The first point to compare</param>
    /// <param name='point2'>The second point to compare</param>
    /// <returns>Whether or not the two points are equal</returns>
    public static bool AreClose(Point point1, Point point2) => AreClose(point1.X, point2.X)
        && AreClose(point1.Y, point2.Y);

    /// <summary>
    ///     Compares two Vector instances for fuzzy equality.  This function
    ///     helps compensate for the fact that double values can
    ///     acquire error when operated upon
    /// </summary>
    /// <param name='vector1'>The first Vector to compare</param>
    /// <param name='vector2'>The second Vector to compare</param>
    /// <returns>Whether or not the two Vector instances are equal</returns>
    public static bool AreClose(Vector vector1, Vector vector2) => AreClose(vector1.X, vector2.X)
        && AreClose(vector1.Y, vector2.Y);

    /// <summary>
    ///     LessThan - Returns whether or not the first double is less than the second double.
    ///     That is, whether or not the first is strictly less than *and* not within epsilon of
    ///     the other number.  Note that this epsilon is proportional to the numbers themselves
    ///     to that AreClose survives scalar multiplication.  Note,
    ///     There are plenty of ways for this to return false even for numbers which
    ///     are theoretically identical, so no code calling this should fail to work if this
    ///     returns false.  This is important enough to repeat:
    ///     NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
    ///     used for optimizations *only*.
    /// </summary>
    /// <returns>    bool - the result of the LessThan comparision.</returns>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    public static bool LessThan(double value1, double value2) => (value1 < value2) && !AreClose(value1, value2);

    /// <summary>
    ///     GreaterThan - Returns whether or not the first double is greater than the second double.
    ///     That is, whether or not the first is strictly greater than *and* not within epsilon of
    ///     the other number.  Note that this epsilon is proportional to the numbers themselves
    ///     to that AreClose survives scalar multiplication.  Note,
    ///     There are plenty of ways for this to return false even for numbers which
    ///     are theoretically identical, so no code calling this should fail to work if this
    ///     returns false.  This is important enough to repeat:
    ///     NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
    ///     used for optimizations *only*.
    /// </summary>
    /// <returns>    bool - the result of the GreaterThan comparision.</returns>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    public static bool GreaterThan(double value1, double value2) => (value1 > value2) && !AreClose(value1, value2);

    /// <summary>
    ///     LessThanOrClose - Returns whether or not the first double is less than or close to
    ///     the second double.  That is, whether or not the first is strictly less than or within
    ///     epsilon of the other number.  Note that this epsilon is proportional to the numbers
    ///     themselves to that AreClose survives scalar multiplication.  Note,
    ///     There are plenty of ways for this to return false even for numbers which
    ///     are theoretically identical, so no code calling this should fail to work if this
    ///     returns false.  This is important enough to repeat:
    ///     NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
    ///     used for optimizations *only*.
    /// </summary>
    /// <returns>    bool - the result of the LessThanOrClose comparision.</returns>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    public static bool LessThanOrClose(double value1, double value2)
        => (value1 < value2) || AreClose(value1, value2);

    /// <summary>
    ///     GreaterThanOrClose - Returns whether or not the first double is greater than or close to
    ///     the second double.  That is, whether or not the first is strictly greater than or within
    ///     epsilon of the other number.  Note that this epsilon is proportional to the numbers
    ///     themselves to that AreClose survives scalar multiplication.  Note,
    ///     There are plenty of ways for this to return false even for numbers which
    ///     are theoretically identical, so no code calling this should fail to work if this
    ///     returns false.  This is important enough to repeat:
    ///     NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
    ///     used for optimizations *only*.
    /// </summary>
    /// <returns>    bool - the result of the GreaterThanOrClose comparision.</returns>
    /// <param name="value1"> The first double to compare. </param>
    /// <param name="value2"> The second double to compare. </param>
    public static bool GreaterThanOrClose(double value1, double value2)
        => (value1 > value2) || AreClose(value1, value2);

    /// <summary>
    ///     IsOne - Returns whether or not the double is "close" to 1.  Same as AreClose(double, 1),
    ///     but this is faster.
    /// </summary>
    /// <returns>    bool - the result of the AreClose comparision.</returns>
    /// <param name="value"> The double to compare to 1. </param>
    public static bool IsOne(double value) => Abs(value - 1.0) < 10.0 * __DoubleEpsilon;

    /// <summary>
    ///     IsZero - Returns whether or not the double is "close" to 0.  Same as AreClose(double, 0),
    ///     but this is faster.
    /// </summary>
    /// <returns>    bool - the result of the AreClose comparision.</returns>
    /// <param name="value"> The double to compare to 0. </param>
    public static bool IsZero(double value) => Abs(value) < 10.0 * __DoubleEpsilon;

    /// <summary>    Test to see if a double is a finite number (is not NaN or Infinity).</summary>
    /// <param name='value'>
    ///     The value to test.
    /// </param>
    /// <returns>    Whether or not the value is a finite number.</returns>
    public static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);

    /// <summary>    Test to see if a double a valid size value (is finite and > 0).</summary>
    /// <param name='value'>
    ///     The value to test.
    /// </param>
    /// <returns>    Whether or not the value is a valid size value.</returns>
    public static bool IsValidSize(double value) => IsFinite(value) && GreaterThanOrClose(value, 0);

    /// <summary>
    ///     Checks whether the double value is not a valid number or not. The standard CLR double.IsNaN()
    ///     function is approximately 100 times slower than this, so please make sure to use DoubleUtil.IsNaN()
    ///     in performance sensitive code.
    /// </summary>
    /// <param name="value">
    ///     The double value to check for.
    /// </param>
    /// <returns>    True if <paramref name="value" /> is not a number. Otherwise true.</returns>
    public static bool IsNaN(double value)
    {
        var t = new NanUnion { DoubleValue = value };

        var exp = t.UintValue & 0xfff0000000000000;
        var man = t.UintValue & 0x000fffffffffffff;

        return exp is 0x7ff0000000000000 or 0xfff0000000000000 && (man != 0);
    }

    #endregion
}