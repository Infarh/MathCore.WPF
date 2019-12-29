using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    /// <summary>ѕреобразователь значений, комбинирующий действие нескольких вложенных преобразователей</summary>
    [MarkupExtensionReturnType(typeof(Combyne))]
    public class Combyne : ValueConverter
    {
        /// <summary>ѕервый примен€емый вложенный преобразователь</summary>
        public IValueConverter First { get; set; }
        /// <summary>¬торой примен€емый вложенный преобразователь</summary>
        public IValueConverter Then { get; set; }
        /// <summary>ћассив остальных вложенных преобразователей</summary>
        public IValueConverter[] Other { get; set; }

        /// <summary>»нициализаци€ нового комбинированного преобразовател€ значений</summary>
        public Combyne() { }

        /// <summary>»нициализаци€ нового комбинированного преобразовател€ значений</summary>
        /// <param name="first">ѕервый вложенный преобразователь значени€</param>
        /// <param name="then">¬торой вложенный преобразователь значени€</param>
        public Combyne(IValueConverter first, IValueConverter then)
        {
            First = first;
            Then = then;
        }

        /// <summary>»нициализаци€ нового комбинированного преобразовател€ значений</summary>
        /// <param name="first">ѕервый вложенный преобразователь значени€</param>
        /// <param name="then">¬торой вложенный преобразователь значени€</param>
        /// <param name="other">ќстальные вложенные преобразовател€ зничений</param>
        public Combyne(IValueConverter first, IValueConverter then, params IValueConverter[] other)
        {
            First = first;
            Then = then;
            Other = other;
        }

        /// <inheritdoc />
        protected override object Convert(object v, Type t, object p, CultureInfo c)
        {
            if (First != null) v = First.Convert(v, t, p, c);
            if (Then != null) v = Then.Convert(v, t, p, c);
            var other = Other;
            //if (other != null) v = other.Where(converter => converter != null).Aggregate(v, (vv, converter) => converter.Convert(vv, t, p, c));
            if (other != null && other.Length > 0)
                for (var i = 0; i < other.Length; i++)
                    if (other[i] != null)
                        v = other[i].Convert(v, t, p, c);
            return v;
        }

        /// <inheritdoc />
        protected override object ConvertBack(object v, Type t, object p, CultureInfo c)
        {
            var other = Other;
            if (other != null) v = other.GetReversed().Where(converter => converter != null).Aggregate(v, (vv, converter) => converter.ConvertBack(vv, t, p, c));
            if (other != null && other.Length > 0)
                for (var i = other.Length-1; i >= 0; i--)
                    if (other[i] != null)
                        v = other[i].ConvertBack(v, t, p, c);

            if (Then != null) v = Then.ConvertBack(v, t, p, c);
            if (First != null) v = First.ConvertBack(v, t, p, c);
            
            return v;
        }
    }
}