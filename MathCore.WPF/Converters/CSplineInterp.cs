using System.Windows.Markup;
using System.Windows.Media;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

// ReSharper disable once IdentifierTypo
[MarkupExtensionReturnType(typeof(CSplineInterp))]
public class CSplineInterp(PointCollection points) : DoubleValueConverter
{
    public CSplineInterp() : this([]) { }

    private MathCore.Interpolation.CubicSpline? _SplineTo;
    private MathCore.Interpolation.CubicSpline? _SplineFrom;
    private double _MinX;
    private double _MinY;
    private double _MaxX;
    private double _MaxY;

    public PointCollection Points { get; set; } = points;

    public override object ProvideValue(IServiceProvider sp)
    {
        if(Points is null || Points.Count == 0) throw new FormatException();

        var x = Points.Select(p => p.X).ToArray();
        var y = Points.Select(p => p.Y).ToArray();
        (_MinX, _MaxX) = x.GetMinMax();
        (_MinY, _MaxY) = y.GetMinMax();
        _SplineTo = new(x, y);
        _SplineFrom = new(y, x);

        return base.ProvideValue(sp);
    }

    /// <inheritdoc />
    protected override double Convert(double v, double? p = null) => _SplineTo!.Value(Math.Max(Math.Min(_MaxX, v), _MinX));

    /// <inheritdoc />
    protected override double ConvertBack(double v, double? p = null) => _SplineFrom!.Value(Math.Max(Math.Min(_MaxY, v), _MinY));
}