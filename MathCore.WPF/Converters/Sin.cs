using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Преобразует значение по функции синуса с масштабом и смещением</summary>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Sin))]
public class Sin : DoubleValueConverter
{
    /// <summary>Множитель выходного значения</summary>
    public double K { get; set; } = 1;

    /// <summary>Аддитивное смещение</summary>
    public double B { get; set; } = 0;

    /// <summary>Частота (коэффициент перед аргументом функции)</summary>
    public double W { get; set; } = Consts.pi2;

    /// <summary>Преобразует входное значение, возвращает NaN если вход NaN</summary>
    protected override double Convert(double v, double? p = null) => double.IsNaN(v) ? v : Math.Sin(W * v) * K + B;

    /// <inheritdoc />
    protected override double ConvertBack(double v, double? p = null) => v;
}