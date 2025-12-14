namespace MathCore.WPF.Notifications;

/// <summary>Позиция отображения уведомления на экране</summary>
public enum ToastNotificationPosition
{
    /// <summary>Правый нижний угол</summary>
    BottomRight,

    /// <summary>Правый верхний угол</summary>
    TopRight,

    /// <summary>Левый нижний угол</summary>
    BottomLeft,

    /// <summary>Левый верхний угол</summary>
    TopLeft,

    /// <summary>По центру экрана</summary>
    Center,

    /// <summary>Сверху по центру</summary>
    TopCenter,

    /// <summary>Снизу по центру</summary>
    BottomCenter
}
