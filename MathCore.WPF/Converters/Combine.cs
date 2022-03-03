using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь значений, комбинирующий действие нескольких вложенных преобразователей</summary>
[MarkupExtensionReturnType(typeof(Combine))]
public class Combine : ValueConverter
{
    /// <summary>Первый применяемый вложенный преобразователь</summary>
    [ConstructorArgument("First")]
    public IValueConverter? First { get; set; }

    /// <summary>Второй применяемый вложенный преобразователь</summary>
    [ConstructorArgument("Then")]
    public IValueConverter? Then { get; set; }

    /// <summary>Массив остальных вложенных преобразователей</summary>
    [ConstructorArgument("Other")]
    public IValueConverter[]? Other { get; set; }

    /// <summary>Инициализация нового комбинированного преобразователя значений</summary>
    public Combine() { }

    /// <summary>Инициализация нового комбинированного преобразователя значений</summary>
    /// <param name="First">Первый вложенный преобразователь значения</param>
    /// <param name="Then">Второй вложенный преобразователь значения</param>
    public Combine(IValueConverter First, IValueConverter Then)
    {
        this.First = First;
        this.Then = Then;
    }

    /// <summary>Инициализация нового комбинированного преобразователя значений</summary>
    /// <param name="First">Первый вложенный преобразователь значения</param>
    /// <param name="Then">Второй вложенный преобразователь значения</param>
    /// <param name="Other">Остальные вложенные преобразователя значений</param>
    public Combine(IValueConverter First, IValueConverter Then, params IValueConverter[] Other)
    {
        this.First = First;
        this.Then = Then;
        this.Other = Other;
    }

    /// <inheritdoc />
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c)
    {
        if (First != null) v = First.Convert(v, t, p, c);
        if (Then != null) v = Then.Convert(v, t, p, c);
        var other = Other;
        if (other is not { Length: > 0 }) return v;

        for (var i = 0; i < other.Length; i++)
            if (other[i] is { } conv)
                v = conv.Convert(v, t, p, c);
        return v;
    }

    /// <inheritdoc />
    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c)
    {
        var other = Other;
        if (other != null)
            for (var i = other.Length - 1; i >= 0; i--)
                if (other[i]  is { } converter)
                    v = converter.ConvertBack(v, t, p, c);

        if (other is { Length: > 0 })
            for (var i = other.Length - 1; i >= 0; i--)
                if (other[i] is { } conv)
                    v = conv.ConvertBack(v, t, p, c);

        if (Then != null) v = Then.ConvertBack(v, t, p, c);
        if (First != null) v = First.ConvertBack(v, t, p, c);

        return v;
    }
}