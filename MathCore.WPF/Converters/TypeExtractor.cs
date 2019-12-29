using System;
using System.Globalization;
using System.Windows.Data;

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(object), typeof(Type))]
    public class TypeExtractor : ValueConverter
    {
        protected override object Convert(object v, Type t, object p, CultureInfo c) => v?.GetType();
    }
}
