#nullable enable

using System.Globalization;
using System.Windows;
using System.Windows.Data;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Масштабирует значение из одного диапазона в другой с помощью линейной интерполяции. Версия на основе Freezable для использования в XAML с наследованием DataContext</summary>
/// <remarks>
/// Это версия класса Mapper, которая наследует от Freezable вместо MarkupExtension.
/// Freezable позволяет конвертеру получать DataContext из дерева XAML,
/// что недоступно при использовании MarkupExtension напрямую.
/// 
/// Конвертер выполняет линейное преобразование значения из исходного диапазона [MinValue, MaxValue] 
/// в целевой диапазон [MinScale, MaxScale]. 
/// 
/// Все свойства реализованы как свойства-зависимости (DependencyProperty) для полной поддержки привязок WPF.
/// </remarks>
/// <example>
/// Пример использования в XAML внутри ресурсов элемента:
/// <code language="xml"><![CDATA[
/// <Window.Resources>
///   <local:MapperConverter x:Key="TempToAngleMapper" 
///                          MinValue="0" MaxValue="100" 
///                          MinScale="0" MaxScale="360" />
/// </Window.Resources>
///
/// <RotateTransform Angle="{Binding Temperature, Converter={StaticResource TempToAngleMapper}}" />
/// ]]></code>
/// 
/// Пример использования с наследованием DataContext через Binding:
/// <code language="xml"><![CDATA[
/// <StackPanel DataContext="{Binding MinValueViewModel}">
///   <!-- MapperConverter наследует DataContext из StackPanel -->
///   <local:MapperConverter x:Key="DynamicMapper" 
///                          MinValue="{Binding MinValue}" 
///                          MaxValue="{Binding MaxValue}"
///                          MinScale="0" MaxScale="100" />
///   
///   <Slider Value="{Binding CurrentValue, Converter={StaticResource DynamicMapper}}" />
/// </StackPanel>
/// ]]></code>
/// </example>
[ValueConversion(typeof(double), typeof(double))]
public sealed class MapperConverter : Freezable, IValueConverter
{
    private double _k = 1;

    /// <summary>Минимальное значение масштаба (свойство-зависимость)</summary>
    public static readonly DependencyProperty MinScaleProperty = DependencyProperty.Register(
        nameof(MinScale),
        typeof(double),
        typeof(MapperConverter),
        new PropertyMetadata(0d, OnRangeChanged));

    /// <summary>Минимальное значение масштаба</summary>
    /// <value>Минимальное значение целевого диапазона. По умолчанию 0</value>
    public double MinScale
    {
        get => (double)GetValue(MinScaleProperty);
        set => SetValue(MinScaleProperty, value);
    }

    /// <summary>Максимальное значение масштаба (свойство-зависимость)</summary>
    public static readonly DependencyProperty MaxScaleProperty = DependencyProperty.Register(
        nameof(MaxScale),
        typeof(double),
        typeof(MapperConverter),
        new PropertyMetadata(1d, OnRangeChanged));

    /// <summary>Максимальное значение масштаба</summary>
    /// <value>Максимальное значение целевого диапазона. По умолчанию 1</value>
    public double MaxScale
    {
        get => (double)GetValue(MaxScaleProperty);
        set => SetValue(MaxScaleProperty, value);
    }

    /// <summary>Минимальное значение исходного диапазона (свойство-зависимость)</summary>
    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
        nameof(MinValue),
        typeof(double),
        typeof(MapperConverter),
        new PropertyMetadata(0d, OnRangeChanged));

    /// <summary>Минимальное значение исходного диапазона</summary>
    /// <value>Минимальное значение входного диапазона. По умолчанию 0</value>
    public double MinValue
    {
        get => (double)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    /// <summary>Максимальное значение исходного диапазона (свойство-зависимость)</summary>
    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
        nameof(MaxValue),
        typeof(double),
        typeof(MapperConverter),
        new PropertyMetadata(1d, OnRangeChanged));

    /// <summary>Максимальное значение исходного диапазона</summary>
    /// <value>Максимальное значение входного диапазона. По умолчанию 1</value>
    public double MaxValue
    {
        get => (double)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    /// <summary>Обработчик изменения свойств диапазона для пересчёта коэффициента масштабирования</summary>
    private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MapperConverter mapper)
            mapper.RecalculateK();
    }

    /// <summary>Пересчитывает коэффициент масштабирования на основе текущих диапазонов</summary>
    private void RecalculateK()
    {
        var denom = MaxValue - MinValue;
        _k = denom == 0 ? 0 : (MaxScale - MinScale) / denom;
    }

    /// <summary>Преобразует значение из исходного диапазона в целевой диапазон</summary>
    /// <param name="value">Значение для преобразования</param>
    /// <param name="targetType">Целевой тип (должен быть double)</param>
    /// <param name="parameter">Дополнительный параметр преобразования (не используется)</param>
    /// <param name="culture">Культура для преобразования</param>
    /// <returns>Значение, масштабированное в целевой диапазон [MinScale, MaxScale], или double.NaN если преобразование не удалось</returns>
    /// <remarks>
    /// Метод преобразует входное значение, используя формулу линейной интерполяции:
    /// result = (x - MinValue) * k + MinScale, где k = (MaxScale - MinScale) / (MaxValue - MinValue)
    /// 
    /// Входное значение автоматически преобразуется в double с использованием логики базового класса DoubleValueConverter.
    /// </remarks>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (!DoubleValueConverter.TryConvertToDouble(value, culture, out var x))
            return double.NaN;

        var result = (x - MinValue) * _k + MinScale;
        return result;
    }

    /// <summary>Выполняет обратное преобразование значения из целевого диапазона в исходный диапазон</summary>
    /// <param name="value">Значение из целевого диапазона для обратного преобразования</param>
    /// <param name="targetType">Целевой тип (должен быть double)</param>
    /// <param name="parameter">Дополнительный параметр преобразования (не используется)</param>
    /// <param name="culture">Культура для преобразования</param>
    /// <returns>Значение, преобразованное обратно в исходный диапазон [MinValue, MaxValue], или double.NaN если коэффициент масштабирования равен нулю или преобразование не удалось</returns>
    /// <remarks>
    /// Метод преобразует входное значение, используя обратную формулу:
    /// result = (x - MinScale) / k + MinValue
    /// 
    /// Если коэффициент k равен нулю (входной диапазон имеет нулевую длину), возвращается double.NaN.
    /// </remarks>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (!DoubleValueConverter.TryConvertToDouble(value, culture, out var x))
            return double.NaN;

        return _k == 0 ? double.NaN : (x - MinScale) / _k + MinValue;
    }

    /// <summary>Создаёт копию текущего объекта Freezable</summary>
    /// <returns>Копия текущего объекта</returns>
    protected override Freezable CreateInstanceCore() => new MapperConverter();
}
