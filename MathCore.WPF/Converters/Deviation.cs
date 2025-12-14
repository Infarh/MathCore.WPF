using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Вычисляет разницу между текущим и предыдущим значением</summary>
[ValueConversion(typeof(double), typeof(double))]
// ReSharper disable once UnusedType.Global
[MarkupExtensionReturnType(typeof(Deviation))]
public class Deviation : DoubleValueConverter
{
    private double _LastValue = double.NaN;

    /// <summary>Возвращает разницу между текущим и предыдущим значением</summary>
    protected override double Convert(double v, double? p = null)
    {
        var dev = v - _LastValue;
        _LastValue = v;
        return dev;
    }
}