using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Масштабирует значение из одного диапазона в другой с помощью линейной интерполяции</summary>
/// <remarks>
/// Конвертер выполняет линейное преобразование значения из исходного диапазона [MinValue, MaxValue] 
/// в целевой диапазон [MinScale, MaxScale]. Используется в WPF для привязки данных с автоматическим 
/// масштабированием значений.
/// </remarks>
/// <example>
/// Пример использования в XAML для масштабирования значения температуры от 0-100°C в диапазон 0-360° для поворота стрелки:
/// <code language="csharp"><![CDATA[
/// <conv:Mapper x:Key="TempToAngleMapper" 
///              MinValue="0" MaxValue="100" 
///              MinScale="0" MaxScale="360" />
/// 
/// <RotateTransform Angle="{Binding Temperature, Converter={StaticResource TempToAngleMapper}}" />
/// ]]></code>
/// Пример использования в C#:
/// <code language="csharp"><![CDATA[
/// var mapper = new Mapper 
/// { 
///     MinValue = 0, 
///     MaxValue = 100, 
///     MinScale = 0, 
///     MaxScale = 360 
/// };
/// 
/// double angle = (double)mapper.Convert(50, typeof(double), null, null); // Результат: 180
/// ]]></code>
/// </example>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Mapper))]
public class Mapper() : DoubleValueConverter
{
    private double _k = 1;

    private double _MinScale;

    /// <summary>Минимальное значение масштаба</summary>
    /// <value>Минимальное значение целевого диапазона. По умолчанию 0</value>
    /// <remarks>Устанавливает нижнюю границу выходного диапазона. При изменении автоматически пересчитывается коэффициент масштабирования</remarks>
    public double MinScale
    {
        get => _MinScale;
        set
        {
            _MinScale = value;
            RecalculateK();
        }
    }

    private double _MaxScale = 1;

    /// <summary>Максимальное значение масштаба</summary>
    /// <value>Максимальное значение целевого диапазона. По умолчанию 1</value>
    /// <remarks>Устанавливает верхнюю границу выходного диапазона. При изменении автоматически пересчитывается коэффициент масштабирования</remarks>
    public double MaxScale
    {
        get => _MaxScale;
        set
        {
            _MaxScale = value;
            RecalculateK();
        }
    }

    private double _MinValue;

    /// <summary>Минимальное значение исходного диапазона</summary>
    /// <value>Минимальное значение входного диапазона. По умолчанию 0</value>
    /// <remarks>Устанавливает нижнюю границу входного диапазона. При изменении автоматически пересчитывается коэффициент масштабирования</remarks>
    public double MinValue
    {
        get => _MinValue;
        set
        {
            _MinValue = value;
            RecalculateK();
        }
    }

    private double _MaxValue = 1;

    /// <summary>Максимальное значение исходного диапазона</summary>
    /// <value>Максимальное значение входного диапазона. По умолчанию 1</value>
    /// <remarks>Устанавливает верхнюю границу входного диапазона. При изменении автоматически пересчитывается коэффициент масштабирования</remarks>
    public double MaxValue
    {
        get => _MaxValue;
        set
        {
            _MaxValue = value;
            RecalculateK();
        }
    }

    private void RecalculateK()
    {
        var denom = _MaxValue - _MinValue;
        _k = denom == 0 ? 0 : (_MaxScale - _MinScale) / denom;
    }

    /// <summary>Преобразует значение из исходного диапазона в целевой диапазон</summary>
    /// <param name="v">Значение для преобразования</param>
    /// <param name="p">Дополнительный параметр, если указан, используется вместо v. По умолчанию null</param>
    /// <returns>Значение, масштабированное в целевой диапазон [MinScale, MaxScale]</returns>
    /// <remarks>
    /// Использует формулу линейной интерполяции: result = (x - MinValue) * k + MinScale, 
    /// где k = (MaxScale - MinScale) / (MaxValue - MinValue)
    /// </remarks>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// var mapper = new Mapper { MinValue = 0, MaxValue = 100, MinScale = -10, MaxScale = 10 };
    /// double result = mapper.Convert(50, null); // Результат: 0 (середина диапазона)
    /// ]]></code>
    /// </example>
    protected override double Convert(double v, double? p = null)
    {
        var x = p ?? v;
        var result = (x - _MinValue) * _k + _MinScale;
        return result;
    }

    /// <summary>Выполняет обратное преобразование значения из целевого диапазона в исходный диапазон</summary>
    /// <param name="x">Значение из целевого диапазона для обратного преобразования</param>
    /// <param name="p">Дополнительный параметр (не используется). По умолчанию null</param>
    /// <returns>Значение, преобразованное обратно в исходный диапазон [MinValue, MaxValue], или double.NaN если коэффициент масштабирования равен нулю</returns>
    /// <remarks>
    /// Использует обратную формулу: result = (x - MinScale) / k + MinValue. 
    /// Если коэффициент k равен нулю (входной диапазон имеет нулевую длину), возвращается double.NaN
    /// </remarks>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// var mapper = new Mapper { MinValue = 0, MaxValue = 100, MinScale = 0, MaxScale = 360 };
    /// double original = mapper.ConvertBack(180, null); // Результат: 50
    /// ]]></code>
    /// </example>
    protected override double ConvertBack(double x, double? p = null) => _k == 0 ? double.NaN : (x - _MinScale) / _k + _MinValue;
}