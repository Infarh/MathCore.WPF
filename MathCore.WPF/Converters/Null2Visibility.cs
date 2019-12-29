using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    [MarkupExtensionReturnType(typeof(Null2Visibility))]
    public class Null2Visibility : ValueConverter
    {
        public bool Inverted { get; set; }

        /// <inheritdoc />
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null ? !Inverted : Inverted;
    }
}