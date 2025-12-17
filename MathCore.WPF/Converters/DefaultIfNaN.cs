using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Возвращает значение по умолчанию, если входное значение является NaN</summary>
[MarkupExtensionReturnType(typeof(DefaultIfNaN))]
public class DefaultIfNaN(double DefaultValue) : ValueConverter
{
    public DefaultIfNaN() : this(default) { }

    [ConstructorArgument(nameof(DefaultValue))]
    public double DefaultValue { get; set; } = DefaultValue;

    /// <summary>
    ///     Возвращает DefaultValue если входное значение <c>NaN</c>
    /// </summary>
    /// <param name="v">Входное значение</param>
    /// <param name="t">Тип целевого свойства</param>
    /// <param name="p">Параметры привязки</param>
    /// <param name="c">Информация о культуре</param>
    /// <returns>
    ///     <see cref="double.IsNaN(double)" /> ? <see cref="DefaultValue" /> : <paramref name="v" />
    /// </returns>
    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) => v switch
    {
        double d => double.IsNaN(d) ? DefaultValue : d,
        _ => Binding.DoNothing
    };
}