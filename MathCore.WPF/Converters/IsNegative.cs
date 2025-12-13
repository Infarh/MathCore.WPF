using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;


/// <summary>Конвертер проверки, что значение отрицательное</summary>
[MarkupExtensionReturnType(typeof(IsNegative))]
[ValueConversion(typeof(double), typeof(bool?))]
public class IsNegative : DoubleToBool
{
    /// <inheritdoc />
    protected override bool? Convert(double v) => v.IsNaN() ? null : v < 0;
}