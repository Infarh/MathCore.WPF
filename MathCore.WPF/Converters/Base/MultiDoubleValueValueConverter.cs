using System.Globalization;

namespace MathCore.WPF.Converters.Base;

/// <summary>Базовый конвертер для операций над массивом double с ограничением Min/Max</summary>
public abstract class MultiDoubleValueValueConverter : MultiValueValueConverter
{
    /// <summary>Минимальное ограничение результата</summary>
    public double? Min { get; set; }

    /// <summary>Максимальное ограничение результата</summary>
    public double? Max { get; set; }

    /// <inheritdoc />
    protected override object Convert(object[]? vv, Type? t, object? p, CultureInfo? c)
    {
        var value = Array.Empty<double>();

        if (vv is { Length: > 0 })
        {
            value = new double[vv.Length];
            for (var i = 0; i < vv.Length; i++)
                value[i] = DoubleValueConverter.ConvertToDouble(vv[i], c);
        }

        var result = Convert(value);

        if (Min is { } min && !double.IsNaN(min))
            result = Math.Max(result, min);
        if (Max is { } max && !double.IsNaN(max))
            result = Math.Min(result, max);

        return result;
    }

    /// <inheritdoc />
    protected override object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) => v is null
        ? null
        : ConvertBack(DoubleValueConverter.ConvertToDouble(v, c))?.Cast<object>().ToArray();

    /// <summary>Выполняет вычисление результата по массиву double</summary>
    /// <param name="vv">Массив входных значений или null</param>
    /// <returns>Числовой результат вычисления</returns>
    protected abstract double Convert(double[]? vv);

    /// <summary>Обратное преобразование значения в массив double, по умолчанию не реализовано</summary>
    protected virtual double[]? ConvertBack(double v)
    {
        return null;
    }
}