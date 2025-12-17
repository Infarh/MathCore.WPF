#nullable enable
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

using MathCore.WPF.Commands;

namespace MathCore.WPF;

/// <summary>Коллекция с поддержкой асинхронного создания, удаления и редактирования элементов с автоматической подпиской на события изменения свойств элементов</summary>
/// <remarks>
/// Эта коллекция предоставляет готовые команды для работы с элементами (добавление, удаление, редактирование)
/// и автоматически управляет подписками на событие PropertyChanged для элементов, реализующих INotifyPropertyChanged.
/// Это исключает ручное управление подписками и помогает избежать утечек памяти при правильном использовании Dispose().
/// 
/// Примечания по использованию:
/// - При создании элементов через AddCommand обработчик CreatorAsync должен быть устойчив к исключениям.
///   Ошибки в создании сообщаются через событие ItemCreationFailed.
/// - Коллекция подписывается на изменения свойств только для элементов, реализующих INotifyPropertyChanged.
///   Другие элементы в коллекции игнорируются.
/// - Обязательно вызовите Dispose() для освобождения всех подписок на события элементов, чтобы избежать утечек памяти.
/// - Команды EditCommand и RemoveCommand доступны только если в коллекции есть элемент,
///   соответствующий параметру команды (задаётся через привязку CanExecute).
/// </remarks>
/// <typeparam name="T">Тип элементов коллекции</typeparam>
/// <param name="CreatorAsync">Асинхронная функция создания нового элемента. Вызывается при выполнении AddCommand</param>
/// <param name="ItemPropertyChanged">Обработчик события PropertyChanged элементов. Вызывается при изменении свойства элемента</param>
/// <param name="Editor">Функция редактирования элемента. Вызывается при выполнении EditCommand</param>
public class ItemsCollection<T>(
    Func<Task<T>> CreatorAsync,
    PropertyChangedEventHandler? ItemPropertyChanged = null,
    Action<T>? Editor = null)
    : SelectableCollection<T>, IDisposable
{
    // Отслеживание подписанных элементов для корректной отписки при Dispose
    private readonly HashSet<INotifyPropertyChanged> _SubscribedItems = [];
    private bool _IsDisposed;

    #region События

    /// <summary>Возникает когда элемент успешно добавлен в коллекцию</summary>
    public event EventHandler<ItemEventArgs<T>>? ItemAdded;

    /// <summary>Возникает когда элемент удалён из коллекции</summary>
    public event EventHandler<ItemEventArgs<T>>? ItemRemoved;

    /// <summary>Возникает когда при создании нового элемента произошла ошибка</summary>
    public event EventHandler<ItemCreationFailedEventArgs<T>>? ItemCreationFailed;

    #endregion

    #region Команды

    [field: MaybeNull, AllowNull]
    /// <summary>Команда для асинхронного создания и добавления нового элемента в коллекцию с автоматическим выбором</summary>
    public ICommand AddCommand => field ??= Command.New(OnAddCommandExecuted);

    /// <summary>Обработчик команды добавления элемента</summary>
    private async Task OnAddCommandExecuted()
    {
        try
        {
            // Создаём новый элемент через асинхронную функцию
            var item = await CreatorAsync().ConfigureAwait(false);
            
            // Добавляем элемент в коллекцию
            Add(item);
            
            // Автоматически выбираем новый элемент
            SelectedItem = item;
            
            // Уведомляем подписчиков об успешном добавлении
            ItemAdded?.Invoke(this, new ItemEventArgs<T>(item));
        }
        catch (Exception error)
        {
            // Обработка ошибок при создании элемента
            var args = new ItemCreationFailedEventArgs<T>(error);
            ItemCreationFailed?.Invoke(this, args);
            
            // Если обработчик события не обработал ошибку, пробрасываем её дальше
            if (!args.IsHandled)
                throw;
        }
    }

    [field: MaybeNull]
    /// <summary>Команда для удаления выбранного элемента из коллекции</summary>
    /// <remarks>Команда доступна, если элемент есть в коллекции (проверяется через Contains)</remarks>
    public ICommand RemoveCommand => field ??= Command.New<T>(OnRemoveCommandExecuted, Contains);

    /// <summary>Обработчик команды удаления элемента</summary>
    private void OnRemoveCommandExecuted(T item)
    {
        Remove(item);
        ItemRemoved?.Invoke(this, new ItemEventArgs<T>(item));
    }

    [field: MaybeNull]
    /// <summary>Команда для редактирования элемента через функцию Editor</summary>
    /// <remarks>Команда доступна, если элемент есть в коллекции (проверяется через Contains).
    /// Элемент должен быть редактируем через функцию Editor, переданную в конструктор</remarks>
    public ICommand EditCommand => field ??= Command.New<T>(t => Editor?.Invoke(t!), Contains);

    #endregion

    /// <summary>Подписывает все элементы коллекции на событие PropertyChanged и инициализирует отслеживание</summary>
    private void SubscribeExistingItems()
    {
        if (ItemPropertyChanged is null) return;

        foreach (var item in this.OfType<INotifyPropertyChanged>())
            SubscribeItem(item);
    }

    /// <summary>Подписывает отдельный элемент на изменение свойств</summary>
    private void SubscribeItem(INotifyPropertyChanged item)
    {
        if (_SubscribedItems.Contains(item)) return;

        item.PropertyChanged += ItemPropertyChanged;
        _SubscribedItems.Add(item);
    }

    /// <summary>Отписывает отдельный элемент от изменения свойств</summary>
    private void UnsubscribeItem(INotifyPropertyChanged item)
    {
        if (!_SubscribedItems.Remove(item)) return;

        item.PropertyChanged -= ItemPropertyChanged;
    }

    /// <summary>Отписывает все элементы от событий PropertyChanged</summary>
    private void UnsubscribeAllItems()
    {
        if (ItemPropertyChanged is null) return;

        foreach (var item in _SubscribedItems)
            item.PropertyChanged -= ItemPropertyChanged;

        _SubscribedItems.Clear();
    }

    /// <inheritdoc cref="SelectableCollection{T}.OnCollectionChanged"/>
    /// <remarks>
    /// Переопределение для управления подписками на события элементов при изменении коллекции.
    /// Обрабатывает все типы уведомлений: Add, Remove, Replace, Reset, Move.
    /// </remarks>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        // Обработка каждого типа изменения коллекции
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                // При добавлении элементов подписываемся на их PropertyChanged
                if (e.NewItems?.OfType<INotifyPropertyChanged>() is { } new_items)
                    foreach (var item in new_items)
                        SubscribeItem(item);
                break;

            case NotifyCollectionChangedAction.Remove:
                // При удалении элементов отписываемся от их PropertyChanged
                if (e.OldItems?.OfType<INotifyPropertyChanged>() is { } old_items)
                    foreach (var item in old_items)
                        UnsubscribeItem(item);
                break;

            case NotifyCollectionChangedAction.Replace:
                // При замене элементов: отписываемся от старых и подписываемся на новые
                if (e.OldItems?.OfType<INotifyPropertyChanged>() is { } replaced_old)
                    foreach (var item in replaced_old)
                        UnsubscribeItem(item);

                if (e.NewItems?.OfType<INotifyPropertyChanged>() is { } replaced_new)
                    foreach (var item in replaced_new)
                        SubscribeItem(item);
                break;

            case NotifyCollectionChangedAction.Reset:
                // При полной очистке коллекции отписываемся от всех элементов
                // и переподписываемся на оставшиеся элементы (если какие-то остались)
                UnsubscribeAllItems();
                SubscribeExistingItems();
                break;

            case NotifyCollectionChangedAction.Move:
                // Move не требует управления подписками, только перемещение позиций
                break;
        }

        // Вызываем базовую реализацию для стандартной обработки события
        base.OnCollectionChanged(e);
    }

    /// <summary>Освобождает ресурсы и отписывает все элементы от событий PropertyChanged</summary>
    /// <remarks>Рекомендуется вызывать этот метод, когда коллекция больше не нужна,
    /// чтобы избежать утечек памяти и циклических ссылок на элементы</remarks>
    public void Dispose()
    {
        if (_IsDisposed) return;

        UnsubscribeAllItems();
        _IsDisposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>Деструктор обеспечивает автоматическую очистку подписок</summary>
    ~ItemsCollection()
    {
        Dispose();
    }
}

