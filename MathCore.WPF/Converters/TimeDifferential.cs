using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер дифференцирования значения по времени</summary>
[MarkupExtensionReturnType(typeof(TimeDifferential))]
public class TimeDifferential(double K, bool IgnoreNaN = false) : SimpleDoubleValueConverter(K)
{
    public TimeDifferential() : this(1) { }

    private DateTime _LastTime = DateTime.Now;

    private double _LastValue = double.NaN;

    [ConstructorArgument(nameof(IgnoreNaN))]
    public bool IgnoreNaN { get; set; } = IgnoreNaN;

    /// <inheritdoc />
    protected override double To(double v, double p)
    {
        if (double.IsNaN(v)) return IgnoreNaN ? v : _LastValue = v;

        var now = DateTime.Now;
        var dt = (now - _LastTime).TotalSeconds;
        _LastTime = now;
        if (double.IsNaN(_LastValue))
        {
            if (!IgnoreNaN) _LastValue = v;
            return double.NaN;
        }

        var dv = v - _LastValue;
        _LastValue = v;
        return dv / dt * Parameter;
    }
}