using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(SingleValue))]
    public class SingleValue : MultiValueValueConverter
    {
        public int Index { get; set; }

        public IValueConverter Next { get; set; }

        public SingleValue() { }
        public SingleValue(int Index) { this.Index = Index; }
        public SingleValue(int Index, IValueConverter Next) : this(Index) { this.Next = Next; }


        /// <inheritdoc />
        protected override object Convert(object[] vv, Type t, object p, CultureInfo c)
        {
            var v = vv == null || Index >= vv.Length ? null : vv[Index];
            return Next == null ? v : Next.Convert(v, t, p, c);
        }
    }
}