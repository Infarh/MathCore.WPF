using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MathCore.WPF.Commands;

/// <summary>Универсальная команда захвата экрана с поддержкой создания PNG и GIF</summary>
[MarkupExtensionReturnType(typeof(ScreenShotCommand))]
public sealed class ScreenShotCommand : Command
{
    /// <summary>Длительность записи GIF в секундах (0 для одиночного кадра)</summary>
    public double GifLengthSec { get; set; } = 0;

    /// <summary>Режим захвата по умолчанию, если параметр не указан</summary>
    public CaptureFallbackMode DefaultMode { get; set; } = CaptureFallbackMode.MainWindow;

    /// <summary>Каталог для сохранения скриншотов <c>./screenshots</c></summary>
    /// <remarks>Если указан относительный путь, он будет использоваться относительно каталога приложения</remarks>
    public string ScreenshotsDirectory { get; set; } = Path.Combine(AppContext.BaseDirectory, "screenshots");

    /// <summary>Формат имени файла для одиночных скриншотов без расширения <c>screen[{timestamp}]</c></summary>
    /// <remarks>Поддерживает токен {timestamp} который заменяется на текущее UTC время</remarks>
    public string FileNameFormat { get; set; } = "screen[{timestamp}]";

    /// <summary>Формат имени файла для GIF анимаций без расширения <c>anim[{timestamp}]</c></summary>
    /// <remarks>Поддерживает токен {timestamp} который заменяется на текущее UTC время</remarks>
    public string FileNameFormatGif { get; set; } = "anim[{timestamp}]";

    public override bool CanExecute(object? parameter) => true;

    /// <summary>Выполняет захват экрана в режиме GIF или PNG в зависимости от настроек</summary>
    /// <param name="parameter">Объект для захвата (Window, UIElement) или null для режима по умолчанию</param>
    public override async void Execute(object? parameter)
    {
        if (GifLengthSec > 0) // GIF режим
        {
            await CaptureGifAsync(parameter);
            return;
        }

        // Одиночный кадр
        var frame = CaptureFrame(parameter);
        SavePng(frame);
    }

    /// <summary>Захватывает последовательность кадров и сохраняет в GIF</summary>
    /// <param name="parameter">Объект для захвата</param>
    private async Task CaptureGifAsync(object? parameter)
    {
        var duration_ms = (int)(GifLengthSec * 1000);
        const int interval = 100; // 10 FPS
        var frame_capacity = (int)Math.Ceiling(duration_ms / (double)interval);
        var frames = new List<BitmapFrame>(frame_capacity);

        var timer = Stopwatch.StartNew();
        while (timer.ElapsedMilliseconds < duration_ms)
        {
            var frame_source = CaptureFrame(parameter);
            var frame = BitmapFrame.Create(frame_source);
            frames.Add(frame);
            await Task.Delay(interval);
        }

        SaveGif(frames);
    }

    /// <summary>Захватывает одиночный кадр в зависимости от типа параметра</summary>
    /// <param name="parameter">Объект для захвата</param>
    /// <returns>Захваченное изображение</returns>
    private BitmapSource CaptureFrame(object? parameter) => parameter switch
    {
        Window win => CaptureWindow(win), // Окно WPF
        UIElement ui => CaptureUIElement(ui), // UI элемент WPF
        _ => CaptureFallback() // Режим по умолчанию
    };

    /// <summary>Захватывает кадр согласно режиму по умолчанию</summary>
    /// <returns>Захваченное изображение</returns>
    private BitmapSource CaptureFallback() => DefaultMode switch
    {
        CaptureFallbackMode.MainWindow => CaptureWindow(Application.Current.MainWindow), // Главное окно приложения
        CaptureFallbackMode.ActiveWindow => CaptureActiveWindow(), // Активное окно ОС
        CaptureFallbackMode.MousePosition => CaptureScreenAtCursor(), // Монитор под курсором
        CaptureFallbackMode.Panorama => CaptureAllScreens(), // Все мониторы
        _ => CaptureWindow(Application.Current.MainWindow) // Безопасный fallback
    };

