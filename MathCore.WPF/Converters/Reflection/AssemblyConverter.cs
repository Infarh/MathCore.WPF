using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using MathCore.Annotations;
using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters.Reflection;

[ValueConversion(typeof(Assembly), typeof(object))]
public abstract class AssemblyConverter : ValueConverter
{
    protected static Func<Assembly, object?> Attribute<T>(Func<T, object?> Converter) where T : Attribute => asm =>
    {
        var a = asm.GetCustomAttributes(typeof(T), false).OfType<T>().FirstOrDefault();
        return a is null ? null : Converter(a);
    };

    private readonly Func<Assembly, object?> _Converter;

    protected AssemblyConverter(Func<Assembly, object?> Converter) => _Converter = Converter;

    /// <inheritdoc />
    protected override object Convert(object v, Type t, object p, CultureInfo c) => _Converter((Assembly)v)!;
}