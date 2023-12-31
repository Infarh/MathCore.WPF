using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

using MathCore.Annotations;
// ReSharper disable EventNeverSubscribedTo.Global

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantExtendsListEntry

namespace MathCore.WPF;

public class CollectionViewFilter<TCriteria, TItem>(ObservableCollection<CollectionViewFilterItem<TCriteria>> filters) 
    : ReadOnlyObservableCollection<CollectionViewFilterItem<TCriteria>>(filters) 
    where TCriteria : notnull
{
    public CollectionViewFilter(ICollectionView view, Func<TItem, TCriteria> selector, string? Name = null) : this([])
    {
        _Name = Name;
        _View = view;
        _Selector = selector;
        ((INotifyCollectionChanged)view.SourceCollection).CollectionChanged += OnCollectionChanged;
        view.CollectionChanged += OnViewCollectionChanged;
    }

    private readonly ICollectionView? _View;
    
    private readonly Func<TItem, TCriteria>? _Selector;
    
    private readonly ObservableCollection<CollectionViewFilterItem<TCriteria>> _FiltersCollection = filters;
    
    private readonly Dictionary<TCriteria, CollectionViewFilterItem<TCriteria>> _Filters = [];
    
    private bool _Enabled;
    
    private bool _AllFiltersDisabled = true;
    
    private string? _Name;

    public bool Enabled
    {
        get => _Enabled;
        set
        {
            if (_Enabled == value) return;
            _Enabled = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Enabled)));
            _View?.Refresh();
        }
    }

    public string? Name
    {
        get => _Name;
        set
        {
            if (string.Equals(_Name, value)) return;
            _Name = value;
            NameChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public event EventHandler? NameChanged;

    private void OnViewCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs? E)
    {
        if (_View is not { } view || _Selector is not { } selector) return;
        var keys = view.OfType<TItem>().GroupBy(selector).Select(g => g.Key).ToList();
        foreach (var filter in _FiltersCollection)
            if (filter.Key is { } key)
                filter.ExistInView = keys.Contains(key);
    }

    private void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
    {
        switch (E.Action)
        {
            default: throw new ArgumentOutOfRangeException();
            case NotifyCollectionChangedAction.Add:
                if (E.NewItems is IEnumerable<TCriteria> added) AddValues(added);
                break;
            case NotifyCollectionChangedAction.Remove:
                if (E.OldItems is IEnumerable<TCriteria> removed) RemoveValues(removed);
                break;
            case NotifyCollectionChangedAction.Replace:
                if (E.OldItems is IEnumerable<TCriteria> old) RemoveValues(old);
                if (E.NewItems is IEnumerable<TCriteria> @new) AddValues(@new);
                break;
            case NotifyCollectionChangedAction.Move: break;
            case NotifyCollectionChangedAction.Reset:
                foreach (var filter in _FiltersCollection) filter.EnabledChanged -= OnFilterEnableChanged;
                _FiltersCollection.Clear();
                _Filters.Clear();

                if (_View is not { IsEmpty: false } || _Selector is not { } selector) break;
                foreach (var group in _View.SourceCollection.OfType<TItem>().GroupBy(selector))
                {
                    var filter = new CollectionViewFilterItem<TCriteria>(group.Key, group);
                    filter.EnabledChanged += OnFilterEnableChanged;
                    _FiltersCollection.Add(filter);
                    _Filters.Add(group.Key, filter);
                }

                break;
        }
    }

    private void AddValues(IEnumerable values)
    {
        if(_Selector is not { } selector) return;
        foreach (TItem value in values)
        {
            var key = selector(value);
            if (!_Filters.TryGetValue(key, out var filter))
            {
                filter = new CollectionViewFilterItem<TCriteria>(key);
                filter.EnabledChanged += OnFilterEnableChanged;
                _Filters.Add(key, filter);
                _FiltersCollection.Add(filter);
            }
            filter.Add(value);
        }
    }

    private void RemoveValues([ItemCanBeNull] IEnumerable values)
    {
        if(_Selector is not { } selector) return;
        foreach (TItem value in values)
        {
            var key = selector(value);
            if (!_Filters.TryGetValue(key, out var filter)) continue;
            filter.Remove(value);
            if (filter.Count != 0) continue;
            filter.EnabledChanged -= OnFilterEnableChanged;
            _FiltersCollection.Remove(filter);
            _Filters.Remove(key);
        }
    }

    private void OnFilterEnableChanged(object? sender, EventArgs? e)
    {
        _AllFiltersDisabled = !_FiltersCollection.Any(f => f.Enabled);
        _View?.Refresh();
    }

    public void Filter(object? Sender, FilterEventArgs E)
    {
        if (E.Item is not TItem item || !_Enabled || _AllFiltersDisabled || _Selector is not { } selector) return;
        var key = selector(item);
        if (!_Filters.TryGetValue(key, out var filter)) return;
        if (!filter.Enabled) E.Accepted = false;
    }
}

