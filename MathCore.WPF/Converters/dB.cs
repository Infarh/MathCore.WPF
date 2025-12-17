using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразует уровень сигнала между логарифмическим дБ и линейным масштабом</summary>
[MarkupExtensionReturnType(typeof(dB))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Стиль", "IDE1006:Стили именования", Justification = "<Ожидание>")]
// ReSharper disable once InconsistentNaming
public class dB : DoubleValueConverter
{
    /// <summary>Использовать формулу для мощности (10*log10) вместо амплитудной (20*log10)</summary>
    public bool ByPower { get; set; }

    /// <summary>Инвертировать преобразование</summary>
    public bool Invert { get; set; }

    /// <summary>Преобразует значение в дБ или обратно в зависимости от флага Invert</summary>
    protected override double Convert(double v, double? p = null) => Invert
        ? (double.IsNaN(v) ? v : (ByPower ? Math.Pow(10, v / 10) : Math.Pow(10, v / 20)))
        : (double.IsNaN(v) ? v : (ByPower ? 10 * Math.Log10(v) : 20 * Math.Log10(v)));

    /// <inheritdoc />
    protected override double ConvertBack(double v, double? p = null) => Invert 
        ? (double.IsNaN(v) ? v : (ByPower ? 10 * Math.Log10(v) : 20 * Math.Log10(v)))
        : (double.IsNaN(v) ? v : (ByPower ? Math.Pow(10, v / 10) : Math.Pow(10, v / 20)));
}