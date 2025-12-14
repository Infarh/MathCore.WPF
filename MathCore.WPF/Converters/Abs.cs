using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Возвращает абсолютное значение числа</summary>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Abs))]
public class Abs : DoubleValueConverter
{
    /// <summary>Возвращает абсолютное значение входного числа</summary>
    protected override double Convert(double v, double? p = null) => Math.Abs(v);

    /// <summary>Обратное преобразование возвращает значение как есть</summary>
    protected override double ConvertBack(double v, double? p = null) => v;
}