public class CollectionViewFilter<TCriteria> : ReadOnlyObservableCollection<CollectionViewFilterItem<TCriteria>> where TCriteria : notnull
{
    private readonly ICollectionView? _View;
    
    private readonly Func<object, TCriteria>? _Selector;
    
    private readonly ObservableCollection<CollectionViewFilterItem<TCriteria>> _FiltersCollection;
    
    private readonly Dictionary<TCriteria, CollectionViewFilterItem<TCriteria>> _Filters = [];
    
    private bool _Enabled;
    
    private bool _AllFiltersDisabled = true;
    
    private string? _Name;

    public bool Enabled
    {
        get => _Enabled;
        set
        {
            if (_Enabled == value) return;
            _Enabled = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Enabled)));
            _View?.Refresh();
        }
    }

    public string? Name
    {
        get => _Name;
        set
        {
            if (string.Equals(_Name, value)) return;
            _Name = value;
            NameChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? NameChanged;

    private CollectionViewFilter(ObservableCollection<CollectionViewFilterItem<TCriteria>> filters) : base(filters) => _FiltersCollection = filters;

    public CollectionViewFilter(ICollectionView view, Func<object, TCriteria> selector, string? Name = null) : this([])
    {
        _Name = Name;
        _View = view;
        _Selector = selector;
        ((INotifyCollectionChanged)view.SourceCollection).CollectionChanged += OnCollectionChanged;
        view.CollectionChanged += OnViewCollectionChanged;
    }

    private void OnViewCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs? E)
    {
        if (_View is not { } view || _Selector is not { } selector) return;
        var keys = view.OfType<object>().GroupBy(selector).Select(g => g.Key).ToList();
        foreach (var filter in _FiltersCollection)
            if (filter.Key is { } key)
                filter.ExistInView = keys.Contains(key);
    }

    private void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
    {
        switch (E.Action)
        {
            default: throw new InvalidOperationException();

            case NotifyCollectionChangedAction.Add:
                if (E.NewItems is IEnumerable<TCriteria> added) AddValues(added);
                break;

            case NotifyCollectionChangedAction.Remove:
                if (E.OldItems is IEnumerable<TCriteria> removed) RemoveValues(removed);
                break;

            case NotifyCollectionChangedAction.Replace:
                if (E.OldItems is IEnumerable<TCriteria> old) RemoveValues(old);
                if (E.NewItems is IEnumerable<TCriteria> @new) AddValues(@new);
                break;

            case NotifyCollectionChangedAction.Move: break;
            case NotifyCollectionChangedAction.Reset:
                ResetCollection();
                break;
        }
    }

    private void ResetCollection()
    {
        foreach (var filter in _FiltersCollection) 
            filter.EnabledChanged -= OnFilterEnableChanged;

        _FiltersCollection.Clear();
        _Filters.Clear();

        if (_View is not { IsEmpty: false } || _Selector is not { } selector) 
            return;

        foreach (var group in _View.SourceCollection.Cast<object>().GroupBy(selector))
        {
            var filter = new CollectionViewFilterItem<TCriteria>(group.Key, group);
            filter.EnabledChanged += OnFilterEnableChanged;
            _FiltersCollection.Add(filter);
            _Filters.Add(group.Key, filter);
        }
    }

    private void AddValues([ItemCanBeNull] IEnumerable values)
    {
        if (_Selector is not { } selector) return;
        foreach (var value in values)
        {
            var key = selector(value);
            if (!_Filters.TryGetValue(key, out var filter))
            {
                filter = new CollectionViewFilterItem<TCriteria>(key);
                filter.EnabledChanged += OnFilterEnableChanged;
                _Filters.Add(key, filter);
                _FiltersCollection.Add(filter);
            }

            filter.Add(value);
        }
    }

    private void RemoveValues([ItemCanBeNull] IEnumerable values)
    {
        if (_Selector is not { } selector) return;
        foreach (var value in values)
        {
            var key = selector(value);
            if (!_Filters.TryGetValue(key, out var filter)) continue;
            filter.Remove(value);
            if (filter.Count != 0) continue;
            filter.EnabledChanged -= OnFilterEnableChanged;
            _FiltersCollection.Remove(filter);
            _Filters.Remove(key);
        }
    }

    private void OnFilterEnableChanged(object? sender, EventArgs? e)
    {
        _AllFiltersDisabled = !_FiltersCollection.Any(f => f.Enabled);
        _View?.Refresh();
    }

    public void Filter(object? Sender, FilterEventArgs E)
    {
        if (_Selector is not { } selector) return;
        if (!_Enabled || _AllFiltersDisabled) return;
        var value = E.Item;
        var key = selector(value);
        if (!_Filters.TryGetValue(key, out var filter)) return;
        if (!filter.Enabled) E.Accepted = false;
    }
}

