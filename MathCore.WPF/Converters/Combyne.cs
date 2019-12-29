using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    /// <summary>��������������� ��������, ������������� �������� ���������� ��������� ����������������</summary>
    [MarkupExtensionReturnType(typeof(Combyne))]
    public class Combyne : ValueConverter
    {
        /// <summary>������ ����������� ��������� ���������������</summary>
        public IValueConverter First { get; set; }
        /// <summary>������ ����������� ��������� ���������������</summary>
        public IValueConverter Then { get; set; }
        /// <summary>������ ��������� ��������� ����������������</summary>
        public IValueConverter[] Other { get; set; }

        /// <summary>������������� ������ ���������������� ��������������� ��������</summary>
        public Combyne() { }

        /// <summary>������������� ������ ���������������� ��������������� ��������</summary>
        /// <param name="first">������ ��������� ��������������� ��������</param>
        /// <param name="then">������ ��������� ��������������� ��������</param>
        public Combyne(IValueConverter first, IValueConverter then)
        {
            First = first;
            Then = then;
        }

        /// <summary>������������� ������ ���������������� ��������������� ��������</summary>
        /// <param name="first">������ ��������� ��������������� ��������</param>
        /// <param name="then">������ ��������� ��������������� ��������</param>
        /// <param name="other">��������� ��������� ��������������� ��������</param>
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