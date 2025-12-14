using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Возвращает System.Type переданного объекта</summary>
[ValueConversion(typeof(object), typeof(Type))]
[MarkupExtensionReturnType(typeof(GetType))]
public class GetType : ValueConverter
{
    /// <summary>Возвращает System.Type для переданного объекта или Binding.DoNothing для null</summary>
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => v is null ? Binding.DoNothing : v.GetType();
}