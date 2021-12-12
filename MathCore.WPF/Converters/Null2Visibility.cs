using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    [MarkupExtensionReturnType(typeof(Null2Visibility))]
    public class Null2Visibility : ValueConverter
    {
        public bool Inverted { get; set; }

        public bool Collapsed { get; set; }

        private Visibility Hidden => Collapsed ? Visibility.Collapsed : Visibility.Hidden;

        /// <inheritdoc />
        protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => v is null 
            ? !Inverted ? Visibility.Visible : Hidden
            : Inverted ? Hidden : Visibility.Visible;

    }
}