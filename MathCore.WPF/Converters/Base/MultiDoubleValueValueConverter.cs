using System.Globalization;

namespace MathCore.WPF.Converters.Base;

public abstract class MultiDoubleValueValueConverter : MultiValueValueConverter
{
    public double? Min { get; set; }

    public double? Max { get; set; }

    /// <inheritdoc />
    protected override object Convert(object[]? vv, Type? t, object? p, CultureInfo? c)
    {
        double[] value = Array.Empty<double>();

        if(vv is { Length: > 0 })
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

    protected abstract double Convert(double[]? vv);

    protected virtual double[]? ConvertBack(double v)
    {
        base.ConvertBack(null, null, null, null);
        return null;
    }
}