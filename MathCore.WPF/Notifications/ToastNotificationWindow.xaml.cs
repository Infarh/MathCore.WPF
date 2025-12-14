using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MathCore.WPF.Notifications;

/// <summary>Окно всплывающего уведомления</summary>
public partial class ToastNotificationWindow : Window
{
    private readonly ToastNotificationViewModel _ViewModel;
    private readonly ToastNotificationSettings _Settings;

    /// <summary>Инициализация окна уведомления</summary>
    /// <param name="ViewModel">Модель-представление уведомления</param>
    /// <param name="Settings">Настройки отображения</param>
    public ToastNotificationWindow(ToastNotificationViewModel ViewModel, ToastNotificationSettings Settings)
    {
        _ViewModel = ViewModel ?? throw new ArgumentNullException(nameof(ViewModel));
        _Settings = Settings ?? throw new ArgumentNullException(nameof(Settings));

        InitializeComponent();

        DataContext = _ViewModel;
        Width = _Settings.Width;
        MinHeight = _Settings.MinHeight;
        MaxHeight = _Settings.MaxHeight;

        // Окно не должно блокировать завершение приложения, если это не требуется явно
        if (!_Settings.KeepApplicationAlive)
        {
            ShowInTaskbar = false;
            Owner = null; // Убираем владельца, чтобы окно не блокировало закрытие главного окна
        }

        if (_Settings.WindowStyle != null)
            Style = _Settings.WindowStyle;

        _ViewModel.CloseRequested += OnCloseRequested;

        Loaded += OnLoaded;
        MouseLeftButtonDown += OnMouseLeftButtonDown;
        MouseRightButtonDown += OnMouseRightButtonDown;
        KeyDown += OnKeyDown;
    }

    private void OnLoaded(object Sender, RoutedEventArgs E)
    {
        FadeIn();

        if (_Settings.DisplayDuration > 0)
        {
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(_Settings.DisplayDuration)
            };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                FadeOut();
            };
            timer.Start();
        }
    }

    private void FadeIn()
    {
        var animation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(_Settings.FadeInDuration));
        BeginAnimation(OpacityProperty, animation);
    }

    private void FadeOut()
    {
        var animation = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(_Settings.FadeOutDuration));
        animation.Completed += (s, e) => Close();
        BeginAnimation(OpacityProperty, animation);
    }

    private void OnCloseRequested(object? Sender, EventArgs E) => FadeOut();

    private void OnMouseLeftButtonDown(object Sender, MouseButtonEventArgs E) => _ViewModel.OnLeftClick();

    private void OnMouseRightButtonDown(object Sender, MouseButtonEventArgs E) => _ViewModel.OnRightClick();

    private void OnKeyDown(object Sender, KeyEventArgs E)
    {
        switch (E.Key)
        {
            case Key.Escape:
            case Key.Space:
            case Key.Enter:
                FadeOut();
                E.Handled = true;
                break;
        }
    }

    protected override void OnClosing(CancelEventArgs E)
    {
        _ViewModel.CloseRequested -= OnCloseRequested;
        base.OnClosing(E);
    }
}
