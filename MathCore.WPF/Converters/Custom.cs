using System;
using System.Globalization;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters;

public class Custom : ValueConverter
{
    public Func<object?, object?>? Forward { get; set; }

    public Func<object?, object?, object?>? ForwardParam { get; set; }

    public Func<object?, object?>? Backward { get; set; }

    public Func<object?, object?, object?>? BackwardParam { get; set; }

    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => 
        Forward is null
            ? ForwardParam?.Invoke(v, p) 
            : Forward(v);

    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => 
        Backward is null
            ? BackwardParam?.Invoke(v, p) 
            : Backward(v);
}