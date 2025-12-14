using System.Windows;
using System.Windows.Controls;

using MathCore.WPF.Behaviors;

namespace MathCore.WPF.Tests.Behaviors;

/// <summary>Регрессионные тесты для DragInCanvasBehavior</summary>
[TestClass]
public class DragInCanvasBehaviorTests
{
    /// <summary>Тест регистрации DependencyProperty</summary>
    [TestMethod]
    public void DependencyProperties_Should_Be_Registered()
    {
        // Arrange & Act
        var xmin_property = DragInCanvasBehavior.XminProperty;
        var xmax_property = DragInCanvasBehavior.XmaxProperty;
        var ymin_property = DragInCanvasBehavior.YminProperty;
        var ymax_property = DragInCanvasBehavior.YmaxProperty;
        var allow_x_property = DragInCanvasBehavior.AllowXProperty;
        var allow_y_property = DragInCanvasBehavior.AllowYProperty;
        var current_x_property = DragInCanvasBehavior.CurrentXProperty;
        var current_y_property = DragInCanvasBehavior.CurrentYProperty;
        var enabled_property = DragInCanvasBehavior.EnabledProperty;

        // Assert
        Assert.IsNotNull(xmin_property, "XminProperty должно быть зарегистрировано");
        Assert.IsNotNull(xmax_property, "XmaxProperty должно быть зарегистрировано");
        Assert.IsNotNull(ymin_property, "YminProperty должно быть зарегистрировано");
        Assert.IsNotNull(ymax_property, "YmaxProperty должно быть зарегистрировано");
        Assert.IsNotNull(allow_x_property, "AllowXProperty должно быть зарегистрировано");
        Assert.IsNotNull(allow_y_property, "AllowYProperty должно быть зарегистрировано");
        Assert.IsNotNull(current_x_property, "CurrentXProperty должно быть зарегистрировано");
        Assert.IsNotNull(current_y_property, "CurrentYProperty должно быть зарегистрировано");
        Assert.IsNotNull(enabled_property, "EnabledProperty должно быть зарегистрировано");
    }

    /// <summary>Тест установки и получения координатных свойств</summary>
    [TestMethod]
    public void CoordinateProperties_Should_SetAndGet_Values()
    {
        // Arrange
        var behavior = new DragInCanvasBehavior();

        // Act & Assert - Xmin
        behavior.Xmin = 10.0;
        Assert.AreEqual(10.0, behavior.Xmin, "Xmin должно возвращать установленное значение");

        // Act & Assert - Xmax
        behavior.Xmax = 100.0;
        Assert.AreEqual(100.0, behavior.Xmax, "Xmax должно возвращать установленное значение");

        // Act & Assert - Ymin
        behavior.Ymin = 20.0;
        Assert.AreEqual(20.0, behavior.Ymin, "Ymin должно возвращать установленное значение");

        // Act & Assert - Ymax
        behavior.Ymax = 200.0;
        Assert.AreEqual(200.0, behavior.Ymax, "Ymax должно возвращать установленное значение");
    }

    /// <summary>Тест флагов разрешений перемещения</summary>
    [TestMethod]
    public void AllowProperties_Should_SetAndGet_Values()
    {
        // Arrange
        var behavior = new DragInCanvasBehavior();

        // Assert - значения по умолчанию
        Assert.IsTrue(behavior.AllowX, "AllowX должно быть true по умолчанию");
        Assert.IsTrue(behavior.AllowY, "AllowY должно быть true по умолчанию");

        // Act & Assert - установка AllowX
        behavior.AllowX = false;
        Assert.IsFalse(behavior.AllowX, "AllowX должно возвращать установленное значение");

        // Act & Assert - установка AllowY
        behavior.AllowY = false;
        Assert.IsFalse(behavior.AllowY, "AllowY должно возвращать установленное значение");
    }

    /// <summary>Тест свойства Enabled</summary>
    [TestMethod]
    public void Enabled_Should_SetAndGet_Value()
    {
        // Arrange
        var behavior = new DragInCanvasBehavior();

        // Assert - значение по умолчанию
        Assert.IsTrue(behavior.Enabled, "Enabled должно быть true по умолчанию");

        // Act
        behavior.Enabled = false;

        // Assert
        Assert.IsFalse(behavior.Enabled, "Enabled должно возвращать установленное значение");
    }

    /// <summary>Тест значений по умолчанию для координатных ограничений</summary>
    [TestMethod]
    public void CoordinateConstraints_Should_Have_NaN_DefaultValues()
    {
        // Arrange
        var behavior = new DragInCanvasBehavior();

        // Assert
        Assert.IsTrue(double.IsNaN(behavior.Xmin), "Xmin должно иметь значение NaN по умолчанию");
        Assert.IsTrue(double.IsNaN(behavior.Xmax), "Xmax должно иметь значение NaN по умолчанию");
        Assert.IsTrue(double.IsNaN(behavior.Ymin), "Ymin должно иметь значение NaN по умолчанию");
        Assert.IsTrue(double.IsNaN(behavior.Ymax), "Ymax должно иметь значение NaN по умолчанию");
    }
}
