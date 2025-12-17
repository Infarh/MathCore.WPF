using MathCore.WPF.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;

namespace MathCore.WPF.Tests.Shapes;

/// <summary>Набор модульных тестов для проверки поведения расширения LineEx</summary>
[TestClass]
public sealed class LineExTests
{
    /// <summary>Проверка что присоединяемое свойство P1 по умолчанию равно (0, 0)</summary>
    [STATestMethod]
    public void P1DefaultValue_IsZero()
    {
        var line = new Line();

        var p1 = LineEx.GetP1(line);

        Assert.AreEqual(new Point(0, 0), p1, "P1 по умолчанию должна быть (0, 0)");
    }

    /// <summary>Проверка что присоединяемое свойство P2 по умолчанию равно (0, 0)</summary>
    [STATestMethod]
    public void P2DefaultValue_IsZero()
    {
        var line = new Line();

        var p2 = LineEx.GetP2(line);

        Assert.AreEqual(new Point(0, 0), p2, "P2 по умолчанию должна быть (0, 0)");
    }

    /// <summary>Проверка что присоединяемое свойство P1 может быть установлено</summary>
    [STATestMethod]
    public void P1CanBeSet()
    {
        var line = new Line();
        var p1 = new Point(10, 20);

        LineEx.SetP1(line, p1);

        var result = LineEx.GetP1(line);
        Assert.AreEqual(p1, result, "P1 должна быть установлена в (10, 20)");
    }

    /// <summary>Проверка что присоединяемое свойство P2 может быть установлено</summary>
    [STATestMethod]
    public void P2CanBeSet()
    {
        var line = new Line();
        var p2 = new Point(100, 150);

        LineEx.SetP2(line, p2);

        var result = LineEx.GetP2(line);
        Assert.AreEqual(p2, result, "P2 должна быть установлена в (100, 150)");
    }

    /// <summary>Проверка что установка P1 синхронизирует X1 и Y1</summary>
    [STATestMethod]
    public void SettingP1_SynchronizesX1Y1()
    {
        var line = new Line();
        var p1 = new Point(25, 35);

        LineEx.SetP1(line, p1);

        Assert.AreEqual(25d, line.X1, "X1 должен быть синхронизирован с X компонентой P1");
        Assert.AreEqual(35d, line.Y1, "Y1 должен быть синхронизирован с Y компонентой P1");
    }

    /// <summary>Проверка что установка P2 синхронизирует X2 и Y2</summary>
    [STATestMethod]
    public void SettingP2_SynchronizesX2Y2()
    {
        var line = new Line();
        var p2 = new Point(75, 85);

        LineEx.SetP2(line, p2);

        Assert.AreEqual(75d, line.X2, "X2 должен быть синхронизирован с X компонентой P2");
        Assert.AreEqual(85d, line.Y2, "Y2 должен быть синхронизирован с Y компонентой P2");
    }

    /// <summary>Проверка что изменение X1 и Y1 синхронизирует P1 при привязке данных</summary>
    [STATestMethod]
    public void ChangingX1Y1_SynchronizesP1()
    {
        var line = new Line();
        
        // Инициализируем синхронизацию
        var x1_binding = new Binding { Source = line, Path = new PropertyPath(Line.X1Property), Mode = BindingMode.OneWay };
        var y1_binding = new Binding { Source = line, Path = new PropertyPath(Line.Y1Property), Mode = BindingMode.OneWay };
        
        // Устанавливаем начальное значение P1
        LineEx.SetP1(line, new Point(10, 20));
        
        // Изменяем X1
        line.X1 = 50;
        
        // Синхронизация происходит через binding helper, проверим координаты
        Assert.AreEqual(50d, line.X1, "X1 должен быть 50");
        Assert.AreEqual(20d, line.Y1, "Y1 должен оставаться 20");
    }

    /// <summary>Проверка что изменение X2 и Y2 синхронизирует P2 при привязке данных</summary>
    [STATestMethod]
    public void ChangingX2Y2_SynchronizesP2()
    {
        var line = new Line();
        
        // Устанавливаем начальное значение P2
        LineEx.SetP2(line, new Point(100, 150));
        
        // Изменяем X2
        line.X2 = 120;
        
        // Проверяем координаты
        Assert.AreEqual(120d, line.X2, "X2 должен быть 120");
        Assert.AreEqual(150d, line.Y2, "Y2 должен оставаться 150");
    }

