using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using MathCore.Annotations;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(SingleValue))]
    public class SingleValue : MultiValueValueConverter
    {
        public int Index { get; set; }

        public IValueConverter? Next { get; set; }

        public SingleValue() { }

        public SingleValue(int Index) => this.Index = Index;

        public SingleValue(int Index, IValueConverter Next) : this(Index) => this.Next = Next;

        /// <inheritdoc />
        [CanBeNull]
        protected override object? Convert([CanBeNull] object[]? vv, Type? t, object? p, CultureInfo? c)
        {
            var v = vv is null || Index >= vv.Length ? null : vv[Index];
            return Next is null ? v : Next.Convert(v, t, p, c);
        }
    }
}