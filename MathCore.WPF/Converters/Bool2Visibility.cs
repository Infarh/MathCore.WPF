using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(bool?), typeof(Visibility))]
    [MarkupExtensionReturnType(typeof(Bool2Visibility))]
    public class Bool2Visibility : ValueConverter
    {
        public bool Inverted { get; set; }

        protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c)
        {
            if (v is null) return Visibility.Collapsed;
            if (v is bool?)
            {
                return (bool)v ? !Inverted : Inverted;
            }
            if (!(v is Visibility)) throw new NotSupportedException();
            return (Visibility)v switch
            {
                Visibility.Visible => !Inverted,
                Visibility.Hidden => Inverted,
                Visibility.Collapsed => null,
                _ => throw new NotSupportedException()
            };
        }

        protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => Convert(v, t, p, c);
    }
}