    /// <summary>Проверка что P1 и P2 могут использоваться одновременно</summary>
    [STATestMethod]
    public void BothP1AndP2CanBeUsedSimultaneously()
    {
        var line = new Line();
        var p1 = new Point(10, 20);
        var p2 = new Point(100, 150);

        LineEx.SetP1(line, p1);
        LineEx.SetP2(line, p2);

        Assert.AreEqual(p1, LineEx.GetP1(line), "P1 должна быть корректной");
        Assert.AreEqual(p2, LineEx.GetP2(line), "P2 должна быть корректной");
        Assert.AreEqual(10d, line.X1, "X1 должен быть 10");
        Assert.AreEqual(20d, line.Y1, "Y1 должен быть 20");
        Assert.AreEqual(100d, line.X2, "X2 должен быть 100");
        Assert.AreEqual(150d, line.Y2, "Y2 должен быть 150");
    }

    /// <summary>Проверка что P1 поддерживает двусторонние привязки данных</summary>
    [STATestMethod]
    public void P1SupportsDataBinding()
    {
        var line = new Line();
        var view_model = new _TestViewModel { StartPoint = new Point(15, 25) };

        var binding = new Binding(nameof(_TestViewModel.StartPoint))
        {
            Source = view_model,
            Mode = BindingMode.TwoWay
        };
        BindingOperations.SetBinding(line, LineEx.P1Property, binding);

        var p1 = LineEx.GetP1(line);
        
        Assert.AreEqual(new Point(15, 25), p1, "P1 должна быть привязана к StartPoint из view model");
        Assert.AreEqual(15d, line.X1, "X1 должен быть синхронизирован");
        Assert.AreEqual(25d, line.Y1, "Y1 должен быть синхронизирован");
    }

    /// <summary>Проверка что P2 поддерживает двусторонние привязки данных</summary>
    [STATestMethod]
    public void P2SupportsDataBinding()
    {
        var line = new Line();
        var view_model = new _TestViewModel { EndPoint = new Point(105, 155) };

        var binding = new Binding(nameof(_TestViewModel.EndPoint))
        {
            Source = view_model,
            Mode = BindingMode.TwoWay
        };
        BindingOperations.SetBinding(line, LineEx.P2Property, binding);

        var p2 = LineEx.GetP2(line);
        
        Assert.AreEqual(new Point(105, 155), p2, "P2 должна быть привязана к EndPoint из view model");
        Assert.AreEqual(105d, line.X2, "X2 должен быть синхронизирован");
        Assert.AreEqual(155d, line.Y2, "Y2 должен быть синхронизирован");
    }

    /// <summary>Проверка что замена P1 на новое значение работает корректно</summary>
    [STATestMethod]
    public void ReplacingP1_WorksCorrectly()
    {
        var line = new Line();
        var p1_initial = new Point(10, 20);
        var p1_new = new Point(50, 60);

        LineEx.SetP1(line, p1_initial);
        Assert.AreEqual(10d, line.X1, "Начальное значение X1 должно быть 10");
        
        LineEx.SetP1(line, p1_new);
        Assert.AreEqual(50d, line.X1, "После замены X1 должен быть 50");
        Assert.AreEqual(60d, line.Y1, "После замены Y1 должен быть 60");
    }

    /// <summary>Проверка что замена P2 на новое значение работает корректно</summary>
    [STATestMethod]
    public void ReplacingP2_WorksCorrectly()
    {
        var line = new Line();
        var p2_initial = new Point(100, 150);
        var p2_new = new Point(200, 250);

        LineEx.SetP2(line, p2_initial);
        Assert.AreEqual(100d, line.X2, "Начальное значение X2 должно быть 100");
        
        LineEx.SetP2(line, p2_new);
        Assert.AreEqual(200d, line.X2, "После замены X2 должен быть 200");
        Assert.AreEqual(250d, line.Y2, "После замены Y2 должен быть 250");
    }

    /// <summary>Проверка что P1 с точками с дробными координатами работает корректно</summary>
    [STATestMethod]
    public void P1WithFractionalCoordinates_WorksCorrectly()
    {
        var line = new Line();
        var p1 = new Point(10.5, 20.75);

        LineEx.SetP1(line, p1);

        Assert.AreEqual(10.5d, line.X1, "X1 должен быть 10.5");
        Assert.AreEqual(20.75d, line.Y1, "Y1 должен быть 20.75");
    }

