using System;
using System.Globalization;

namespace MathCore.WPF.Converters
{
    public class Custom : ValueConverter
    {
        public Func<object, object> Forvard { get; set; }
        public Func<object, object, object> ForvardParam { get; set; }
        public Func<object, object> Backvard { get; set; }
        public Func<object, object, object> BackvardParam { get; set; }

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Forvard != null ? Forvard(value) : ForvardParam?.Invoke(value, parameter);

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Backvard != null ? Backvard(value) : BackvardParam?.Invoke(value, parameter);
    }
}
