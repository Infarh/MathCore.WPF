using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double[]), typeof(Point))]
    [MarkupExtensionReturnType(typeof(Values2Point))]
    public class Values2Point : MultiValueValueConverter
    {
        protected override object Convert(object[] vv, Type t, object p, CultureInfo c) => new Point((double)vv[0], (double)vv[1]);

        protected override object[] ConvertBack(object v, Type[] tt, object p, CultureInfo c) => new object[] { ((Point)v).X, ((Point)v).Y };
    }
}