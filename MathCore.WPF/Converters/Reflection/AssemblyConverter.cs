using System.Globalization;
using System.Reflection;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters.Reflection;

/// <summary>Базовый конвертер для извлечения значения из атрибутов сборки</summary>
public abstract class AssemblyConverter(Func<Assembly, object?> Converter) : ValueConverter
{
    /// <summary>Возвращает функцию получения значения из атрибута сборки</summary>
    protected static Func<Assembly, object?> Attribute<T>(Func<T, object?> Converter) where T : Attribute => asm =>
    {
        var a = asm.GetCustomAttributes(typeof(T), false).OfType<T>().FirstOrDefault();
        return a is null ? null : Converter(a);
    };

    /// <summary>Преобразует объект Assembly с помощью переданного Converter; возвращает Binding.DoNothing для неподходящих входов</summary>
    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) => v is Assembly asm ? Converter(asm) : Binding.DoNothing;
}