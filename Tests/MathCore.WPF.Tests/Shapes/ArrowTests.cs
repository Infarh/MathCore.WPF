using MathCore.WPF.Shapes;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MathCore.WPF.Tests.Shapes;

/// <summary>Набор модульных тестов для проверки поведения стрелки Arrow</summary>
[TestClass]
public sealed class ArrowTests
{
    /// <summary>Проверка что координаты начальной точки по умолчанию равны (0, 0)</summary>
    [STATestMethod]
    public void StartPointDefaultValue_IsZero()
    {
        var arrow = new Arrow();

        Assert.AreEqual(0d, arrow.X1, "X-координата начальной точки по умолчанию должна быть 0");
        Assert.AreEqual(0d, arrow.Y1, "Y-координата начальной точки по умолчанию должна быть 0");
    }

    /// <summary>Проверка что координаты конечной точки по умолчанию равны (0, 0)</summary>
    [STATestMethod]
    public void EndPointDefaultValue_IsZero()
    {
        var arrow = new Arrow();

        Assert.AreEqual(0d, arrow.X2, "X-координата конечной точки по умолчанию должна быть 0");
        Assert.AreEqual(0d, arrow.Y2, "Y-координата конечной точки по умолчанию должна быть 0");
    }

    /// <summary>Проверка что ширина головы стрелки по умолчанию равна 10</summary>
    [STATestMethod]
    public void ArrowHeadWidthDefaultValue_Is10()
    {
        var arrow = new Arrow();

        Assert.AreEqual(10d, arrow.ArrowHeadWidth, "Ширина головы стрелки по умолчанию должна быть 10");
    }

    /// <summary>Проверка что длина головы стрелки по умолчанию равна 15</summary>
    [STATestMethod]
    public void ArrowHeadLengthDefaultValue_Is15()
    {
        var arrow = new Arrow();

        Assert.AreEqual(15d, arrow.ArrowHeadLength, "Длина головы стрелки по умолчанию должна быть 15");
    }

    /// <summary>Проверка что контур головы стрелки по умолчанию замкнут</summary>
    [STATestMethod]
    public void IsArrowHeadClosedDefaultValue_IsTrue()
    {
        var arrow = new Arrow();

        Assert.AreEqual(true, arrow.IsArrowHeadClosed, "Контур головы стрелки по умолчанию должен быть замкнут");
    }

    /// <summary>Проверка что координаты начальной точки устанавливаются корректно</summary>
    [STATestMethod]
    public void StartPointCanBeSet()
    {
        var arrow = new Arrow { X1 = 10, Y1 = 20 };

        Assert.AreEqual(10d, arrow.X1, "X-координата начальной точки должна быть 10");
        Assert.AreEqual(20d, arrow.Y1, "Y-координата начальной точки должна быть 20");
    }

    /// <summary>Проверка что координаты конечной точки устанавливаются корректно</summary>
    [STATestMethod]
    public void EndPointCanBeSet()
    {
        var arrow = new Arrow { X2 = 100, Y2 = 150 };

        Assert.AreEqual(100d, arrow.X2, "X-координата конечной точки должна быть 100");
        Assert.AreEqual(150d, arrow.Y2, "Y-координата конечной точки должна быть 150");
    }

    /// <summary>Проверка что ширина головы стрелки устанавливается корректно</summary>
    [STATestMethod]
    public void ArrowHeadWidthCanBeSet()
    {
        var arrow = new Arrow { ArrowHeadWidth = 20 };

        Assert.AreEqual(20d, arrow.ArrowHeadWidth, "Ширина головы стрелки должна быть 20");
    }

    /// <summary>Проверка что длина головы стрелки устанавливается корректно</summary>
    [STATestMethod]
    public void ArrowHeadLengthCanBeSet()
    {
        var arrow = new Arrow { ArrowHeadLength = 25 };

        Assert.AreEqual(25d, arrow.ArrowHeadLength, "Длина головы стрелки должна быть 25");
    }

    /// <summary>Проверка что отрицательная ширина головы стрелки приводится к нулю</summary>
    [STATestMethod]
    public void NegativeArrowHeadWidth_IsCoercedToZero()
    {
        var arrow = new Arrow { ArrowHeadWidth = -10 };

        Assert.AreEqual(0d, arrow.ArrowHeadWidth, "Отрицательная ширина головы стрелки должна быть приведена к нулю");
    }

