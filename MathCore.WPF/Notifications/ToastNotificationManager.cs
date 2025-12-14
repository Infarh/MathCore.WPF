using System.Collections.ObjectModel;
using System.Media;
using System.Windows;

namespace MathCore.WPF.Notifications;

/// <summary>Менеджер всплывающих уведомлений</summary>
public class ToastNotificationManager
{
    private static readonly Lazy<ToastNotificationManager> __DefaultInstance = new(() => new());

    /// <summary>Синглтон-экземпляр менеджера по умолчанию</summary>
    public static ToastNotificationManager Default => __DefaultInstance.Value;

    private readonly object _SyncRoot = new();
    private readonly Queue<ToastNotificationViewModel> _NotificationsQueue = new();
    private readonly ObservableCollection<ToastNotificationWindow> _ActiveWindows = [];

    /// <summary>Настройки отображения уведомлений</summary>
    public ToastNotificationSettings Settings { get; }

    /// <summary>Активные окна уведомлений</summary>
    public ReadOnlyObservableCollection<ToastNotificationWindow> ActiveWindows { get; }

    /// <summary>Инициализация менеджера уведомлений</summary>
    public ToastNotificationManager() : this(new ToastNotificationSettings()) { }

    /// <summary>Инициализация менеджера уведомлений с заданными настройками</summary>
    /// <param name="Settings">Настройки отображения уведомлений</param>
    public ToastNotificationManager(ToastNotificationSettings Settings)
    {
        this.Settings = Settings ?? throw new ArgumentNullException(nameof(Settings));
        ActiveWindows = new ReadOnlyObservableCollection<ToastNotificationWindow>(_ActiveWindows);
    }

    /// <summary>Показать уведомление</summary>
    /// <param name="Title">Заголовок</param>
    /// <param name="Message">Текст сообщения</param>
    /// <param name="Icon">Тип иконки</param>
    public void Show(string? Title, string Message, ToastNotificationIcon Icon = ToastNotificationIcon.Information)
    {
        var view_model = new ToastNotificationViewModel
        {
            Title = Title,
            Message = Message,
            Icon = Icon
        };

        Show(view_model);
    }

    /// <summary>Показать уведомление с пользовательским содержимым</summary>
    /// <param name="Title">Заголовок</param>
    /// <param name="CustomContent">Пользовательское содержимое</param>
    /// <param name="Icon">Тип иконки</param>
    public void ShowCustom(string? Title, object CustomContent, ToastNotificationIcon Icon = ToastNotificationIcon.None)
    {
        var view_model = new ToastNotificationViewModel
        {
            Title = Title,
            CustomContent = CustomContent,
            Icon = Icon
        };

        Show(view_model);
    }

    /// <summary>Показать уведомление</summary>
    /// <param name="ViewModel">Модель-представление уведомления</param>
    public void Show(ToastNotificationViewModel ViewModel)
    {
        if (ViewModel is null)
            throw new ArgumentNullException(nameof(ViewModel));

        lock (_SyncRoot)
        {
            if (_ActiveWindows.Count >= Settings.MaxVisibleNotifications)
            {
                _NotificationsQueue.Enqueue(ViewModel);
                return;
            }

            ShowNotificationWindow(ViewModel);
        }
    }

    private void ShowNotificationWindow(ToastNotificationViewModel ViewModel)
    {
        Application.Current?.Dispatcher.Invoke(() =>
        {
            var window = new ToastNotificationWindow(ViewModel, Settings);
            
            window.Closed += OnWindowClosed;

            PositionWindow(window);

            _ActiveWindows.Add(window);

            if (Settings.PlaySound)
                PlayNotificationSound(ViewModel.Icon);

            window.Show();
        });
    }