    /// <summary>Захватывает UI элемент WPF через рендеринг</summary>
    /// <param name="element">UI элемент для захвата</param>
    /// <returns>Захваченное изображение</returns>
    private static BitmapSource CaptureUIElement(UIElement element)
    {
        var size = new System.Windows.Size(element.RenderSize.Width, element.RenderSize.Height);
        var renderer = new RenderTargetBitmap(
            (int)size.Width, (int)size.Height,
            96, 96, PixelFormats.Pbgra32);

        renderer.Render(element);
        return renderer;
    }

    /// <summary>Захватывает активное окно операционной системы</summary>
    /// <returns>Захваченное изображение</returns>
    private static BitmapSource CaptureActiveWindow()
    {
        var hwnd = GetForegroundWindow();
        return CaptureScreenFromHandle(hwnd);
    }

    /// <summary>Захватывает монитор, на котором находится курсор мыши</summary>
    /// <returns>Захваченное изображение</returns>
    private static BitmapSource CaptureScreenAtCursor()
    {
        GetCursorPos(out var p);
        var monitor = MonitorFromPoint(p, MONITOR_DEFAULTTONEAREST);
        return CaptureMonitorBounds(monitor);
    }

    /// <summary>Захватывает панораму всех подключенных мониторов</summary>
    /// <returns>Захваченное изображение</returns>
    private static BitmapSource CaptureAllScreens()
    {
        var bounds = GetAllMonitorsBounds();
        return CaptureScreenBounds(bounds);
    }

    /// <summary>Захватывает монитор, на котором расположено окно с указанным дескриптором</summary>
    /// <param name="hwnd">Дескриптор окна</param>
    /// <returns>Захваченное изображение</returns>
    private static BitmapSource CaptureScreenFromHandle(IntPtr hwnd)
    {
        if (hwnd == IntPtr.Zero)
            return CaptureAllScreens(); // Без привязки к окну берём панораму

        var monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
        return CaptureMonitorBounds(monitor);
    }

    /// <summary>Захватывает окно WPF</summary>
    /// <param name="window">Окно для захвата</param>
    /// <returns>Захваченное изображение</returns>
    private static BitmapSource CaptureWindow(Window window)
    {
        if (window is null) throw new ArgumentNullException(nameof(window));

        var hwnd = new WindowInteropHelper(window).Handle;
        var rect = GetWindowRect(hwnd);
        return CaptureScreenBounds(rect);
    }

    /// <summary>Захватывает область экрана с указанными границами</summary>
    /// <param name="bounds">Границы области захвата</param>
    /// <returns>Захваченное изображение</returns>
    private static BitmapSource CaptureScreenBounds(System.Drawing.Rectangle bounds)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return BitmapSource.Create(1, 1, 96, 96, PixelFormats.Pbgra32, null, new byte[4], 4); // Защита от невалидных размеров

        using var bmp = new Bitmap(bounds.Width, bounds.Height);
        using var g = Graphics.FromImage(bmp);

