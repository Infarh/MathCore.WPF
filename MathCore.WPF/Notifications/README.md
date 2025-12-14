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
✅ **Кроссплатформенность** - .NET Framework 4.6.1 — .NET 10  

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
    PlaySound = true                           // системные звуки
};

var manager = new ToastNotificationManager(settings);
manager.Show("Заголовок", "Сообщение", ToastNotificationIcon.Information);
```

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

## Лицензия

MIT License - свободное использование в любых проектах
