using System;
using System.Globalization;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters
{
    public class CustomMulti : MultiValueValueConverter
    {
        public Func<object[]?, object?>? Forward { get; set; }

        public Func<object[]?, object?, object?>? ForwardParam { get; set; }

        public Func<object?, object[]?>? Backward { get; set; }

        public Func<object?, object?, object[]?>? BackwardParam { get; set; }

        [CanBeNull]
        protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => 
            Forward is null 
                ? ForwardParam?.Invoke(vv, p) 
                : Forward(vv);

        [CanBeNull]
        protected override object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) => 
            Backward is null 
                ? BackwardParam?.Invoke(v, p) 
                : Backward(v);
    }
}