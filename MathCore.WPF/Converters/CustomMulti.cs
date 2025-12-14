using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters;

/// <summary>Позволяет задать пользовательские делегаты для многозначных Convert/ConvertBack</summary>
[MarkupExtensionReturnType(typeof(CustomMulti))]
public class CustomMulti : MultiValueValueConverter
{
    /// <summary>Делегат прямого преобразования для массива входных значений</summary>
    public Func<object[]?, object?>? Forward { get; set; }

    /// <summary>Делегат прямого преобразования с параметром</summary>
    public Func<object[]?, object?, object?>? ForwardParam { get; set; }

    /// <summary>Делегат обратного преобразования без параметра</summary>
    public Func<object?, object[]?>? Backward { get; set; }

    /// <summary>Делегат обратного преобразования с параметром</summary>
    public Func<object?, object?, object[]?>? BackwardParam { get; set; }

    /// <inheritdoc />
    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) =>
        Forward is null
            ? ForwardParam?.Invoke(vv, p)
            : Forward(vv);

    /// <inheritdoc />
    protected override object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) =>
        Backward is null
            ? BackwardParam?.Invoke(v, p)
            : Backward(v);
}