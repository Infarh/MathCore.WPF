using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

[TestClass]
public class SecondsToTimeSpanTests
{
    /// <summary>Проверка преобразования секунд в TimeSpan</summary>
    [TestMethod]
    public void Convert_DoubleSeconds_ReturnsTimeSpan()
    {
        var converter = new SecondsToTimeSpan();

        var result = (TimeSpan)((IValueConverter)converter).Convert(3600.0, typeof(TimeSpan), null, CultureInfo.InvariantCulture)!;

        Assert.That.Value(result.TotalSeconds).IsEqual(3600.0);
        Assert.That.Value(result.Hours).IsEqual(1);
    }

    /// <summary>Проверка преобразования int секунд в TimeSpan</summary>
    [TestMethod]
    public void Convert_IntSeconds_ReturnsTimeSpan()
    {
        var converter = new SecondsToTimeSpan();

        var result = (TimeSpan)((IValueConverter)converter).Convert(60, typeof(TimeSpan), null, CultureInfo.InvariantCulture)!;

        Assert.That.Value(result.TotalSeconds).IsEqual(60.0);
        Assert.That.Value(result.Minutes).IsEqual(1);
    }

    /// <summary>Проверка преобразования TimeSpan в секунды</summary>
    [TestMethod]
    public void Convert_TimeSpan_ReturnsSeconds()
    {
        var converter = new SecondsToTimeSpan();
        var timeSpan = TimeSpan.FromMinutes(5);

        var result = (double)((IValueConverter)converter).Convert(timeSpan, typeof(double), null, CultureInfo.InvariantCulture)!;

        Assert.That.Value(result).IsEqual(300.0);
    }

    /// <summary>Проверка двустороннего преобразования</summary>
    [TestMethod]
    public void Convert_ThenConvertBack_ReturnsOriginal()
    {
        var converter = new SecondsToTimeSpan();
        var original = 7200.0;

        var timeSpan = (TimeSpan)((IValueConverter)converter).Convert(original, typeof(TimeSpan), null, CultureInfo.InvariantCulture)!;
        var back = (double)((IValueConverter)converter).ConvertBack(timeSpan, typeof(double), null, CultureInfo.InvariantCulture)!;

        Assert.That.Value(back).IsEqual(original, 1e-10);
    }

    /// <summary>Проверка преобразования нуля</summary>
    [TestMethod]
    public void Convert_Zero_ReturnsZeroTimeSpan()
    {
        var converter = new SecondsToTimeSpan();

        var result = (TimeSpan)((IValueConverter)converter).Convert(0.0, typeof(TimeSpan), null, CultureInfo.InvariantCulture)!;

        Assert.That.Value(result).IsEqual(TimeSpan.Zero);
    }

    /// <summary>Проверка преобразования отрицательных секунд</summary>
    [TestMethod]
    public void Convert_NegativeSeconds_ReturnsNegativeTimeSpan()
    {
        var converter = new SecondsToTimeSpan();

        var result = (TimeSpan)((IValueConverter)converter).Convert(-60.0, typeof(TimeSpan), null, CultureInfo.InvariantCulture)!;

        Assert.That.Value(result.TotalSeconds).IsEqual(-60.0);
    }

    /// <summary>Проверка преобразования дробных секунд</summary>
    [TestMethod]
    public void Convert_FractionalSeconds_ReturnsAccurateTimeSpan()
    {
        var converter = new SecondsToTimeSpan();

        var result = (TimeSpan)((IValueConverter)converter).Convert(1.5, typeof(TimeSpan), null, CultureInfo.InvariantCulture)!;

        Assert.That.Value(result.TotalSeconds).IsEqual(1.5, 1e-10);
    }
}
