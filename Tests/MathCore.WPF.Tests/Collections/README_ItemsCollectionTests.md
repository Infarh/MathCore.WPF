# Модульные тесты для ItemsCollection&lt;T&gt;

## Обзор

Файл `ItemsCollectionTests.cs` содержит полный набор модульных тестов для класса `ItemsCollection<T>`, демонстрирующих все основные функции и сценарии использования этой коллекции.

## Структура тестов

Тесты разделены на несколько категорий:

### 1. Инициализация и базовая функциональность
- **Constructor_CreatesEmptyCollection** — проверяет создание пустой коллекции и инициализацию команд
- **Collection_InheritsSelectableCollectionBehavior** — проверяет наследование поведения SelectableCollection

### 2. Добавление элементов (AddCommand)
- **AddCommand_CreatesAndAddsNewItem** — демонстрирует асинхронное создание и добавление элемента
- **AddCommand_CanExecute_ReturnsTrue** — проверяет доступность команды добавления
- **AddCommand_HandleCreationException** — демонстрирует обработку ошибок при создании

### 3. Удаление элементов (RemoveCommand)
- **RemoveCommand_RemovesItemFromCollection** — демонстрирует удаление элемента
- **RemoveCommand_CanExecute_OnlyForExistingItems** — проверяет, что команда доступна только для существующих элементов

### 4. Редактирование элементов (EditCommand)
- **EditCommand_InvokesEditorAction** — демонстрирует вызов функции редактирования
- **EditCommand_CanExecute_OnlyForExistingItems** — проверяет условие доступности

### 5. Отслеживание изменений свойств (PropertyChanged)
- **ItemPropertyChanged_SubscribesToNewItems** — демонстрирует автоматическую подписку на события
- **ItemPropertyChanged_UnsubscribesFromRemovedItems** — проверяет отписку при удалении
- **ItemPropertyChanged_HandlesReplaceAction** — демонстрирует обработку замены элементов
- **ItemPropertyChanged_HandlesResetAction** — демонстрирует обработку очистки коллекции

### 6. Управление ресурсами (IDisposable)
- **Dispose_UnsubscribesAllItems** — демонстрирует освобождение ресурсов
- **Dispose_CanBeCalledMultipleTimes** — проверяет безопасность множественных вызовов

### 7. Интеграционные тесты
- **IntegrationScenario_ComplexOperations** — демонстрирует сложный сценарий с несколькими операциями
- **Collection_RaisesCollectionChangedEvents** — проверяет корректность событий коллекции

## Тестовая модель

В тестах используется класс `TestItem : INotifyPropertyChanged`:

```csharp
private sealed class TestItem : INotifyPropertyChanged
{
    public string Name { get; set; }
    public int Value { get; set; }
    public int ChangeCount { get; private set; }
    public event PropertyChangedEventHandler? PropertyChanged;
}
```

Этот класс позволяет отслеживать изменения свойств и проверять корректность работы механизма подписок.

## Примеры использования из тестов

### Пример 1: Базовое создание коллекции

```csharp
var collection = new ItemsCollection<TestItem>(
    CreatorAsync: CreateTestItemAsync,  // Асинхронная функция создания
    ItemPropertyChanged: (s, e) => Console.WriteLine($"Изменилось: {e.PropertyName}"),
    Editor: item => item.Value++  // Функция редактирования
);
```

### Пример 2: Обработка событий

```csharp
collection.ItemAdded += (_, e) => Console.WriteLine($"Добавлен: {e.Item.Name}");
collection.ItemRemoved += (_, e) => Console.WriteLine($"Удален: {e.Item.Name}");
collection.ItemCreationFailed += (_, e) =>
{
    Console.WriteLine($"Ошибка: {e.Error.Message}");
    e.IsHandled = true;  // Подавить пробрасывание
};
```

### Пример 3: Использование команд

```csharp
// Добавление элемента асинхронно
collection.AddCommand.Execute(null);

// Удаление элемента
var item = collection.SelectedItem;
collection.RemoveCommand.Execute(item);

// Редактирование элемента
collection.EditCommand.Execute(item);
```

### Пример 4: Отслеживание изменений свойств

```csharp
var item = new TestItem { Name = "Initial", Value = 10 };
collection.Add(item);

// Все эти изменения будут отслежены
item.Name = "Updated";  // Вызовет ItemPropertyChanged
item.Value = 20;         // Вызовет ItemPropertyChanged
```

### Пример 5: Управление ресурсами

```csharp
using (var collection = new ItemsCollection<TestItem>(...))
{
    // Работа с коллекцией
} // Dispose будет вызван автоматически
```

## Запуск тестов

### Visual Studio
1. Откройте Test Explorer (Тест → Обозреватель тестов)
2. Найдите тесты в `MathCore.WPF.Tests → Collections → ItemsCollectionTests`
3. Нажмите "Запустить все" или выберите отдельные тесты

### Command Line
```bash
dotnet test Tests/MathCore.WPF.Tests/ --filter "ItemsCollectionTests"
```

## Ключевые принципы

1. **Автоматическая подписка** — коллекция автоматически подписывается на `PropertyChanged` элементов, реализующих `INotifyPropertyChanged`

2. **Управление подписками** — подписки корректно создаются при добавлении, удаляются при удалении и переинициализируются при очистке коллекции

3. **Асинхронность** — команда AddCommand асинхронно создаёт элементы и обработает ошибки

4. **События** — `ItemAdded`, `ItemRemoved`, `ItemCreationFailed` позволяют реагировать на операции

5. **IDisposable** — коллекция корректно освобождает ресурсы и отписывает все элементы

6. **XAML привязка** — все команды могут быть привязаны к кнопкам в XAML

## Учебные материалы

Дополнительная документация доступна в файле `ItemsCollectionUsageGuide.cs`, который содержит:
- Примеры для каждого сценария использования
- Соответствие между примерами и тестами
- Шпаргалку с часто используемыми операциями
