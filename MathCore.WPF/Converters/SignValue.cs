﻿using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(SignValue))]
public class SignValue : DoubleValueConverter
{
    public double K { get; set; } = 1;

    public double B { get; set; } = 0;

    public double W { get; set; } = 1;

    [ConstructorArgument(nameof(Delta))]
    public double Delta { get; set; }

    [ConstructorArgument(nameof(Inverse))]
    public bool Inverse { get; set; }

    public SignValue() { }

    public SignValue(double Delta) => this.Delta = Delta;

    public SignValue(bool Inverse) => this.Inverse = Inverse;

    protected override double Convert(double v, double? p = null) => 
        double.IsNaN(v) 
            ? double.NaN 
            : Math.Abs(v) <= Delta 
                ? 0 
                : Inverse 
                    ? -Math.Sign(W * v) * K + B
                    : Math.Sign(W * v) * K + B;
}