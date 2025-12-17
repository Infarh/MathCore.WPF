using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters;

/// <summary>Позволяет задать пользовательские делегаты для Convert/ConvertBack</summary>
[MarkupExtensionReturnType(typeof(Custom))]
public class Custom : ValueConverter
{
    /// <summary>Функция прямого преобразования без параметра</summary>
    public Func<object?, object?>? Forward { get; set; }

    /// <summary>Функция прямого преобразования с параметром</summary>
    public Func<object?, object?, object?>? ForwardParam { get; set; }

    /// <summary>Функция обратного преобразования без параметра</summary>
    public Func<object?, object?>? Backward { get; set; }

    /// <summary>Функция обратного преобразования с параметром</summary>
    public Func<object?, object?, object?>? BackwardParam { get; set; }

    /// <inheritdoc />
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) =>
        Forward is null
            ? ForwardParam?.Invoke(v, p)
            : Forward(v) ?? Binding.DoNothing;

    /// <inheritdoc />
    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) =>
        Backward is null
            ? BackwardParam?.Invoke(v, p)
            : Backward(v) ?? Binding.DoNothing;
}