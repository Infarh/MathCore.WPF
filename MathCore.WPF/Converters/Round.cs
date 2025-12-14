using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedType.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace MathCore.WPF.Converters;

/// <summary>Округляет вещественное число с заданным количеством знаков</summary>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Round))]
public class Round(int Digits, MidpointRounding Rounding) : DoubleValueConverter
{
    /// <summary>Множитель для масштабирования перед округлением</summary>
    public double K { get; set; } = 1;

    public Round() : this(0) { }

    public Round(int Digits) : this(Digits, default) { }

    [ConstructorArgument(nameof(Digits))]
    public int Digits { get; set; } = Digits;

    [ConstructorArgument(nameof(Rounding))]
    public MidpointRounding Rounding { get; set; } = Rounding;

    /// <summary>Преобразует значение, округляя его</summary>
    /// <param name="v">Входное значение</param>
    /// <param name="p">Параметр преобразования, приоритетнее входного значения</param>
    /// <returns>Округлённое значение или NaN при NaN входе</returns>
    protected override double Convert(double v, double? p = null)
    {
        if (double.IsNaN(v)) return v;
        return Digits >= 0
            ? Math.Round(v * K, Digits, Rounding) / K
            : Math.Round(v * K, Rounding) / K;
    }

    /// <summary>Обратное преобразование просто возвращает значение как есть</summary>
    protected override double ConvertBack(double v, double? p = null) => v;
}