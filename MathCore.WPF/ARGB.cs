using System;
using System.Windows.Markup;
using System.Windows.Media;
using MathCore.WPF.Extensions;
// ReSharper disable InconsistentNaming

namespace MathCore.WPF
{
    public class ARGB : MarkupExtension
    {
        public byte Alpha { get; set; }
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        public ARGB() { }

        public ARGB(byte alpha) => Alpha = alpha;

        public ARGB(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public ARGB(byte alpha, byte red, byte green, byte blue)
            : this(alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public override object ProvideValue(IServiceProvider sp)
        {
            var color = Color.FromArgb(Alpha, Red, Green, Blue);
            var destinatio_type = sp.GetDestinationTypeProvider()?.GetDestinationType() ?? typeof(object);

            if (destinatio_type.IsAssignableFrom(typeof(Color))) return color;
            if (destinatio_type.IsAssignableFrom(typeof(Brush))) return new SolidColorBrush(color);
            throw new NotSupportedException();
        }
    }
}
