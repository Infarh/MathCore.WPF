using System.Globalization;
using System.Reflection;
using System.Windows.Data;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters.Reflection;

[ValueConversion(typeof(Assembly), typeof(object))]
public abstract class AssemblyConverter(Func<Assembly, object?> Converter) : ValueConverter
{
    protected static Func<Assembly, object?> Attribute<T>(Func<T, object?> Converter) where T : Attribute => asm =>
    {
        var a = asm.GetCustomAttributes(typeof(T), false).OfType<T>().FirstOrDefault();
        return a is null ? null : Converter(a);
    };

    /// <inheritdoc />
    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) => v is null ? null : Converter((Assembly)v);
}