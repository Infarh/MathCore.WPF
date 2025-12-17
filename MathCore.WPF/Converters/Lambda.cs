using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Lambda конвертер с типизированными делегатами</summary>
[MarkupExtensionReturnType(typeof(Lambda<,>))]
public class Lambda<TValue, TResult>(Lambda<TValue, TResult>.Converter Converter, Lambda<TValue, TResult>.ConverterBack? BackConverter = null) : ValueConverter
{
    /// <summary>Именованный делегат преобразования</summary>
    public delegate TResult Converter(TValue Value, Type? TargetValueType, object? Parameter, CultureInfo? Culture);

    /// <summary>Именованный делегат обратного преобразования</summary>
    public delegate TValue ConverterBack(TResult Value, Type? SourceValueType, object? Parameter, CultureInfo? Culture);

    public Lambda(
       Func<TValue, TResult> Converter,
       Func<TResult, TValue>? BackConverter = null)
       : this((v, _, _, _) => Converter(v), BackConverter is null ? null : ((v, _, _, _) => BackConverter(v)))
    { }

    private readonly Converter _Converter = Converter;

    private readonly ConverterBack _BackConverter = BackConverter ?? ((_, _, _, _) => throw new NotSupportedException());

   /// <inheritdoc />
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) =>
        v is null
            ? null
            : v is TValue value
                ? _Converter(value, t, p, c)
                : Binding.DoNothing;

    /// <inheritdoc />
    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) =>
        v is null
            ? null
            : v is TResult result
                ? _BackConverter(result, t, p, c)
                : Binding.DoNothing;
}