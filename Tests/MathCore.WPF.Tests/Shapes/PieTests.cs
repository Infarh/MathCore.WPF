using MathCore.WPF.Shapes;
using System.Windows;
using System.Windows.Controls;

namespace MathCore.WPF.Tests.Shapes;

/// <summary>Набор модульных тестов для проверки поведения сектора Pie</summary>
[TestClass]
public sealed class PieTests
{
    /// <summary>Проверка что значение внешнего радиуса в допустимом диапазоне [0;1] не изменяется при установке</summary>
    [STATestMethod]
    public void OuterRadiusWithinRange_IsNotChanged()
    {
        var pie = new Pie { OuterRadius = 0.7 };

        Assert.AreEqual(0.7d, pie.OuterRadius, "Внешний радиус в диапазоне [0;1] не должен изменяться при установке");
    }

    /// <summary>Проверка что значение внутреннего радиуса в допустимом диапазоне [0;1] не изменяется при установке</summary>
    [STATestMethod]
    public void InnerRadiusWithinRange_IsNotChanged()
    {
        var pie = new Pie { InnerRadius = 0.3 };

        Assert.AreEqual(0.3d, pie.InnerRadius, "Внутренний радиус в диапазоне [0;1] не должен изменяться при установке");
    }

    /// <summary>Проверка что внутренний радиус не может быть больше внешнего</summary>
    [STATestMethod]
    public void InnerRadiusGreaterThanOuterRadius_IsCoercedToOuterRadius()
    {
        var pie = new Pie { OuterRadius = 0.5, InnerRadius = 0.8 };

        Assert.IsTrue(pie.InnerRadius <= pie.OuterRadius, "Внутренний радиус должен быть меньше или равен внешнему радиусу");
    }

    /// <summary>Проверка что при нулевой ширине фигуры формируется пустая геометрия сектора</summary>
    [STATestMethod]
    public void ZeroWidth_ReturnsEmptyGeometry()
    {
        var pie = new Pie
        {
            Width = 0,
            Height = 100,
            OuterRadius = 1,
            StartAngle = 0,
            StopAngle = 180
        };

        Assert.IsTrue(pie.RenderedGeometry.IsEmpty(), "При нулевой ширине фигуры геометрия сектора должна быть пустой");
    }

    /// <summary>Проверка что при нулевой высоте фигуры формируется пустая геометрия сектора</summary>
    [STATestMethod]
    public void ZeroHeight_ReturnsEmptyGeometry()
    {
        var pie = new Pie
        {
            Width = 100,
            Height = 0,
            OuterRadius = 1,
            StartAngle = 0,
            StopAngle = 180
        };

        Assert.IsTrue(pie.RenderedGeometry.IsEmpty(), "При нулевой высоте фигуры геометрия сектора должна быть пустой");
    }

    /// <summary>Проверка что при равных начальном и конечном углах строится нулевой сектор и геометрия пуста</summary>
    [STATestMethod]
    public void ZeroSector_WhenAnglesAreEqual_ReturnsEmptyGeometry()
    {
        var pie = new Pie
        {
            Width = 100,
            Height = 100,
            OuterRadius = 1,
            StartAngle = 45,
            StopAngle = 45
        };

        Assert.IsTrue(pie.RenderedGeometry.IsEmpty(), "При равных начальном и конечном углах геометрия сектора должна быть пустой");
    }

    /// <summary>Проверка что свойство IsAligned по умолчанию имеет значение false</summary>
    [STATestMethod]
    public void IsAlignedDefaultValue_IsFalse()
    {
        var pie = new Pie();

        Assert.AreEqual(false, pie.IsAligned, "Свойство IsAligned по умолчанию должно быть false");
    }

    /// <summary>Проверка что свойство IsAligned может быть установлено в true</summary>
    [STATestMethod]
    public void IsAlignedCanBeSetToTrue()
    {
        var pie = new Pie { IsAligned = true };

        Assert.AreEqual(true, pie.IsAligned, "Свойство IsAligned должно быть установлено в true");
    }

