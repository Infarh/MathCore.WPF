#nullable enable

using System.Windows.Markup;

namespace MathCore.WPF.Converters;

/// <summary>Расширение разметки для удобного создания экземпляров MapperConverter в XAML</summary>
/// <remarks>
/// Это расширение разметки предоставляет краткий синтаксис для создания MapperConverter
/// при использовании в привязках, которые не требуют наследования DataContext.
/// 
/// Используйте это расширение когда все параметры известны на момент разработки
/// и не должны быть привязаны к DataContext.
/// 
/// Для случаев, когда параметры должны быть привязаны к источникам данных,
/// используйте MapperConverter напрямую как элемент ресурсов с привязками.
/// </remarks>
/// <example>
/// Пример использования в XAML для простого статического маппера:
/// <code language="xml"><![CDATA[
/// <RotateTransform Angle="{Binding Temperature, 
///     Converter={local:MapperF MinValue=0, MaxValue=100, MinScale=0, MaxScale=360}}" />
/// ]]></code>
/// 
/// Пример в коде на C#:
/// <code language="csharp"><![CDATA[
/// var extension = new MapperF { MinValue = 0, MaxValue = 100, MinScale = 0, MaxScale = 360 };
/// IValueConverter converter = (IValueConverter)extension.ProvideValue(null);
/// double angle = (double)converter.Convert(50, typeof(double), null, null); // Результат: 180
/// ]]></code>
/// </example>
[MarkupExtensionReturnType(typeof(MapperConverter))]
public class MapperF : MarkupExtension
{
    /// <summary>Минимальное значение исходного диапазона</summary>
    public double MinValue { get; set; }

    /// <summary>Максимальное значение исходного диапазона</summary>
    public double MaxValue { get; set; } = 1;

    /// <summary>Минимальное значение целевого диапазона (масштаба)</summary>
    public double MinScale { get; set; }

    /// <summary>Максимальное значение целевого диапазона (масштаба)</summary>
    public double MaxScale { get; set; } = 1;

    /// <summary>Возвращает новый экземпляр MapperConverter с установленными параметрами</summary>
    /// <param name="serviceProvider">Поставщик сервисов (не используется)</param>
    /// <returns>Новый экземпляр MapperConverter с заданными значениями диапазонов</returns>
    /// <remarks>
    /// Каждый вызов ProvideValue создаёт новый независимый экземпляр MapperConverter.
    /// Это гарантирует, что каждая привязка получает свой экземпляр конвертера.
    /// </remarks>
    public override object ProvideValue(IServiceProvider? serviceProvider) =>
        new MapperConverter
        {
            MinValue = MinValue,
            MaxValue = MaxValue,
            MinScale = MinScale,
            MaxScale = MaxScale
        };
}
