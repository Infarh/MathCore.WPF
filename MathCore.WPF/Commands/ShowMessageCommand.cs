using System.Windows;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Commands;

public class ShowMessageCommand(string? Title) : Command
{
    public ShowMessageCommand() : this(null) { }

    public string? Title { get; set; } = Title;

    public MessageBoxButton Buttons { get; set; } = MessageBoxButton.OK;

    public MessageBoxImage Image { get; set; } = MessageBoxImage.Information;

    public MessageBoxResult DefaultResult { get; set; } = MessageBoxResult.OK;

    public MessageBoxOptions Options { get; set; } = MessageBoxOptions.None;

    #region Result : MessageBoxResult - Результат выполнения команды

    /// <summary>Результат выполнения команды</summary>
    private MessageBoxResult _Result;

    /// <summary>Результат выполнения команды</summary>
    public MessageBoxResult Result
    {
        get => _Result;
        set => Set(ref _Result, value);
    }

    #endregion

    public override bool CanExecute(object? parameter) => !string.IsNullOrEmpty(parameter as string);

    public override void Execute(object? parameter) => Result = MessageBox.Show((string)parameter!, Title ?? "Message", Buttons, Image, DefaultResult, Options);
}