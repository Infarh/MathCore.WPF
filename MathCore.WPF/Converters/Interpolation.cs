using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    [MarkupExtensionReturnType(typeof(Interpolation))]
    public class Interpolation : ValueConverter
    {
        private Polynom _Polynom;
        private Point[] _Points;

        public Point[] Points
        {
            get => _Points;
            set
            {
                if(ReferenceEquals(_Points, value)) return;
                _Polynom = new MathCore.Interpolation.Lagrange(value.Select(p => p.X).ToArray(), value.Select(p => p.Y).ToArray()).Polynom;
                _Points = value;
            }
        }

        protected override object Convert(object v, Type t, object p, CultureInfo c) => _Polynom.Value((double)v);

        protected override object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotSupportedException();
    }
}
