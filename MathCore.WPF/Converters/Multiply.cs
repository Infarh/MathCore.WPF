using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь умножения значения на вещественное число</summary>
[MarkupExtensionReturnType(typeof(Multiply))]
public class Multiply(double K) : SimpleDoubleValueConverter(K, (v, k) => v * k, (r, k) => r / k)
{
    /// <summary>Создать преобразователь, умножающий на 1</summary>
    public Multiply() : this(1) { }
}