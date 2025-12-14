using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Преобразует число в обратное значение (1/x) или параметр/x</summary>
[MarkupExtensionReturnType(typeof(Inverse))]
public class Inverse : SimpleDoubleValueConverter
{
    /// <summary>Возвращает обратное значение: параметр / v или 1 / v</summary>
    protected override double Convert(double v, double? p = null)
    {
        if (v == 0) return double.NaN;
        return p is double k ? k / v : 1 / v;
    }
}