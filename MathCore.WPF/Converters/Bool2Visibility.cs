using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(bool?), typeof(Visibility))]
    [MarkupExtensionReturnType(typeof(Bool2Visibility))]
    public class Bool2Visibility : ValueConverter
    {
        public bool Inversed { get; set; }

        protected override object Convert(object v, Type t, object p, CultureInfo c)
        {
            if(v is bool?)
            {
                if(!(v as bool?).HasValue) return Visibility.Collapsed;
                return (bool)v ? !Inversed : Inversed;
            }
            if (!(v is Visibility)) throw new NotSupportedException();
            switch((Visibility)v)
            {
                case Visibility.Visible:
                    return !Inversed;
                case Visibility.Hidden:
                    return Inversed;
                case Visibility.Collapsed:
                    return null;
            }
            throw new NotSupportedException();
        }

        protected override object ConvertBack(object v, Type t, object p, CultureInfo c) => Convert(v, t, p, c);
    }
}