using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

using MathCore.WPF.Attributes;
using MathCore.WPF.Commands;
using MathCore.WPF.Services;
using MathCore.WPF.UIEvents;
using MathCore.WPF.ViewModels;
using MathCore.WPF.WindowTest.Services.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace MathCore.WPF.WindowTest.ViewModels;

[MarkupExtensionReturnType(typeof(MainWindowViewModel))]
internal partial class MainWindowViewModel : ViewModel
{
    #region Title : string - Заголовок

    /// <summary>Заголовок</summary>
    private string _Title = "Заголовок!";

    /// <summary>Заголовок</summary>
    public string Title { get => _Title; set => Set(ref _Title, value); }

    #endregion

    #region Status : string - Статус

    /// <summary>Статус</summary>
    private string _Status = "Ready!";

    /// <summary>Статус</summary>
    public string Status
    {
        get => _Status;
        set => SetValue(ref _Status, value).Then(StatusChangedEvent.Invoke);
    }

    #endregion

    private Command? _OtherCommand;

    public ICommand OtherCommand => _OtherCommand ??= LambdaCommand
       .OnExecute(p => MessageBox.Show(p.ToString()), p => p is not null)
       .OnError(e => MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error))
       .TraceErrors()
       .SkipErrors();

    private void OtherViewModelAction()
    {
        // Какая-то другая логика переключает возможность выполнения команды
        _OtherCommand!.IsCanExecute = _OtherCommand.IsCanExecute;
    }


    private LambdaCommandAsync? _ReadFileCommand;

    public ICommand ReadFileCommand => _ReadFileCommand ??=
        new LambdaCommandAsync(OnReadCommandExecutedAsync, CanReadFileCommandExecute);

    private static bool CanReadFileCommandExecute() => true;

    private static async Task OnReadCommandExecutedAsync()
    {
        var dialog = new OpenFileDialog
        {
            Title  = "Выбор файла",
            Filter = "Текст (*.txt)|*.txt|Все файлы (*.*)|*.*"
        };

        if (dialog.ShowDialog() != true)
            return;

        using var reader = new StreamReader(dialog.OpenFile());
        var       text   = await reader.ReadToEndAsync();

        MessageBox.Show(text);
    }


    #region Command TestCommand - Тестовая команда

    private ICommand? _TestCommand;

    /// <summary>Тестовая команда</summary>
    public ICommand TestCommand => _TestCommand ??= Command.New(OnTestCommandExecuted, CanTestCommandExecute);

    /// <summary>Проверка возможности выполнения - Тестовая команда</summary>
    private static bool CanTestCommandExecute(object? p) => true;

    /// <summary>Логика выполнения - Тестовая команда</summary>
    private Task OnTestCommandExecuted(object? p)
    {
        Status = $"Test {DateTime.Now:hh:mm:ss.ffff}";
        return Task.CompletedTask;
    }

    #endregion

    #region Command ObjectCommandCommand - Summary

    /// <summary>Summary</summary>
    private LambdaCommand? _ObjectCommandCommand;

    /// <summary>Summary</summary>
    public ICommand ObjectCommandCommand => _ObjectCommandCommand ??= (OnObjectCommandCommandExecuted, CanObjectCommandCommandExecute);

    /// <summary>Проверка возможности выполнения - Summary</summary>
    private bool CanObjectCommandCommandExecute(object? p) => true;

    /// <summary>Логика выполнения - Summary</summary>
    private void OnObjectCommandCommandExecuted(object? p)
    {

    }

    #endregion

    #region Command StringCommand : string - Summary

    /// <summary>Summary</summary>
    private LambdaCommand<string>? _StringCommand;

    /// <summary>Summary</summary>
    public ICommand StringCommand => _StringCommand ??= (OnStringCommandExecuted, CanStringCommandExecute);

    /// <summary>Проверка возможности выполнения - Summary</summary>
    private bool CanStringCommandExecute(string p) => true;

    /// <summary>Проверка возможности выполнения - Summary</summary>
    private void OnStringCommandExecuted(string p)
    {

    }

    #endregion

    private IProgressInfo? _Progress;

    #region Command ProgressShowCommand - Показать прогресс

    /// <summary>Показать прогресс</summary>
    private LambdaCommand? _ProgressShowCommand;

    /// <summary>Показать прогресс</summary>
    public ICommand ProgressShowCommand => _ProgressShowCommand
        ??= new(OnProgressShowCommandExecuted);

    /// <summary>Логика выполнения - Показать прогресс</summary>
    private void OnProgressShowCommandExecuted()
    {
        if (_Progress is null)
        {
            var user_dialog = App.Services.GetRequiredService<IUserDialog>();

            _Progress = user_dialog.Progress("Прогресс", "Статус");
            Task.Delay(5000).ContinueWith(_ => OnProgressShowCommandExecuted());
        }
        else
        {
            _Progress.Dispose();
            _Progress = null;
        }
    }

    #endregion
    public IModelEvent StatusChangedEvent { get; }

    [Command]
    private void OnGeneratedCommandExecuted(object? p)
    {
        Debug.WriteLine("Generated command executed");
        MessageBox.Show("Generated command executed");
    }

    public MainWindowViewModel() => StatusChangedEvent = new ModelEvent(this);
}