using System.Windows;

using MathCore.WPF.Services;
using MathCore.WPF.ViewModels;
using MathCore.WPF.Views.Windows;

namespace MathCore.WPF.Dialogs;

public static class ProgressDialog
{
    public static IProgressInfo Create(string Title, string Status = null, string Information = null)
    {
        var progress_model = new ProgressViewModel(Title)
        {
            StatusValue = Status,
            InformationValue = Information
        };

        var current_window = UserDialogService.CurrentWindow;

        var progress_view = Application.Current.Dispatcher.CheckAccess()
            ? new() { DataContext = progress_model, Owner = current_window }
            : Application.Current.Dispatcher.Invoke(() => new ProgressDialogWindow { DataContext = progress_model, Owner = current_window });

        void OnDisposed(object? s, EventArgs e)
        {
            if (progress_view.Dispatcher.CheckAccess())
                progress_view.Close();
            else
                progress_view.Dispatcher.Invoke(() => OnDisposed(s, e));
        }

        progress_model.Disposed += OnDisposed;
        progress_view.Closing += (s, e) => e.Cancel = ((Window)s).DataContext is ProgressViewModel
        {
            IsDisposed: false,
            Cancellable: false,
        };
        progress_view.Closed += (_, _) => progress_model.CancelCommand.Execute(default);

        var window_shown = false;

        void OnNeedToShowDialog()
        {
            if (!window_shown)
            {
                progress_view.Show();
                window_shown = true;
            }
            else
                progress_view.Focus();
        }

        progress_model.ShowDialog += (_, _) =>
        {
            if (progress_view.CheckAccess())
                OnNeedToShowDialog();
            else
                progress_view.Dispatcher.Invoke(OnNeedToShowDialog);
        };

        return progress_model;
    }

    public static IProgressInfo Show(string Title, string Status = null, string Information = null)
    {
        var progress_model = new ProgressViewModel(Title)
        {
            StatusValue = Status,
            InformationValue = Information
        };

        var current_window = UserDialogService.CurrentWindow;

        var progress_view = Application.Current.Dispatcher.CheckAccess()
            ? new() { DataContext = progress_model, Owner = current_window }
            : Application.Current.Dispatcher.Invoke(() => new ProgressDialogWindow { DataContext = progress_model, Owner = current_window });

        void OnDisposed(object? s, EventArgs e)
        {
            if (progress_view.Dispatcher.CheckAccess())
                progress_view.Close();
            else
                progress_view.Dispatcher.Invoke(() => OnDisposed(s, e));
        }

        progress_model.Disposed += OnDisposed;
        progress_view.Closing += (s, e) => e.Cancel = ((Window)s).DataContext is ProgressViewModel
        {
            IsDisposed: false,
            Cancellable: false,
        };
        progress_view.Closed += (_, _) => progress_model.CancelCommand.Execute(default);
        progress_view.Show();

        return progress_model;
    }
}
