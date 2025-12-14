using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Масштабирует значение из одного диапазона в другой</summary>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Mapper))]
public class Mapper() : DoubleValueConverter
{
    private double _k = 1;

    private double _MinScale;

    /// <summary>Минимальное значение масштаба</summary>
    public double MinScale
    {
        get => _MinScale;
        set
        {
            _MinScale = value;
            RecalculateK();
        }
    }

    private double _MaxScale = 1;

    /// <summary>Максимальное значение масштаба</summary>
    public double MaxScale
    {
        get => _MaxScale;
        set
        {
            _MaxScale = value;
            RecalculateK();
        }
    }

    private double _MinValue;

    /// <summary>Минимальное значение исходного диапазона</summary>
    public double MinValue
    {
        get => _MinValue;
        set
        {
            _MinValue = value;
            RecalculateK();
        }
    }

    private double _MaxValue = 1;

    /// <summary>Максимальное значение исходного диапазона</summary>
    public double MaxValue
    {
        get => _MaxValue;
        set
        {
            _MaxValue = value;
            RecalculateK();
        }
    }

    private void RecalculateK()
    {
        var denom = (_MaxValue - _MinValue);
        _k = denom == 0 ? 0 : (_MaxScale - _MinScale) / denom;
    }

    /// <inheritdoc />
    protected override double Convert(double v, double? p = null)
    {
        var x = (p ?? v);
        var result = (x - _MinValue) * _k + _MinScale;
        return result;
    }

    /// <inheritdoc />
    protected override double ConvertBack(double x, double? p = null) => _k == 0 ? double.NaN : (x - _MinScale) / _k + _MinValue;
}