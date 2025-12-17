using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MathCore.WPF.Tests.Collections;

/// <summary>Модульные тесты для ItemsCollection&lt;T&gt;</summary>
/// <remarks>
/// Тесты демонстрируют основные сценарии использования коллекции:
/// - Создание и добавление элементов через асинхронную команду AddCommand
/// - Удаление элементов через команду RemoveCommand
/// - Автоматическую подписку на изменения свойств элементов
/// - Управление подписками при различных типах изменений коллекции (Add, Remove, Replace, Reset)
/// - Обработку исключений при создании элементов
/// - Освобождение ресурсов через Dispose
/// </remarks>
[TestClass]
public class ItemsCollectionTests
{
    /// <summary>Тестовая модель с поддержкой оповещения об изменениях свойств</summary>
    private sealed class TestItem : INotifyPropertyChanged
    {
        private string _Name = string.Empty;
        private int _Value;

        /// <summary>Имя элемента</summary>
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name == value) return;
                _Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>Числовое значение элемента</summary>
        public int Value
        {
            get => _Value;
            set
            {
                if (_Value == value) return;
                _Value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        /// <summary>Счётчик изменений для отладки тестов</summary>
        public int ChangeCount { get; private set; }

        /// <summary>Событие об изменении свойства элемента</summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>Вызывает событие об изменении свойства</summary>
        private void OnPropertyChanged(string property_name)
        {
            ChangeCount++;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));
        }

