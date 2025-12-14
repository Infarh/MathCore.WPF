using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Проверяет, является ли число положительным</summary>
[MarkupExtensionReturnType(typeof(IsPositive))]
[ValueConversion(typeof(double), typeof(bool?))]
public class IsPositive : DoubleToBool
{
    /// <summary>Возвращает null для NaN, иначе true если значение положительно</summary>
    protected override bool? Convert(double v) => double.IsNaN(v) ? null : v > 0;
}