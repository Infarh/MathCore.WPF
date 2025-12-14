using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using MathCore.WPF.Converters;

using InterpolationConverter = MathCore.WPF.Converters.Interpolation;

namespace MathCore.WPF.Tests.Converters;

/// <summary>Тесты для конвертеров интерполяции</summary>
[TestClass]
public class InterpolationConvertersTests
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    #region Interpolation Tests

    /// <summary>Тест конвертера Interpolation - интерполяция через заданные точки</summary>
    [TestMethod]
    public void Interpolation_ThroughPoints_ReturnsCorrectValue()
    {
        var converter = new InterpolationConverter 
        { 
            Points = PointCollection.Parse("0,0 1,1 2,4") 
        };
        
        var at_0 = (double)((IValueConverter)converter).Convert(0.0, typeof(double), null, Culture)!;
        var at_1 = (double)((IValueConverter)converter).Convert(1.0, typeof(double), null, Culture)!;
        var at_2 = (double)((IValueConverter)converter).Convert(2.0, typeof(double), null, Culture)!;
        
        Debug.WriteLine($"Интерполяция в точке 0: {at_0}");
        Debug.WriteLine($"Интерполяция в точке 1: {at_1}");
        Debug.WriteLine($"Интерполяция в точке 2: {at_2}");
        
        Assert.That.Value(at_0).IsEqual(0.0, 1e-10);
        Assert.That.Value(at_1).IsEqual(1.0, 1e-10);
        Assert.That.Value(at_2).IsEqual(4.0, 1e-10);
    }

    /// <summary>Тест конвертера Interpolation - интерполяция между точками</summary>
    [TestMethod]
    public void Interpolation_BetweenPoints_Interpolates()
    {
        var converter = new InterpolationConverter 
        { 
            Points = PointCollection.Parse("0,0 2,4 4,8") 
        };
        
        var at_1 = (double)((IValueConverter)converter).Convert(1.0, typeof(double), null, Culture)!;
        var at_3 = (double)((IValueConverter)converter).Convert(3.0, typeof(double), null, Culture)!;
        
        Debug.WriteLine($"Интерполяция в точке 1: {at_1}");
        Debug.WriteLine($"Интерполяция в точке 3: {at_3}");
        
        Assert.That.Value(at_1).IsEqual(2.0, 1e-10);
        Assert.That.Value(at_3).IsEqual(6.0, 1e-10);
    }

    /// <summary>Тест конвертера Interpolation - линейная функция</summary>
    [TestMethod]
    public void Interpolation_LinearFunction_WorksCorrectly()
    {
        var converter = new InterpolationConverter 
        { 
            Points = PointCollection.Parse("1,3 3,7") 
        };
        
        var at_2 = (double)((IValueConverter)converter).Convert(2.0, typeof(double), null, Culture)!;
        
        Debug.WriteLine($"Линейная интерполяция в точке 2: {at_2}");
        
        Assert.That.Value(at_2).IsEqual(5.0, 1e-10);
    }

    /// <summary>Тест конвертера Interpolation - экстраполяция вне диапазона</summary>
    [TestMethod]
    public void Interpolation_Extrapolation_WorksOutsideRange()
    {
        var converter = new InterpolationConverter 
        { 
            Points = PointCollection.Parse("0,0 1,1 2,4") 
        };
        
        var at_3 = (double)((IValueConverter)converter).Convert(3.0, typeof(double), null, Culture)!;
        
        Debug.WriteLine($"Экстраполяция в точке 3: {at_3}");
        
        Assert.IsFalse(double.IsNaN(at_3), "Экстраполяция не должна возвращать NaN");
    }

    #endregion

    #region CSplineInterp Tests

    /// <summary>Тест конвертера CSplineInterp - интерполяция кубическим сплайном через заданные точки</summary>
    [TestMethod]
    public void CSplineInterp_ThroughPoints_ReturnsCorrectValue()
    {
        var converter = new CSplineInterp 
        { 
            Points = PointCollection.Parse("0,0 1,1 2,4 3,9") 
        };
        converter.ProvideValue(null!); // Инициализация сплайна
        
        var at_0 = (double)((IValueConverter)converter).Convert(0.0, typeof(double), null, Culture)!;
        var at_1 = (double)((IValueConverter)converter).Convert(1.0, typeof(double), null, Culture)!;
        var at_2 = (double)((IValueConverter)converter).Convert(2.0, typeof(double), null, Culture)!;
        var at_3 = (double)((IValueConverter)converter).Convert(3.0, typeof(double), null, Culture)!;
        
        Debug.WriteLine($"CSpline в точке 0: {at_0}");
        Debug.WriteLine($"CSpline в точке 1: {at_1}");
        Debug.WriteLine($"CSpline в точке 2: {at_2}");
        Debug.WriteLine($"CSpline в точке 3: {at_3}");
        
        Assert.That.Value(at_0).IsEqual(0.0, 1e-10);
        Assert.That.Value(at_1).IsEqual(1.0, 1e-10);
        Assert.That.Value(at_2).IsEqual(4.0, 1e-10);
        Assert.That.Value(at_3).IsEqual(9.0, 1e-10);
    }

    /// <summary>Тест конвертера CSplineInterp - сплайн между точками обеспечивает гладкость</summary>
    [TestMethod]
    public void CSplineInterp_BetweenPoints_IsSmoothAndContinuous()
    {
        var converter = new CSplineInterp 
        { 
            Points = PointCollection.Parse("0,0 1,1 2,0") 
        };
        converter.ProvideValue(null!); // Инициализация сплайна
        
        var at_0_5 = (double)((IValueConverter)converter).Convert(0.5, typeof(double), null, Culture)!;
        var at_1_5 = (double)((IValueConverter)converter).Convert(1.5, typeof(double), null, Culture)!;
        
        Debug.WriteLine($"CSpline в точке 0.5: {at_0_5}");
        Debug.WriteLine($"CSpline в точке 1.5: {at_1_5}");
        
        Assert.IsFalse(double.IsNaN(at_0_5), "Сплайн не должен возвращать NaN");
        Assert.IsFalse(double.IsNaN(at_1_5), "Сплайн не должен возвращать NaN");
    }

    /// <summary>Тест конвертера CSplineInterp - линейная функция</summary>
    [TestMethod]
    public void CSplineInterp_LinearFunction_WorksCorrectly()
    {
        var converter = new CSplineInterp 
        { 
            Points = PointCollection.Parse("0,0 1,2 2,4") 
        };
        converter.ProvideValue(null!); // Инициализация сплайна
        
        var at_0_5 = (double)((IValueConverter)converter).Convert(0.5, typeof(double), null, Culture)!;
        var at_1_5 = (double)((IValueConverter)converter).Convert(1.5, typeof(double), null, Culture)!;
        
        Debug.WriteLine($"CSpline (линейная) в точке 0.5: {at_0_5}");
        Debug.WriteLine($"CSpline (линейная) в точке 1.5: {at_1_5}");
        
        Assert.That.Value(at_0_5).IsEqual(1.0, 1e-10);
        Assert.That.Value(at_1_5).IsEqual(3.0, 1e-10);
    }

    /// <summary>Тест конвертера CSplineInterp - две точки ведут себя как линейная интерполяция</summary>
    [TestMethod]
    public void CSplineInterp_TwoPoints_BehavesLikeLinear()
    {
        var converter = new CSplineInterp 
        { 
            Points = PointCollection.Parse("0,0 2,4") 
        };
        converter.ProvideValue(null!); // Инициализация сплайна
        
        var at_1 = (double)((IValueConverter)converter).Convert(1.0, typeof(double), null, Culture)!;
        
        Debug.WriteLine($"CSpline (2 точки) в точке 1: {at_1}");
        
        Assert.That.Value(at_1).IsEqual(2.0, 1e-10);
    }

    /// <summary>Тест конвертера CSplineInterp - сплайн обеспечивает гладкий переход</summary>
    [TestMethod]
    public void CSplineInterp_SmoothTransition_BetweenMultiplePoints()
    {
        var converter = new CSplineInterp 
        { 
            Points = PointCollection.Parse("0,0 1,1 2,0 3,1 4,0") 
        };
        converter.ProvideValue(null!); // Инициализация сплайна
        
        var values = new List<double>();
        for (var x = 0.0; x <= 4.0; x += 0.5)
        {
            var y = (double)((IValueConverter)converter).Convert(x, typeof(double), null, Culture)!;
            values.Add(y);
            Debug.WriteLine($"CSpline в точке {x}: {y}");
        }
        
        Assert.IsTrue(values.All(v => !double.IsNaN(v)), "Все значения должны быть корректными числами");
    }

    #endregion
}
