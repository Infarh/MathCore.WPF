using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters
{
    /// <summary>��������������� ��������, ������������� �������� ���������� ��������� ����������������</summary>
    [MarkupExtensionReturnType(typeof(Combine))]
    public class Combine : ValueConverter
    {
        /// <summary>������ ����������� ��������� ���������������</summary>
        [ConstructorArgument("First")]
        public IValueConverter? First { get; set; }

        /// <summary>������ ����������� ��������� ���������������</summary>
        [ConstructorArgument("Then")]
        public IValueConverter? Then { get; set; }

        /// <summary>������ ��������� ��������� ����������������</summary>
        [ConstructorArgument("Other")]
        public IValueConverter[]? Other { get; set; }

        /// <summary>������������� ������ ���������������� ��������������� ��������</summary>
        public Combine() { }

        /// <summary>������������� ������ ���������������� ��������������� ��������</summary>
        /// <param name="First">������ ��������� ��������������� ��������</param>
        /// <param name="Then">������ ��������� ��������������� ��������</param>
        public Combine(IValueConverter First, IValueConverter Then)
        {
            this.First = First;
            this.Then = Then;
        }

        /// <summary>������������� ������ ���������������� ��������������� ��������</summary>
        /// <param name="First">������ ��������� ��������������� ��������</param>
        /// <param name="Then">������ ��������� ��������������� ��������</param>
        /// <param name="Other">��������� ��������� ��������������� ��������</param>
        public Combine(IValueConverter First, IValueConverter Then, params IValueConverter[] Other)
        {
            this.First = First;
            this.Then = Then;
            this.Other = Other;
        }

        /// <inheritdoc />
        protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c)
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
        protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c)
        {
            var other = Other;
            if (other != null)
                for (var i = other.Length - 1; i >= 0; i--)
                {
                    var converter = other[i];
                    if (converter != null) 
                        v = converter.ConvertBack(v, t, p, c);
                }

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