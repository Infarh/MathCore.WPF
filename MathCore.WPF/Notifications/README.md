# Toast Notifications для MathCore.WPF

Система всплывающих уведомлений в стиле Windows для WPF приложений с полной поддержкой MVVM.

## Возможности

✅ **Простой API** - минимум кода для показа уведомлений  
✅ **MVVM-паттерн** - команды и привязки данных  
✅ **Настраиваемость** - позиция, длительность, анимация, стили  
✅ **Очередь уведомлений** - автоматическое управление несколькими уведомлениями  
✅ **7 типов иконок** - Information, Success, Warning, Error, Critical, Question, None  
✅ **7 позиций** - все углы экрана + центр  
✅ **Взаимодействие** - клики мышью, клавиатура, события  
✅ **Пользовательский контент** - любой XAML внутри уведомления  
✅ **Не блокирует завершение приложения** - автоматическая настройка ShutdownMode  
✅ **Кроссплатформенность** - .NET Framework 4.6.1 — .NET 10  

## ⚠️ КРИТИЧЕСКИ ВАЖНО: Завершение работы приложения

### Как это работает

WPF приложения по умолчанию используют `ShutdownMode.OnLastWindowClose`, что означает завершение работы при закрытии **последнего** окна. Поскольку уведомления — это тоже окна, они **блокировали бы** выгрузку приложения.

**Решение:** При первом показе уведомления менеджер **автоматически** изменяет режим завершения на `ShutdownMode.OnMainWindowClose`, если `KeepApplicationAlive = false` (по умолчанию).

### Что происходит автоматически

```csharp
// При первом вызове Show()
ToastNotificationManager.Default.Show("Заголовок", "Сообщение", ToastNotificationIcon.Information);

// Менеджер автоматически выполняет:
if (Application.Current.ShutdownMode == ShutdownMode.OnLastWindowClose)
{
    Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
}
// + Подписывается на Application.Current.Exit
// + Автоматически закрывает все уведомления при завершении
```

### Результат

✅ Закрытие главного окна → немедленное завершение приложения  
✅ Все активные уведомления закрываются автоматически  
✅ Процесс корректно выгружается из памяти  
✅ Очередь уведомлений очищается  
✅ Никаких ручных настроек не требуется  

## Быстрый старт

### 1. Простейшее использование

```csharp
using MathCore.WPF.Notifications;

// Показать уведомление через синглтон
ToastNotificationManager.Default.Show(
    "Операция выполнена", 
    "Файл успешно сохранён", 
    ToastNotificationIcon.Success
);

// Приложение закроется корректно даже при активных уведомлениях
```

### 2. Использование в MVVM

#### ViewModel

```csharp
using MathCore.WPF.Commands;
using MathCore.WPF.Notifications;
using MathCore.WPF.ViewModels;

public class MyViewModel : ViewModel
{
    private readonly ToastNotificationManager _notificationManager = 
        ToastNotificationManager.Default;

    public ICommand SaveCommand => field ??= Command.New(
        OnSaveExecuted,
        CanSaveExecute);

    private bool CanSaveExecute() => /* логика проверки */;

    private void OnSaveExecuted()
    {
        try
        {
            // Логика сохранения
            SaveData();

            // Показываем уведомление об успехе
            _notificationManager.Show(
                "Сохранено", 
                "Данные успешно сохранены", 
                ToastNotificationIcon.Success
            );
        }
        catch (Exception ex)
        {
            // Показываем уведомление об ошибке
            _notificationManager.Show(
                "Ошибка сохранения", 
                ex.Message, 
                ToastNotificationIcon.Error
            );
        }
    }
}
```

#### XAML

```xaml
<Window xmlns:notifications="clr-namespace:MathCore.WPF.Notifications;assembly=MathCore.WPF">
    <Button Command="{Binding SaveCommand}" Content="Сохранить"/>
</Window>
```

### 3. Использование команд напрямую в XAML

