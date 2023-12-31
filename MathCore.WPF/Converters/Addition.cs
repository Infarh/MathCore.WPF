using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь сложения значения с вещественным числом</summary>
[MarkupExtensionReturnType(typeof(Addition))]
public class Addition(double P) : SimpleDoubleValueConverter(P, (v, p) => v + p, (r, p) => r - p)
{
    public Addition() : this(0) { }
}