    /// <summary>Проверка что при изменении свойства StopAngle автоматически обновляется свойство Angle</summary>
    [STATestMethod]
    public void StopAngleChanged_AngleIsUpdated()
    {
        var pie = new Pie { StartAngle = 0, StopAngle = 180 };

        Assert.AreEqual(180d, pie.Angle, "При изменении StopAngle свойство Angle должно обновиться");
    }

    /// <summary>Проверка что при изменении свойства Angle автоматически обновляется свойство StopAngle</summary>
    [STATestMethod]
    public void AngleChanged_StopAngleIsUpdated()
    {
        var pie = new Pie { StartAngle = 0, Angle = 90 };

        Assert.AreEqual(90d, pie.StopAngle, "При изменении Angle свойство StopAngle должно обновиться");
    }

    /// <summary>Проверка что полный круг (360 градусов) с нулевым внутренним радиусом формирует корректную геометрию</summary>
    [STATestMethod]
    public void FullCircleWithZeroInnerRadius_ReturnsValidGeometry()
    {
        var pie = new Pie
        {
            OuterRadius = 1,
            InnerRadius = 0,
            StartAngle = 0,
            StopAngle = 360
        };

        var canvas = new Canvas();
        canvas.Children.Add(pie);
        pie.Measure(new(200, 200));
        pie.Arrange(new(0, 0, 100, 100));

        Assert.IsFalse(pie.RenderedGeometry.IsEmpty(), "Полный круг должен иметь непустую геометрию");
    }

    /// <summary>Проверка что кольцо (360 градусов с ненулевым внутренним радиусом) формирует корректную геометрию</summary>
    [STATestMethod]
    public void FullRingWithInnerRadius_ReturnsValidGeometry()
    {
        var pie = new Pie
        {
            OuterRadius = 1,
            InnerRadius = 0.5,
            StartAngle = 0,
            StopAngle = 360
        };

        var canvas = new Canvas();
        canvas.Children.Add(pie);
        pie.Measure(new(200, 200));
        pie.Arrange(new(0, 0, 100, 100));

        Assert.IsFalse(pie.RenderedGeometry.IsEmpty(), "Полное кольцо должно иметь непустую геометрию");
    }

    /// <summary>Проверка что начальный угол по умолчанию равен 0</summary>
    [STATestMethod]
    public void StartAngleDefaultValue_IsZero()
    {
        var pie = new Pie();

        Assert.AreEqual(0d, pie.StartAngle, "Начальный угол по умолчанию должен быть 0");
    }

    /// <summary>Проверка что конечный угол по умолчанию равен 360</summary>
    [STATestMethod]
    public void StopAngleDefaultValue_Is360()
    {
        var pie = new Pie();

        Assert.AreEqual(360d, pie.StopAngle, "Конечный угол по умолчанию должен быть 360");
    }

    /// <summary>Проверка что угол раствора по умолчанию равен 360</summary>
    [STATestMethod]
    public void AngleDefaultValue_Is360()
    {
        var pie = new Pie();

        Assert.AreEqual(360d, pie.Angle, "Угол раствора по умолчанию должен быть 360");
    }

    /// <summary>Проверка что внешний радиус по умолчанию равен 1</summary>
    [STATestMethod]
    public void OuterRadiusDefaultValue_IsOne()
    {
        var pie = new Pie();

        Assert.AreEqual(1d, pie.OuterRadius, "Внешний радиус по умолчанию должен быть 1");
    }

    /// <summary>Проверка что внутренний радиус по умолчанию равен 0</summary>
    [STATestMethod]
    public void InnerRadiusDefaultValue_IsZero()
    {
        var pie = new Pie();

        Assert.AreEqual(0d, pie.InnerRadius, "Внутренний радиус по умолчанию должен быть 0");
    }
}
