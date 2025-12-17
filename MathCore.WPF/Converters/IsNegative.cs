using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Проверяет, является ли число отрицательным</summary>
[MarkupExtensionReturnType(typeof(IsNegative))]
[ValueConversion(typeof(double), typeof(bool?))]
public class IsNegative : DoubleToBool
{
    /// <summary>Возвращает null для NaN, иначе true если значение отрицательно</summary>
    protected override bool? Convert(double v) => double.IsNaN(v) ? null : v < 0;
}