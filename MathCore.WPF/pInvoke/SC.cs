using System.Diagnostics.CodeAnalysis;

namespace MathCore.WPF.pInvoke;

/// <summary>Системные команды</summary>
[SuppressMessage("ReSharper", "InconsistentNaming"), SuppressMessage("ReSharper", "IdentifierTypo"), SuppressMessage("ReSharper", "UnusedMember.Global")]
internal enum SC : uint
{
    /// <summary>Изменить размер окна</summary>
    SIZE = 0xF000,
    /// <summary>Переместить окно</summary>
    MOVE = 0xF010,
    /// <summary>Свернуть окно</summary>
    MINIMIZE = 0xF020,
    /// <summary>Развернуть окно</summary>
    MAXIMIZE = 0xF030,
    /// <summary>Активировать следующее окно</summary>
    NEXTWINDOW = 0xF040,
    /// <summary>Активировать предыдущее окно</summary>
    PREVWINDOW = 0xF050,
    /// <summary>Закрыть окно</summary>
    CLOSE = 0xF060,
    /// <summary>Вертикальная прокрутка</summary>
    VSCROLL = 0xF070,
    /// <summary>Горизонтальная прокрутка</summary>
    HSCROLL = 0xF080,
    /// <summary>Открыть системное меню мышью</summary>
    MOUSEMENU = 0xF090,
    /// <summary>Открыть системное меню с клавиатуры</summary>
    KEYMENU = 0xF100,
    /// <summary>Упорядочить значки</summary>
    ARRANGE = 0xF110,
    /// <summary>Восстановить окно</summary>
    RESTORE = 0xF120,
    /// <summary>Открыть список задач</summary>
    TASKLIST = 0xF130,
    /// <summary>Запустить экранную заставку</summary>
    SCREENSAVE = 0xF140,
    /// <summary>Обработать системную горячую клавишу</summary>
    HOTKEY = 0xF150,
    #region WINVER >= 0x0400 - Windows 95
    //#if(WINVER >= 0x0400) //Windows 95
    /// <summary>Команда по умолчанию</summary>
    DEFAULT = 0xF160,
    /// <summary>Управление питанием монитора</summary>
    MONITORPOWER = 0xF170,
    /// <summary>Контекстная справка</summary>
    CONTEXTHELP = 0xF180,
    /// <summary>Разделитель меню</summary>
    SEPARATOR = 0xF00F,
    //#endif /* WINVER >= 0x0400 */
    #endregion

    #region WINVER >= 0x0600 - Windows Vista
    //#if(WINVER >= 0x0600) //Windows Vista
    /// <summary>Флаг безопасного режима</summary>
    ISSECURE = 0x00000001,
    //#endif /* WINVER >= 0x0600 */
    #endregion

    /// <summary>Синоним MINIMIZE</summary>
    [Obsolete]
    ICON = MINIMIZE,
    /// <summary>Синоним MAXIMIZE</summary>
    [Obsolete]
    ZOOM = MAXIMIZE,
}