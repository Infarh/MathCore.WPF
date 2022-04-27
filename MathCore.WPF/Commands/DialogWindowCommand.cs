using System.Windows;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.Commands;

public class DialogWindowCommand : Command
{
    private Window? _Window;

    private Func<Window> _WindowFactory;

    public DialogWindowCommand Window(Window window)
    {
        if (window is null) throw new ArgumentNullException(nameof(window));

        _ViewModelFactory = () => window;
        return this;
    }

    public DialogWindowCommand Window(Func<Window> WindowFactory)
    {
        _WindowFactory = WindowFactory ?? throw new ArgumentNullException(nameof(WindowFactory));
        return this;
    }

    private Func<object?> _ViewModelFactory;

    public DialogWindowCommand ViewModel(object? ViewModel)
    {
        _ViewModelFactory = () => ViewModel;
        return this;
    }

    public DialogWindowCommand ViewModel(Func<object?> ViewModelFactory)
    {
        _ViewModelFactory = ViewModelFactory ?? throw new ArgumentNullException(nameof(ViewModelFactory));
        return this;
    }

    private event EventHandler<EventArgs<bool?>>? DialogCompleted; 

    public DialogWindowCommand OnDialogCompleted(EventHandler<EventArgs<bool?>> Handler)
    {
        DialogCompleted += Handler ?? throw new ArgumentNullException(nameof(Handler));
        return this;
    }

    public DialogWindowCommand OnDialogCompleted(Action<bool?> Handler)
    {
        if (Handler is null) throw new ArgumentNullException(nameof(Handler));
        DialogCompleted += (_, e) => Handler(e);
        return this;
    }

    public bool Modal { get; set; }

    public bool RestoreIfExist { get; set; } = true;

    public override bool CanExecute(object? parameter)
    {
        if (!base.CanExecute(parameter)) return false;

        return _Window is null || RestoreIfExist;
    }

    public override void Execute(object? parameter)
    {
        if (_Window is { } window)
        {
            window.Focus();
            return;
        }

        if (_WindowFactory is null)
            throw new InvalidOperationException("Отсутствует метод создания нового окна");

        window = _WindowFactory();

        if (_ViewModelFactory is { } view_model_factory && view_model_factory() is { } model)
        {
            window.DataContext = model;

            if (model is DialogViewModel vm)
                vm.Completed += (_, e) =>
                {
                    window.DialogResult = e;
                    window.Close();
                };
        }

        window.Closed += OnWindowClosed;
        _Window = window;

        if (Modal)
            window.ShowDialog();
        else
            window.Show();
    }

    private void OnWindowClosed(object? Sender, EventArgs E)
    {
        if (Sender is not Window window)
            return;

        window.Closed -= OnWindowClosed;

        if (!ReferenceEquals(_Window, window))
            _Window = null;

        DialogCompleted?.Invoke(this, window.DialogResult);
    }
}