```xaml
<Window xmlns:notifications="clr-namespace:MathCore.WPF.Notifications;assembly=MathCore.WPF">
    <!-- Информационное уведомление -->
    <Button Content="Показать информацию">
        <Button.Command>
            <notifications:ShowInformationToastCommand 
                Message="Это информационное сообщение"/>
        </Button.Command>
    </Button>

    <!-- Успех -->
    <Button Content="Показать успех">
        <Button.Command>
            <notifications:ShowSuccessToastCommand 
                Title="Готово"
                Message="Операция завершена"/>
        </Button.Command>
    </Button>

    <!-- Предупреждение -->
    <Button Content="Показать предупреждение">
        <Button.Command>
            <notifications:ShowWarningToastCommand 
                Message="Внимание! Проверьте данные"/>
        </Button.Command>
    </Button>

    <!-- Ошибка -->
    <Button Content="Показать ошибку">
        <Button.Command>
            <notifications:ShowErrorToastCommand 
                Message="Произошла ошибка"/>
        </Button.Command>
    </Button>
</Window>
```

## Настройка менеджера

### Индивидуальный менеджер с настройками

```csharp
var settings = new ToastNotificationSettings
{
    Position = ToastNotificationPosition.TopRight,
    DisplayDuration = 3000,                    // 3 секунды
    FadeInDuration = 200,                      // анимация появления
    FadeOutDuration = 200,                     // анимация исчезновения
    MaxVisibleNotifications = 3,               // макс. одновременно
    ScreenMargin = 15,                         // отступ от края
    NotificationSpacing = 10,                  // расстояние между уведомлениями
    Width = 400,                               // ширина
    MinHeight = 80,
    MaxHeight = 250,
    PlaySound = true,                          // системные звуки
    KeepApplicationAlive = false,              // не блокировать завершение (по умолчанию)
    CloseOnApplicationShutdown = true          // закрывать при завершении
};

var manager = new ToastNotificationManager(settings);
manager.Show("Заголовок", "Сообщение", ToastNotificationIcon.Information);
```

### ⚠️ Управление временем жизни приложения

#### Стандартное поведение (рекомендуется)

**По умолчанию** (`KeepApplicationAlive = false`):

```csharp
var manager = ToastNotificationManager.Default;
manager.Show("Сообщение", "Текст", ToastNotificationIcon.Information);

// При закрытии главного окна:
// ✅ Приложение завершается НЕМЕДЛЕННО
// ✅ Менеджер автоматически устанавливает ShutdownMode.OnMainWindowClose
// ✅ Все уведомления закрываются автоматически
// ✅ Процесс выгружается из памяти
```

#### Критические уведомления (блокируют завершение)

**Только для критически важных случаев** (`KeepApplicationAlive = true`):

```csharp
var settings = new ToastNotificationSettings
{
    KeepApplicationAlive = true,               // блокировать завершение
    CloseOnApplicationShutdown = false,        // не закрывать автоматически
    DisplayDuration = 0                        // не закрывать по таймеру
};

var manager = new ToastNotificationManager(settings);
manager.Show(
    "КРИТИЧЕСКОЕ ПРЕДУПРЕЖДЕНИЕ",
    "Несохранённые данные будут потеряны!",
    ToastNotificationIcon.Critical
);

// При этих настройках:
// ⚠️ ShutdownMode НЕ изменяется
// ⚠️ Приложение НЕ завершится пока есть уведомления
// ⚠️ Пользователь должен вручную закрыть уведомление
```

**Когда использовать `KeepApplicationAlive = true`:**
- ❗ Критические ошибки, требующие подтверждения
- ❗ Несохранённые данные перед выходом
- ❗ Завершение длительных операций
- ❗ Уведомления о потере данных

**⚠️ НЕ используйте** для обычных информационных сообщений!

### Изменение настроек во время работы

```csharp
public class MyViewModel : ViewModel
{
    private readonly ToastNotificationManager _manager;

    public MyViewModel()
    {
        _manager = new ToastNotificationManager(new ToastNotificationSettings());
    }

    #region SelectedPosition : ToastNotificationPosition

    private ToastNotificationPosition _SelectedPosition = 
        ToastNotificationPosition.BottomRight;

    public ToastNotificationPosition SelectedPosition 
    { 
        get => _SelectedPosition; 
        set 
        {
            if (Set(ref _SelectedPosition, value))
                _manager.Settings.Position = value;
        } 
    }

    #endregion
}
```

