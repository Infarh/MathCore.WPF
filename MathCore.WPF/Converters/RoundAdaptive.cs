using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Адаптивное округление числа, выбирающее количество знаков по значению</summary>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(RoundAdaptive))]
public class RoundAdaptive(int Digits, MidpointRounding Rounding) : DoubleValueConverter
{
    public RoundAdaptive() : this(0) { }

    public RoundAdaptive(int Digits) : this(Digits, default) { }


    [ConstructorArgument(nameof(Digits))]
    public int Digits { get; set; } = Digits;

    [ConstructorArgument(nameof(Rounding))]
    public MidpointRounding Rounding { get; set; } = Rounding;

    /// <summary>Преобразует значение с адаптивным округлением</summary>
    protected override double Convert(double v, double? p = null)
    {
        if (double.IsNaN(v)) return v;
        try
        {
            return v.RoundAdaptive(Digits);
        }
        catch
        {
            return Math.Round(v, Digits, Rounding);
        }
    }

    /// <summary>Обратное преобразование не изменяет значение</summary>
    protected override double ConvertBack(double v, double? p = null) => v;
}