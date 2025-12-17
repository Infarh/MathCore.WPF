using MathCore.WPF.Shapes;

namespace MathCore.WPF.Tests.Shapes;

/// <summary>Набор модульных тестов для проверки поведения дуги Arc</summary>
[TestClass]
public sealed class ArcTests
{
    /// <summary>Проверка что отрицательное значение радиуса принудительно приводится к нулю</summary>
    [STATestMethod]
    public void ZeroRadius_IsCoercedToZero()
    {
        var arc = new Arc { R = -1 };

        Assert.AreEqual(0d, arc.R, "Отрицательный радиус должен быть приведён к нулю");
    }

    /// <summary>Проверка что значение радиуса больше единицы принудительно ограничивается единицей</summary>
    [STATestMethod]
    public void RadiusGreaterThanOne_IsCoercedToOne()
    {
        var arc = new Arc { R = 2 };

        Assert.AreEqual(1d, arc.R, "Радиус больше единицы должен быть приведён к единице");
    }

    /// <summary>Проверка что значение радиуса в допустимом диапазоне [0;1] не изменяется при установке</summary>
    [STATestMethod]
    public void RadiusWithinRange_IsNotChanged()
    {
        var arc = new Arc { R = 0.5 };

        Assert.AreEqual(0.5d, arc.R, "Радиус в диапазоне [0;1] не должен изменяться при установке");
    }

    /// <summary>Проверка что при нулевой ширине фигуры формируется пустая геометрия дуги</summary>
    [STATestMethod]
    public void ZeroWidth_ReturnsEmptyGeometry()
    {
        var arc = new Arc
        {
            Width = 0,
            Height = 100,
            R = 1,
            StartAngle = 0,
            StopAngle = 180
        };

        Assert.IsTrue(arc.RenderedGeometry.IsEmpty(), "При нулевой ширине фигуры геометрия дуги должна быть пустой");
    }

    /// <summary>Проверка что при нулевой высоте фигуры формируется пустая геометрия дуги</summary>
    [STATestMethod]
    public void ZeroHeight_ReturnsEmptyGeometry()
    {
        var arc = new Arc
        {
            Width = 100,
            Height = 0,
            R = 1,
            StartAngle = 0,
            StopAngle = 180
        };

        Assert.IsTrue(arc.RenderedGeometry.IsEmpty(), "При нулевой высоте фигуры геометрия дуги должна быть пустой");
    }

    /// <summary>Проверка что при равных начальном и конечном углах строится нулевая дуга и геометрия пуста</summary>
    [STATestMethod]
    public void ZeroArc_WhenAnglesAreEqual_ReturnsEmptyGeometry()
    {
        var arc = new Arc
        {
            Width = 100,
            Height = 100,
            R = 1,
            StartAngle = 10,
            StopAngle = 10
        };

        Assert.IsTrue(arc.RenderedGeometry.IsEmpty(), "При равных начальном и конечном углах геометрия дуги должна быть пустой");
    }
}