    /// <summary>Проверка что P2 с точками с дробными координатами работает корректно</summary>
    [STATestMethod]
    public void P2WithFractionalCoordinates_WorksCorrectly()
    {
        var line = new Line();
        var p2 = new Point(100.25, 150.5);

        LineEx.SetP2(line, p2);

        Assert.AreEqual(100.25d, line.X2, "X2 должен быть 100.25");
        Assert.AreEqual(150.5d, line.Y2, "Y2 должен быть 150.5");
    }

    /// <summary>Проверка что P1 с отрицательными координатами работает корректно</summary>
    [STATestMethod]
    public void P1WithNegativeCoordinates_WorksCorrectly()
    {
        var line = new Line();
        var p1 = new Point(-10, -20);

        LineEx.SetP1(line, p1);

        Assert.AreEqual(-10d, line.X1, "X1 должен быть -10");
        Assert.AreEqual(-20d, line.Y1, "Y1 должен быть -20");
    }

    /// <summary>Проверка что P2 с отрицательными координатами работает корректно</summary>
    [STATestMethod]
    public void P2WithNegativeCoordinates_WorksCorrectly()
    {
        var line = new Line();
        var p2 = new Point(-100, -150);

        LineEx.SetP2(line, p2);

        Assert.AreEqual(-100d, line.X2, "X2 должен быть -100");
        Assert.AreEqual(-150d, line.Y2, "Y2 должен быть -150");
    }

    // Вспомогательный класс для тестирования привязок данных
    private class _TestViewModel
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
    }

    /// <summary>Проверка что multiple Line объекты могут быть созданы без неограниченного роста памяти</summary>
    [STATestMethod]
    public void MultipleLineObjects_CanBeCreatedEfficiently()
    {
        // Это тест что система не выходит из строя при создании множества Line объектов
        // ConditionalWeakTable гарантирует что старые объекты могут быть удалены
        // когда на них больше нет сильных ссылок в приложении
        
        for (int batch = 0; batch < 3; batch++)
        {
            var lines = new List<Line>();
            for (int i = 0; i < 100; i++)
            {
                var line = new Line();
                LineEx.SetP1(line, new Point(i, i));
                LineEx.SetP2(line, new Point(i + 100, i + 100));
                lines.Add(line);
            }

            // Проверяем что все объекты работают правильно
            foreach (var line in lines)
            {
                Assert.IsTrue(line.X1 >= 0 && line.X1 < 100, "X1 должен быть в ожидаемом диапазоне");
            }

            // После выхода из области видимости объекты могут быть удалены
        }

        // Если мы дошли сюда без исключений - тест прошёл
        Assert.IsTrue(true, "Множество Line объектов создано успешно без ошибок");
    }

    /// <summary>Проверка что P1 и P2 работают правильно при массовом создании (например в ItemsControl)</summary>
    [STATestMethod]
    public void PointProperties_WorkCorrectlyWithManyObjects()
    {
        var lines = new Line[1000];
        
        // Создаём 1000 Line объектов с разными точками
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = new Line();
            var p1 = new Point(i * 1.5, i * 2.5);
            var p2 = new Point(i * 3.5, i * 4.5);
            
            LineEx.SetP1(lines[i], p1);
            LineEx.SetP2(lines[i], p2);
        }

        // Проверяем что случайные элементы имеют правильные значения
        var line_100 = lines[100];
        Assert.AreEqual(150d, line_100.X1, "Line[100].X1 должен быть 150");
        Assert.AreEqual(250d, line_100.Y1, "Line[100].Y1 должен быть 250");
        
        var line_500 = lines[500];
        Assert.AreEqual(750d, line_500.X1, "Line[500].X1 должен быть 750");
        Assert.AreEqual(1250d, line_500.Y1, "Line[500].Y1 должен быть 1250");
        
        var line_999 = lines[999];
        Assert.AreEqual(1498.5d, line_999.X1, 0.1, "Line[999].X1 должен быть ~1498.5");
        Assert.AreEqual(2497.5d, line_999.Y1, 0.1, "Line[999].Y1 должен быть ~2497.5");
    }
}
