using System.Windows;
using System.Windows.Controls;

using MathCore.WPF.Behaviors;

namespace MathCore.WPF.Tests.Behaviors;

/// <summary>Регрессионные тесты для Resize</summary>
[TestClass]
public class ResizeTests
{
    /// <summary>Тест регистрации DependencyProperty</summary>
    [TestMethod]
    public void DependencyProperties_Should_Be_Registered()
    {
        // Arrange & Act
        var area_size_property = Resize.AreaSizeProperty;
        var top_resizing_property = Resize.TopResizingProperty;
        var bottom_resizing_property = Resize.BottomResizingProperty;
        var left_resizing_property = Resize.LeftResizingProperty;
        var right_resizing_property = Resize.RightResizingProperty;

        // Assert
        Assert.IsNotNull(area_size_property, "AreaSizeProperty должно быть зарегистрировано");
        Assert.IsNotNull(top_resizing_property, "TopResizingProperty должно быть зарегистрировано");
        Assert.IsNotNull(bottom_resizing_property, "BottomResizingProperty должно быть зарегистрировано");
        Assert.IsNotNull(left_resizing_property, "LeftResizingProperty должно быть зарегистрировано");
        Assert.IsNotNull(right_resizing_property, "RightResizingProperty должно быть зарегистрировано");
    }

    /// <summary>Тест значения по умолчанию для AreaSize</summary>
    [TestMethod]
    public void AreaSize_Should_Have_Default_Value_Of_3()
    {
        // Arrange
        var behavior = new Resize();

        // Assert
        Assert.AreEqual(3.0, behavior.AreaSize, "AreaSize должно иметь значение 3.0 по умолчанию");
    }

    /// <summary>Тест установки и получения AreaSize</summary>
    [TestMethod]
    public void AreaSize_Should_SetAndGet_Value()
    {
        // Arrange
        var behavior = new Resize();

        // Act
        behavior.AreaSize = 5.0;

        // Assert
        Assert.AreEqual(5.0, behavior.AreaSize, "AreaSize должно возвращать установленное значение");
    }

    /// <summary>Тест значений по умолчанию для флагов изменения размера</summary>
    [TestMethod]
    public void ResizingFlags_Should_Be_False_ByDefault()
    {
        // Arrange
        var behavior = new Resize();

        // Assert
        Assert.IsFalse(behavior.TopResizing, "TopResizing должно быть false по умолчанию");
        Assert.IsFalse(behavior.BottomResizing, "BottomResizing должно быть false по умолчанию");
        Assert.IsFalse(behavior.LeftResizing, "LeftResizing должно быть false по умолчанию");
        Assert.IsFalse(behavior.RightResizing, "RightResizing должно быть false по умолчанию");
    }

    /// <summary>Тест установки и получения флагов изменения размера</summary>
    [TestMethod]
    public void ResizingFlags_Should_SetAndGet_Values()
    {
        // Arrange
        var behavior = new Resize();

        // Act
        behavior.TopResizing = true;
        behavior.BottomResizing = true;
        behavior.LeftResizing = true;
        behavior.RightResizing = true;

        // Assert
        Assert.IsTrue(behavior.TopResizing, "TopResizing должно возвращать установленное значение");
        Assert.IsTrue(behavior.BottomResizing, "BottomResizing должно возвращать установленное значение");
        Assert.IsTrue(behavior.LeftResizing, "LeftResizing должно возвращать установленное значение");
        Assert.IsTrue(behavior.RightResizing, "RightResizing должно возвращать установленное значение");
    }

    /// <summary>Тест независимости флагов изменения размера</summary>
    [TestMethod]
    public void ResizingFlags_Should_Be_Independent()
    {
        // Arrange
        var behavior = new Resize();

        // Act - устанавливаем только TopResizing
        behavior.TopResizing = true;

        // Assert - остальные должны остаться false
        Assert.IsTrue(behavior.TopResizing, "TopResizing должно быть true");
        Assert.IsFalse(behavior.BottomResizing, "BottomResizing должно остаться false");
        Assert.IsFalse(behavior.LeftResizing, "LeftResizing должно остаться false");
        Assert.IsFalse(behavior.RightResizing, "RightResizing должно остаться false");

        // Act - устанавливаем только LeftResizing
        behavior.LeftResizing = true;

        // Assert
        Assert.IsTrue(behavior.TopResizing, "TopResizing должно сохранить значение true");
        Assert.IsTrue(behavior.LeftResizing, "LeftResizing должно быть true");
        Assert.IsFalse(behavior.BottomResizing, "BottomResizing должно остаться false");
        Assert.IsFalse(behavior.RightResizing, "RightResizing должно остаться false");
    }

    /// <summary>Тест, что AreaSize может принимать различные значения</summary>
    [TestMethod]
    public void AreaSize_Should_Accept_Various_Values()
    {
        // Arrange
        var behavior = new Resize();
        var test_values = new[] { 0.5, 1.0, 5.0, 10.0, 20.0 };

        foreach (var value in test_values)
        {
            // Act
            behavior.AreaSize = value;

            // Assert
            Assert.AreEqual(value, behavior.AreaSize, $"AreaSize должно возвращать установленное значение {value}");
        }
    }

    /// <summary>Тест типа владельца для DependencyProperty</summary>
    [TestMethod]
    public void DependencyProperties_Should_Have_Correct_OwnerType()
    {
        // Arrange & Act
        var area_size_property = Resize.AreaSizeProperty;
        var top_resizing_property = Resize.TopResizingProperty;
        var bottom_resizing_property = Resize.BottomResizingProperty;
        var left_resizing_property = Resize.LeftResizingProperty;
        var right_resizing_property = Resize.RightResizingProperty;

        // Assert
        Assert.AreEqual(typeof(Resize), area_size_property.OwnerType, "AreaSizeProperty должно принадлежать Resize");
        Assert.AreEqual(typeof(Resize), top_resizing_property.OwnerType, "TopResizingProperty должно принадлежать Resize");
        Assert.AreEqual(typeof(Resize), bottom_resizing_property.OwnerType, "BottomResizingProperty должно принадлежать Resize");
        Assert.AreEqual(typeof(Resize), left_resizing_property.OwnerType, "LeftResizingProperty должно принадлежать Resize");
        Assert.AreEqual(typeof(Resize), right_resizing_property.OwnerType, "RightResizingProperty должно принадлежать Resize");
    }
}
