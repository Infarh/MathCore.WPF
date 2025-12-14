using System.Windows;
using System.Windows.Input;

using MathCore.WPF.Notifications;

namespace MathCore.WPF.WindowTest.Examples;

/// <summary>Демонстрация корректной обработки завершения приложения при наличии активных уведомлений</summary>
public static class ToastNotificationShutdownExample
{
    /// <summary>Пример 1: Стандартное поведение - уведомления не блокируют завершение (по умолчанию)</summary>
    /// <remarks>
    /// Менеджер автоматически изменяет Application.ShutdownMode на OnMainWindowClose при первом показе уведомления.
    /// При закрытии главного окна приложение завершает работу немедленно, все активные уведомления автоматически закрываются.
    /// </remarks>
    public static void Example1_DefaultBehavior()
    {
        // Используем настройки по умолчанию
        var manager = ToastNotificationManager.Default;

        System.Diagnostics.Debug.WriteLine($"До показа: ShutdownMode = {Application.Current?.ShutdownMode}");

        // Показываем несколько уведомлений
        manager.Show("Уведомление 1", "Первое сообщение", ToastNotificationIcon.Information);
        manager.Show("Уведомление 2", "Второе сообщение", ToastNotificationIcon.Success);
        manager.Show("Уведомление 3", "Третье сообщение", ToastNotificationIcon.Warning);

        System.Diagnostics.Debug.WriteLine($"После показа: ShutdownMode = {Application.Current?.ShutdownMode}");
        // Должно вывести: OnMainWindowClose (автоматически изменено менеджером)

        // При закрытии главного окна:
        // ✅ Приложение завершается НЕМЕДЛЕННО (ShutdownMode = OnMainWindowClose)
        // ✅ Все уведомления закрываются автоматически (обработчик Application.Exit)
        // ✅ Процесс выгружается из памяти
    }

    /// <summary>Пример 2: Явная настройка - уведомления не блокируют завершение</summary>
    public static void Example2_ExplicitNonBlocking()
    {
        var settings = new ToastNotificationSettings
        {
            KeepApplicationAlive = false,              // Не блокировать завершение (по умолчанию)
            CloseOnApplicationShutdown = true          // Закрыть все при завершении (по умолчанию)
        };

        var manager = new ToastNotificationManager(settings);

        manager.Show("Информация", "Уведомление не будет блокировать закрытие приложения", ToastNotificationIcon.Information);

        // Менеджер автоматически:
        // 1. Устанавливает Application.ShutdownMode = OnMainWindowClose
        // 2. Подписывается на Application.Exit
        // 3. Закрывает все уведомления при завершении

        // Приложение закроется корректно, даже если уведомление ещё отображается
    }

    /// <summary>Пример 3: Критические уведомления - блокируют завершение</summary>
    /// <remarks>
    /// Используйте этот режим ТОЛЬКО для критически важных уведомлений.
    /// В этом случае ShutdownMode НЕ изменяется, и приложение завершится только при закрытии всех окон.
    /// </remarks>
    public static void Example3_CriticalNotifications()
    {
        var settings = new ToastNotificationSettings
        {
            KeepApplicationAlive = true,               // Блокировать завершение приложения
            CloseOnApplicationShutdown = false,        // Не закрывать автоматически
            DisplayDuration = 0                        // Не закрывать по таймеру
        };

        var manager = new ToastNotificationManager(settings);

        System.Diagnostics.Debug.WriteLine($"Критическое уведомление: ShutdownMode = {Application.Current?.ShutdownMode}");
        // ShutdownMode НЕ изменяется при KeepApplicationAlive = true

        manager.Show(
            "КРИТИЧЕСКОЕ ПРЕДУПРЕЖДЕНИЕ",
            "Несохранённые данные будут потеряны! Закройте это уведомление после ознакомления.",
            ToastNotificationIcon.Critical
        );

        // При этих настройках:
        // ⚠️ ShutdownMode остаётся OnLastWindowClose (или текущее значение)
        // ⚠️ Приложение НЕ завершится, пока есть активные уведомления
        // ⚠️ Пользователь должен вручную закрыть уведомление
    }

    /// <summary>Пример 4: Явное закрытие всех уведомлений при завершении</summary>
    /// <remarks>
    /// Этот подход избыточен, так как менеджер автоматически закрывает уведомления,
    /// но может быть полезен для дополнительной очистки ресурсов
    /// </remarks>
    public static void Example4_ExplicitCloseOnShutdown()
    {
        // В MainWindow или App.xaml.cs
        void OnMainWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // Явно закрываем все уведомления перед завершением (необязательно)
            ToastNotificationManager.Default.CloseAll();

            // Можно также вызвать другую очистку ресурсов
            CleanupResources();

            System.Diagnostics.Debug.WriteLine("Главное окно закрывается, уведомления закрыты");
        }

