using System;
using System.Globalization;
using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters
{
    public class Lambda<TValue, TResult> : ValueConverter
    {
        public delegate TResult Converter(TValue Value, Type? TargetValueType, object? Parameter, CultureInfo? Culture);

        public delegate TValue ConverterBack(TResult Value, Type? SourceValueType, object? Parameter, CultureInfo? Culture);

        [NotNull]
        private readonly Converter _Converter;

        private readonly ConverterBack _BackConverter;

        public Lambda(
            [NotNull]Func<TValue, TResult> Converter, 
            [CanBeNull] Func<TResult, TValue>? BackConverter = null)
            : this((v, t, p, c) => Converter(v), BackConverter is null ? null : (ConverterBack)((v, t, p, c) => BackConverter(v)))
        { }

        public Lambda([NotNull]Converter Converter, [CanBeNull] ConverterBack? BackConverter = null)
        {
            _Converter = Converter;
            _BackConverter = BackConverter ?? ((v, t, p, c) => throw new NotSupportedException());
        }

        /// <inheritdoc />
        protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => 
            v is null 
                ? (object?) null 
                : _Converter((TValue)v, t, p, c);

        /// <inheritdoc />
        protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) =>
            v is null
                ? (object?) null
                : _BackConverter((TResult) v, t, p, c);
    } 
}