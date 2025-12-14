using System.Windows;

namespace MathCore.WPF.Notifications;

/// <summary>Настройки уведомлений</summary>
public class ToastNotificationSettings
{
    /// <summary>Позиция отображения уведомлений</summary>
    public ToastNotificationPosition Position { get; set; } = ToastNotificationPosition.BottomRight;

    /// <summary>Длительность отображения уведомления (в миллисекундах, 0 - не закрывать автоматически)</summary>
    public int DisplayDuration { get; set; } = 5000;

    /// <summary>Длительность анимации появления (в миллисекундах)</summary>
    public int FadeInDuration { get; set; } = 300;

    /// <summary>Длительность анимации исчезновения (в миллисекундах)</summary>
    public int FadeOutDuration { get; set; } = 300;

    /// <summary>Максимальное количество одновременно отображаемых уведомлений</summary>
    public int MaxVisibleNotifications { get; set; } = 5;

    /// <summary>Отступ от края экрана (в пикселях)</summary>
    public double ScreenMargin { get; set; } = 10;

    /// <summary>Расстояние между уведомлениями (в пикселях)</summary>
    public double NotificationSpacing { get; set; } = 10;

    /// <summary>Ширина уведомления</summary>
    public double Width { get; set; } = 350;

    /// <summary>Минимальная высота уведомления</summary>
    public double MinHeight { get; set; } = 80;

    /// <summary>Максимальная высота уведомления</summary>
    public double MaxHeight { get; set; } = 200;

    /// <summary>Стиль окна уведомления</summary>
    public Style? WindowStyle { get; set; }

    /// <summary>Воспроизводить системный звук при появлении уведомления</summary>
    public bool PlaySound { get; set; } = true;
}
