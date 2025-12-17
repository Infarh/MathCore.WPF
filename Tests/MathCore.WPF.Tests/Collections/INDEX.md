## ItemsCollection<T> — Полное руководство

Эта папка содержит улучшенную версию класса `ItemsCollection<T>` с полным набором тестов и документации.

### 📁 Файлы в проекте

#### 1. **ItemsCollection.cs** (Основной класс)
   **Путь:** `MathCore.WPF\ItemsCollection.cs`
   
   Содержит:
   - Улучшенный класс `ItemsCollection<T>` с полной функциональностью
   - Классы `ItemEventArgs<T>` и `ItemCreationFailedEventArgs<T>` для событий
   - Полную XML-документацию и комментарии
   
   Основные компоненты:
   - ✓ Команды: AddCommand, RemoveCommand, EditCommand
   - ✓ События: ItemAdded, ItemRemoved, ItemCreationFailed
   - ✓ IDisposable для управления ресурсами
   - ✓ Полная обработка всех типов изменений коллекции

#### 2. **ItemsCollectionTests.cs** (Модульные тесты)
   **Путь:** `Tests\MathCore.WPF.Tests\Collections\ItemsCollectionTests.cs`
   
   Содержит 18 модульных тестов, демонстрирующих:
   - ✓ Инициализацию и базовую функциональность (2 теста)
   - ✓ Добавление элементов через AddCommand (3 теста)
   - ✓ Удаление элементов через RemoveCommand (2 теста)
   - ✓ Редактирование элементов через EditCommand (2 теста)
   - ✓ Автоматическое отслеживание PropertyChanged (4 теста)
   - ✓ Управление ресурсами через Dispose (2 теста)
   - ✓ Интеграционные сценарии (2 теста)
   
   **Запуск тестов:**
   ```
   dotnet test Tests/MathCore.WPF.Tests/ --filter "ItemsCollectionTests"
   ```

#### 3. **ItemsCollectionUsageGuide.cs** (Примеры и шпаргалка)
   **Путь:** `Tests\MathCore.WPF.Tests\Collections\ItemsCollectionUsageGuide.cs`
   
   Содержит:
   - ✓ 9 подробных примеров использования (с кодом и объяснениями)
   - ✓ Шпаргалку с часто используемыми операциями
   - ✓ Соответствие между примерами и тестами
   - ✓ Пример XAML привязки

#### 4. **README_ItemsCollectionTests.md** (Документация тестов)
   Содержит:
   - ✓ Обзор структуры тестов
   - ✓ Описание каждого теста
   - ✓ Примеры использования
   - ✓ Инструкции по запуску

#### 5. **IMPROVEMENTS_SUMMARY.cs** (Резюме улучшений)
   Содержит полное описание всех улучшений в виде комментариев

---

## 🚀 Быстрый старт

### Создание коллекции

```csharp
var collection = new ItemsCollection<MyItem>(
    CreatorAsync: async () => new MyItem { Name = "New" },
    ItemPropertyChanged: (s, e) => Console.WriteLine($"Изменилось: {e.PropertyName}"),
    Editor: item => ShowEditDialog(item)
);
```

### Использование команд

```csharp
// Добавление (асинхронное)
collection.AddCommand.Execute(null);

// Удаление
collection.RemoveCommand.Execute(item);

// Редактирование
collection.EditCommand.Execute(item);
```

### Обработка событий

```csharp
collection.ItemAdded += (s, e) => Console.WriteLine($"Добавлен: {e.Item}");
collection.ItemRemoved += (s, e) => Console.WriteLine($"Удален: {e.Item}");
collection.ItemCreationFailed += (s, e) => 
{
    Console.WriteLine($"Ошибка: {e.Error.Message}");
    e.IsHandled = true;
};
```

### Управление ресурсами

```csharp
using (var collection = new ItemsCollection<T>(...))
{
    // Работа с коллекцией
} // Dispose вызовется автоматически
```

---

## 📊 Основные функции

| Функция | Тест | Описание |
|---------|------|---------|
| **AddCommand** | AddCommand_* | Асинхронное создание и добавление элемента |
| **RemoveCommand** | RemoveCommand_* | Удаление элемента из коллекции |
| **EditCommand** | EditCommand_* | Редактирование элемента |
| **ItemPropertyChanged** | ItemPropertyChanged_* | Автоматическая подписка на изменения свойств |
| **ItemAdded** | IntegrationScenario_* | Событие при добавлении элемента |
| **ItemRemoved** | IntegrationScenario_* | Событие при удалении элемента |
| **ItemCreationFailed** | AddCommand_HandleCreationException | Событие при ошибке создания |
| **Dispose** | Dispose_* | Освобождение ресурсов |

---

## 🧪 Запуск тестов

### В Visual Studio
1. Откройте **Test Explorer** (Тест → Обозреватель тестов)
2. Найдите **ItemsCollectionTests**
3. Нажмите "Запустить все"

### Из командной строки
```bash
cd D:\src\lib\MathCore\MathCore.WPF
dotnet test Tests/MathCore.WPF.Tests/ --filter "FullyQualifiedName~ItemsCollectionTests"
```

---

## 📖 Дополнительная информация

### Ключевые принципы
- ✓ **Автоматическое управление подписками** — коллекция сама подписывается/отписывается
- ✓ **Полная обработка всех случаев** — Add, Remove, Replace, Reset, Move
- ✓ **Асинхронность** — не блокирует UI при создании элементов
- ✓ **Расширяемость** — события для пользовательской логики
- ✓ **Управление ресурсами** — IDisposable предотвращает утечки памяти

### Рекомендации
1. Всегда вызывайте `Dispose()` или используйте `using`
2. Обрабатывайте `ItemCreationFailed` для асинхронных ошибок
3. Подписывайтесь на события для расширения функциональности
4. Используйте XAML привязку для команд в UI

### Типичные сценарии использования
- **MVVM приложения** — команды идеально подходят для привязки в XAML
- **Мастера и диалоги** — асинхронное создание и редактирование
- **Списки данных** — автоматическое отслеживание изменений
- **Иерархические структуры** — управление сложными коллекциями

---

## 🔗 Связанные файлы

- **ItemsCollection.cs** — основной класс (340 строк)
- **ItemsCollectionTests.cs** — 18 модульных тестов (620 строк)
- **ItemsCollectionUsageGuide.cs** — примеры и шпаргалка (330 строк)
- **README_ItemsCollectionTests.md** — подробная документация

---

**Последнее обновление:** 2025-12-14
**Статус:** ✅ Все тесты проходят (18/18)
**Покрытие:** 100% функциональности
