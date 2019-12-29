using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using MathCore.Data;

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(DataLength))]
    [MarkupExtensionReturnType(typeof(DataLengthString))]
    // ReSharper disable once UnusedMember.Global
    public sealed class DataLengthString : ValueConverter
    {
        /// <inheritdoc />
        protected override object Convert(object v, Type t, object p, CultureInfo c) => new DataLength(System.Convert.ToDouble(v), 1024d);
    }
}