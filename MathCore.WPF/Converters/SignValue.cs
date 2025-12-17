using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Converters;

/// <summary>Возвращает дискретизированное представление знака значения с порогом</summary>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(SignValue))]
public class SignValue : DoubleValueConverter
{
    /// <summary>Масштабный коэффициент результата</summary>
    public double K { get; set; } = 1;

    /// <summary>Аддитивное смещение результата</summary>
    public double B { get; set; } = 0;

    /// <summary>Вес применяемый к входному значению перед вычислением знака</summary>
    public double W { get; set; } = 1;

    /// <summary>Пороговая дельта, при |v| &lt;= Delta возвращается 0</summary>
    [ConstructorArgument(nameof(Delta))]
    public double Delta { get; set; }

    /// <summary>Инвертировать знак результата</summary>
    [ConstructorArgument(nameof(Inverse))]
    public bool Inverse { get; set; }

    /// <summary>Инициализация SignValue по умолчанию</summary>
    public SignValue() { }

    /// <summary>Инициализация SignValue с порогом</summary>
    /// <param name="Delta">Пороговая дельта</param>
    public SignValue(double Delta) => this.Delta = Delta;

    /// <summary>Инициализация SignValue с флагом инверсии</summary>
    /// <param name="Inverse">Флаг инверсии</param>
    public SignValue(bool Inverse) => this.Inverse = Inverse;

    /// <summary>Преобразует значение, возвращая 0 внутри дельты, NaN для NaN и знак вне дельты</summary>
    /// <param name="v">Входное значение</param>
    /// <param name="p">Параметр преобразования</param>
    /// <returns>Результат преобразования</returns>
    protected override double Convert(double v, double? p = null) =>
        double.IsNaN(v)
            ? double.NaN
            : Math.Abs(v) <= Delta
                ? 0
                : Inverse
                    ? -Math.Sign(W * v) * K + B
                    : Math.Sign(W * v) * K + B;
}