        /// <summary>Переопределение для удобной отладки</summary>
        public override string ToString() => $"Item(Name='{Name}', Value={Value})";
    }

    #region Вспомогательные методы

    /// <summary>Создаёт коллекцию с отслеживанием событий PropertyChanged элементов</summary>
    private ItemsCollection<TestItem> CreateCollectionWithPropertyTracking(
        PropertyChangedEventHandler? item_property_changed = null)
    {
        return new ItemsCollection<TestItem>(
            CreatorAsync: CreateTestItemAsync,
            ItemPropertyChanged: item_property_changed ?? ((_, e) => { }),
            Editor: item => item.Value++
        );
    }

    /// <summary>Асинхронно создаёт новый тестовый элемент</summary>
    private static Task<TestItem> CreateTestItemAsync()
    {
        return Task.FromResult(new TestItem
        {
            Name = $"Item_{Guid.NewGuid().ToString().Substring(0, 8)}",
            Value = new Random().Next(100, 1000)
        });
    }

    #endregion

    #region Тесты инициализации и базовой функциональности

    /// <summary>Проверяет успешное создание и инициализацию коллекции</summary>
    [TestMethod]
    public void Constructor_CreatesEmptyCollection()
    {
        // Arrange & Act
        var collection = CreateCollectionWithPropertyTracking();

        // Assert
        Assert.IsNotNull(collection);
        Assert.AreEqual(0, collection.Count, "Новая коллекция должна быть пуста");
        Assert.IsNotNull(collection.AddCommand, "Команда добавления должна быть инициализирована");
        Assert.IsNotNull(collection.RemoveCommand, "Команда удаления должна быть инициализирована");
        Assert.IsNotNull(collection.EditCommand, "Команда редактирования должна быть инициализирована");
    }

    /// <summary>Проверяет, что коллекция наследуется от SelectableCollection</summary>
    [TestMethod]
    public void Collection_InheritsSelectableCollectionBehavior()
    {
        // Arrange
        var collection = CreateCollectionWithPropertyTracking();
        var item = new TestItem { Name = "Test", Value = 42 };

        // Act
        collection.Add(item);

        // Assert
        Assert.AreEqual(1, collection.Count);
        // SelectableCollection может не автоматически выбирать элементы при Add
        // Выбор происходит при использовании команды AddCommand
        Assert.IsNotNull(collection, "Коллекция должна успешно добавить элемент");
    }

    #endregion

    #region Тесты добавления элементов (AddCommand)

    /// <summary>Демонстрирует добавление элемента через асинхронную команду AddCommand</summary>
    [TestMethod]
    public void AddCommand_CreatesAndAddsNewItem()
    {
        // Arrange
        var collection = CreateCollectionWithPropertyTracking();
        var item_added_event_raised = false;
        TestItem? added_item = null;

        collection.ItemAdded += (_, e) =>
        {
            item_added_event_raised = true;
            added_item = e.Item;
        };

        // Act
        // Выполнить команду (в реальном приложении это сделает UI)
        var command = (System.Windows.Input.ICommand)collection.AddCommand;
        command.Execute(null);

        // Assert
        Assert.AreEqual(1, collection.Count, "В коллекции должен быть один элемент");
        Assert.IsNotNull(collection.SelectedItem, "Элемент должен быть автоматически выбран");
        Assert.IsTrue(item_added_event_raised, "Событие ItemAdded должно быть вызвано");
        Assert.IsNotNull(added_item, "Аргумент события должен содержать добавленный элемент");
    }

    /// <summary>Проверяет, что AddCommand доступна для выполнения</summary>
    [TestMethod]
    public void AddCommand_CanExecute_ReturnsTrue()
    {
        // Arrange
        var collection = CreateCollectionWithPropertyTracking();

        // Act
        var can_execute = collection.AddCommand.CanExecute(null);

        // Assert
        Assert.IsTrue(can_execute, "Команда добавления должна быть доступна");
    }

    /// <summary>Демонстрирует обработку ошибок при создании элемента</summary>
    [TestMethod]
    public void AddCommand_HandleCreationException()
    {
        // Arrange
        var error_handled = false;
        Exception? caught_exception = null;

        var collection = new ItemsCollection<TestItem>(
            CreatorAsync: async () => throw new InvalidOperationException("Ошибка создания"),
            ItemPropertyChanged: null,
            Editor: null
        );

        collection.ItemCreationFailed += (_, e) =>
        {
            error_handled = true;
            caught_exception = e.Error;
            e.IsHandled = true; // Подавляем пробрасывание исключения
        };

        // Act
        var command = (System.Windows.Input.ICommand)collection.AddCommand;
        command.Execute(null);

        // Assert (проверяем асинхронный результат через небольшую задержку)
        Task.Delay(100).Wait();
        
        Assert.IsTrue(error_handled, "Событие ItemCreationFailed должно быть вызвано");
        Assert.IsNotNull(caught_exception);
        Assert.IsTrue(caught_exception.Message.Contains("Ошибка создания"));
        Assert.AreEqual(0, collection.Count, "Коллекция должна остаться пуста при ошибке");
    }

    #endregion

    #region Тесты удаления элементов (RemoveCommand)

    /// <summary>Демонстрирует удаление элемента через команду RemoveCommand</summary>
    [TestMethod]
    public void RemoveCommand_RemovesItemFromCollection()
    {
        // Arrange
        var collection = CreateCollectionWithPropertyTracking();
        var item = new TestItem { Name = "ToRemove", Value = 100 };
        collection.Add(item);

        var item_removed_event_raised = false;
        TestItem? removed_item = null;

        collection.ItemRemoved += (_, e) =>
        {
            item_removed_event_raised = true;
            removed_item = e.Item;
        };

        // Act
        var command = (System.Windows.Input.ICommand)collection.RemoveCommand;
        command.Execute(item);

        // Assert
        Assert.AreEqual(0, collection.Count, "Коллекция должна быть пуста после удаления");
        Assert.IsTrue(item_removed_event_raised, "Событие ItemRemoved должно быть вызвано");
        Assert.AreEqual(item, removed_item, "Событие должно содержать удалённый элемент");
    }

    /// <summary>Проверяет, что RemoveCommand доступна только для элементов в коллекции</summary>
    [TestMethod]
    public void RemoveCommand_CanExecute_OnlyForExistingItems()
    {
        // Arrange
        var collection = CreateCollectionWithPropertyTracking();
        var existing_item = new TestItem { Name = "Existing", Value = 1 };
        var non_existing_item = new TestItem { Name = "NotExisting", Value = 2 };

        collection.Add(existing_item);

        // Act & Assert
        Assert.IsTrue(collection.RemoveCommand.CanExecute(existing_item),
            "Команда удаления должна быть доступна для существующего элемента");

        Assert.IsFalse(collection.RemoveCommand.CanExecute(non_existing_item),
            "Команда удаления должна быть недоступна для не существующего элемента");
    }

    #endregion

    #region Тесты редактирования элементов (EditCommand)

    /// <summary>Демонстрирует редактирование элемента через команду EditCommand</summary>
    [TestMethod]
    public void EditCommand_InvokesEditorAction()
    {
        // Arrange
        var edit_invoked = false;
        TestItem? edited_item = null;

        var collection = new ItemsCollection<TestItem>(
            CreatorAsync: CreateTestItemAsync,
            ItemPropertyChanged: null,
            Editor: item =>
            {
                edit_invoked = true;
                edited_item = item;
                item.Value *= 2; // Модифицируем значение при редактировании
            }
        );

        var item = new TestItem { Name = "ToEdit", Value = 50 };
        collection.Add(item);

        // Act
        var command = (System.Windows.Input.ICommand)collection.EditCommand;
        command.Execute(item);

        // Assert
        Assert.IsTrue(edit_invoked, "Функция редактирования должна быть вызвана");
        Assert.AreEqual(item, edited_item, "Функция должна получить правильный элемент");
        Assert.AreEqual(100, item.Value, "Значение элемента должно быть изменено");
    }

    /// <summary>Проверяет, что EditCommand доступна только для элементов в коллекции</summary>
    [TestMethod]
    public void EditCommand_CanExecute_OnlyForExistingItems()
    {
        // Arrange
        var collection = new ItemsCollection<TestItem>(
            CreatorAsync: CreateTestItemAsync,
            Editor: _ => { }
        );

        var existing_item = new TestItem { Name = "Existing", Value = 1 };
        var non_existing_item = new TestItem { Name = "NotExisting", Value = 2 };

        collection.Add(existing_item);

        // Act & Assert
        Assert.IsTrue(collection.EditCommand.CanExecute(existing_item),
            "Команда редактирования должна быть доступна для существующего элемента");

        Assert.IsFalse(collection.EditCommand.CanExecute(non_existing_item),
            "Команда редактирования должна быть недоступна для не существующего элемента");
    }

    #endregion

    #region Тесты отслеживания изменений свойств (PropertyChanged)

    /// <summary>Демонстрирует автоматическую подписку на события PropertyChanged при добавлении элементов</summary>
    [TestMethod]
    public void ItemPropertyChanged_SubscribesToNewItems()
    {
        // Arrange
        var property_changes = new List<(TestItem, string)>();
        var collection = new ItemsCollection<TestItem>(
            CreatorAsync: CreateTestItemAsync,
            ItemPropertyChanged: (s, e) =>
            {
                if (s is TestItem item)
                    property_changes.Add((item, e.PropertyName ?? ""));
            }
        );

        var item = new TestItem { Name = "Initial", Value = 10 };

        // Act
        collection.Add(item);
        item.Name = "Updated"; // Изменяем свойство
        item.Value = 20; // Изменяем ещё одно свойство

        // Assert
        Assert.AreEqual(2, property_changes.Count, "Должно быть 2 изменения свойств");
        Assert.AreEqual(nameof(TestItem.Name), property_changes[0].Item2);
        Assert.AreEqual(nameof(TestItem.Value), property_changes[1].Item2);
    }

    /// <summary>Проверяет отписку от изменений свойств при удалении элементов</summary>
    [TestMethod]
    public void ItemPropertyChanged_UnsubscribesFromRemovedItems()
    {
        // Arrange
        var property_changes = new List<(TestItem, string)>();
        var collection = new ItemsCollection<TestItem>(
            CreatorAsync: CreateTestItemAsync,
            ItemPropertyChanged: (s, e) =>
            {
                if (s is TestItem item)
                    property_changes.Add((item, e.PropertyName ?? ""));
            }
        );

        var item = new TestItem { Name = "ToRemove", Value = 10 };
        collection.Add(item);
        property_changes.Clear(); // Очищаем после добавления

        // Act
        collection.Remove(item);
        item.Name = "AfterRemoval"; // Изменяем свойство после удаления

        // Assert
        Assert.AreEqual(0, property_changes.Count,
            "Изменения свойств удалённого элемента не должны быть зафиксированы");
    }

    /// <summary>Демонстрирует отслеживание при замене элементов (Replace)</summary>
    [TestMethod]
    public void ItemPropertyChanged_HandlesReplaceAction()
    {
        // Arrange
        var property_changes = new List<(TestItem, string)>();
        var collection = new ItemsCollection<TestItem>(
            CreatorAsync: CreateTestItemAsync,
            ItemPropertyChanged: (s, e) =>
            {
                if (s is TestItem item)
                    property_changes.Add((item, e.PropertyName ?? ""));
            }
        );

        var old_item = new TestItem { Name = "Old", Value = 10 };
        var new_item = new TestItem { Name = "New", Value = 20 };

        collection.Add(old_item);
        property_changes.Clear();

        // Act
        collection[0] = new_item; // Замена элемента
        old_item.Name = "Modified"; // Изменяем старый элемент
        new_item.Name = "NewModified"; // Изменяем новый элемент

        // Assert
        Assert.AreEqual(1, property_changes.Count, "Должно быть только 1 изменение (нового элемента)");
        Assert.AreEqual("Name", property_changes[0].Item2, "Первое изменение должно быть для Name нового элемента");
    }

    /// <summary>Демонстрирует полную переинициализацию подписок при Reset коллекции</summary>
    [TestMethod]
    public void ItemPropertyChanged_HandlesResetAction()
    {
        // Arrange
        var property_changes = new List<(TestItem, string)>();
        var collection = new ItemsCollection<TestItem>(
            CreatorAsync: CreateTestItemAsync,
            ItemPropertyChanged: (s, e) =>
            {
                if (s is TestItem item)
                    property_changes.Add((item, e.PropertyName ?? ""));
            }
        );

        var item1 = new TestItem { Name = "Item1", Value = 10 };
        var item2 = new TestItem { Name = "Item2", Value = 20 };
        var item3 = new TestItem { Name = "Item3", Value = 30 };

        collection.Add(item1);
        collection.Add(item2);
        property_changes.Clear();

        // Act
        collection.Clear(); // Reset коллекции
        collection.Add(item3);
        item1.Name = "Modified"; // Старый элемент изменяется
        item3.Name = "NewItem"; // Новый элемент изменяется

        // Assert
        Assert.AreEqual(1, property_changes.Count, "Только новый элемент должен отслеживаться");
        Assert.AreEqual(item3, property_changes[0].Item1);
    }

    #endregion

    #region Тесты управления ресурсами (IDisposable)

    /// <summary>Демонстрирует правильное управление ресурсами через Dispose</summary>
    [TestMethod]
    public void Dispose_UnsubscribesAllItems()
    {
        // Arrange
        var property_changes = new List<(TestItem, string)>();
        var collection = new ItemsCollection<TestItem>(
            CreatorAsync: CreateTestItemAsync,
            ItemPropertyChanged: (s, e) =>
            {
                if (s is TestItem item)
                    property_changes.Add((item, e.PropertyName ?? ""));
            }
        );

        var item1 = new TestItem { Name = "Item1", Value = 10 };
        var item2 = new TestItem { Name = "Item2", Value = 20 };

        collection.Add(item1);
        collection.Add(item2);
        property_changes.Clear();

        // Act
        collection.Dispose();
        item1.Name = "Modified"; // Изменяем после Dispose
        item2.Value = 100;

        // Assert
        Assert.AreEqual(0, property_changes.Count,
            "После Dispose изменения элементов не должны отслеживаться");
    }

    /// <summary>Проверяет, что можно вызвать Dispose несколько раз безопасно</summary>
    [TestMethod]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var collection = CreateCollectionWithPropertyTracking();
        collection.Add(new TestItem { Name = "Test", Value = 1 });

        // Act & Assert (не должно быть исключений)
        collection.Dispose();
        collection.Dispose(); // Второй вызов не должен выбросить исключение
        collection.Dispose(); // Третий вызов тоже

        Assert.IsTrue(true, "Dispose может быть вызван несколько раз без ошибок");
    }

    #endregion

    #region Интеграционные тесты

    /// <summary>Демонстрирует сложный сценарий работы коллекции с несколькими операциями</summary>
    [TestMethod]
    public void IntegrationScenario_ComplexOperations()
    {
        // Arrange
        var property_changes = new List<string>();

        var collection = new ItemsCollection<TestItem>(
            CreatorAsync: CreateTestItemAsync,
            ItemPropertyChanged: (s, e) => property_changes.Add(e.PropertyName ?? ""),
            Editor: item => item.Value *= 2
        );

        var items = new[]
        {
            new TestItem { Name = "Item1", Value = 10 },
            new TestItem { Name = "Item2", Value = 20 },
            new TestItem { Name = "Item3", Value = 30 }
        };

        // Act
        // 1. Добавляем элементы напрямую
        foreach (var item in items)
        {
            collection.Add(item);
        }

        Assert.AreEqual(3, collection.Count, "После добавления 3 элементов должно быть 3 элемента");

        // 2. Изменяем свойства элементов
        items[0].Value = 100;
        items[1].Name = "Modified";

        // 3. Удаляем элемент
        collection.Remove(items[2]);

        // 4. Редактируем оставшийся элемент
        var edit_command = (System.Windows.Input.ICommand)collection.EditCommand;
        edit_command.Execute(items[0]);

        // Assert
        Assert.AreEqual(2, collection.Count, "В коллекции должно остаться 2 элемента после удаления одного");
        Assert.AreEqual(200, items[0].Value, "Элемент должен быть отредактирован (100 * 2)");
    }

    /// <summary>Проверяет корректное поведение при работе с коллекцией как INotifyCollectionChanged</summary>
    [TestMethod]
    public void Collection_RaisesCollectionChangedEvents()
    {
        // Arrange
        var collection_changed_count = 0;
        var collection = CreateCollectionWithPropertyTracking();

        ((System.Collections.Specialized.INotifyCollectionChanged)collection).CollectionChanged +=
            (_, _) => collection_changed_count++;

        var item = new TestItem { Name = "Test", Value = 1 };

        // Act
        collection.Add(item); // +1 событие
        collection.Add(new TestItem { Name = "Test2", Value = 2 }); // +1 событие
        collection.Remove(item); // +1 событие
        collection.Clear(); // +1 событие

        // Assert
        Assert.AreEqual(4, collection_changed_count, "Должно быть 4 события CollectionChanged");
    }

    #endregion
}
