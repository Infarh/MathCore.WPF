using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;

/// <summary>Возвращает сборку, в которой определён указанный тип</summary>
[MarkupExtensionReturnType(typeof(GetTypeAssembly))]
[ValueConversion(typeof(Type), typeof(Assembly))]
public class GetTypeAssembly : ValueConverter
{
    /// <summary>Возвращает сборку, в которой определён указаный тип</summary>
    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) => v is Type ty ? ty.Assembly : Binding.DoNothing;
}