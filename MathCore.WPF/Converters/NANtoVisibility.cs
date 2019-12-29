using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(NANtoVisibility))]
    [ValueConversion(typeof(double), typeof(Visibility))]
    public class NANtoVisibility : ValueConverter
    {
        public bool Colapse { get; set; }

        public bool Inverted { get; set; }

        /// <inheritdoc />
        protected override object Convert(object v, Type t, object p, CultureInfo c) => Inverted
            ? (!double.IsNaN((double) v)
                ? (Colapse ? Visibility.Collapsed : Visibility.Hidden)
                : Visibility.Visible)
            : (double.IsNaN((double) v)
                ? (Colapse ? Visibility.Collapsed : Visibility.Hidden)
                : Visibility.Visible);
    }
}