    /// <summary>Проверка что отрицательная длина головы стрелки приводится к нулю</summary>
    [STATestMethod]
    public void NegativeArrowHeadLength_IsCoercedToZero()
    {
        var arrow = new Arrow { ArrowHeadLength = -15 };

        Assert.AreEqual(0d, arrow.ArrowHeadLength, "Отрицательная длина головы стрелки должна быть приведена к нулю");
    }

    /// <summary>Проверка что флаг замкнутости контура головы стрелки может быть установлен в false</summary>
    [STATestMethod]
    public void IsArrowHeadClosedCanBeSetToFalse()
    {
        var arrow = new Arrow { IsArrowHeadClosed = false };

        Assert.AreEqual(false, arrow.IsArrowHeadClosed, "Флаг замкнутости контура головы стрелки должен быть false");
    }

    /// <summary>Проверка что стрелка с нулевой длиной формирует пустую геометрию</summary>
    [STATestMethod]
    public void ZeroLengthArrow_ReturnsEmptyGeometry()
    {
        var arrow = new Arrow
        {
            X1 = 50,
            Y1 = 50,
            X2 = 50,
            Y2 = 50,
            ArrowHeadWidth = 10,
            ArrowHeadLength = 15
        };

        var canvas = new Canvas();
        canvas.Children.Add(arrow);
        arrow.Measure(new Size(200, 200));
        arrow.Arrange(new Rect(0, 0, 200, 200));

        Assert.IsTrue(arrow.RenderedGeometry.IsEmpty(), "Стрелка с нулевой длиной должна иметь пустую геометрию");
    }

    /// <summary>Проверка что стрелка с ненулевой длиной формирует непустую геометрию</summary>
    [STATestMethod]
    public void NonZeroLengthArrow_ReturnsValidGeometry()
    {
        var arrow = new Arrow
        {
            X1 = 10,
            Y1 = 10,
            X2 = 100,
            Y2 = 100,
            ArrowHeadWidth = 10,
            ArrowHeadLength = 15
        };

        var canvas = new Canvas();
        canvas.Children.Add(arrow);
        arrow.Measure(new Size(200, 200));
        arrow.Arrange(new Rect(0, 0, 200, 200));

        Assert.IsFalse(arrow.RenderedGeometry.IsEmpty(), "Стрелка с ненулевой длиной должна иметь непустую геометрию");
    }

    /// <summary>Проверка что горизонтальная стрелка формирует корректную геометрию</summary>
    [STATestMethod]
    public void HorizontalArrow_ReturnsValidGeometry()
    {
        var arrow = new Arrow
        {
            X1 = 10,
            Y1 = 50,
            X2 = 100,
            Y2 = 50,
            ArrowHeadWidth = 10,
            ArrowHeadLength = 15
        };

        var canvas = new Canvas();
        canvas.Children.Add(arrow);
        arrow.Measure(new Size(200, 200));
        arrow.Arrange(new Rect(0, 0, 200, 200));

        Assert.IsFalse(arrow.RenderedGeometry.IsEmpty(), "Горизонтальная стрелка должна иметь непустую геометрию");
    }

    /// <summary>Проверка что вертикальная стрелка формирует корректную геометрию</summary>
    [STATestMethod]
    public void VerticalArrow_ReturnsValidGeometry()
    {
        var arrow = new Arrow
        {
            X1 = 50,
            Y1 = 10,
            X2 = 50,
            Y2 = 100,
            ArrowHeadWidth = 10,
            ArrowHeadLength = 15
        };

        var canvas = new Canvas();
        canvas.Children.Add(arrow);
        arrow.Measure(new Size(200, 200));
        arrow.Arrange(new Rect(0, 0, 200, 200));

        Assert.IsFalse(arrow.RenderedGeometry.IsEmpty(), "Вертикальная стрелка должна иметь непустую геометрию");
    }

    /// <summary>Проверка что стрелка с нулевой шириной головы формирует корректную геометрию</summary>
    [STATestMethod]
    public void ArrowWithZeroHeadWidth_ReturnsValidGeometry()
    {
        var arrow = new Arrow
        {
            X1 = 10,
            Y1 = 10,
            X2 = 100,
            Y2 = 100,
            ArrowHeadWidth = 0,
            ArrowHeadLength = 15
        };

        var canvas = new Canvas();
        canvas.Children.Add(arrow);
        arrow.Measure(new Size(200, 200));
        arrow.Arrange(new Rect(0, 0, 200, 200));

        Assert.IsFalse(arrow.RenderedGeometry.IsEmpty(), "Стрелка с нулевой шириной головы должна иметь непустую геометрию");
    }