    private void PositionWindow(ToastNotificationWindow Window)
    {
        var screen_bounds = SystemParameters.WorkArea;
        var total_height = 0.0;

        foreach (var active_window in _ActiveWindows)
            total_height += active_window.ActualHeight + Settings.NotificationSpacing;

        var position = Settings.Position;

        switch (position)
        {
            case ToastNotificationPosition.BottomRight:
                Window.Left = screen_bounds.Right - Settings.Width - Settings.ScreenMargin;
                Window.Top = screen_bounds.Bottom - Settings.MinHeight - Settings.ScreenMargin - total_height;
                break;

            case ToastNotificationPosition.TopRight:
                Window.Left = screen_bounds.Right - Settings.Width - Settings.ScreenMargin;
                Window.Top = screen_bounds.Top + Settings.ScreenMargin + total_height;
                break;

            case ToastNotificationPosition.BottomLeft:
                Window.Left = screen_bounds.Left + Settings.ScreenMargin;
                Window.Top = screen_bounds.Bottom - Settings.MinHeight - Settings.ScreenMargin - total_height;
                break;

            case ToastNotificationPosition.TopLeft:
                Window.Left = screen_bounds.Left + Settings.ScreenMargin;
                Window.Top = screen_bounds.Top + Settings.ScreenMargin + total_height;
                break;

            case ToastNotificationPosition.Center:
                Window.Left = (screen_bounds.Width - Settings.Width) / 2;
                Window.Top = (screen_bounds.Height - Settings.MinHeight) / 2 - total_height / 2;
                break;

            case ToastNotificationPosition.TopCenter:
                Window.Left = (screen_bounds.Width - Settings.Width) / 2;
                Window.Top = screen_bounds.Top + Settings.ScreenMargin + total_height;
                break;

            case ToastNotificationPosition.BottomCenter:
                Window.Left = (screen_bounds.Width - Settings.Width) / 2;
                Window.Top = screen_bounds.Bottom - Settings.MinHeight - Settings.ScreenMargin - total_height;
                break;
        }
    }

    private void OnWindowClosed(object? Sender, EventArgs E)
    {
        if (Sender is not ToastNotificationWindow window)
            return;

        window.Closed -= OnWindowClosed;

        lock (_SyncRoot)
        {
            _ActiveWindows.Remove(window);

            RepositionWindows();

            if (_NotificationsQueue.Count > 0)
            {
                var next_notification = _NotificationsQueue.Dequeue();
                ShowNotificationWindow(next_notification);
            }
        }
    }

    private void RepositionWindows()
    {
        var total_height = 0.0;

        foreach (var window in _ActiveWindows)
        {
            var screen_bounds = SystemParameters.WorkArea;
            var position = Settings.Position;

            switch (position)
            {
                case ToastNotificationPosition.BottomRight:
                    window.Top = screen_bounds.Bottom - window.ActualHeight - Settings.ScreenMargin - total_height;
                    break;

                case ToastNotificationPosition.TopRight:
                    window.Top = screen_bounds.Top + Settings.ScreenMargin + total_height;
                    break;

                case ToastNotificationPosition.BottomLeft:
                    window.Top = screen_bounds.Bottom - window.ActualHeight - Settings.ScreenMargin - total_height;
                    break;

                case ToastNotificationPosition.TopLeft:
                    window.Top = screen_bounds.Top + Settings.ScreenMargin + total_height;
                    break;

                case ToastNotificationPosition.TopCenter:
                    window.Top = screen_bounds.Top + Settings.ScreenMargin + total_height;
                    break;

                case ToastNotificationPosition.BottomCenter:
                    window.Top = screen_bounds.Bottom - window.ActualHeight - Settings.ScreenMargin - total_height;
                    break;
            }

            total_height += window.ActualHeight + Settings.NotificationSpacing;
        }
    }

    private static void PlayNotificationSound(ToastNotificationIcon Icon)
    {
        try
        {
            switch (Icon)
            {
                case ToastNotificationIcon.Error:
                case ToastNotificationIcon.Critical:
                    SystemSounds.Hand.Play();
                    break;

                case ToastNotificationIcon.Warning:
                    SystemSounds.Exclamation.Play();
                    break;

                case ToastNotificationIcon.Question:
                    SystemSounds.Question.Play();
                    break;

                case ToastNotificationIcon.Information:
                case ToastNotificationIcon.Success:
                    SystemSounds.Asterisk.Play();
                    break;
            }
        }
        catch
        {
            // Игнорируем ошибки воспроизведения звука
        }
    }

    /// <summary>Закрыть все активные уведомления</summary>
    public void CloseAll()
    {
        lock (_SyncRoot)
        {
            _NotificationsQueue.Clear();

            var windows = _ActiveWindows.ToArray();
            foreach (var window in windows)
                window.Close();
        }
    }
}