/// <summary>Аргументы события при добавлении или удалении элемента из коллекции</summary>
/// <typeparam name="T">Тип элемента коллекции</typeparam>
public sealed class ItemEventArgs<T> : EventArgs
{
    /// <summary>Элемент, с которым произошло событие</summary>
    public T Item { get; }

    /// <summary>Инициализирует новый экземпляр аргументов события</summary>
    /// <param name="Item">Элемент, с которым произошло событие</param>
    public ItemEventArgs(T Item) => this.Item = Item;
}

/// <summary>Аргументы события при ошибке создания элемента</summary>
/// <typeparam name="T">Тип элемента коллекции</typeparam>
public sealed class ItemCreationFailedEventArgs<T> : EventArgs
{
    /// <summary>Исключение, которое произошло при создании элемента</summary>
    public Exception Error { get; }

    /// <summary>Флаг, указывающий обработана ли ошибка обработчиком события</summary>
    /// <remarks>Установите в true, если вы обработали ошибку и не хотите её повторного выброса</remarks>
    public bool IsHandled { get; set; }

    /// <summary>Инициализирует новый экземпляр аргументов события</summary>
    /// <param name="Error">Исключение при создании элемента</param>
    public ItemCreationFailedEventArgs(Exception Error) => this.Error = Error;
}
