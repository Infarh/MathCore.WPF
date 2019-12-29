using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using MathCore.Annotations;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(CombyneMulty))]
    public class CombyneMulty : MultiValueValueConverter
    {
        [NotNull]
        public IMultiValueConverter First { get; set; }

        [CanBeNull]
        public IValueConverter Then { get; set; }

        protected override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = First.Convert(values, targetType, parameter, culture);
            if(Then != null) result = Then.Convert(result, targetType, parameter, culture);
            return result;
        }

        protected override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if(Then != null) value = Then.ConvertBack(value, value != null ? value.GetType() : typeof(object), parameter, culture);
            return First.ConvertBack(value, targetTypes, parameter, culture);
        }
    }
}