## Пользовательское содержимое

```csharp
// Создаём пользовательский контрол
var customContent = new StackPanel
{
    Children =
    {
        new TextBlock 
        { 
            Text = "Загрузка файла...", 
            FontWeight = FontWeights.Bold 
        },
        new ProgressBar 
        { 
            Height = 20, 
            IsIndeterminate = true, 
            Margin = new Thickness(0, 5, 0, 0) 
        }
    }
};

// Показываем
manager.ShowCustom("Процесс", customContent, ToastNotificationIcon.None);
```

## Обработка событий

```csharp
var viewModel = new ToastNotificationViewModel
{
    Title = "Уведомление",
    Message = "Кликните для подробностей",
    Icon = ToastNotificationIcon.Information
};

// Подписываемся на события
viewModel.LeftClick += (s, e) =>
{
    // Обработка клика левой кнопкой
    OpenDetailsWindow();
};

viewModel.RightClick += (s, e) =>
{
    // Обработка клика правой кнопкой
    ShowContextMenu();
};

viewModel.CloseRequested += (s, e) =>
{
    // Уведомление закрывается
    LogNotificationClosed();
};

// Показываем
manager.Show(viewModel);
```

## Кастомизация стилей

### Применение собственного стиля

```xaml
<!-- Создаём свой стиль в ResourceDictionary -->
<Style x:Key="MyCustomToastStyle" 
       TargetType="{x:Type Window}"
       BasedOn="{StaticResource DefaultToastNotificationWindowStyle}">
    <!-- Переопределяем цвета, размеры и т.д. -->
</Style>
```

```csharp
// Применяем стиль к менеджеру
var settings = new ToastNotificationSettings
{
    WindowStyle = Application.Current.FindResource("MyCustomToastStyle") as Style
};

var manager = new ToastNotificationManager(settings);
```

## Управление уведомлениями

```csharp
// Закрыть все активные уведомления
manager.CloseAll();

// Получить список активных окон (readonly)
var activeWindows = manager.ActiveWindows;
var count = activeWindows.Count;
```

## Позиции отображения

```csharp
public enum ToastNotificationPosition
{
    BottomRight,    // Правый нижний угол (по умолчанию)
    TopRight,       // Правый верхний угол
    BottomLeft,     // Левый нижний угол
    TopLeft,        // Левый верхний угол
    Center,         // Центр экрана
    TopCenter,      // Сверху по центру
    BottomCenter    // Снизу по центру
}
```

## Типы иконок

```csharp
public enum ToastNotificationIcon
{
    None,           // Без иконки
    Information,    // Информация (синяя)
    Success,        // Успех (зелёная)
    Warning,        // Предупреждение (оранжевая)
    Error,          // Ошибка (красная)
    Critical,       // Критическая ошибка (тёмно-красная)
    Question        // Вопрос (синяя)
}
```

## Примеры использования

### Прогресс-операция с уведомлениями

```csharp
public class DownloadViewModel : ViewModel
{
    private readonly ToastNotificationManager _notifications = 
        ToastNotificationManager.Default;

    public ICommand DownloadCommand => field ??= Command.NewBackground(
        async () =>
        {
            _notifications.Show(
                "Загрузка", 
                "Начало загрузки файла...", 
                ToastNotificationIcon.Information
            );

            try
            {
                await DownloadFileAsync();

                _notifications.Show(
                    "Готово", 
                    "Файл успешно загружен", 
                    ToastNotificationIcon.Success
                );
            }
            catch (Exception ex)
            {
                _notifications.Show(
                    "Ошибка загрузки", 
                    ex.Message, 
                    ToastNotificationIcon.Error
                );
            }
        });
}
```

### Валидация с уведомлениями

