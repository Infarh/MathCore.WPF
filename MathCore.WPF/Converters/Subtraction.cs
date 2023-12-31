using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь вычитания вещественного числа из значения</summary>
[MarkupExtensionReturnType(typeof(Subtraction))]
public class Subtraction(double P) : SimpleDoubleValueConverter(P, (v, p) => v - p, (r, p) => r + p)
{
    public Subtraction() : this(0) { }
}