/// <summary>Фильтр модели представления коллекций</summary>
public static class CollectionViewFilter
{
    /// <summary>Задействованные представления</summary>
    private static readonly HashSet<CollectionViewSource> __Collections = [];

    /// <summary>Регулярное выражение проверки корректности имени свойства</summary>
    private static readonly Regex __PropertyNameRegex = new(@"@?[a-zA-Z][\w_]*", RegexOptions.Compiled | RegexOptions.Singleline);

    #region Свойство Converter : IValueConverter - Объект-конвертер, выделяющий из объектов фильтруемое значение

    /// <summary>Объект-конвертер, выделяющий из объектов фильтруемое значение</summary>
    public static readonly DependencyProperty ConverterProperty =
        DependencyProperty.RegisterAttached(
            "Converter",
            typeof(IValueConverter),
            typeof(CollectionViewFilter),
            new PropertyMetadata(default(IValueConverter)));

    /// <summary>Объект-конвертер, выделяющий из объектов фильтруемое значение</summary>
    public static void SetConverter(DependencyObject element, IValueConverter value) => element.SetValue(ConverterProperty, value);

    /// <summary>Объект-конвертер, выделяющий из объектов фильтруемое значение</summary>
    [AttachedPropertyBrowsableForType(typeof(CollectionViewSource))]
    public static IValueConverter? GetConverter(DependencyObject element) => (IValueConverter?)element.GetValue(ConverterProperty);

    #endregion

    #region Свойство StringComparisonType : StringComparison - Метод сравнения строк (по умолчанию InvariantCultureIgnoreCase)

    /// <summary>Метод сравнения строк (по умолчанию InvariantCultureIgnoreCase)</summary>
    public static readonly DependencyProperty StringComparisonTypeProperty =
        DependencyProperty.RegisterAttached(
            "StringComparisonType",
            typeof(StringComparison),
            typeof(CollectionViewFilter),
            new PropertyMetadata(StringComparison.InvariantCultureIgnoreCase, PropertyChanged));

