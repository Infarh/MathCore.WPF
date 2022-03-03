using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;
using MathCore.Annotations;
using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;

[MarkupExtensionReturnType(typeof(GetTypeAssembly))]
[ValueConversion(typeof(Type), typeof(Assembly))]
public class GetTypeAssembly : ValueConverter
{
    /// <inheritdoc />
    protected override object Convert(object v, Type t, object p, CultureInfo c) => ((Type)v).Assembly;
}