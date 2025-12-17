using MathCore.WPF.Notifications;

namespace MathCore.WPF.WindowTest.Examples;

/// <summary>Примеры использования системы Toast-уведомлений</summary>
public static class ToastNotificationExamples
{
    /// <summary>Пример 1: Простейшее использование</summary>
    public static void Example1_SimpleUsage()
    {
        // Самый простой способ показать уведомление
        ToastNotificationManager.Default.Show(
            "Информация", 
            "Это простое уведомление", 
            ToastNotificationIcon.Information
        );
    }

    /// <summary>Пример 2: Разные типы уведомлений</summary>
    public static void Example2_DifferentTypes()
    {
        var manager = ToastNotificationManager.Default;

        // Информация
        manager.Show("Информация", "Информационное сообщение", ToastNotificationIcon.Information);

        // Успех
        manager.Show("Успешно", "Операция выполнена успешно", ToastNotificationIcon.Success);

        // Предупреждение
        manager.Show("Внимание", "Обратите внимание на это", ToastNotificationIcon.Warning);

        // Ошибка
        manager.Show("Ошибка", "Произошла ошибка", ToastNotificationIcon.Error);

        // Критическая ошибка
        manager.Show("Критично", "Критическая ошибка системы!", ToastNotificationIcon.Critical);

        // Вопрос
        manager.Show("Вопрос", "Вы уверены?", ToastNotificationIcon.Question);

        // Без иконки
        manager.Show("Сообщение", "Уведомление без иконки", ToastNotificationIcon.None);
    }

    /// <summary>Пример 3: Настройка менеджера</summary>
    public static ToastNotificationManager Example3_CustomSettings()
    {
        // Создаём настройки
        var settings = new ToastNotificationSettings
        {
            Position = ToastNotificationPosition.TopRight,  // Правый верхний угол
            DisplayDuration = 3000,                         // 3 секунды
            FadeInDuration = 250,                           // анимация появления
            FadeOutDuration = 250,                          // анимация исчезновения
            MaxVisibleNotifications = 3,                    // максимум 3 одновременно
            ScreenMargin = 20,                              // отступ 20 пикселей
            NotificationSpacing = 10,                       // расстояние между уведомлениями
            Width = 400,                                    // ширина 400 пикселей
            MinHeight = 100,
            MaxHeight = 300,
            PlaySound = true                                // воспроизводить звуки
        };

        // Создаём менеджер с настройками
        var manager = new ToastNotificationManager(settings);

        return manager;
    }

    /// <summary>Пример 4: Разные позиции</summary>
    public static void Example4_DifferentPositions()
    {
        // Создаём менеджеры для разных позиций
        var positions = new[]
        {
            ToastNotificationPosition.BottomRight,
            ToastNotificationPosition.TopRight,
            ToastNotificationPosition.BottomLeft,
            ToastNotificationPosition.TopLeft,
            ToastNotificationPosition.Center,
            ToastNotificationPosition.TopCenter,
            ToastNotificationPosition.BottomCenter
        };

        foreach (var position in positions)
        {
            var settings = new ToastNotificationSettings { Position = position };
            var manager = new ToastNotificationManager(settings);
            
            manager.Show(
                $"Позиция: {position}", 
                $"Уведомление в позиции {position}", 
                ToastNotificationIcon.Information
            );

            // Небольшая задержка между показами
            System.Threading.Thread.Sleep(500);
        }
    }

    /// <summary>Пример 5: Обработка событий</summary>
    public static void Example5_EventHandling()
    {
        // Создаём ViewModel уведомления
        var viewModel = new ToastNotificationViewModel
        {
            Title = "Кликните меня",
            Message = "Попробуйте кликнуть по уведомлению",
            Icon = ToastNotificationIcon.Question
        };

        // Подписываемся на события
        viewModel.LeftClick += (sender, args) =>
        {
            System.Diagnostics.Debug.WriteLine("Левая кнопка мыши нажата!");
            
            // Показываем новое уведомление
            ToastNotificationManager.Default.Show(
                "Клик обработан", 
                "Вы кликнули левой кнопкой", 
                ToastNotificationIcon.Success
            );
        };

        viewModel.RightClick += (sender, args) =>
        {
            System.Diagnostics.Debug.WriteLine("Правая кнопка мыши нажата!");
            
            ToastNotificationManager.Default.Show(
                "Клик обработан", 
                "Вы кликнули правой кнопкой", 
                ToastNotificationIcon.Information
            );
        };

        viewModel.CloseRequested += (sender, args) =>
        {
            System.Diagnostics.Debug.WriteLine("Уведомление закрывается");
        };

        // Показываем уведомление
        ToastNotificationManager.Default.Show(viewModel);
    }

