using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using MathCore.WPF.Behaviors;

namespace MathCore.WPF.Tests.Behaviors;

/// <summary>Регрессионные тесты для UserInputBehavior</summary>
[TestClass]
public class UserInputBehaviorTests
{
    /// <summary>Простая команда для тестирования</summary>
    private class TestCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public int ExecuteCount { get; private set; }
        public object? LastParameter { get; private set; }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            ExecuteCount++;
            LastParameter = parameter;
        }
    }

    /// <summary>Тест регистрации DependencyProperty для команд</summary>
    [TestMethod]
    public void CommandProperties_Should_Be_Registered()
    {
        // Arrange & Act
        var left_down_property = UserInputBehavior.LeftMouseDownCommandProperty;
        var left_up_property = UserInputBehavior.LeftMouseUpCommandProperty;
        var wheel_property = UserInputBehavior.MouseWheelCommandProperty;
        var key_down_property = UserInputBehavior.KeyDownCommandProperty;
        var key_up_property = UserInputBehavior.KeyUpCommandProperty;

        // Assert
        Assert.IsNotNull(left_down_property, "LeftMouseDownCommandProperty должно быть зарегистрировано");
        Assert.IsNotNull(left_up_property, "LeftMouseUpCommandProperty должно быть зарегистрировано");
        Assert.IsNotNull(wheel_property, "MouseWheelCommandProperty должно быть зарегистрировано");
        Assert.IsNotNull(key_down_property, "KeyDownCommandProperty должно быть зарегистрировано");
        Assert.IsNotNull(key_up_property, "KeyUpCommandProperty должно быть зарегистрировано");
    }

    /// <summary>Тест установки и получения команд</summary>
    [TestMethod]
    public void CommandProperties_Should_SetAndGet_Values()
    {
        // Arrange
        var behavior = new UserInputBehavior();
        var left_down_command = new TestCommand();
        var left_up_command = new TestCommand();
        var wheel_command = new TestCommand();
        var key_down_command = new TestCommand();
        var key_up_command = new TestCommand();

        // Act
        behavior.LeftMouseDownCommand = left_down_command;
        behavior.LeftMouseUpCommand = left_up_command;
        behavior.MouseWheelCommand = wheel_command;
        behavior.KeyDownCommand = key_down_command;
        behavior.KeyUpCommand = key_up_command;

        // Assert
        Assert.AreSame(left_down_command, behavior.LeftMouseDownCommand, "LeftMouseDownCommand должна возвращать установленное значение");
        Assert.AreSame(left_up_command, behavior.LeftMouseUpCommand, "LeftMouseUpCommand должна возвращать установленное значение");
        Assert.AreSame(wheel_command, behavior.MouseWheelCommand, "MouseWheelCommand должна возвращать установленное значение");
        Assert.AreSame(key_down_command, behavior.KeyDownCommand, "KeyDownCommand должна возвращать установленное значение");
        Assert.AreSame(key_up_command, behavior.KeyUpCommand, "KeyUpCommand должна возвращать установленное значение");
    }

    /// <summary>Тест, что KeyDownCommand и KeyUpCommand независимы от MouseWheelCommand</summary>
    [TestMethod]
    public void KeyCommands_Should_Be_Independent_From_MouseWheelCommand()
    {
        // Arrange
        var behavior = new UserInputBehavior();
        var wheel_command = new TestCommand();
        var key_down_command = new TestCommand();
        var key_up_command = new TestCommand();

        // Act - устанавливаем разные команды
        behavior.MouseWheelCommand = wheel_command;
        behavior.KeyDownCommand = key_down_command;
        behavior.KeyUpCommand = key_up_command;

        // Assert - проверяем, что команды не перекрываются
        Assert.AreSame(wheel_command, behavior.MouseWheelCommand, "MouseWheelCommand должна сохранить своё значение");
        Assert.AreSame(key_down_command, behavior.KeyDownCommand, "KeyDownCommand должна иметь своё значение");
        Assert.AreSame(key_up_command, behavior.KeyUpCommand, "KeyUpCommand должна иметь своё значение");
        
        Assert.AreNotSame(behavior.KeyDownCommand, behavior.MouseWheelCommand, "KeyDownCommand не должна быть той же, что MouseWheelCommand");
        Assert.AreNotSame(behavior.KeyUpCommand, behavior.MouseWheelCommand, "KeyUpCommand не должна быть той же, что MouseWheelCommand");
    }

    /// <summary>Тест значений по умолчанию для команд</summary>
    [TestMethod]
    public void CommandProperties_Should_Be_Null_ByDefault()
    {
        // Arrange
        var behavior = new UserInputBehavior();

        // Assert
        Assert.IsNull(behavior.LeftMouseDownCommand, "LeftMouseDownCommand должна быть null по умолчанию");
        Assert.IsNull(behavior.LeftMouseUpCommand, "LeftMouseUpCommand должна быть null по умолчанию");
        Assert.IsNull(behavior.MouseWheelCommand, "MouseWheelCommand должна быть null по умолчанию");
        Assert.IsNull(behavior.KeyDownCommand, "KeyDownCommand должна быть null по умолчанию");
        Assert.IsNull(behavior.KeyUpCommand, "KeyUpCommand должна быть null по умолчанию");
    }

    /// <summary>Тест регистрации свойства Position</summary>
    [TestMethod]
    public void Position_Property_Should_SetAndGet_Values()
    {
        // Arrange
        var behavior = new UserInputBehavior();
        var position = new Point(100, 200);

        // Act
        behavior.Position = position;

        // Assert
        Assert.AreEqual(position, behavior.Position, "Position должна возвращать установленное значение");
    }

    /// <summary>Тест значения по умолчанию для Position</summary>
    [TestMethod]
    public void Position_Should_Be_Zero_ByDefault()
    {
        // Arrange
        var behavior = new UserInputBehavior();

        // Assert
        Assert.AreEqual(new Point(0, 0), behavior.Position, "Position должна иметь значение (0,0) по умолчанию");
    }
}
