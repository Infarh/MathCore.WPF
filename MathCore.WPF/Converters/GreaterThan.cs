using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;


/// <summary>Конвертер проверки, что значение больше заданного порога</summary>
[MarkupExtensionReturnType(typeof(GreaterThan))]
[ValueConversion(typeof(double), typeof(bool?))]
public class GreaterThan(double value) : DoubleToBool
{
    public GreaterThan() : this(double.NegativeInfinity) { }

    public double Value { get; set; } = value;

    protected override bool? Convert(double v) => v.IsNaN() ? null : v > Value;
}