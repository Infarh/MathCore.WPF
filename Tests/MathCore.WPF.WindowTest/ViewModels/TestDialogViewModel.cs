using System.Windows;
using System.Windows.Input;

using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

internal class TestDialogViewModel : DialogViewModel
{
    public TestDialogViewModel() => Title = $"Тестовый диалог с пользователем {DateTime.Now}";

    #region Command ShowMessageCommand - Показать сообщение

    /// <summary>Показать сообщение</summary>
    private Command? _ShowMessageCommand;

    /// <summary>Показать сообщение</summary>
    public ICommand ShowMessageCommand => _ShowMessageCommand ??=
        Command.New<string>(
            msg => MessageBox.Show(msg),
            msg => msg is { Length: > 0 });

    #endregion
}