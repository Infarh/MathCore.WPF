using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Converters;

/// <summary>Вычисляет среднее значение последовательности чисел</summary>
[MarkupExtensionReturnType(typeof(AverageMulti))]
public class AverageMulti() : MultiValueValueConverter
{
    /// <summary>Вычисляет среднее значение последовательности чисел</summary>
    /// <returns>Среднее арифметическое элементов массива; Binding.DoNothing для некорректных входных данных</returns>
    protected override object? Convert(object?[]? vv, Type? t, object? p, CultureInfo? c)
    {
        switch (vv)
        {
            case null:
                return null;
            case [null]:
                return double.NaN;
        }

        if (!DoubleValueConverter.TryConvertToDouble(vv[0], c, out var v))
            return double.NaN;

        for (var i = 1; i < vv.Length; i++)
        {
            if (vv[i] is null) return double.NaN;

            if (!DoubleValueConverter.TryConvertToDouble(vv[i], c, out var value))
                return double.NaN;

            v += value;
        }

        return v / vv.Length;
    }
}