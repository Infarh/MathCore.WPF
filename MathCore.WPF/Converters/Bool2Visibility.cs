using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразует булево значение в Visibility</summary>
[ValueConversion(typeof(bool?), typeof(Visibility))]
[MarkupExtensionReturnType(typeof(Bool2Visibility))]
public class Bool2Visibility : ValueConverter
{
    /// <summary>Инвертировать результат преобразования</summary>
    public bool Inverted { get; set; }

    /// <summary>Использовать Collapsed вместо Hidden</summary>
    public bool Collapsed { get; set; }

    private Visibility Hidden => Collapsed ? Visibility.Collapsed : Visibility.Hidden;

    /// <summary>Преобразует булево значение в Visibility; возвращает null для null входа</summary>
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) =>
        v switch
        {
            null => null,
            Visibility => v,
            true => !Inverted ? Visibility.Visible : Hidden,
            false => Inverted ? Visibility.Visible : Hidden,
            _ => Binding.DoNothing
        };

    /// <summary>Обратное преобразование Visibility в bool</summary>
    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) =>
        v switch
        {
            null => null,
            bool b => b,
            Visibility.Visible => !Inverted,
            Visibility.Hidden => Inverted,
            Visibility.Collapsed => Inverted,
            _ => Binding.DoNothing
        };
}