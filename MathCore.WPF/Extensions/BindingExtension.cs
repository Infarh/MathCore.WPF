using System;
using System.Globalization;
using System.Windows.Data;

namespace MathCore.WPF.Extensions
{
    //[Copyright("http://habrahabr.ru/users/Makeman/", url = "http://habrahabr.ru/post/254115/")]
    public abstract class BindingExtension : Binding, IValueConverter
    {
        protected BindingExtension() => Source = Converter = this;

        protected BindingExtension(object source)
        {
            Source = source;
            Converter = this;
        }

        protected BindingExtension(RelativeSource relativeSource)
        {
            RelativeSource = relativeSource;
            Converter = this;
        }

        public abstract object Convert(object value, Type TargetType, object parameter, CultureInfo culture);

        public virtual object ConvertBack(object value, Type TargetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