    /// <summary>Установка метода сравнения строк для указанной модели представления коллекции</summary>
    /// <param name="element">Представление, для которой производится установка значения метода сравнения строк</param>
    /// <param name="value">Устанавливаемый метод сравнения строк</param>
    public static void SetStringComparisonType(DependencyObject element, StringComparison value) => element.SetValue(StringComparisonTypeProperty, value);

    /// <summary>Получить метод сравнения строк для указанной модели представления коллекции</summary>
    /// <param name="element">Модель представления коллекции, для которой устанавливается метод сравнения строк</param>
    /// <returns>Метод сравнения строк</returns>
    [AttachedPropertyBrowsableForType(typeof(CollectionViewSource))]
    public static StringComparison GetStringComparisonType(DependencyObject element) => (StringComparison)element.GetValue(StringComparisonTypeProperty);

    #endregion

    #region Свойство TextCompareType : TextCompareType - Способ сравнения строк

    /// <summary>Способ сравнения строк</summary>
    public static readonly DependencyProperty TextCompareTypeProperty =
        DependencyProperty.RegisterAttached(
            "TextCompareType",
            typeof(TextCompareType),
            typeof(CollectionViewFilter),
            new PropertyMetadata(TextCompareType.Contains, PropertyChanged));

    /// <summary>Установка способа сравнения строк для модели представления коллекции</summary>
    /// <param name="element">Модель представления коллекции, для которой устанавливается способ сравнения строк</param>
    /// <param name="value">Устанавливаемый способ сравнения строк</param>
    public static void SetTextCompareType(DependencyObject element, TextCompareType value) => element.SetValue(TextCompareTypeProperty, value);

    /// <summary>Получить способ сравнения строк для указанной модели представления коллекции</summary>
    /// <param name="element">Модель представления коллекции, для которой требуется получить способ сравнения строк</param>
    /// <returns>Способ сравнения строк</returns>
    [AttachedPropertyBrowsableForType(typeof(CollectionViewSource))]
    public static TextCompareType GetTextCompareType(DependencyObject element) => (TextCompareType)element.GetValue(TextCompareTypeProperty);

    #endregion

    #region Свойство PropertyPath : string - Имя свойства элемента коллекции, по которому будет производиться фильтрация её элементов

    /// <summary>Имя свойства элемента коллекции, по которому будет производиться фильтрация её элементов</summary>
    public static readonly DependencyProperty PropertyNameProperty =
        DependencyProperty.RegisterAttached(
            "PropertyName",
            typeof(string),
            typeof(CollectionViewFilter),
            new PropertyMetadata(
                default(string), PropertyChanged),
            v => v is null || v is string value && (value.Length == 0 || __PropertyNameRegex.IsMatch(value)));

    /// <summary>Установить имя фильтруемого свойства для модели представления коллекции</summary>
    /// <param name="element">Модель представления коллекции, для которой устанавливается имя фильтруемого свойства её элемента</param>
    /// <param name="value">Имя свойства</param>
    public static void SetPropertyName(DependencyObject element, string value) => element.SetValue(PropertyNameProperty, value);

    /// <summary>Получить имя фильтруемого свойства для модели представления коллекции</summary>
    /// <param name="element">Модель представления коллекции, имя фильтруемого свойство которой требуется получить</param>
    /// <returns>Имя фильтруемого свойства элементов коллекции</returns>
    [AttachedPropertyBrowsableForType(typeof(CollectionViewSource))]
    public static string GetPropertyName(DependencyObject element) => (string)element.GetValue(PropertyNameProperty);

    #endregion

    #region Свойство FilterText : string - Текст фильтра

    /// <summary>Текст фильтра</summary>
    public static readonly DependencyProperty FilterTextProperty =
        DependencyProperty.RegisterAttached(
            "FilterText",
            typeof(string),
            typeof(CollectionViewFilter),
            new PropertyMetadata(default(string), PropertyChanged));

