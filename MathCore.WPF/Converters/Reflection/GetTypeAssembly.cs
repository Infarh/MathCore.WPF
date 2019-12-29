using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace MathCore.WPF.Converters.Reflection
{
    [ValueConversion(typeof(Type), typeof(Assembly))]
    public class GetTypeAssembly : ValueConverter
    {
        /// <inheritdoc />
        protected override object Convert(object v, Type t, object p, CultureInfo c) => ((Type)v).Assembly;
    }
}
