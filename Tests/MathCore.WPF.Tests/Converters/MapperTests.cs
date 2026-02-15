using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class MapperTests
{
    [TestMethod]
    /// <summary>Проверка преобразования значения конвертером Mapper из одного диапазона в другой</summary>
    public void Convert()
    {
        const double min_scale = 15;
        const double max_scale = 345;
        const double min_value = 10;
        const double max_value = 220;
        IValueConverter converter = new Mapper
        {
            MinScale = min_scale,
            MaxScale = max_scale,
            MinValue = min_value,
            MaxValue = max_value
        };

        var at_0         = converter.Convert(0, typeof(double), null, CultureInfo.CurrentCulture);
        var at_min_value = converter.Convert(min_value, typeof(double), null, CultureInfo.CurrentCulture);
        var at_max_value = converter.Convert(max_value, typeof(double), null, CultureInfo.CurrentCulture);
        var zero         = (double)converter.Convert(0.45454545454545, typeof(double), null, CultureInfo.CurrentCulture)!;

        Assert.That.Value(at_0).IsEqual(-0.7142857142857135);
        Assert.That.Value(at_min_value).IsEqual(min_scale);
        Assert.That.Value(at_max_value).IsEqual(max_scale);
        Assert.That.Value(zero).IsEqual(0, 1e-14);
    }

    [TestMethod]
    /// <summary>Проверка обратного преобразования конвертером Mapper из целевого диапазона в исходный</summary>
    public void ConvertBack()
    {
        const double min_scale = 15;
        const double max_scale = 345;
        const double min_value = 10;
        const double max_value = 220;
        IValueConverter converter = new Mapper
        {
            MinScale = min_scale,
            MaxScale = max_scale,
            MinValue = min_value,
            MaxValue = max_value
        };

        var at_scale_0   = (double)converter.ConvertBack(0, typeof(double), null, CultureInfo.CurrentCulture)!;
        var at_scale_min = converter.ConvertBack(min_scale, typeof(double), null, CultureInfo.CurrentCulture);
        var at_scale_max = converter.ConvertBack(max_scale, typeof(double), null, CultureInfo.CurrentCulture);
        var at_scale_360 = (double)converter.ConvertBack(360, typeof(double), null, CultureInfo.CurrentCulture)!;

        Assert.That.Value(at_scale_0).IsEqual(0.45454545454545, 5.1e-15);
        Assert.That.Value(at_scale_min).IsEqual(min_value);
        Assert.That.Value(at_scale_max).IsEqual(max_value);
        Assert.That.Value(at_scale_360).IsEqual(229.54545454545456);
    }
}

[TestClass]
public class MapperConverterTests
{
    [TestMethod]
    /// <summary>Проверка базового преобразования значения конвертером MapperConverter с заданными диапазонами</summary>
    public void Convert_WithBasicRange_ReturnsCorrectValue()
    {
        const double min_scale = 15;
        const double max_scale = 345;
        const double min_value = 10;
        const double max_value = 220;
        
        var converter = new MapperConverter
        {
            MinScale = min_scale,
            MaxScale = max_scale,
            MinValue = min_value,
            MaxValue = max_value
        };

        var result_at_0 = (double)converter.Convert(0, typeof(double), null, CultureInfo.CurrentCulture);
        var result_at_min_value = converter.Convert(min_value, typeof(double), null, CultureInfo.CurrentCulture);
        var result_at_max_value = converter.Convert(max_value, typeof(double), null, CultureInfo.CurrentCulture);
        var result_at_middle = (double)converter.Convert(0.45454545454545, typeof(double), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result_at_0).IsEqual(-0.7142857142857135);
        Assert.That.Value(result_at_min_value).IsEqual(min_scale);
        Assert.That.Value(result_at_max_value).IsEqual(max_scale);
        Assert.That.Value(result_at_middle).IsEqual(0, 1e-14);
    }

    [TestMethod]
    /// <summary>Проверка обратного преобразования конвертером MapperConverter из целевого диапазона в исходный</summary>
    public void ConvertBack_WithBasicRange_ReturnsCorrectValue()
    {
        const double min_scale = 15;
        const double max_scale = 345;
        const double min_value = 10;
        const double max_value = 220;
        
        var converter = new MapperConverter
        {
            MinScale = min_scale,
            MaxScale = max_scale,
            MinValue = min_value,
            MaxValue = max_value
        };

        var result_at_scale_0 = (double)converter.ConvertBack(0, typeof(double), null, CultureInfo.CurrentCulture);
        var result_at_scale_min = converter.ConvertBack(min_scale, typeof(double), null, CultureInfo.CurrentCulture);
        var result_at_scale_max = converter.ConvertBack(max_scale, typeof(double), null, CultureInfo.CurrentCulture);
        var result_at_scale_360 = (double)converter.ConvertBack(360, typeof(double), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result_at_scale_0).IsEqual(0.45454545454545, 5.1e-15);
        Assert.That.Value(result_at_scale_min).IsEqual(min_value);
        Assert.That.Value(result_at_scale_max).IsEqual(max_value);
        Assert.That.Value(result_at_scale_360).IsEqual(229.54545454545456);
    }

