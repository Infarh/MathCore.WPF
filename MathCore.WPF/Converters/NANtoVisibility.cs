using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(NaNtoVisibility))]
[ValueConversion(typeof(double), typeof(Visibility))]
public class NaNtoVisibility : ValueConverter
{
    public bool Inverted { get; set; }

    public bool Collapsed { get; set; }

    private Visibility Hidden => Collapsed ? Visibility.Collapsed : Visibility.Hidden;

    /// <inheritdoc />
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => 
        v is null 
            ? null 
            : Inverted
                ? !double.IsNaN((double)v) ? Hidden : Visibility.Visible
                : double.IsNaN((double)v) ? Hidden : Visibility.Visible;
}