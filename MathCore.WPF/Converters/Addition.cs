using System.Windows.Markup;
using System.Windows.Data;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь сложения значения с вещественным числом</summary>
/// <param name="P">Добавочное значение, которое прибавляется к входному значению</param>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Addition))]
public class Addition(double P) : SimpleDoubleValueConverter(P, (v, p) => v + p, (r, p) => r - p)
{
    public Addition() : this(0) { }
}