    [TestMethod]
    /// <summary>Проверка преобразования значения конвертером MapperConverter при нулевом диапазоне входных значений</summary>
    public void ConvertBack_WithZeroRange_ReturnsNaN()
    {
        var converter = new MapperConverter
        {
            MinValue = 10,
            MaxValue = 10, // Нулевой диапазон
            MinScale = 0,
            MaxScale = 100
        };

        var result = converter.ConvertBack(50, typeof(double), null, CultureInfo.CurrentCulture);

        Assert.That.Value((double)result).IsNaN();
    }

    [TestMethod]
    /// <summary>Проверка преобразования строкового значения конвертером MapperConverter в диапазон значений</summary>
    public void Convert_WithStringInput_ConvertsToDouble()
    {
        var converter = new MapperConverter
        {
            MinValue = 0,
            MaxValue = 100,
            MinScale = 0,
            MaxScale = 360
        };

        var result = (double)converter.Convert("50", typeof(double), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(180);
    }

    [TestMethod]
    /// <summary>Проверка преобразования значения null конвертером MapperConverter</summary>
    public void Convert_WithNullInput_ReturnsNaN()
    {
        var converter = new MapperConverter
        {
            MinValue = 0,
            MaxValue = 100,
            MinScale = 0,
            MaxScale = 360
        };

        var result = converter.Convert(null, typeof(double), null, CultureInfo.CurrentCulture);

        Assert.That.Value((double)result).IsNaN();
    }

    [TestMethod]
    /// <summary>Проверка изменения свойств конвертера MapperConverter и пересчёта коэффициента масштабирования</summary>
    public void PropertyChange_UpdatesDependencyPropertyValue()
    {
        var converter = new MapperConverter
        {
            MinValue = 0,
            MaxValue = 100,
            MinScale = 0,
            MaxScale = 360
        };

        var result_50_before = (double)converter.Convert(50, typeof(double), null, CultureInfo.CurrentCulture);

        converter.MaxScale = 180; // Изменяем максимальный масштаб

        var result_50_after = (double)converter.Convert(50, typeof(double), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result_50_before).IsEqual(180); // 50 из [0,100] в [0,360] = 180
        Assert.That.Value(result_50_after).IsEqual(90);   // 50 из [0,100] в [0,180] = 90
    }
}

[TestClass]
public class MapperFTests
{
    [TestMethod]
    /// <summary>Проверка создания экземпляра MapperConverter через расширение разметки MapperF</summary>
    public void ProvideValue_CreatesMapperConverterInstance()
    {
        var extension = new MapperF
        {
            MinValue = 0,
            MaxValue = 100,
            MinScale = 0,
            MaxScale = 360
        };

        var converter = extension.ProvideValue(null);

        Assert.IsNotNull(converter);
        Assert.IsInstanceOfType(converter, typeof(MapperConverter));
    }

    [TestMethod]
    /// <summary>Проверка правильного копирования параметров расширения разметки MapperF в созданный конвертер</summary>
    public void ProvideValue_ConverterHasCorrectParameters()
    {
        const double min_value = 10;
        const double max_value = 220;
        const double min_scale = 15;
        const double max_scale = 345;

        var extension = new MapperF
        {
            MinValue = min_value,
            MaxValue = max_value,
            MinScale = min_scale,
            MaxScale = max_scale
        };

        var converter = (MapperConverter)extension.ProvideValue(null);

        Assert.That.Value(converter.MinValue).IsEqual(min_value);
        Assert.That.Value(converter.MaxValue).IsEqual(max_value);
        Assert.That.Value(converter.MinScale).IsEqual(min_scale);
        Assert.That.Value(converter.MaxScale).IsEqual(max_scale);
    }

    [TestMethod]
    /// <summary>Проверка преобразования значения через конвертер, созданный расширением разметки MapperF</summary>
    public void ProvideValue_CreatedConverterWorks()
    {
        var extension = new MapperF
        {
            MinValue = 0,
            MaxValue = 100,
            MinScale = 0,
            MaxScale = 360
        };

        var converter = (IValueConverter)extension.ProvideValue(null);
        var result = (double)converter.Convert(50, typeof(double), null, CultureInfo.CurrentCulture);

        Assert.That.Value(result).IsEqual(180);
    }

    [TestMethod]
    /// <summary>Проверка создания независимых экземпляров конвертера при нескольких вызовах расширения разметки MapperF</summary>
    public void ProvideValue_CreatesIndependentInstances()
    {
        var extension = new MapperF
        {
            MinValue = 0,
            MaxValue = 100,
            MinScale = 0,
            MaxScale = 360
        };

        var converter1 = extension.ProvideValue(null);
        var converter2 = extension.ProvideValue(null);

        Assert.AreNotSame(converter1, converter2);
    }
}