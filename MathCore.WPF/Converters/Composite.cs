using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Linq;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[ContentProperty("Converters")]
[MarkupExtensionReturnType(typeof(Composite))]
/// <summary>Последовательно применяет набор вложенных конвертеров</summary>
public class Composite : ValueConverter, IAddChild
{
    private readonly List<IValueConverter> _Converters = [];

    /// <summary>Коллекция конвертеров</summary>
    public IList<IValueConverter> Converters => _Converters;

    #region IValueConverter

    /// <summary>Применяет последовательно все внутренние конвертеры</summary>
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => _Converters.Aggregate(v, (V, C) => C.Convert(V, t, p, c));

    /// <summary>Обратное преобразование применяет конвертеры в обратном порядке</summary>
    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c)
    {
        for (var i = _Converters.Count - 1; i >= 0; i--)
            v = _Converters[i].ConvertBack(v, t, p, c);
        return v;
    }

    #endregion

    /// <summary>Добавляет дочерний объект-конвертер в коллекцию</summary>
    public void AddChild(object value)
    {
        switch (value)
        {
            case null: throw new ArgumentNullException(nameof(value));
            case IValueConverter converter:
                _Converters.Add(converter);
                break;
            default: throw new ArgumentException($"Объект {value.GetType()} не реализует интерфейс {typeof(IValueConverter)}");
        }
    }

    /// <summary>Добавление текста в коллекцию не поддерживается</summary>
    public void AddText(string text) => throw new NotSupportedException();
}