using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Вычисляет остаток от деления значения на заданный модуль</summary>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Mod))]
public class Mod(double M) : DoubleValueConverter
{
    public Mod() : this(double.NaN) { }

    public double M { get; set; } = M;

    /// <summary>Вычисляет остаток от деления значения на M или возвращает NaN при некорректных входных данных</summary>
    protected override double Convert(double v, double? p = null)
    {
        var value = p ?? v;
        if (double.IsNaN(value)) return double.NaN;
        if (double.IsNaN(M)) return value;
        if (M == 0) return double.NaN;
        return value % M;
    }
}