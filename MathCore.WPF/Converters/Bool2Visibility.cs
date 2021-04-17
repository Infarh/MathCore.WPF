using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

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

        protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) =>
            v switch
            {
                null => Visibility.Collapsed,
                bool b => b ? !Inverted : Inverted,
                _ => v is not Visibility visibility
                    ? throw new NotSupportedException()
                    : visibility switch
                    {
                        Visibility.Visible => !Inverted,
                        Visibility.Hidden => Inverted,
                        Visibility.Collapsed => null,
                        _ => throw new NotSupportedException()
                    }
            };

        protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => Convert(v, t, p, c);
    }
}