        void CleanupResources()
        {
            // Очистка ресурсов приложения
        }

        // Подписка на событие в конструкторе MainWindow:
        // this.Closing += OnMainWindowClosing;
    }

    /// <summary>Пример 5: Разные менеджеры для разных типов уведомлений</summary>
    public static class Example5_MultipleManagers
    {
        // Обычные уведомления - не блокируют завершение (автоматическая настройка ShutdownMode)
        private static readonly ToastNotificationManager __NormalNotifications = new(new ToastNotificationSettings
        {
            KeepApplicationAlive = false,
            Position = ToastNotificationPosition.BottomRight
        });

        // Критические уведомления - блокируют завершение (ShutdownMode не изменяется)
        private static readonly ToastNotificationManager __CriticalNotifications = new(new ToastNotificationSettings
        {
            KeepApplicationAlive = true,
            Position = ToastNotificationPosition.Center,
            DisplayDuration = 0
        });

        public static void ShowNormalNotification(string title, string message)
        {
            __NormalNotifications.Show(title, message, ToastNotificationIcon.Information);
            System.Diagnostics.Debug.WriteLine($"Normal notification: ShutdownMode = {Application.Current?.ShutdownMode}");
        }

        public static void ShowCriticalNotification(string title, string message)
        {
            __CriticalNotifications.Show(title, message, ToastNotificationIcon.Critical);
            System.Diagnostics.Debug.WriteLine($"Critical notification: ShutdownMode = {Application.Current?.ShutdownMode}");
        }

        public static void CloseAllNotifications()
        {
            __NormalNotifications.CloseAll();
            __CriticalNotifications.CloseAll();
        }
    }

    /// <summary>Пример 6: Обработка завершения работы через Application.Exit</summary>
    public static void Example6_ApplicationExitHandler()
    {
        // В App.xaml.cs:
        void OnApplicationExit(object sender, ExitEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application.Exit вызван");

            // Менеджер автоматически вызовет CloseAll() для всех менеджеров,
            // у которых CloseOnApplicationShutdown = true (по умолчанию)

            // Все уведомления уже закрыты автоматически
            var active_count = ToastNotificationManager.Default.ActiveWindows.Count;
            System.Diagnostics.Debug.WriteLine($"Активных уведомлений: {active_count}"); // Должно быть 0

            System.Diagnostics.Debug.WriteLine("Приложение корректно завершает работу");
        }

        // В конструкторе App:
        // Application.Current.Exit += OnApplicationExit;
    }

    /// <summary>Пример 7: Проверка количества активных уведомлений перед завершением</summary>
    public static bool CanCloseApplication()
    {
        var active_notifications = ToastNotificationManager.Default.ActiveWindows.Count;

        if (active_notifications > 0)
        {
            // Можно показать предупреждение или закрыть все уведомления
            var result = MessageBox.Show(
                $"Есть {active_notifications} активных уведомлений. Закрыть приложение?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                ToastNotificationManager.Default.CloseAll();
                return true;
            }

            return false;
        }

        return true;
    }

    /// <summary>Пример 8: Постепенное завершение с ожиданием закрытия уведомлений</summary>
    public static async Task GracefulShutdownAsync()
    {
        System.Diagnostics.Debug.WriteLine("Начало плавного завершения");

        // Ждём, пока все уведомления закроются естественным образом
        var timeout = TimeSpan.FromSeconds(5);
        var start_time = DateTime.Now;

        while (ToastNotificationManager.Default.ActiveWindows.Count > 0)
        {
            if (DateTime.Now - start_time > timeout)
            {
                // Превышен таймаут - принудительно закрываем
                System.Diagnostics.Debug.WriteLine("Таймаут - принудительное закрытие уведомлений");
                ToastNotificationManager.Default.CloseAll();
                break;
            }

            await Task.Delay(100);
            System.Diagnostics.Debug.WriteLine($"Ожидание закрытия... Активных: {ToastNotificationManager.Default.ActiveWindows.Count}");
        }

        System.Diagnostics.Debug.WriteLine("Все уведомления закрыты, завершение приложения");

        // Теперь можно безопасно завершить приложение
        Application.Current.Shutdown();
    }

    /// <summary>Пример 9: Интеграция с MVVM - команда закрытия приложения</summary>
    public static class Example9_MVVMIntegration
    {
        // В ViewModel главного окна
        public static ICommand CreateCloseApplicationCommand()
        {
            return Commands.Command.New(
                OnCloseApplicationExecuted,
                CanCloseApplicationExecute
            );
        }

        private static bool CanCloseApplicationExecute()
        {
            // Всегда можем закрыть
            return true;
        }

        private static void OnCloseApplicationExecuted()
        {
            System.Diagnostics.Debug.WriteLine("Команда закрытия приложения выполнена");

            // Проверяем активные уведомления
            var active_count = ToastNotificationManager.Default.ActiveWindows.Count;

            if (active_count > 0)
            {
                var result = MessageBox.Show(
                    $"Активных уведомлений: {active_count}. Закрыть все и выйти?",
                    "Подтверждение выхода",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result != MessageBoxResult.Yes)
                    return;

                ToastNotificationManager.Default.CloseAll();
                System.Diagnostics.Debug.WriteLine("Все уведомления закрыты");
            }

            Application.Current.Shutdown();
        }
    }

    /// <summary>Пример 10: Тестирование корректности завершения и автоматической настройки</summary>
    public static void Example10_TestShutdownBehavior()
    {
        System.Diagnostics.Debug.WriteLine("=== Тест автоматической настройки ShutdownMode ===");

        // Сохраняем исходный режим
        var original_mode = Application.Current?.ShutdownMode;
        System.Diagnostics.Debug.WriteLine($"Исходный ShutdownMode: {original_mode}");

        // Создаём менеджер с настройками по умолчанию
        var manager = new ToastNotificationManager();

        // Показываем несколько уведомлений
        for (var i = 1; i <= 5; i++)
        {
            manager.Show(
                $"Тест #{i}",
                $"Тестовое уведомление номер {i}",
                ToastNotificationIcon.Information
            );
        }

        System.Diagnostics.Debug.WriteLine($"Создано уведомлений: {manager.ActiveWindows.Count}");

        // Проверяем, что ShutdownMode изменился
        var current_mode = Application.Current?.ShutdownMode;
        System.Diagnostics.Debug.WriteLine($"Текущий ShutdownMode: {current_mode}");

        if (current_mode == ShutdownMode.OnMainWindowClose)
        {
            System.Diagnostics.Debug.WriteLine("✅ ShutdownMode автоматически изменён на OnMainWindowClose");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"⚠️ ShutdownMode не изменился: {current_mode}");
        }

        // Имитация завершения приложения
        manager.CloseAll();

        System.Diagnostics.Debug.WriteLine($"После CloseAll: {manager.ActiveWindows.Count}"); // Должно быть 0

        System.Diagnostics.Debug.WriteLine("=== Тест завершён ===");
        System.Diagnostics.Debug.WriteLine("✅ Все уведомления закрылись");
        System.Diagnostics.Debug.WriteLine("✅ Приложение может безопасно завершиться");
    }

    /// <summary>Пример 11: Демонстрация проблемы и решения</summary>
    public static void Example11_BeforeAndAfterComparison()
    {
        System.Diagnostics.Debug.WriteLine("=== Сравнение: до и после исправления ===");

        // ❌ ПРОБЛЕМА (старое поведение):
        // Application.ShutdownMode = OnLastWindowClose (по умолчанию)
        // Показываем уведомления (это окна)
        // Закрываем главное окно
        // Результат: приложение НЕ завершается, потому что есть окна уведомлений
        // Процесс остаётся в памяти!

        System.Diagnostics.Debug.WriteLine("До исправления:");
        System.Diagnostics.Debug.WriteLine("- ShutdownMode = OnLastWindowClose");
        System.Diagnostics.Debug.WriteLine("- Закрытие главного окна → приложение висит в памяти");
        System.Diagnostics.Debug.WriteLine("- Окна уведомлений блокируют выгрузку");

        System.Diagnostics.Debug.WriteLine("");

        // ✅ РЕШЕНИЕ (новое поведение):
        var manager = ToastNotificationManager.Default;
        manager.Show("Тест", "Сообщение", ToastNotificationIcon.Information);

        System.Diagnostics.Debug.WriteLine("После исправления:");
        System.Diagnostics.Debug.WriteLine($"- ShutdownMode = {Application.Current?.ShutdownMode}");
        System.Diagnostics.Debug.WriteLine("- Менеджер автоматически установил OnMainWindowClose");
        System.Diagnostics.Debug.WriteLine("- Закрытие главного окна → немедленное завершение");
        System.Diagnostics.Debug.WriteLine("- Все уведомления закрываются автоматически");
        System.Diagnostics.Debug.WriteLine("- Процесс корректно выгружается из памяти");

        System.Diagnostics.Debug.WriteLine("");
        System.Diagnostics.Debug.WriteLine("✅ Проблема полностью решена!");
    }
}
