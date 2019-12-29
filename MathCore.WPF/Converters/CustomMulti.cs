using System;
using System.Globalization;

namespace MathCore.WPF.Converters
{
    public class CustomMulti : MultiValueValueConverter
    {
        public Func<object[], object> Forvard { get; set; }
        public Func<object[], object, object> ForvardParam { get; set; }
        public Func<object, object[]> Backvard { get; set; }
        public Func<object, object, object[]> BackvardParam { get; set; }

        protected override object Convert(object[] vv, Type t, object p, CultureInfo c) => Forvard != null ? Forvard(vv) : ForvardParam?.Invoke(vv, p);

        protected override object[] ConvertBack(object v, Type[] tt, object p, CultureInfo c) => Backvard != null ? Backvard(v) : BackvardParam?.Invoke(v, p);
    }
}