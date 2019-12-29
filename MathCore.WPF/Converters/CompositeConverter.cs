using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [ContentProperty("Converters")]
    public class CompositeConverter : ValueConverter
    {
        private readonly IList<IValueConverter> _Converters = new List<IValueConverter>();

        /// <summary>Коллекция конвертеров</summary>
        public Collection<IValueConverter> Converters
        {
            get
            {
                Contract.Ensures(Contract.Result<Collection<IValueConverter>>() != null);
                return new Collection<IValueConverter>(_Converters);
            }
        }

        #region IValueConverter

        /// <inheritdoc />
        protected override object Convert(object v, Type t, object p, CultureInfo c) => Converters.Aggregate(v, (V, C) => C.Convert(V, t, p, c));

        /// <inheritdoc />
        protected override object ConvertBack(object v, Type t, object p, CultureInfo c) => Converters.Reverse().Aggregate(v, (V, C) => C.ConvertBack(V, t, p, c));

        #endregion

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Требуется для CodeContracts")]
        private void ObjectInvariant() => Contract.Invariant(_Converters != null);
    }
}