    /// <summary>Пример 6: Пользовательское содержимое</summary>
    public static void Example6_CustomContent()
    {
        // Создаём пользовательский контрол с ProgressBar
        var customContent = new System.Windows.Controls.StackPanel
        {
            Orientation = System.Windows.Controls.Orientation.Vertical,
            Children =
            {
                new System.Windows.Controls.TextBlock 
                { 
                    Text = "Загрузка данных...", 
                    FontWeight = System.Windows.FontWeights.Bold,
                    Margin = new System.Windows.Thickness(0, 0, 0, 5)
                },
                new System.Windows.Controls.ProgressBar 
                { 
                    Height = 20, 
                    IsIndeterminate = true,
                    Margin = new System.Windows.Thickness(0, 5, 0, 0)
                },
                new System.Windows.Controls.TextBlock
                {
                    Text = "Пожалуйста, подождите...",
                    FontSize = 11,
                    Foreground = System.Windows.Media.Brushes.Gray,
                    Margin = new System.Windows.Thickness(0, 5, 0, 0)
                }
            }
        };

        // Показываем уведомление с пользовательским содержимым
        ToastNotificationManager.Default.ShowCustom(
            "Загрузка", 
            customContent, 
            ToastNotificationIcon.None
        );
    }

    /// <summary>Пример 7: Несколько уведомлений с очередью</summary>
    public static void Example7_QueueManagement()
    {
        // Настраиваем менеджер с ограничением в 3 уведомления
        var settings = new ToastNotificationSettings
        {
            MaxVisibleNotifications = 3,
            DisplayDuration = 2000
        };
        
        var manager = new ToastNotificationManager(settings);

        // Показываем 6 уведомлений - они встанут в очередь
        for (var i = 1; i <= 6; i++)
        {
            manager.Show(
                $"Уведомление #{i}", 
                $"Это уведомление номер {i} из 6", 
                ToastNotificationIcon.Information
            );
        }
        
        // Первые 3 будут показаны сразу, остальные 3 встанут в очередь
        // и будут показаны автоматически по мере закрытия предыдущих
    }

    /// <summary>Пример 8: Закрытие всех уведомлений</summary>
    public static void Example8_CloseAll()
    {
        var manager = ToastNotificationManager.Default;

        // Показываем несколько уведомлений
        for (var i = 1; i <= 5; i++)
        {
            manager.Show(
                $"Сообщение {i}", 
                $"Содержимое сообщения {i}", 
                ToastNotificationIcon.Information
            );
        }

        // Ждём немного
        System.Threading.Thread.Sleep(1000);

        // Закрываем все уведомления одной командой
        manager.CloseAll();
    }

    /// <summary>Пример 9: Использование в асинхронных операциях</summary>
    public static async Task Example9_AsyncOperations()
    {
        var manager = ToastNotificationManager.Default;

        // Показываем уведомление о начале операции
        manager.Show(
            "Загрузка", 
            "Начинается загрузка данных...", 
            ToastNotificationIcon.Information
        );

        try
        {
            // Имитация асинхронной операции
            await Task.Delay(3000);

            // Уведомление об успехе
            manager.Show(
                "Готово", 
                "Данные успешно загружены!", 
                ToastNotificationIcon.Success
            );
        }
        catch (Exception ex)
        {
            // Уведомление об ошибке
            manager.Show(
                "Ошибка", 
                $"Произошла ошибка: {ex.Message}", 
                ToastNotificationIcon.Error
            );
        }
    }

    /// <summary>Пример 10: Интеграция с логированием</summary>
    public static void Example10_LoggingIntegration()
    {
        var manager = ToastNotificationManager.Default;

        // Функция для логирования с уведомлением
        void LogWithNotification(string level, string message)
        {
            // Логируем в консоль/файл
            System.Diagnostics.Debug.WriteLine($"[{level}] {message}");

            // Показываем уведомление
            var icon = level switch
            {
                "ERROR" => ToastNotificationIcon.Error,
                "WARN" => ToastNotificationIcon.Warning,
                "INFO" => ToastNotificationIcon.Information,
                "SUCCESS" => ToastNotificationIcon.Success,
                _ => ToastNotificationIcon.None
            };

            manager.Show(level, message, icon);
        }

        // Использование
        LogWithNotification("INFO", "Приложение запущено");
        LogWithNotification("WARN", "Низкий уровень заряда батареи");
        LogWithNotification("ERROR", "Не удалось подключиться к серверу");
        LogWithNotification("SUCCESS", "Синхронизация завершена");
    }
}
