using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Преобразует значение по функции косинуса с масштабом и смещением</summary>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Cos))]
public class Cos : DoubleValueConverter
{
    /// <summary>Множитель выходного значения</summary>
    public double K { get; set; } = 1;

    /// <summary>Аддитивное смещение</summary>
    public double B { get; set; } = 0;

    /// <summary>Частота (коэффициент перед аргументом функции)</summary>
    public double W { get; set; } = Consts.pi2;

    /// <summary>Возвращает NaN для NaN входа, иначе Math.Cos(W * v) * K + B</summary>
    protected override double Convert(double v, double? p = null) => double.IsNaN(v) ? v : Math.Cos(W * v) * K + B;

    /// <inheritdoc />
    protected override double ConvertBack(double v, double? p = null) => v;
}