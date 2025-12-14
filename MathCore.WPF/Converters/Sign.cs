using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Возвращает знак числа с коэффициентами масштабирования и смещения</summary>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Sign))]
public class Sign : DoubleValueConverter
{
    /// <summary>Масштабный коэффициент для результата</summary>
    public double K { get; set; } = 1;

    /// <summary>Аддитивное смещение результата</summary>
    public double B { get; set; } = 0;

    /// <summary>Вес на входе перед вычислением знака</summary>
    public double W { get; set; } = 1;

    /// <summary>Возвращает NaN для NaN входа, иначе знак входного значения, масштабированный и со смещением</summary>
    protected override double Convert(double v, double? p = null) => double.IsNaN(v) ? v : Math.Sign(W * v) * K + B;

    /// <inheritdoc />
    protected override double ConvertBack(double v, double? p = null) => v;
}