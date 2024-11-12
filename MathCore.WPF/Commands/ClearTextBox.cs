using System.Windows.Controls;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Commands;

/// <summary>Команда очистки текстового поля.</summary>
public class ClearTextBox : Command
{
    /// <summary>Определяет, может ли команда быть выполнена.</summary>
    /// <param name="parameter">Параметр команды.</param>
    /// <returns>True, если команда может быть выполнена, иначе False.</returns>
    public override bool CanExecute(object? parameter)
        // Команда может быть выполнена, если параметр является текстовым полем с ненулевой длиной текста.
        => parameter is TextBox { Text.Length: > 0 };

    /// <summary>Выполняет команду.</summary>
    /// <param name="parameter">Параметр команды.</param>
    public override void Execute(object? parameter)
        // Если параметр является текстовым полем, очищает его.
        => (parameter as TextBox)?.Clear();
}