```csharp
public class FormViewModel : ViewModel
{
    public ICommand ValidateCommand => field ??= Command.New(
        () =>
        {
            var errors = ValidateForm();

            if (errors.Count == 0)
            {
                ToastNotificationManager.Default.Show(
                    "Проверка пройдена", 
                    "Все поля заполнены корректно", 
                    ToastNotificationIcon.Success
                );
            }
            else
            {
                ToastNotificationManager.Default.Show(
                    "Ошибки валидации", 
                    $"Найдено ошибок: {errors.Count}", 
                    ToastNotificationIcon.Warning
                );
            }
        });
}
```

## Советы и рекомендации

1. **Используйте синглтон** для простых случаев: `ToastNotificationManager.Default`

2. **Создавайте отдельные менеджеры** для разных зон приложения с разными настройками

3. **Не показывайте слишком много уведомлений** - используйте `MaxVisibleNotifications`

4. **Краткость** - сообщения должны быть короткими и понятными

5. **Правильные иконки** - используйте соответствующие типы для разных ситуаций

6. **Длительность** - 3-5 секунд оптимально для большинства случаев

7. **Позиционирование** - правый нижний угол привычнее для пользователей Windows

8. **Не блокируйте завершение** - используйте `KeepApplicationAlive = false` (по умолчанию)

9. **Автозакрытие** - включайте `CloseOnApplicationShutdown = true` для корректной выгрузки

10. **Никаких ручных настроек** - менеджер автоматически настраивает ShutdownMode

## Устранение проблем

### Приложение не завершается при закрытии главного окна

**Симптомы:** При наличии активных уведомлений приложение остаётся в памяти после закрытия главного окна.

**Причина:** WPF по умолчанию использует `ShutdownMode.OnLastWindowClose`, что означает завершение при закрытии последнего окна (включая уведомления).

**Решение:** Система автоматически исправляет это при первом показе уведомления:

```csharp
// Просто используйте менеджер как обычно
ToastNotificationManager.Default.Show("Заголовок", "Сообщение", ToastNotificationIcon.Information);

// Менеджер автоматически устанавливает:
// Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
```

**Проверка:**
```csharp
// После первого показа уведомления
Debug.WriteLine(Application.Current.ShutdownMode); // Должно быть: OnMainWindowClose
```

**Если проблема сохраняется:**
1. Убедитесь, что `KeepApplicationAlive = false` (по умолчанию)
2. Проверьте, что используете последнюю версию библиотеки
3. Убедитесь, что главное окно установлено: `Application.Current.MainWindow != null`

### Уведомления продолжают показываться после закрытия приложения

**Проблема:** Уведомления из очереди продолжают появляться после закрытия главного окна.

**Решение:** Система автоматически очищает очередь при завершении, но можно явно вызвать:

```csharp
// В обработчике закрытия главного окна (необязательно)
protected override void OnClosing(CancelEventArgs e)
{
    ToastNotificationManager.Default.CloseAll();
    base.OnClosing(e);
}
```

### Окна уведомлений появляются в панели задач

**Проблема:** Иконки уведомлений видны в панели задач Windows.

**Решение:** Это происходит только при `KeepApplicationAlive = true`. Используйте значение по умолчанию (`false`), и окна не будут появляться в панели задач.

## Техническая информация

### Как работает автоматическая настройка

1. **Первый вызов `Show()`** → Вызывается `EnsureInitialized()`
2. **Проверка `ShutdownMode`** → Если `OnLastWindowClose` и `KeepApplicationAlive = false`
3. **Изменение режима** → `Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose`
4. **Подписка на события** → `Application.Current.Exit += OnApplicationExit`
5. **Автоматическое закрытие** → При `Exit` вызывается `CloseAll()`

### Совместимость

- ✅ .NET Framework 4.6.1 — 4.8
- ✅ .NET 5, 6, 7, 8, 9, 10
- ✅ Windows 7 — Windows 11
- ✅ x86, x64, ARM64

### Производительность

- Ленивая инициализация (только при первом использовании)
- Потокобезопасная очередь уведомлений
- Минимальное влияние на UI-поток
- Оптимизированное позиционирование окон
