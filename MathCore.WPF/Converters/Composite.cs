using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using MathCore.Annotations;
using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[ContentProperty("Converters")]
public class Composite : ValueConverter, IAddChild
{
    private readonly IList<IValueConverter> _Converters = new List<IValueConverter>();

    /// <summary>Коллекция конвертеров</summary>
    public IList<IValueConverter> Converters => _Converters;

    #region IValueConverter

    /// <inheritdoc />
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => _Converters.Aggregate(v, (V, C) => C.Convert(V, t, p, c));

    /// <inheritdoc />
    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => _Converters.Reverse().Aggregate(v, (V, C) => C.ConvertBack(V, t, p, c));

    #endregion

    public void AddChild(object value)
    {
        switch (value)
        {
            default: throw new ArgumentException($"Объект {value.GetType()} не реализует интерфейс {typeof(IValueConverter)}");
            case null: throw new ArgumentNullException(nameof(value));
            case IValueConverter converter:
                _Converters.Add(converter);
                break;
        }
    }

    public void AddText(string text) => throw new NotSupportedException();
}