// ReSharper disable UnusedType.Global
namespace MathCore.WPF.Commands;

/// <summary>Команда отмены диалога.</summary>
public class CancelDialog : DialogCommand
{
    /// <summary>Выполняет команду отмены диалога.</summary>
    /// <param name="parameter">Параметр команды (не используется).</param>
    public override void Execute(object? parameter) 
        // Если параметр не передан, то по умолчанию используется значение false.
        => base.Execute(parameter ?? false);
}