using System.Windows.Markup;

using MathCore.Values;
using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Converters;

/// <summary>Последовательное скользящее усреднение значений с фиксированным окном</summary>
/// <param name="Length">Размер окна</param>
[MarkupExtensionReturnType(typeof(Average))]
public class Average(int Length) : SimpleDoubleValueConverter
{
    public Average() : this(0) { }

    private readonly AverageValue _Value = new(Length);

    /// <summary>Размер окна</summary>
    public int Length
    {
        get => _Value.Length;
        set => _Value.Length = value;
    }

    /// <inheritdoc />
    protected override double To(double v, double p)
    {
        if (!double.IsNaN(v)) return _Value.AddValue(v);
        _Value.Reset();
        return v;
    }
}
