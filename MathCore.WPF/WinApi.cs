using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace MathCore.WPF;

/// <summary>Импорт функций Win32 API</summary>
public static class WinApi
{
    /// <summary>Структура WinAPI, описывающая координаты точки</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        /// <summary>Координата X</summary>
        public int X;

        /// <summary>Координата Y</summary>
        public int Y;
    }

    /// <summary>Создаёт вспомогательное окно, которое получает сообщения от иконки в области уведомлений</summary>
    [DllImport("USER32.DLL", EntryPoint = "CreateWindowExW", SetLastError = true)]
    public static extern IntPtr CreateWindowEx(
        int dwExStyle,
        [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
        [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
        int dwStyle,
        int x, int y,
        int nWidth, int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    /// <summary>Обрабатывает сообщение стандартной оконной процедурой</summary>
    [DllImport("USER32.DLL")]
    public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wparam, IntPtr lparam);

    /// <summary>Регистрирует класс вспомогательного окна</summary>
    [DllImport("USER32.DLL", EntryPoint = "RegisterClassW", SetLastError = true)]
    public static extern short RegisterClass(ref WindowClass lpWndClass);

    /// <summary>Регистрирует идентификатор для пользовательского оконного сообщения</summary>
    /// <param name="lpString">Имя сообщения</param>
    /// <returns>Зарегистрированный идентификатор сообщения</returns>
    [DllImport("User32.Dll", EntryPoint = "RegisterWindowMessageW")]
    public static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

    /// <summary>
    /// Используется для уничтожения скрытого вспомогательного окна,
    /// которое получает сообщения от иконки в области уведомлений
    /// </summary>
    /// <param name="hWnd">Дескриптор окна</param>
    /// <returns>Истина, если операция прошла успешно</returns>
    [DllImport("USER32.DLL", SetLastError = true)]
    public static extern bool DestroyWindow(IntPtr hWnd);


    /// <summary>Устанавливает фокус ввода на указанное окно</summary>
    /// <param name="hWnd">Дескриптор окна</param>
    /// <returns>Истина, если операция прошла успешно</returns>
    [DllImport("USER32.DLL")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    /// <summary>
    /// Возвращает максимальное количество миллисекунд между
    /// первым и вторым нажатием кнопки мыши, чтобы ОС
    /// восприняла их как двойной щелчок
    /// </summary>
    /// <returns>
    /// Максимальное количество миллисекунд между первым и вторым нажатием
    /// кнопки мыши, при котором ОС считает действие двойным щелчком
    /// </returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern int GetDoubleClickTime();

    /// <summary>Возвращает экранные координаты текущего физического положения курсора</summary>
    [DllImport("USER32.DLL", SetLastError = true)]
    public static extern bool GetPhysicalCursorPos(ref Point lpPoint);

    /// <summary>Возвращает экранные координаты текущего положения курсора</summary>
    [DllImport("USER32.DLL", SetLastError = true)]
    public static extern bool GetCursorPos(ref Point lpPoint);
}

/// <summary>
/// Структура WinAPI WNDCLASS, описывающая окно
/// Используется для получения оконных сообщений
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct WindowClass
{
#pragma warning disable 1591

    public uint style;
    public WindowProcedureHandler lpfnWndProc;
    public int cbClsExtra;
    public int cbWndExtra;
    public IntPtr hInstance;
    public IntPtr hIcon;
    public IntPtr hCursor;
    public IntPtr hbrBackground;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string lpszMenuName;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string lpszClassName;

#pragma warning restore 1591
}

/// <summary>
/// Делегат обратного вызова, который используется Windows API
/// для доставки оконных сообщений
/// </summary>
public delegate IntPtr WindowProcedureHandler(IntPtr hwnd, uint uMsg, IntPtr wparam, IntPtr lparam);

/// <summary>
/// Получает сообщения от иконки в области уведомлений через
/// оконные сообщения вспомогательного скрытого окна
/// </summary>
public class WindowMessageSink : IDisposable
{
    #region members

    /// <summary>
    /// Идентификатор пользовательских сообщений,
    /// получаемых от иконки в области уведомлений
    /// </summary>
    public const int CallbackMessageId = 0x400;

    /// <summary>
    /// Идентификатор сообщения, получаемого при создании или
    /// перезапуске панели задач
    /// </summary>
    private uint taskbarRestartMessageId;

    /// <summary>
    /// Флаг, показывающий, что событие отпускания кнопки мыши
    /// является следствием двойного щелчка и должно быть подавлено
    /// </summary>
    private bool isDoubleClick;

    /// <summary>
    /// Делегат, обрабатывающий сообщения скрытого
    /// нативного окна, которое получает оконные сообщения
    /// Хранение ссылки не позволяет сборщику мусора
    /// освободить делегат, пока окно используется
    /// </summary>
    private WindowProcedureHandler messageHandler;

    /// <summary>Идентификатор класса окна</summary>
    internal string WindowId { get; private set; }

    /// <summary>Дескриптор окна, получающего сообщения</summary>
    internal IntPtr MessageWindowHandle { get; private set; }

    /// <summary>
    /// Версия иконки в области уведомлений
    /// Определяет интерпретацию входящих сообщений
    /// </summary>
    public NotifyIconVersion Version { get; set; }

    #endregion

    #region events

    /// <summary>Событие запроса изменения состояния пользовательской подсказки (показать/скрыть)</summary>
    public event Action<bool> ChangeToolTipStateRequest;

    /// <summary>
    /// Вызывается при клике или перемещении мыши
    /// в области иконки в трее
    /// </summary>
    public event Action<MouseEvent> MouseEventReceived;

    /// <summary>
    /// Вызывается при отображении или закрытии всплывающей
    /// подсказки (balloon Tooltip). Флаг указывает текущее состояние
    /// </summary>
    public event Action<bool> BalloonToolTipChanged;

    /// <summary>
    /// Вызывается при создании или перезапуске панели задач
    /// Требует повторной регистрации иконки в области уведомлений
    /// </summary>
    public event Action TaskbarCreated;

    #endregion

    #region construction

    /// <summary>
    /// Создаёт новый объект-приёмник сообщений для указанной
    /// версии иконки в области уведомлений
    /// </summary>
    /// <param name="version">Версия поведения иконки</param>
    public WindowMessageSink(NotifyIconVersion version)
    {
        Version = version;
        CreateMessageWindow();
    }

    private WindowMessageSink()
    {
    }

    /// <summary>
    /// Создаёт "пустой" экземпляр, который предоставляет
    /// нулевой дескриптор вместо реального оконного хэндла
    /// Используется во время разработки (design-time)
    /// </summary>
    /// <returns>Экземпляр-пустышка без реального окна</returns>
    internal static WindowMessageSink CreateEmpty() =>
        new()
        {
            MessageWindowHandle = IntPtr.Zero,
            Version             = NotifyIconVersion.Vista
        };

    #endregion

    #region CreateMessageWindow

    /// <summary>
    /// Создаёт вспомогательное окно для получения
    /// сообщений от иконки в области уведомлений
    /// </summary>
    private void CreateMessageWindow()
    {
        // генерируем уникальный идентификатор окна
        WindowId = "WPFTaskbarIcon_" + DateTime.Now.Ticks;

        // регистрируем обработчик оконных сообщений
        messageHandler = OnWindowMessageReceived;

        // создаём простой класс окна, ссылающийся на обработчик messageHandler
        WindowClass wc;

        wc.style         = 0;
        wc.lpfnWndProc   = messageHandler;
        wc.cbClsExtra    = 0;
        wc.cbWndExtra    = 0;
        wc.hInstance     = IntPtr.Zero;
        wc.hIcon         = IntPtr.Zero;
        wc.hCursor       = IntPtr.Zero;
        wc.hbrBackground = IntPtr.Zero;
        wc.lpszMenuName  = "";
        wc.lpszClassName = WindowId;

        // регистрируем класс окна
        WinApi.RegisterClass(ref wc);

        // получаем идентификатор сообщения, используемого для
        // уведомления о перезапуске панели задач, чтобы
        // можно было повторно добавить иконку
        taskbarRestartMessageId = WinApi.RegisterWindowMessage("TaskbarCreated");

        // создаём вспомогательное окно для получения сообщений
        MessageWindowHandle = WinApi.CreateWindowEx(0, WindowId, "", 0, 0, 0, 1, 1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

        if (MessageWindowHandle == IntPtr.Zero)
        {
#if SILVERLIGHT
                throw new Exception("Message window handle was not a valid pointer.");
#else
            throw new Win32Exception("Message window handle was not a valid pointer");
#endif
        }
    }

    #endregion

    #region Handle Window Messages

    /// <summary>Обработчик обратного вызова, получающий сообщения из области уведомлений</summary>
    private IntPtr OnWindowMessageReceived(IntPtr hwnd, uint messageId, IntPtr wparam, IntPtr lparam)
    {
        if (messageId == taskbarRestartMessageId)
        {
            // панель задач была перезапущена (например, после падения проводника) – пересоздаём иконку
            TaskbarCreated?.Invoke();
        }

        // пересылаем сообщение во внутреннюю обработку
        ProcessWindowMessage(messageId, wparam, lparam);

        // передаём сообщение стандартной оконной процедуре
        return WinApi.DefWindowProc(hwnd, messageId, wparam, lparam);
    }


    /// <summary>Обрабатывает входящие системные сообщения</summary>
    /// <param name="msg">Идентификатор сообщения</param>
    /// <param name="wParam">
    /// Для версии <see cref="NotifyIconVersion.Vista"/> и выше параметр может
    /// использоваться для получения координат мыши (сейчас не используется)
    /// </param>
    /// <param name="lParam">Информация о событии</param>
    private void ProcessWindowMessage(uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg != CallbackMessageId) return;

        switch (lParam.ToInt32())
        {
            case 0x200:
                MouseEventReceived?.Invoke(MouseEvent.MouseMove);
                break;

            case 0x201:
                MouseEventReceived?.Invoke(MouseEvent.IconLeftMouseDown);
                break;

            case 0x202:
                if (!isDoubleClick)
                {
                    MouseEventReceived?.Invoke(MouseEvent.IconLeftMouseUp);
                }
                isDoubleClick = false;
                break;

            case 0x203:
                isDoubleClick = true;
                MouseEventReceived?.Invoke(MouseEvent.IconDoubleClick);
                break;

            case 0x204:
                MouseEventReceived?.Invoke(MouseEvent.IconRightMouseDown);
                break;

            case 0x205:
                MouseEventReceived?.Invoke(MouseEvent.IconRightMouseUp);
                break;

            case 0x206:
                // двойной щелчок правой кнопкой мыши – событие не генерируем
                break;

            case 0x207:
                MouseEventReceived?.Invoke(MouseEvent.IconMiddleMouseDown);
                break;

            case 520:
                MouseEventReceived?.Invoke(MouseEvent.IconMiddleMouseUp);
                break;

            case 0x209:
                // двойной щелчок средней кнопкой мыши – событие не генерируем
                break;

            case 0x402:
                BalloonToolTipChanged?.Invoke(true);
                break;

            case 0x403:
            case 0x404:
                BalloonToolTipChanged?.Invoke(false);
                break;

            case 0x405:
                MouseEventReceived?.Invoke(MouseEvent.BalloonToolTipClicked);
                break;

            case 0x406:
                ChangeToolTipStateRequest?.Invoke(true);
                break;

            case 0x407:
                ChangeToolTipStateRequest?.Invoke(false);
                break;

            default:
                Debug.WriteLine("Unhandled NotifyIcon message ID: " + lParam);
                break;
        }
    }

    #endregion

    #region Dispose

    /// <summary>Флаг, устанавливаемый в true после вызова <c>Dispose</c></summary>
    public bool IsDisposed { get; private set; }


    /// <summary>Освобождает ресурсы объекта</summary>
    /// <remarks>
    /// Метод не является виртуальным по задумке. Производные классы
    /// должны переопределять <see cref="Dispose(bool)"/>
    /// </remarks>
    public void Dispose()
    {
        Dispose(true);

        // объект будет очищен методом Dispose, поэтому убираем его из очереди финализации
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Деструктор вызывается только в том случае, если метод <see cref="Dispose()"/>
    /// не был вызван. Даёт базовому классу возможность финализировать объект
    /// <para>
    /// Важно: не определяйте деструкторы в классах-потомках
    /// </para>
    /// </summary>
    ~WindowMessageSink() => Dispose(false);


    /// <summary>
    /// Удаляет оконный хук, получающий сообщения,
    /// и закрывает вспомогательное окно
    /// </summary>
    private void Dispose(bool disposing)
    {
        // если объект уже освобождён, ничего не делаем
        if (IsDisposed) return;
        IsDisposed = true;

        // всегда уничтожаем неуправляемый дескриптор (даже при вызове из GC)
        WinApi.DestroyWindow(MessageWindowHandle);
        messageHandler = null;
    }

    #endregion
}

/// <summary>События мыши, связанные с кликами по иконке</summary>
public enum MouseEvent
{
    /// <summary>
    /// Курсор мыши был перемещён в пределах
    /// области иконки в панели задач
    /// </summary>
    MouseMove,

    /// <summary>Нажата правая кнопка мыши</summary>
    IconRightMouseDown,

    /// <summary>Нажата левая кнопка мыши</summary>
    IconLeftMouseDown,

    /// <summary>Отпущена правая кнопка мыши</summary>
    IconRightMouseUp,

    /// <summary>Отпущена левая кнопка мыши</summary>
    IconLeftMouseUp,

    /// <summary>Нажата средняя кнопка мыши</summary>
    IconMiddleMouseDown,

    /// <summary>Отпущена средняя кнопка мыши</summary>
    IconMiddleMouseUp,

    /// <summary>По иконке в панели задач выполнен двойной щелчок</summary>
    IconDoubleClick,

    /// <summary>Клик по всплывающей подсказке (balloon)</summary>
    BalloonToolTipClicked
}

/// <summary>
/// Версия поведения иконки в области уведомлений
/// Чем выше версия, тем больше доступных возможностей
/// </summary>
public enum NotifyIconVersion
{
    /// <summary>
    /// Поведение по умолчанию (старая версия Win95)
    /// Ожидает размер структуры <see cref="NotifyIconData"/> 488 байт
    /// </summary>
    Win95 = 0x0,

    /// <summary>
    /// Поведение, соответствующее Windows 2000 и выше
    /// Ожидает размер структуры <see cref="NotifyIconData"/> 504 байта
    /// </summary>
    Win2000 = 0x3,

    /// <summary>
    /// Расширенная поддержка всплывающих подсказок (balloon),
    /// доступная в Windows Vista и более поздних версиях
    /// </summary>
    Vista = 0x4
}

/// <summary>
/// Структура, передаваемая при конфигурации иконки в области уведомлений
/// Настраивается частично в зависимости от значений <see cref="IconDataMembers"/>
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct NotifyIconData
{
    /// <summary>Размер структуры в байтах</summary>
    public uint cbSize;

    /// <summary>
    /// Дескриптор окна, получающего уведомления, связанные с иконкой
    /// в области уведомлений панели задач. Пара hWnd и uID используется
    /// оболочкой (Shell) для однозначной идентификации иконки при вызове Shell_NotifyIcon
    /// </summary>
    public IntPtr WindowHandle;

    /// <summary>
    /// Идентификатор иконки в области уведомлений, определяемый приложением
    /// Пара hWnd и uID используется оболочкой для идентификации иконки при вызове Shell_NotifyIcon
    /// Можно иметь несколько иконок, связанных с одним окном, используя разные uID
    /// </summary>
    public uint TaskbarIconId;

    /// <summary>
    /// Флаги, указывающие, какие поля структуры содержат валидные данные
    /// Может быть комбинацией констант NIF_XXX
    /// </summary>
    public IconDataMembers ValidMembers;

    /// <summary>
    /// Идентификатор сообщения, определяемый приложением
    /// Система использует его для отправки уведомлений окну WindowHandle
    /// </summary>
    public uint CallbackMessageId;

    /// <summary>
    /// Дескриптор иконки для отображения (обычно Icon.Handle)
    /// </summary>
    public IntPtr IconHandle;

    /// <summary>
    /// Текст стандартной всплывающей подсказки (ToolTip)
    /// Максимум 64 символа включая завершающий NULL, в версиях 5.0 и выше – до 128 символов
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string ToolTipText;


    /// <summary>Состояние иконки. При изменении не забывайте задавать <see cref="StateMask"/></summary>
    public IconState IconState;

    /// <summary>
    /// Маска битов состояния, которые нужно получить или изменить
    /// Например, установка значения <see cref="IconState.Hidden"/>
    /// приводит к изменению только признака скрытия иконки
    /// </summary>
    public IconState StateMask;

    /// <summary>
    /// Текст всплывающей подсказки balloon. Максимум 255 символов
    /// Чтобы убрать подсказку, задайте флаг NIF_INFO в uFlags
    /// и установите szInfo в пустую строку
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string BalloonText;

    /// <summary>
    /// В основном используется для задания версии при вызове
    /// <see cref="WinApi.Shell_NotifyIcon"/> с командой <see cref="NotifyCommand.SetVersion"/>
    /// Для старых версий также применяется для задания таймаутов balloon-подсказок
    /// </summary>
    public uint VersionOrTimeout;

    /// <summary>
    /// Заголовок всплывающей подсказки balloon (выводится жирным шрифтом над текстом)
    /// Максимум 63 символа
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string BalloonTitle;

    /// <summary>
    /// Добавляет иконку во всплывающую подсказку balloon слева от заголовка
    /// Если <see cref="BalloonTitle"/> пуст, иконка не показывается
    /// </summary>
    public BalloonFlags BalloonFlags;

    /// <summary>
    /// Windows XP (Shell32.dll версии 6.0) и выше
    /// Windows 7 и новее: зарегистрированный GUID, идентифицирующий иконку
    /// Переопределяет uID и является рекомендуемым способом идентификации иконки
    /// Windows XP – Vista: зарезервировано
    /// </summary>
    public Guid TaskbarIconGuid;

    /// <summary>
    /// Windows Vista (Shell32.dll версии 6.0.6) и выше
    /// Дескриптор пользовательской иконки, отображаемой во всплывающей подсказке
    /// независимо от основной иконки в трее. Если поле не NULL и установлен
    /// флаг <see cref="BalloonFlags.User"/>, используется эта иконка
    /// Если поле равно NULL, используется поведение по умолчанию
    /// </summary>
    public IntPtr CustomBalloonIconHandle;


    /// <summary>
    /// Создаёт структуру данных по умолчанию, описывающую
    /// скрытую иконку в области уведомлений без установленной иконки
    /// </summary>
    /// <param name="handle">Дескриптор окна приёмника сообщений</param>
    /// <returns>Инициализированная структура <see cref="NotifyIconData"/></returns>
    public static NotifyIconData CreateDefault(IntPtr handle)
    {
        var data = new NotifyIconData();

        if (Environment.OSVersion.Version.Major >= 6)
        {
            // для Vista и новее используем текущий размер структуры
            data.cbSize = (uint)Marshal.SizeOf(data);
        }
        else
        {
            // для XP/2003 требуется другой размер, иначе отдельные функции
            // (например, balloon-подсказки) могут не работать
            data.cbSize = 952; // NOTIFYICONDATAW_V3_SIZE

            // фиксированный таймаут для balloon-подсказки
            data.VersionOrTimeout = 10;
        }

        data.WindowHandle      = handle;
        data.TaskbarIconId     = 0x0;
        data.CallbackMessageId = WindowMessageSink.CallbackMessageId;
        data.VersionOrTimeout  = (uint)NotifyIconVersion.Win95;

        data.IconHandle = IntPtr.Zero;

        // по умолчанию скрываем иконку
        data.IconState = IconState.Hidden;
        data.StateMask = IconState.Hidden;

        // задаём флаги валидных полей
        data.ValidMembers = IconDataMembers.Message
            | IconDataMembers.Icon
            | IconDataMembers.Tip;

        // обнуляем строки
        data.ToolTipText = data.BalloonText = data.BalloonTitle = string.Empty;

        return data;
    }
}

/// <summary>
/// Флаги, определяющие иконку, отображаемую во всплывающей подсказке balloon
/// </summary>
public enum BalloonFlags
{
    /// <summary>Иконка не отображается</summary>
    None = 0x00,

    /// <summary>Отображается иконка информации</summary>
    Info = 0x01,

    /// <summary>Отображается иконка предупреждения</summary>
    Warning = 0x02,

    /// <summary>Отображается иконка ошибки</summary>
    Error = 0x03,

    /// <summary>
    /// Windows XP Service Pack 2 и выше
    /// Использовать пользовательскую иконку в качестве иконки заголовка
    /// </summary>
    User = 0x04,

    /// <summary>
    /// Windows XP (Shell32.dll версии 6.0) и выше
    /// Не воспроизводить связанный со всплывающей подсказкой звук
    /// </summary>
    NoSound = 0x10,

    /// <summary>
    /// Windows Vista (Shell32.dll версии 6.0.6) и выше
    /// Использовать крупную иконку с размерами SM_CXICON x SM_CYICON
    /// вместо маленькой SM_CXSMICON x SM_CYSMICON
    /// </summary>
    LargeIcon = 0x20,

    /// <summary>Windows 7 и выше</summary>
    RespectQuietTime = 0x80
}

/// <summary>
/// Флаги, указывающие, какие поля структуры <see cref="NotifyIconData"/>
/// заполнены и содержат валидные данные либо доп. настройки отображения
/// </summary>
[Flags]
public enum IconDataMembers
{
    /// <summary>Установлен идентификатор сообщения</summary>
    Message = 0x01,

    /// <summary>Установлена иконка уведомления</summary>
    Icon = 0x02,

    /// <summary>Установлен текст всплывающей подсказки</summary>
    Tip = 0x04,

    /// <summary>
    /// Установлено состояние иконки (<see cref="IconState"/>)
    /// Применимо к полям <see cref="NotifyIconData.IconState"/>
    /// и <see cref="NotifyIconData.StateMask"/>
    /// </summary>
    State = 0x08,

    /// <summary>
    /// Установлена всплывающая подсказка balloon
    /// Используются поля <see cref="NotifyIconData.BalloonText"/>,
    /// <see cref="NotifyIconData.BalloonTitle"/>, <see cref="NotifyIconData.BalloonFlags"/>
    /// и <see cref="NotifyIconData.VersionOrTimeout"/>
    /// </summary>
    Info = 0x10,

    // Внутренний идентификатор. Зарезервировано, не используется.
    //Guid = 0x20,

    /// <summary>
    /// Windows Vista (Shell32.dll версии 6.0.6) и выше
    /// Если подсказку нельзя показать немедленно, она отбрасывается
    /// Используется для подсказок, отображающих актуальное состояние, теряющее
    /// смысл при задержке (например, «Входящий звонок»)
    /// Должен комбинироваться с флагом <see cref="Info"/>
    /// </summary>
    Realtime = 0x40,

    /// <summary>
    /// Windows Vista (Shell32.dll версии 6.0.6) и выше
    /// Использовать стандартную подсказку вместо пользовательского всплывающего UI
    /// при версии NOTIFYICON_VERSION_4
    /// Флаг действует до следующего вызова Shell_NotifyIcon
    /// </summary>
    UseLegacyToolTips = 0x80
}

/// <summary>
/// Состояние иконки, позволяющее, в том числе, скрыть её
/// </summary>
public enum IconState
{
    /// <summary>Иконка отображается</summary>
    Visible = 0x00,

    /// <summary>Иконка скрыта</summary>
    Hidden = 0x01,

    // Shared = 0x02 – совместно используемая иконка (не поддерживается, зарезервировано)
}

internal class AppBarInfo
{
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("shell32.dll")]
    private static extern uint SHAppBarMessage(uint dwMessage, ref APPBARDATA data);

    [DllImport("user32.dll")]
    private static extern int SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);


    private const int ABE_BOTTOM = 3;
    private const int ABE_LEFT = 0;
    private const int ABE_RIGHT = 2;
    private const int ABE_TOP = 1;

    private const int ABM_GETTASKBARPOS = 0x00000005;

    // Константы для SystemParametersInfo
    private const uint SPI_GETWORKAREA = 0x0030;

    private APPBARDATA m_data;

    /// <summary>Край экрана, к которому примыкает панель задач</summary>
    public ScreenEdge Edge => (ScreenEdge)m_data.uEdge;


    /// <summary>Рабочая область экрана с учётом панели задач</summary>
    public Rectangle WorkArea
    {
        get
        {
            var bResult = 0;
            var rc      = new RECT();
            var rawRect = Marshal.AllocHGlobal(Marshal.SizeOf(rc));
            bResult = SystemParametersInfo(SPI_GETWORKAREA, 0, rawRect, 0);
            rc      = (RECT)Marshal.PtrToStructure(rawRect, rc.GetType());

            if (bResult != 1) return new(0, 0, 0, 0);
            Marshal.FreeHGlobal(rawRect);
            return new(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
        }
    }


    /// <summary>Получает положение произвольной панели задач (AppBar) по классу и заголовку окна</summary>
    /// <param name="strClassName">Имя класса окна панели</param>
    /// <param name="strWindowName">Заголовок окна панели (может быть null)</param>
    public void GetPosition(string strClassName, string strWindowName)
    {
        m_data        = new();
        m_data.cbSize = (uint)Marshal.SizeOf(m_data.GetType());

        if (FindWindow(strClassName, strWindowName) == IntPtr.Zero)
            throw new("Failed to find an AppBar that matched the given criteria");

        if (SHAppBarMessage(ABM_GETTASKBARPOS, ref m_data) != 1)
            throw new("Failed to communicate with the given AppBar");
    }


    /// <summary>Получает положение системной панели задач Windows</summary>
    public void GetSystemTaskBarPosition() => GetPosition("Shell_TrayWnd", null);

    /// <summary>Край экрана, на котором расположена панель задач</summary>
    public enum ScreenEdge
    {
        /// <summary>Положение не определено</summary>
        Undefined = -1,

        /// <summary>Левая сторона экрана</summary>
        Left = ABE_LEFT,

        /// <summary>Верхняя сторона экрана</summary>
        Top = ABE_TOP,

        /// <summary>Правая сторона экрана</summary>
        Right = ABE_RIGHT,

        /// <summary>Нижняя сторона экрана</summary>
        Bottom = ABE_BOTTOM
    }


    [StructLayout(LayoutKind.Sequential)]
    private struct APPBARDATA
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uCallbackMessage;
        public uint uEdge;
        public RECT rc;
        public int lParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }
}