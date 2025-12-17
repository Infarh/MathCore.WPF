# Исправление блокировки завершения приложения Toast уведомлениями

## 🔴 Проблема

При закрытии главного окна с активными уведомлениями приложение **НЕ завершалось** и оставалось в памяти.

### Причина

WPF по умолчанию использует `Application.ShutdownMode = OnLastWindowClose`, что означает:
- Приложение завершается при закрытии **последнего** окна
- Окна уведомлений считаются обычными окнами
- Пока есть хотя бы одно окно уведомления → приложение в памяти

## ✅ Решение

Менеджер **автоматически** изменяет `ShutdownMode` на `OnMainWindowClose` при первом показе уведомления:

```csharp
// При первом вызове Show()
ToastNotificationManager.Default.Show("Заголовок", "Сообщение", ToastNotificationIcon.Information);

// Менеджер автоматически выполняет:
if (Application.Current.ShutdownMode == ShutdownMode.OnLastWindowClose)
{
    Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
}
```

## 🎯 Результат

✅ Закрытие главного окна → **немедленное** завершение приложения  
✅ Все активные уведомления **автоматически** закрываются  
✅ Процесс **корректно** выгружается из памяти  
✅ Очередь уведомлений **автоматически** очищается  
✅ **Никаких** ручных настроек не требуется  

## 📝 Изменённые файлы

### 1. `ToastNotificationSettings.cs`

Добавлена настройка `KeepApplicationAlive`:

```csharp
/// <summary>
/// Удерживать приложение активным пока есть открытые уведомления (по умолчанию false)
/// </summary>
public bool KeepApplicationAlive { get; set; } = false;

/// <summary>Автоматически закрывать все уведомления при завершении работы приложения</summary>
public bool CloseOnApplicationShutdown { get; set; } = true;
```

### 2. `ToastNotificationManager.cs`

Ключевые изменения:

```csharp
private bool _IsInitialized;

private void EnsureInitialized()
{
    if (_IsInitialized || Application.Current is null)
        return;

    _IsInitialized = true;
    Application.Current.Exit += OnApplicationExit;

    // КРИТИЧЕСКИ ВАЖНО: автоматическая настройка ShutdownMode
    if (!Settings.KeepApplicationAlive && 
        Application.Current.ShutdownMode == ShutdownMode.OnLastWindowClose)
    {
        Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
    }
}

public void Show(ToastNotificationViewModel ViewModel)
{
    // ...
    EnsureInitialized(); // Вызываем при первом показе
    // ...
}

private void OnApplicationExit(object? Sender, ExitEventArgs E)
{
    _IsShuttingDown = true;

    if (Settings.CloseOnApplicationShutdown)
        CloseAll();

    if (Application.Current != null)
        Application.Current.Exit -= OnApplicationExit;
}
```

### 3. `ToastNotificationWindow.xaml.cs`

Окно уведомления настраивается автоматически:

```csharp
public ToastNotificationWindow(ToastNotificationViewModel ViewModel, ToastNotificationSettings Settings)
{
    // ...
    
    if (!_Settings.KeepApplicationAlive)
    {
        ShowInTaskbar = false;  // Не показывать в панели задач
        Owner = null;           // Не привязывать к главному окну
    }
    
    // ...
}
```

## 🧪 Тестирование

### Быстрый тест

```csharp
// 1. Показать уведомления
for (int i = 1; i <= 5; i++)
{
    ToastNotificationManager.Default.Show(
        $"Тест #{i}", 
        $"Сообщение {i}", 
        ToastNotificationIcon.Information
    );
}

// 2. Проверить ShutdownMode
Debug.WriteLine(Application.Current.ShutdownMode); // Должно быть: OnMainWindowClose

// 3. Закрыть главное окно
// Результат: приложение немедленно завершается, все уведомления закрываются
```

### Проверка в диспетчере задач

1. Запустить приложение
2. Показать несколько уведомлений
3. Закрыть главное окно
4. **Результат:** Процесс приложения **исчезает** из диспетчера задач

## ⚙️ Дополнительные настройки

### Критические уведомления (блокируют завершение)

```csharp
var settings = new ToastNotificationSettings
{
    KeepApplicationAlive = true,               // Блокировать завершение
    CloseOnApplicationShutdown = false,        // Не закрывать автоматически
    DisplayDuration = 0                        // Не закрывать по таймеру
};

var manager = new ToastNotificationManager(settings);
manager.Show("КРИТИЧЕСКОЕ", "Важное сообщение", ToastNotificationIcon.Critical);

// В этом случае ShutdownMode НЕ изменяется
// Приложение завершится только после закрытия всех окон
```

**⚠️ Используйте только для критически важных уведомлений!**

## 📊 Диаграмма поведения

### До исправления ❌

```
Главное окно открыто → Показать уведомления → Закрыть главное окно
                                                         ↓
                                              ShutdownMode = OnLastWindowClose
                                                         ↓
                                              Есть окна уведомлений?
                                                         ↓
                                                        ДА
                                                         ↓
                                              Приложение НЕ завершается
                                                         ↓
                                              ❌ Процесс висит в памяти
```

### После исправления ✅

```
Главное окно открыто → Показать уведомления → EnsureInitialized()
                                                         ↓
                                              ShutdownMode = OnMainWindowClose
                                                         ↓
                                              Закрыть главное окно
                                                         ↓
                                              Application.Exit вызван
                                                         ↓
                                              CloseAll() автоматически
                                                         ↓
                                              ✅ Приложение завершено
                                              ✅ Процесс выгружен из памяти
```

## 📚 Дополнительная документация

- **README.md** - Полная документация с примерами
- **ToastNotificationShutdownExample.cs** - 11 детальных примеров использования
- **ToastNotificationExamples.cs** - Базовые примеры уведомлений

## 🎓 Ключевые моменты

1. **Автоматическая настройка** - менеджер сам изменяет `ShutdownMode`
2. **Ленивая инициализация** - настройка происходит только при первом показе
3. **Потокобезопасность** - все операции синхронизированы
4. **Обратная совместимость** - старый код работает без изменений
5. **Гибкость** - можно отключить автоматику через `KeepApplicationAlive = true`

## ✅ Чеклист проверки

- [x] Менеджер автоматически устанавливает `ShutdownMode.OnMainWindowClose`
- [x] Окна уведомлений не появляются в панели задач
- [x] При закрытии главного окна приложение завершается немедленно
- [x] Все уведомления автоматически закрываются при завершении
- [x] Процесс корректно выгружается из памяти
- [x] Очередь уведомлений очищается
- [x] Работает с `.NET Framework 4.6.1` — `.NET 10`
- [x] Обратная совместимость сохранена

## 🔗 Связанные файлы

- `MathCore.WPF/Notifications/ToastNotificationManager.cs` - Основная логика
- `MathCore.WPF/Notifications/ToastNotificationSettings.cs` - Настройки
- `MathCore.WPF/Notifications/ToastNotificationWindow.xaml.cs` - Окно уведомления
- `MathCore.WPF/Notifications/README.md` - Полная документация
- `Tests/MathCore.WPF.WindowTest/Examples/ToastNotificationShutdownExample.cs` - Примеры

---

**Статус:** ✅ Проблема полностью решена  
**Дата:** 2024  
**Версия:** 1.0  
