using MathCore.Values;
using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Converters;

public class Average(int Length) : SimpleDoubleValueConverter
{
    private readonly AverageValue _Value = new(Length);

    public int Length
    {
        get => _Value.Length;
        set => _Value.Length = value;
    }

    public Average() : this(0) { }

    /// <inheritdoc />
    protected override double To(double v, double p)
    {
        if (!double.IsNaN(v)) return _Value.AddValue(v);
        _Value.Reset();
        return v;
    }
}