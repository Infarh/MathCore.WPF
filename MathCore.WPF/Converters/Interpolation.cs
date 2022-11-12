using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Interpolation))]
public class Interpolation : DoubleValueConverter
{
    private Polynom? _Polynom;
    private PointCollection? _Points;

    public PointCollection? Points
    {
        get => _Points;
        set
        {
            if (ReferenceEquals(_Points, value)) return;
            _Points = value;
            if (value is null)
            {
                _Polynom = null;
                return;
            }
            _Polynom = new MathCore.Interpolation.Lagrange(value.Select(p => p.X).ToArray(), value.Select(p => p.Y).ToArray()).Polynom;
        }
    }

    protected override double Convert(double v, double? p = null) => _Polynom!.Value(v);
}