    /// <summary>Проверка что стрелка с нулевой длиной головы формирует корректную геометрию</summary>
    [STATestMethod]
    public void ArrowWithZeroHeadLength_ReturnsValidGeometry()
    {
        var arrow = new Arrow
        {
            X1 = 10,
            Y1 = 10,
            X2 = 100,
            Y2 = 100,
            ArrowHeadWidth = 10,
            ArrowHeadLength = 0
        };

        var canvas = new Canvas();
        canvas.Children.Add(arrow);
        arrow.Measure(new Size(200, 200));
        arrow.Arrange(new Rect(0, 0, 200, 200));

        Assert.IsFalse(arrow.RenderedGeometry.IsEmpty(), "Стрелка с нулевой длиной головы должна иметь непустую геометрию");
    }

    /// <summary>Проверка что стрелка с незамкнутым контуром головы формирует корректную геометрию</summary>
    [STATestMethod]
    public void ArrowWithOpenHead_ReturnsValidGeometry()
    {
        var arrow = new Arrow
        {
            X1 = 10,
            Y1 = 10,
            X2 = 100,
            Y2 = 100,
            ArrowHeadWidth = 10,
            ArrowHeadLength = 15,
            IsArrowHeadClosed = false
        };

        var canvas = new Canvas();
        canvas.Children.Add(arrow);
        arrow.Measure(new Size(200, 200));
        arrow.Arrange(new Rect(0, 0, 200, 200));

        Assert.IsFalse(arrow.RenderedGeometry.IsEmpty(), "Стрелка с незамкнутым контуром головы должна иметь непустую геометрию");
    }

    /// <summary>Проверка что диагональная стрелка формирует корректную геометрию</summary>
    [STATestMethod]
    public void DiagonalArrow_ReturnsValidGeometry()
    {
        var arrow = new Arrow
        {
            X1 = 10,
            Y1 = 10,
            X2 = 100,
            Y2 = 100,
            ArrowHeadWidth = 10,
            ArrowHeadLength = 15
        };

        var canvas = new Canvas();
        canvas.Children.Add(arrow);
        arrow.Measure(new Size(200, 200));
        arrow.Arrange(new Rect(0, 0, 200, 200));

        var geometry = arrow.RenderedGeometry;
        Assert.IsFalse(geometry.IsEmpty(), "Диагональная стрелка должна иметь непустую геометрию");

        Debug.WriteLine($"Диагональная стрелка: Bounds = {geometry.Bounds}, тип = {geometry.GetType().Name}");
    }

    /// <summary>Проверка что стрелка направленная вправо формирует корректную геометрию</summary>
    [STATestMethod]
    public void RightDirectedArrow_ReturnsValidGeometry()
    {
        var arrow = new Arrow
        {
            X1 = 0,
            Y1 = 50,
            X2 = 100,
            Y2 = 50,
            ArrowHeadWidth = 15,
            ArrowHeadLength = 20
        };

        var canvas = new Canvas();
        canvas.Children.Add(arrow);
        arrow.Measure(new Size(200, 200));
        arrow.Arrange(new Rect(0, 0, 200, 200));

        var geometry = arrow.RenderedGeometry;
        Assert.IsFalse(geometry.IsEmpty(), "Стрелка направленная вправо должна иметь непустую геометрию");

        Debug.WriteLine($"Стрелка вправо: Bounds = {geometry.Bounds}");
    }

    /// <summary>Проверка что стрелка направленная влево формирует корректную геометрию</summary>
    [STATestMethod]
    public void LeftDirectedArrow_ReturnsValidGeometry()
    {
        var arrow = new Arrow
        {
            X1 = 100,
            Y1 = 50,
            X2 = 0,
            Y2 = 50,
            ArrowHeadWidth = 15,
            ArrowHeadLength = 20
        };

        var canvas = new Canvas();
        canvas.Children.Add(arrow);
        arrow.Measure(new Size(200, 200));
        arrow.Arrange(new Rect(0, 0, 200, 200));

        var geometry = arrow.RenderedGeometry;
        Assert.IsFalse(geometry.IsEmpty(), "Стрелка направленная влево должна иметь непустую геометрию");

        Debug.WriteLine($"Стрелка влево: Bounds = {geometry.Bounds}");
    }
}
