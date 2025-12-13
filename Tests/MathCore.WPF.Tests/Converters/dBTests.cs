using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертера децибелов</summary>
[TestClass]
public class dBTests
{
    /// <summary>Конвертация по мощности вычисляет децибелы</summary>
    [TestMethod]
    public void Convert_ByPower_CalculatesDecibels()
    {
        IValueConverter converter = new dB { ByPower = true };
        var result = converter.Convert(100.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(20.0, (double)result, 0.0001);
    }

    /// <summary>Конвертация по амплитуде вычисляет децибелы</summary>
    [TestMethod]
    public void Convert_ByAmplitude_CalculatesDecibels()
    {
        IValueConverter converter = new dB { ByPower = false };
        var result = converter.Convert(10.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(20.0, (double)result, 0.0001);
    }

    /// <summary>Конвертация NaN возвращает NaN</summary>
    [TestMethod]
    public void Convert_NaN_ReturnsNaN()
    {
        IValueConverter converter = new dB();
        var result = converter.Convert(double.NaN, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.IsTrue(double.IsNaN((double)result));
    }

    /// <summary>Обратное преобразование восстанавливает значение</summary>
    [TestMethod]
    public void ConvertBack_RestoresValue()
    {
        IValueConverter converter = new dB { ByPower = true };
        var result = converter.ConvertBack(20.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.AreEqual(100.0, (double)result, 0.0001);
    }
}
