using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Интерполирует значение по коллекции контрольных точек</summary>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Interpolation))]
public class Interpolation : DoubleValueConverter
{
    private Polynom? _Polynom;
    private PointCollection? _Points;

    /// <summary>Коллекция контрольных точек</summary>
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

    /// <summary>Вычисляет значение полинома или возвращает Binding.DoNothing, если полином не инициализирован</summary>
    protected override double Convert(double v, double? p = null) => _Polynom is null ? throw new InvalidOperationException("Polynom not initialized; set Points before using converter") : _Polynom.Value(v);
}