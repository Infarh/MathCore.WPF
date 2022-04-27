using System.Globalization;

namespace MathCore.WPF.Converters.Base;

public abstract class MultiDoubleValueValueConverter : MultiValueValueConverter
{
    /// <inheritdoc />
    protected override object Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => 
        Convert(Array.ConvertAll(vv ?? Array.Empty<object>(), x => DoubleValueConverter.ConvertToDouble(x)));

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