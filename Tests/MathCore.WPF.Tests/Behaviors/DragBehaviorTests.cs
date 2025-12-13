using System.Windows;
using System.Windows.Controls;

using MathCore.WPF.Behaviors;

namespace MathCore.WPF.Tests.Behaviors;

/// <summary>Регрессионные тесты для DragBehavior</summary>
[TestClass]
public class DragBehaviorTests
{
    /// <summary>Тест регистрации DependencyProperty с правильным типом владельца</summary>
    [TestMethod]
    public void DependencyProperties_Should_Have_Correct_OwnerType()
    {
        // Arrange & Act
        var xmin_property = DragBehavior.XminProperty;
        var xmax_property = DragBehavior.XmaxProperty;
        var ymin_property = DragBehavior.YminProperty;
        var ymax_property = DragBehavior.YmaxProperty;
        var allow_x_property = DragBehavior.AllowXProperty;
        var allow_y_property = DragBehavior.AllowYProperty;

        // Assert - проверяем, что свойства зарегистрированы
        Assert.IsNotNull(xmin_property, "XminProperty должно быть зарегистрировано");
        Assert.IsNotNull(xmax_property, "XmaxProperty должно быть зарегистрировано");
        Assert.IsNotNull(ymin_property, "YminProperty должно быть зарегистрировано");
        Assert.IsNotNull(ymax_property, "YmaxProperty должно быть зарегистрировано");
        Assert.IsNotNull(allow_x_property, "AllowXProperty должно быть зарегистрировано");
        Assert.IsNotNull(allow_y_property, "AllowYProperty должно быть зарегистрировано");

        // Assert - проверяем тип владельца
        Assert.AreEqual(typeof(DragBehavior), xmin_property.OwnerType, "XminProperty должно принадлежать DragBehavior");
        Assert.AreEqual(typeof(DragBehavior), xmax_property.OwnerType, "XmaxProperty должно принадлежать DragBehavior");
        Assert.AreEqual(typeof(DragBehavior), ymin_property.OwnerType, "YminProperty должно принадлежать DragBehavior");
        Assert.AreEqual(typeof(DragBehavior), ymax_property.OwnerType, "YmaxProperty должно принадлежать DragBehavior");
        Assert.AreEqual(typeof(DragBehavior), allow_x_property.OwnerType, "AllowXProperty должно принадлежать DragBehavior");
        Assert.AreEqual(typeof(DragBehavior), allow_y_property.OwnerType, "AllowYProperty должно принадлежать DragBehavior");
    }

    /// <summary>Тест установки и получения значений координатных свойств</summary>
    [TestMethod]
    public void CoordinateProperties_Should_SetAndGet_Values()
    {
        // Arrange
        var behavior = new DragBehavior();

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

    /// <summary>Тест установки и получения значений флагов разрешений</summary>
    [TestMethod]
    public void AllowProperties_Should_SetAndGet_Values()
    {
        // Arrange
        var behavior = new DragBehavior();

        // Act & Assert - AllowX по умолчанию true
        Assert.IsTrue(behavior.AllowX, "AllowX должно быть true по умолчанию");

        // Act & Assert - AllowY по умолчанию true
        Assert.IsTrue(behavior.AllowY, "AllowY должно быть true по умолчанию");

        // Act & Assert - установка AllowX
        behavior.AllowX = false;
        Assert.IsFalse(behavior.AllowX, "AllowX должно возвращать установленное значение");

        // Act & Assert - установка AllowY
        behavior.AllowY = false;
        Assert.IsFalse(behavior.AllowY, "AllowY должно возвращать установленное значение");
    }

    /// <summary>Тест значений по умолчанию для координатных свойств</summary>
    [TestMethod]
    public void CoordinateProperties_Should_Have_NaN_DefaultValues()
    {
        // Arrange
        var behavior = new DragBehavior();

        // Assert
        Assert.IsTrue(double.IsNaN(behavior.Xmin), "Xmin должно иметь значение NaN по умолчанию");
        Assert.IsTrue(double.IsNaN(behavior.Xmax), "Xmax должно иметь значение NaN по умолчанию");
        Assert.IsTrue(double.IsNaN(behavior.Ymin), "Ymin должно иметь значение NaN по умолчанию");
        Assert.IsTrue(double.IsNaN(behavior.Ymax), "Ymax должно иметь значение NaN по умолчанию");
    }

    /// <summary>Тест свойства Enabled по умолчанию</summary>
    [TestMethod]
    public void Enabled_Should_Be_False_ByDefault()
    {
        // Arrange
        var behavior = new DragBehavior();

        // Assert
        Assert.IsFalse(behavior.Enabled, "Enabled должно быть false по умолчанию");
    }

    /// <summary>Тест установки свойства Enabled</summary>
    [TestMethod]
    public void Enabled_Should_SetAndGet_Value()
    {
        // Arrange
        var behavior = new DragBehavior();

        // Act
        behavior.Enabled = true;

        // Assert
        Assert.IsTrue(behavior.Enabled, "Enabled должно возвращать установленное значение");
    }
}
