using System.Globalization;

namespace MathCore.WPF.Converters.Base;

public abstract class MultiDoubleValueValueConverter : MultiValueValueConverter
{
    public double? Min { get; set; }

    public double? Max { get; set; }

    /// <inheritdoc />
    protected override object Convert(object[]? vv, Type? t, object? p, CultureInfo? c)
    {
        var result = Convert(Array.ConvertAll(vv ?? [], DoubleValueConverter.ConvertToDouble));

        if (Min is { } min && !double.IsNaN(min))
            result = Math.Max(result, min);
        if (Max is { } max && !double.IsNaN(max))
            result = Math.Min(result, max);

        return result;
    }

    /// <inheritdoc />
    protected override object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) =>
        ConvertBack(DoubleValueConverter.ConvertToDouble(v))?.Cast<object>().ToArray();

    protected abstract double Convert(double[]? vv);

    protected virtual double[]? ConvertBack(double v)
    {
        base.ConvertBack(null, null, null, null);
        return null;
    }
}