using System.Windows.Markup;
using System.Windows.Media;
using System.Linq;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Интерполяция значений кубическим сплайном по заданной коллекции точек</summary>
/// <param name="points">Коллекция контрольных точек</param>
[MarkupExtensionReturnType(typeof(CSplineInterp))]
public class CSplineInterp(PointCollection points) : DoubleValueConverter
{
    /// <summary>Инициализация интерполяции кубическим сплайном по коллекции точек</summary>
    public CSplineInterp() : this([]) { }

    private MathCore.Interpolation.CubicSpline? _SplineTo;
    private MathCore.Interpolation.CubicSpline? _SplineFrom;
    private double _MinX;
    private double _MinY;
    private double _MaxX;
    private double _MaxY;

    /// <summary>Коллекция точек для интерполяции</summary>
    public PointCollection Points { get; set; } = points;

    /// <summary>Инициализация сплайнов; вызывает ошибку при пустой или отсутствующей коллекции</summary>
    public override object ProvideValue(IServiceProvider sp)
    {
        if (Points is null || Points.Count == 0) throw new ArgumentException("Points must contain at least one point", nameof(Points));

        var x = Points.Select(p => p.X).ToArray();
        var y = Points.Select(p => p.Y).ToArray();
        (_MinX, _MaxX) = x.GetMinMax();
        (_MinY, _MaxY) = y.GetMinMax();
        _SplineTo = new(x, y);
        _SplineFrom = new(y, x);

        return base.ProvideValue(sp);
    }

    private void EnsureInitialized()
    {
        if (_SplineTo is null || _SplineFrom is null)
            throw new InvalidOperationException("Spline not initialized; call ProvideValue before using the converter");
    }

    /// <summary>Вычисляет значение сплайна в заданной точке с ограничением по диапазону</summary>
    protected override double Convert(double v, double? p = null)
    {
        EnsureInitialized();
        return _SplineTo!.Value(Math.Max(Math.Min(_MaxX, v), _MinX));
    }

    /// <summary>Обратное преобразование через обратный сплайн</summary>
    protected override double ConvertBack(double v, double? p = null)
    {
        EnsureInitialized();
        return _SplineFrom!.Value(Math.Max(Math.Min(_MaxY, v), _MinY));
    }
}