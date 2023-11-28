using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь деления значения на вещественное число</summary>
[MarkupExtensionReturnType(typeof(Divide))]
// ReSharper disable once UnusedType.Global
public class Divide(double K) : SimpleDoubleValueConverter(K, (v, k) => v / k, (r, k) => r * k)
{
    public Divide() : this(1) { }
}