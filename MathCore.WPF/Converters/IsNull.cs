using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Проверяет, является ли значение null</summary>
[MarkupExtensionReturnType(typeof(IsNull))]
public class IsNull(bool Inverted) : ValueConverter
{
    public IsNull() : this(false) { }

    [ConstructorArgument(nameof(Inverted))]
    public bool Inverted { get; set; } = Inverted;

    /// <summary>Возвращает true если значение null (с учётом Inverted)</summary>
    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) => Inverted ^ (v is null);
}