    /// <summary>Установить текст фильтра для модели представления коллекции</summary>
    /// <param name="element">Модель представления коллекции, текст фильтра для которой требуется установить</param>
    /// <param name="value">Устанавливаемый текст фильтра</param>
    public static void SetFilterText(DependencyObject element, string value) => element.SetValue(FilterTextProperty, value);

    /// <summary>Получить значение текста фильтра для модели представления коллекции</summary>
    /// <param name="element">Модель представления коллекции, текст фильтра для которой требуется получить</param>
    /// <returns>Текст фильтра, установленный для модели представления коллекции</returns>
    [AttachedPropertyBrowsableForType(typeof(CollectionViewSource))]
    public static string GetFilterText(DependencyObject element) => (string)element.GetValue(FilterTextProperty);

    #endregion

    #region Свойство DelayTime : int - Время задержки обновления модели представления коллекции в миллисекундах (по умолчанию 500мс)

    /// <summary>Время задержки обновления модели представления коллекции в миллисекундах (по умолчанию 500мс)</summary>
    public static readonly DependencyProperty DelayTimeProperty =
        DependencyProperty.RegisterAttached(
            "DelayTime",
            typeof(int),
            typeof(CollectionViewFilter),
            new PropertyMetadata(500));

    public static void SetDelayTime(DependencyObject element, int value) => element.SetValue(DelayTimeProperty, value);

    [AttachedPropertyBrowsableForType(typeof(CollectionViewSource))]
    public static int GetDelayTime(DependencyObject element) => (int)element.GetValue(DelayTimeProperty);

    #endregion

    /* ------------------------------------------------------------------------------------------------------------------- */

    /// <summary>Время входа в метод изменения значения свойства фильтра в миллисекундах с начала работы системы</summary>
    private static int __PropertyChangedEnterTime;

