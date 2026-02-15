using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMethodReturnValue.Global

namespace MathCore.WPF.pInvoke;

internal static class User32
{
    private const string FileName = "user32.dll";

    /// <summary>Отправляет сообщение окну</summary>
    /// <param name="hWnd">Дескриптор окна</param>
    /// <param name="Msg">Код сообщения</param>
    /// <param name="wParam">Дополнительный параметр сообщения</param>
    /// <param name="lParam">Дополнительный параметр сообщения</param>
    /// <returns>Результат обработки сообщения</returns>
    [DllImport(FileName, CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(this IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    /// <summary>Отправляет сообщение окну</summary>
    /// <param name="hWnd">Дескриптор окна</param>
    /// <param name="Msg">Код сообщения</param>
    /// <param name="wParam">Дополнительный параметр сообщения</param>
    /// <param name="lParam">Дополнительный параметр сообщения</param>
    /// <returns>Результат обработки сообщения</returns>
    [DllImport(FileName, CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(this IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

    /// <summary>Отправляет сообщение окну</summary>
    /// <param name="window">Окно-получатель</param>
    /// <param name="Msg">Код сообщения</param>
    /// <param name="wParam">Дополнительный параметр сообщения</param>
    /// <param name="lParam">Дополнительный параметр сообщения</param>
    /// <returns>Результат обработки сообщения</returns>
    public static IntPtr SendMessage(this Window window, uint Msg, IntPtr wParam, IntPtr lParam) => SendMessage(window.GetWindowHandle(), Msg, wParam, lParam);

    /// <summary>Отправляет сообщение окну</summary>
    /// <param name="window">Окно-получатель</param>
    /// <param name="Msg">Код сообщения</param>
    /// <param name="wParam">Дополнительный параметр сообщения</param>
    /// <param name="lParam">Дополнительный параметр сообщения</param>
    /// <returns>Результат обработки сообщения</returns>
    public static IntPtr SendMessage(this Window window, WM Msg, IntPtr wParam, IntPtr lParam) => SendMessage(window.GetWindowHandle(), Msg, wParam, lParam);

    /// <summary>Отправляет сообщение окну</summary>
    /// <param name="window">Окно-получатель</param>
    /// <param name="Msg">Код сообщения</param>
    /// <param name="wParam">Дополнительный параметр сообщения</param>
    /// <param name="lParam">Дополнительный параметр сообщения</param>
    /// <returns>Результат обработки сообщения</returns>
    public static IntPtr SendMessage(this Window window, WM Msg, SC wParam, IntPtr lParam = default) => SendMessage(window.GetWindowHandle(), (uint)Msg, (IntPtr)wParam, lParam == default ? (IntPtr)' ' : lParam);

    /// <summary>Получает сведения о мониторе</summary>
    /// <param name="hMonitor">Дескриптор монитора</param>
    /// <param name="lpmi">Структура с информацией о мониторе</param>
    /// <returns><c>true</c>, если вызов успешен; иначе <c>false</c></returns>
    [DllImport(FileName)]
    public static extern bool GetMonitorInfo(this IntPtr hMonitor, MonitorInfo lpmi);

    /// <summary>Получает текущую позицию курсора</summary>
    /// <param name="lpPoint">Координаты курсора</param>
    /// <returns><c>true</c>, если вызов успешен; иначе <c>false</c></returns>
    [DllImport(FileName)]
    public static extern bool GetCursorPos(ref System.Windows.Point lpPoint);

    /// <summary>Возвращает монитор, связанный с окном</summary>
    /// <param name="handle">Дескриптор окна</param>
    /// <param name="flags">Флаги поиска монитора</param>
    /// <returns>Дескриптор монитора</returns>
    [DllImport(FileName)]
    public static extern IntPtr MonitorFromWindow(this IntPtr handle, int flags);

    /// <summary>Регистрирует системную горячую клавишу</summary>
    /// <param name="hWnd">
    /// Дескриптор окна, которое будет получать сообщения WM_HOTKEY, сгенерированные горячей клавишей
    /// Если параметр равен NULL, сообщения WM_HOTKEY отправляются в очередь сообщений вызывающего потока и должны обрабатываться в цикле сообщений
    /// </param>
    /// <param name="id">
    /// Идентификатор горячей клавиши
    /// Если параметр hWnd равен NULL, горячая клавиша связывается с текущим потоком, а не с конкретным окном
    /// </param>
    /// <param name="modifiers">
    /// Клавиши-модификаторы, которые должны быть нажаты вместе с клавишей, указанной в параметре key, чтобы сгенерировать сообщение WM_HOTKEY
    /// Возможные значения: MOD_ALT (0x0001), MOD_CONTROL (0x0002), MOD_SHIFT (0x0004), MOD_WIN (0x0008)
    /// </param>
    /// <param name="key">Виртуальный код клавиши горячей комбинации</param>
    /// <returns><c>true</c>, если регистрация успешна; иначе <c>false</c></returns>
    [DllImport(FileName, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeys modifiers, Keys key);

    /// <summary>Освобождает ранее зарегистрированную горячую клавишу</summary>
    /// <param name="hWnd">
    /// Дескриптор окна, связанного с освобождаемой горячей клавишей
    /// Если горячая клавиша не связана с окном, параметр должен быть NULL
    /// </param>
    /// <param name="id">Идентификатор освобождаемой горячей клавиши</param>
    /// <returns><c>true</c>, если операция успешна; иначе <c>false</c></returns>
    [DllImport(FileName, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    /// <summary>Показывает или скрывает окно</summary>
    /// <param name="hwnd">Дескриптор окна</param>
    /// <param name="command">Код команды отображения</param>
    /// <returns>Результат выполнения операции</returns>
    [DllImport(FileName)]
    public static extern int ShowWindow(IntPtr hwnd, int command);

    /// <summary>Находит окно по классу и заголовку</summary>
    /// <param name="lpClassName">Имя класса окна</param>
    /// <param name="lpWindowName">Заголовок окна</param>
    /// <returns>Дескриптор найденного окна</returns>
    [DllImport(FileName, SetLastError = true)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    /// <summary>Возвращает прямоугольник окна в экранных координатах</summary>
    /// <param name="hWnd">Дескриптор окна</param>
    /// <param name="lpRect">Прямоугольник окна</param>
    /// <returns><c>true</c>, если вызов успешен; иначе <c>false</c></returns>
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(IntPtr hWnd, ref Rect lpRect);
}