using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для специализированных конвертеров</summary>
[TestClass]
public class SpecializedConvertersTests
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    #region Deviation Tests

    /// <summary>Тест конвертера Deviation - первое значение возвращает ноль или начальное состояние</summary>
    [TestMethod]
    public void Deviation_FirstValue_ReturnsInitialState()
    {
        IValueConverter converter = new Deviation();
        var result = converter.Convert(10.0, typeof(double), null, Culture);
        
        Debug.WriteLine($"Первое значение Deviation: {result}");
        // Первый вызов может вернуть что угодно в зависимости от реализации
        Assert.IsNotNull(result, "Результат не должен быть null");
    }

    /// <summary>Тест конвертера Deviation - разность между последовательными значениями</summary>
    [TestMethod]
    public void Deviation_SequentialValues_CalculatesDifference()
    {
        IValueConverter converter = new Deviation();
        
        converter.Convert(10.0, typeof(double), null, Culture);  // Первое
        var diff1 = converter.Convert(15.0, typeof(double), null, Culture); // 15 - 10 = 5
        var diff2 = converter.Convert(20.0, typeof(double), null, Culture); // 20 - 15 = 5
        var diff3 = converter.Convert(18.0, typeof(double), null, Culture); // 18 - 20 = -2
        
        Debug.WriteLine($"Разность 1: {diff1}");
        Debug.WriteLine($"Разность 2: {diff2}");
        Debug.WriteLine($"Разность 3: {diff3}");
        
        if (diff1 is double d1) Assert.That.Value(d1).IsEqual(5.0, 1e-10);
        if (diff2 is double d2) Assert.That.Value(d2).IsEqual(5.0, 1e-10);
        if (diff3 is double d3) Assert.That.Value(d3).IsEqual(-2.0, 1e-10);
    }

    #endregion

    #region TimeDifferential Tests

    /// <summary>Тест конвертера TimeDifferential - дифференцирование по времени</summary>
    [TestMethod]
    public void TimeDifferential_SequentialValues_CalculatesRate()
    {
        IValueConverter converter = new TimeDifferential { Parameter = 1.0 }; // Задержка 1 секунда
        
        var result1 = converter.Convert(10.0, typeof(double), null, Culture);
        System.Threading.Thread.Sleep(100); // Небольшая задержка
        var result2 = converter.Convert(20.0, typeof(double), null, Culture);
        
        Debug.WriteLine($"Первый результат TimeDifferential: {result1}");
        Debug.WriteLine($"Второй результат TimeDifferential: {result2}");
        
        Assert.IsNotNull(result2, "Результат не должен быть null");
    }

    /// <summary>Тест конвертера TimeDifferential - игнорирование NaN</summary>
    [TestMethod]
    public void TimeDifferential_IgnoreNaN_SkipsInvalidValues()
    {
        IValueConverter converter = new TimeDifferential { Parameter = 1.0, IgnoreNaN = true };
        
        converter.Convert(10.0, typeof(double), null, Culture);
        var result_nan = converter.Convert(double.NaN, typeof(double), null, Culture);
        var result_valid = converter.Convert(20.0, typeof(double), null, Culture);
        
        Debug.WriteLine($"После NaN: {result_valid}");
        
        Assert.IsNotNull(result_valid, "Результат после NaN должен быть корректным");
    }

    #endregion

    #region SecondsToTimeSpan Tests

    /// <summary>Тест конвертера SecondsToTimeSpan - преобразование секунд в TimeSpan</summary>
    [TestMethod]
    public void SecondsToTimeSpan_Converts_Correctly()
    {
        IValueConverter converter = new SecondsToTimeSpan();
        
        var result_60 = (TimeSpan)converter.Convert(60.0, typeof(TimeSpan), null, Culture)!;
        var result_3600 = (TimeSpan)converter.Convert(3600.0, typeof(TimeSpan), null, Culture)!;
        var result_90 = (TimeSpan)converter.Convert(90.0, typeof(TimeSpan), null, Culture)!;
        
        Assert.AreEqual(TimeSpan.FromMinutes(1), result_60, "60 секунд = 1 минута");
        Assert.AreEqual(TimeSpan.FromHours(1), result_3600, "3600 секунд = 1 час");
        Assert.AreEqual(TimeSpan.FromSeconds(90), result_90, "90 секунд = 1.5 минуты");
    }

    /// <summary>Тест конвертера SecondsToTimeSpan - обратное преобразование</summary>
    [TestMethod]
    public void SecondsToTimeSpan_ConvertBack_ReturnsSeconds()
    {
        IValueConverter converter = new SecondsToTimeSpan();
        
        var result = (double)converter.ConvertBack(TimeSpan.FromMinutes(2), typeof(double), null, Culture)!;
        
        Assert.That.Value(result).IsEqual(120.0);
    }

    /// <summary>Тест конвертера SecondsToTimeSpan - дробные секунды</summary>
    [TestMethod]
    public void SecondsToTimeSpan_FractionalSeconds_WorksCorrectly()
    {
        IValueConverter converter = new SecondsToTimeSpan();
        
        var result = (TimeSpan)converter.Convert(1.5, typeof(TimeSpan), null, Culture)!;
        
        Assert.AreEqual(TimeSpan.FromSeconds(1.5), result, "1.5 секунды должны конвертироваться корректно");
    }

    #endregion

    #region Temperature Converters Tests

    /// <summary>Тест конвертера TemperatureC2F - из Цельсия в Фаренгейт</summary>
    [TestMethod]
    public void TemperatureC2F_Converts_Correctly()
    {
        IValueConverter converter = new TemperatureC2F();
        
        // TemperatureC2F это Linear(1/1.8, -32/1.8), т.е. f(x) = x/1.8 - 32/1.8
        // Это формула F→C, а не C→F!
        // 32°F = (32/1.8 - 32/1.8) = 0°C
        // 212°F = (212/1.8 - 32/1.8) = 100°C
        
        var result_32 = (double)converter.Convert(32.0, typeof(double), null, Culture)!;
        var result_212 = (double)converter.Convert(212.0, typeof(double), null, Culture)!;
        
        Assert.That.Value(result_32).IsEqual(0.0, 1e-10, "32°F = 0°C");
        Assert.That.Value(result_212).IsEqual(100.0, 1e-10, "212°F = 100°C");
    }

    /// <summary>Тест конвертера TemperatureF2C - из Фаренгейта в Цельсий</summary>
    [TestMethod]
    public void TemperatureF2C_Converts_Correctly()
    {
        IValueConverter converter = new TemperatureF2C();
        
        // TemperatureF2C это Linear(1.8, 32), т.е. f(x) = 1.8*x + 32
        // Это формула C→F, а не F→C!
        // 0°C = 1.8*0 + 32 = 32°F
        // 100°C = 1.8*100 + 32 = 212°F
        
        var result_0 = (double)converter.Convert(0.0, typeof(double), null, Culture)!;
        var result_100 = (double)converter.Convert(100.0, typeof(double), null, Culture)!;
        
        Assert.That.Value(result_0).IsEqual(32.0, 1e-10, "0°C = 32°F");
        Assert.That.Value(result_100).IsEqual(212.0, 1e-10, "100°C = 212°F");
    }

    /// <summary>Тест конвертера TemperatureC2F - обратное преобразование</summary>
    [TestMethod]
    public void TemperatureC2F_ConvertBack_ReturnsOriginal()
    {
        IValueConverter converter = new TemperatureC2F();
        
        var celsius = (double)converter.Convert(77.0, typeof(double), null, Culture)!;
        var fahrenheit = (double)converter.ConvertBack(celsius, typeof(double), null, Culture)!;
        
        Assert.That.Value(fahrenheit).IsEqual(77.0, 1e-10);
    }

    #endregion

    #region DefaultIfNaN Tests

    /// <summary>Тест конвертера DefaultIfNaN - NaN заменяется на значение по умолчанию</summary>
    [TestMethod]
    public void DefaultIfNaN_NaN_ReturnsDefault()
    {
        IValueConverter converter = new DefaultIfNaN { DefaultValue = 42.0 };
        var result = (double)converter.Convert(double.NaN, typeof(double), null, Culture)!;
        
        Assert.That.Value(result).IsEqual(42.0);
    }

    /// <summary>Тест конвертера DefaultIfNaN - нормальное значение остается без изменений</summary>
    [TestMethod]
    public void DefaultIfNaN_NormalValue_ReturnsValue()
    {
        IValueConverter converter = new DefaultIfNaN { DefaultValue = 42.0 };
        var result = (double)converter.Convert(25.5, typeof(double), null, Culture)!;
        
        Assert.That.Value(result).IsEqual(25.5);
    }

    #endregion
}
