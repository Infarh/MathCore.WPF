using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь определения знака числа по формуле: result = sign(W * value) * K + B</summary>
/// <remarks>
/// Возвращает -1 для отрицательных чисел, 0 для нуля и 1 для положительных чисел.
/// Обратное преобразование не поддерживается, так как sign(x) = a теряет информацию о величине
/// </remarks>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Sign))]
public class Sign : DoubleValueConverter
{
    /// <summary>Коэффициент масштабирования</summary>
    public double K { get; set; } = 1;

    /// <summary>Смещение результата</summary>
    public double B { get; set; } = 0;

    /// <summary>Множитель входного значения</summary>
    public double W { get; set; } = 1;

    /// <inheritdoc />
    protected override double Convert(double v, double? p = null) => double.IsNaN(v) ? v : Math.Sign(W * v) * K + B;

    /// <inheritdoc />
    protected override double ConvertBack(double v, double? p = null) => throw new NotSupportedException("Обратное преобразование функции sign не поддерживается");
}