        g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy); // Источник экран

        var hbitmap = bmp.GetHbitmap();
        try
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                hbitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
        finally
        {
            DeleteObject(hbitmap); // Предотвращаем утечку GDI
        }
    }

    /// <summary>Сохраняет одиночный кадр в PNG файл</summary>
    /// <param name="frame">Кадр для сохранения</param>
    private void SavePng(BitmapSource frame)
    {
        if (frame is null) throw new ArgumentNullException(nameof(frame));

        // Подготавливаем каталог для сохранения (поддержка относительных путей)
        var dir = Path.IsPathRooted(ScreenshotsDirectory)
            ? ScreenshotsDirectory
            : Path.Combine(AppContext.BaseDirectory, ScreenshotsDirectory);

        Directory.CreateDirectory(dir);

        // Формируем имя файла с заменой токена {timestamp}
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss.fff");
        var name = (FileNameFormat ?? "screen[{timestamp}]").Replace("{timestamp}", timestamp);
        name = SanitizeFileName(name);
        var file = Path.Combine(dir, name + ".png");

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(frame));

        using var fs = File.Create(file);
        encoder.Save(fs);
    }

    /// <summary>Сохраняет последовательность кадров в GIF файл</summary>
    /// <param name="frames">Кадры для сохранения</param>
    private void SaveGif(IEnumerable<BitmapFrame> frames)
    {
        if (frames is null) throw new ArgumentNullException(nameof(frames));

        // Подготавливаем каталог для сохранения (поддержка относительных путей)
        var dir = Path.IsPathRooted(ScreenshotsDirectory)
            ? ScreenshotsDirectory
            : Path.Combine(AppContext.BaseDirectory, ScreenshotsDirectory);

        Directory.CreateDirectory(dir);

        // Формируем имя файла с заменой токена {timestamp}
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss.fff");
        var name = (FileNameFormatGif ?? "anim[{timestamp}]").Replace("{timestamp}", timestamp);
        name = SanitizeFileName(name);
        var file = Path.Combine(dir, name + ".gif");

        var encoder = new GifBitmapEncoder();
        foreach (var f in frames)
            encoder.Frames.Add(f);

        using var fs = File.Create(file);
        encoder.Save(fs);
    }

    /// <summary>Возвращает дескриптор активного окна на текущем рабочем столе</summary>
    /// <returns>Дескриптор окна или <see cref="IntPtr.Zero"/></returns>
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    /// <summary>Возвращает позицию курсора в экранных координатах</summary>
    /// <param name="lpPoint">Экранные координаты курсора</param>
    /// <returns><c>true</c> при успехе</returns>
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    /// <summary>Возвращает дескриптор монитора,Nearest к заданной точке</summary>
    /// <param name="pt">Точка в экранных координатах</param>
    /// <param name="dwFlags">Флаги выбора монитора</param>
    /// <returns>Дескриптор монитора или <see cref="IntPtr.Zero"/></returns>
    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

    /// <summary>Возвращает дескриптор монитора,Nearest к окну</summary>
    /// <param name="hwnd">Дескриптор окна</param>
    /// <param name="dwFlags">Флаги выбора монитора</param>
    /// <returns>Дескриптор монитора или <see cref="IntPtr.Zero"/></returns>
    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    /// <summary>Заполняет структуру с информацией о мониторе</summary>
    /// <param name="hMonitor">Дескриптор монитора</param>
    /// <param name="lpmi">Структура для заполнения, поле <c>cbSize</c> должно быть инициализировано</param>
    /// <returns><c>true</c> при успехе</returns>
    [DllImport("user32.dll")]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    /// <summary>Перечисляет мониторы, отображаемые на указанном контексте устройства</summary>
    /// <param name="hdc">Контекст устройства или <see cref="IntPtr.Zero"/> для всего экрана</param>
    /// <param name="lprcClip">Область ограничения или <see cref="IntPtr.Zero"/></param>
    /// <param name="lpfnEnum">Callback, вызываемый для каждого монитора</param>
    /// <param name="dwData">Пользовательские данные для callback</param>
    /// <returns><c>true</c> при успехе</returns>
    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

    /// <summary>Возвращает прямоугольник окна в экранных координатах</summary>
    /// <param name="hwnd">Дескриптор окна</param>
    /// <param name="lpRect">Прямоугольник окна в экранных координатах</param>
    /// <returns><c>true</c> при успехе</returns>
    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

    /// <summary>Освобождает GDI-объект, созданный через WinAPI</summary>
    /// <param name="hObject">Дескриптор GDI-объекта</param>
    /// <returns><c>true</c> при успехе</returns>
    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    private const uint MONITOR_DEFAULTTONEAREST = 2;

    private static string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "screen";

        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');

        return name;
    }

    /// <summary>Получает прямоугольник окна по его дескриптору</summary>
    /// <param name="hwnd">Дескриптор окна</param>
    /// <returns>Прямоугольник окна или пустой прямоугольник при ошибке</returns>
    private static System.Drawing.Rectangle GetWindowRect(IntPtr hwnd)
    {
        if (hwnd == IntPtr.Zero)
            return default;

        if (!GetWindowRect(hwnd, out var rect))
            return default;

        return rect.ToRectangle();
    }

    /// <summary>Захватывает область монитора по его дескриптору</summary>
    /// <param name="monitor">Дескриптор монитора</param>
    /// <returns>Захваченное изображение</returns>
    private static BitmapSource CaptureMonitorBounds(IntPtr monitor)
    {
        var bounds = GetMonitorBounds(monitor);
        return CaptureScreenBounds(bounds);
    }

    /// <summary>Получает границы монитора по его дескриптору</summary>
    /// <param name="monitor">Дескриптор монитора</param>
    /// <returns>Прямоугольник монитора или пустой прямоугольник при ошибке</returns>
    private static System.Drawing.Rectangle GetMonitorBounds(IntPtr monitor)
    {
        if (monitor == IntPtr.Zero)
            return default;

        var info = new MONITORINFO(cbSize: Marshal.SizeOf<MONITORINFO>());
        if (!GetMonitorInfo(monitor, ref info))
            return default;

        return info.rcMonitor.ToRectangle();
    }

    /// <summary>Вычисляет объединённые границы всех подключённых мониторов</summary>
    /// <returns>Прямоугольник, охватывающий все мониторы</returns>
    private static System.Drawing.Rectangle GetAllMonitorsBounds()
    {
        var any = false;
        var union = default(System.Drawing.Rectangle);

        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
            (hMonitor, _, _, _) =>
            {
                var bounds = GetMonitorBounds(hMonitor);
                if (bounds.IsEmpty)
                    return true; // Пропускаем невалидные мониторы

                union = any ? Rectangle.Union(union, bounds) : bounds;
                any = true;
                return true; // Продолжаем перечисление
            },
            IntPtr.Zero);

        return union;
    }

    /// <summary>Точка в экранных координатах WinAPI</summary>
    private struct POINT { public int X; public int Y; }

    /// <summary>Прямоугольник WinAPI в экранных координатах</summary>
    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        /// <summary>Преобразует WinAPI прямоугольник в <see cref="Rectangle"/></summary>
        /// <returns>Эквивалентный <see cref="Rectangle"/></returns>
        public System.Drawing.Rectangle ToRectangle() =>
            Rectangle.FromLTRB(Left, Top, Right, Bottom);
    }

    /// <summary>WinAPI структура с информацией о мониторе</summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;

        public MONITORINFO(int cbSize)
        {
            this.cbSize = cbSize;
            rcMonitor = default;
            rcWork = default;
            dwFlags = 0;
        }
    }

    /// <summary>Callback WinAPI для перечисления мониторов</summary>
    /// <param name="hMonitor">Дескриптор монитора</param>
    /// <param name="hdcMonitor">Контекст устройства монитора</param>
    /// <param name="lprcMonitor">Указатель на прямоугольник монитора</param>
    /// <param name="dwData">Пользовательские данные</param>
    /// <returns><c>true</c> чтобы продолжить перечисление</returns>
    private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, IntPtr lprcMonitor, IntPtr dwData);
}

/// <summary>Режимы захвата экрана по умолчанию</summary>
public enum CaptureFallbackMode
{
    /// <summary>Главное окно приложения</summary>
    MainWindow,
    /// <summary>Активное окно операционной системы</summary>
    ActiveWindow,
    /// <summary>Монитор под курсором мыши</summary>
    MousePosition,
    /// <summary>Панорама всех мониторов</summary>
    Panorama
}