    /// <summary>Метод обновления значения присоединённого свойства зависимости для фильтра модели представления коллекции</summary>
    /// <param name="D">Модель представления коллекции, для которой изменяется значение свойства</param>
    /// <param name="E">Информация об изменившемся свойстве</param>
    private static async void PropertyChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        if (D is not CollectionViewSource view_source) return;
        using (view_source.DeferRefresh())
        {
            var enter_time = __PropertyChangedEnterTime = Environment.TickCount;

            if (!__Collections.Contains(view_source))
                Initialize(view_source);

            var update_delay_time = GetDelayTime(D);
            if (update_delay_time > 0)
                await Task.Delay(update_delay_time).ConfigureAwait(true);

            if (__PropertyChangedEnterTime == enter_time)
                view_source.View?.Refresh();
        }
    }

    /// <summary>Инициализация новой модели представления коллекции</summary>
    /// <param name="collection_view_source">Модель представления коллекции, которую требуется инициализировать</param>
    private static void Initialize(CollectionViewSource? collection_view_source)
    {
        if (collection_view_source is null || !__Collections.Add(collection_view_source)) return;
        collection_view_source.Filter += CollectionViewSource_OnFilter;
    }

    /// <summary>Информация о свойстве для указанного типа</summary>
    private readonly struct TypeProperty : IEquatable<TypeProperty>
    {
        /// <summary>Тип, информацию о свойстве которого требуется сохранить</summary>
        private readonly Type _Type;

        /// <summary>Имя свойства</summary>
        private readonly string _Property;

        /// <summary>Инициализация новой структуры с информацией о свойстве типа объекта</summary>
        /// <param name="item">Объект, имя свойства которого требуется получить</param>
        /// <param name="property">Имя свойства объекта</param>
        public TypeProperty(object item, string property)
        {
            _Type = item.GetType();
            _Property = property;
        }

        /// <summary>Получить делегат метод извлечения значения свойства</summary>
        /// <returns>Делегат, извлекающий значение свойства объекта</returns>
        public Delegate GetProperty()
        {
            var property = _Type.GetProperty(_Property, BindingFlags.Instance | BindingFlags.Public);

            if (property is null) throw new InvalidOperationException("Указанное свойство в типе не найдено");
            if (!property.CanRead) throw new InvalidOperationException("Свойство только для записи");

            var method_info = property.GetGetMethod();
            return Delegate.CreateDelegate(typeof(Func<,>).MakeGenericType(_Type, method_info!.ReturnType), method_info);
        }

        /// <inheritdoc />
        public bool Equals(TypeProperty other) => _Type == other._Type && string.Equals(_Property, other._Property);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is TypeProperty property && Equals(property);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_Type != null ? _Type.GetHashCode() : 0) * 397) ^ (_Property != null ? _Property.GetHashCode() : 0);
            }
        }
    }

    /// <summary>Словарь свойств типов объектов</summary>
    private static readonly Dictionary<TypeProperty, Delegate> __Properties = [];

    /// <summary>Метод фильтрации элементов модели представления коллекции</summary>
    /// <param name="sender">Модель представления коллекции, фильтрацию объекта которой требуется осуществить</param>
    /// <param name="e">Информация о объекте, который надо отфильтровать</param>
    private static void CollectionViewSource_OnFilter(object sender, FilterEventArgs e)
    {
        if (sender is not CollectionViewSource view_source) return;

        if (GetFilterText(view_source) is not { Length: > 0 } text) return;

        var item = e.Item;
        if (GetPropertyName(view_source) is { Length: > 0 } path)
        {
            var property = __Properties.GetValueOrAddNew(new TypeProperty(item, path), tp => tp.GetProperty());
            item = property.DynamicInvoke(item)!;
        }
        else if (GetConverter(view_source) is { } converter)
            item = converter.Convert(item, typeof(string), null, CultureInfo.CurrentCulture);

        FilterCollection(e, item, view_source, text);
    }

    private static void FilterCollection(FilterEventArgs e, object? item, CollectionViewSource view_source, string text)
    {
        var item_text = item?.ToString() ?? string.Empty;
        var compare_type = GetTextCompareType(view_source);
        var string_comparison = GetStringComparisonType(view_source);
        switch (compare_type)
        {
            default: throw new InvalidEnumArgumentException(nameof(compare_type), (int)compare_type, typeof(TextCompareType));

            case TextCompareType.Contains:
                if (item_text.IndexOf(text, string_comparison) < 0) e.Accepted = false;
                break;

            case TextCompareType.Equals:
                if (!item_text.Equals(text, string_comparison)) e.Accepted = false;
                break;

            case TextCompareType.StartWith:
                if (!item_text.StartsWith(text, string_comparison)) e.Accepted = false;
                break;

            case TextCompareType.EndWith:
                if (!item_text.EndsWith(text, string_comparison)) e.Accepted = false;
                break;
        }
    }

    #region Свойство Filters : CollectionViewFiltersCollection - Набор критериев фильтрации коллекции

    /// <summary>Набор критериев фильтрации коллекции</summary>
    public static readonly DependencyProperty FiltersProperty =
        DependencyProperty.RegisterAttached(
            "ShadowFilters",
            typeof(CollectionViewFiltersCollection),
            typeof(CollectionViewFilter),
            new FrameworkPropertyMetadata(default(CollectionViewFiltersCollection)));

    public static void SetFilters(DependencyObject element, CollectionViewFiltersCollection items) => element.SetValue(
        FiltersProperty, items.SetCollectionView((CollectionViewSource)element));

    public static CollectionViewFiltersCollection GetFilters(DependencyObject element)
    {
        if (element.GetValue(FiltersProperty) is not CollectionViewFiltersCollection filters)
            SetFilters(element, filters = new CollectionViewFiltersCollection((CollectionViewSource)element));
        return filters;
    }

    #endregion
}