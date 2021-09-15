using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

using MathCore.Annotations;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantExtendsListEntry

namespace MathCore.WPF
{
    /// <summary>Способ сравнения строк</summary>
    public enum TextCompareType
    {
        /// <summary>Строка должна содержать искомую строку</summary>
        Contains,
        /// <summary>Строки должны совпадать</summary>
        Equals,
        /// <summary>Строка должна начинаться с искомой последовательности</summary>
        StartWith,
        /// <summary>Строка должна заканчиваться искомой последовательностью</summary>
        EndWith
    }

    /// <summary>Фильтр модели представления коллекций</summary>
    public static class CollectionViewFilter
    {
        /// <summary>Задействованные представления</summary>
        private static readonly HashSet<CollectionViewSource> __Collections = new();
        /// <summary>Регулярное выражение проверки корректности имени свойства</summary>
        private static readonly Regex __PropertyNameRegex = new(@"@?[a-zA-Z][\w_]*", RegexOptions.Compiled | RegexOptions.Singleline);

        #region Converter

        public static readonly DependencyProperty ConverterProperty =
           DependencyProperty.RegisterAttached(
               "Converter",
               typeof(IValueConverter),
               typeof(CollectionViewFilter),
               new PropertyMetadata(default(IValueConverter)));

        public static void SetConverter(DependencyObject element, IValueConverter value) => element.SetValue(ConverterProperty, value);

        [AttachedPropertyBrowsableForType(typeof(CollectionViewSource))]
        public static IValueConverter GetConverter(DependencyObject element) => (IValueConverter)element.GetValue(ConverterProperty);

        #endregion

        #region StringComparisonType

        /// <summary>Метод сравнения строк</summary>
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

        #region TextCompareType

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

        #region PropertyPath

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

        #region FilterText

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

        #region DelayTime

        /// <summary>Время задержки обновления модели представления коллекции в миллисекундах</summary>
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
                if (!__Collections.Contains(view_source)) Initialize(view_source);
                await Task.Delay(GetDelayTime(D)).ConfigureAwait(true);
                if (__PropertyChangedEnterTime == enter_time) view_source.View?.Refresh();
            }
        }

        /// <summary>Инициализация новой модели представления коллекции</summary>
        /// <param name="collection_view_source">Модель представления коллекции, которую требуется инициализировать</param>
        private static void Initialize(CollectionViewSource? collection_view_source)
        {
            if (collection_view_source is null) return;
            __Collections.Add(collection_view_source);
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
        private static readonly Dictionary<TypeProperty, Delegate> __Properties = new();
        /// <summary>Метод фильтрации элементов модели представления коллекции</summary>
        /// <param name="sender">Модель представления коллекции, фильтрацию объекта которой требуется осуществить</param>
        /// <param name="e">Информация о объекте, который надо отфильтровать</param>
        private static void CollectionViewSource_OnFilter(object sender, FilterEventArgs e)
        {
            if (sender is not CollectionViewSource view_source) return;
            var text = GetFilterText(view_source);
            var item = e.Item;
            if (string.IsNullOrEmpty(text) || item is null) return;
            var path = GetPropertyName(view_source);

            if (!string.IsNullOrEmpty(path))
            {
                var property = __Properties.GetValueOrAddNew(new TypeProperty(item, path), tp => tp.GetProperty());
                item = property.DynamicInvoke(item)!;
            }
            else
            {
                var converter = GetConverter(view_source);
                if (converter != null)
                    item = converter.Convert(item, typeof(string), null, CultureInfo.CurrentCulture);
            }

            var item_text = item?.ToString() ?? "";
            var compare_type = GetTextCompareType(view_source);
            var string_comparison = GetStringComparisonType(view_source);
            switch (compare_type)
            {
                default: throw new ArgumentOutOfRangeException();
                case TextCompareType.Contains:
                    if (item_text.IndexOf(text, string_comparison) < 0) e.Accepted = false;
                    break;
                case TextCompareType.Equals:
                    if (!string.Equals(item_text, text, string_comparison)) e.Accepted = false;
                    break;
                case TextCompareType.StartWith:
                    if (!item_text.StartsWith(text, string_comparison)) e.Accepted = false;
                    break;
                case TextCompareType.EndWith:
                    if (!item_text.EndsWith(text, string_comparison)) e.Accepted = false;
                    break;
            }
        }

        public static readonly DependencyProperty FiltersProperty =
            DependencyProperty.RegisterAttached(
                "ShadowFilters",
                typeof(CollectionViewFiltersCollection),
                typeof(CollectionViewFilter),
                new FrameworkPropertyMetadata(default(CollectionViewFiltersCollection)));

        public static void SetFilters(DependencyObject element, CollectionViewFiltersCollection items) => element.SetValue(FiltersProperty, items.SetCollectionView((CollectionViewSource)element));

        public static CollectionViewFiltersCollection GetFilters(DependencyObject element)
        {
            if (element.GetValue(FiltersProperty) is not CollectionViewFiltersCollection filters)
                SetFilters(element, filters = new CollectionViewFiltersCollection((CollectionViewSource)element));
            return filters;
        }
    }

    public class CollectionViewFiltersCollection : FreezableCollection<CollectionViewFilterItem>, IList
    {
        private CollectionViewSource _Source;

        public CollectionViewFiltersCollection(CollectionViewSource item)
        {
            _Source = item;
            ((INotifyCollectionChanged)this).CollectionChanged += OnCollectionChanged;
        }

        public CollectionViewFiltersCollection SetCollectionView(CollectionViewSource item)
        {
            _Source = item;
            return this;
        }

        protected void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems is IEnumerable<CollectionViewFilterItem> added)
                        foreach (var item in added) item.SetSource(_Source);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems is IEnumerable<CollectionViewFilterItem> removed)
                        foreach (var item in removed) item.SetSource(null);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems is IEnumerable<CollectionViewFilterItem> @new)
                        foreach (var item in @new) item.SetSource(_Source);
                    if (e.OldItems is IEnumerable<CollectionViewFilterItem> old)
                        foreach (var item in old) item.SetSource(null);
                    break;
            }
        }

        /// <inheritdoc />
        protected override Freezable CreateInstanceCore()
        {
            var collection = new CollectionViewFiltersCollection(_Source);
            collection.AddItems(this.Select(f => (CollectionViewFilterItem)f.Clone()));
            return collection;
        }
    }

    public abstract class CollectionViewFilterItem : Freezable
    {
        /// <summary>Обновить представление</summary>
        protected static void RefreshSource(DependencyObject? s, DependencyPropertyChangedEventArgs e) => ((CollectionViewFilterItem)s)?.RefreshSource();

        #region ValueConverter - конвертер фильтруемого значения

        /// <summary>Конвертер фильтруемого значения</summary>
        public static readonly DependencyProperty ValueConverterProperty =
            DependencyProperty.Register(
                nameof(ValueConverter),
                typeof(IValueConverter),
                typeof(CollectionViewFilterItem),
                new PropertyMetadata(default(IValueConverter)));

        /// <summary>Конвертер фильтруемого значения</summary>
        public IValueConverter? ValueConverter { get => (IValueConverter)GetValue(ValueConverterProperty); set => SetValue(ValueConverterProperty, value); }

        #endregion

        #region FiltredProperty - имя фильтруемого свойства объекта

        /// <summary>Имя фильтруемого свойства объекта</summary>
        public static readonly DependencyProperty FiltredPropertyProperty =
            DependencyProperty.Register(
                nameof(FiltredProperty),
                typeof(string),
                typeof(CollectionViewFilterItem),
                new PropertyMetadata(default(string)));

        /// <summary>Имя фильтруемого свойства объекта</summary>
        public string? FiltredProperty { get => (string)GetValue(FiltredPropertyProperty); set => SetValue(FiltredPropertyProperty, value); }

        #endregion

        #region Enabled - активность фильтра

        /// <summary>Активность фильтра</summary>
        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.Register(
                nameof(Enabled),
                typeof(bool),
                typeof(CollectionViewFilterItem),
                new PropertyMetadata(true, RefreshSource));

        /// <summary>Активность фильтра</summary>
        public bool Enabled { get => (bool)GetValue(EnabledProperty); set => SetValue(EnabledProperty, value); }

        #endregion

        protected CollectionViewSource? _Source;

        public virtual void SetSource(CollectionViewSource? source)
        {
            if (_Source != null) _Source.Filter -= OnFilter;
            _Source = source;
            if (source != null) source.Filter += OnFilter;
        }

        protected abstract void OnFilter(object Sender, FilterEventArgs E);

        protected void RefreshSource() => _Source?.View?.Refresh();

        private static readonly Dictionary<string, Delegate> __Getter = new();

        protected object? GetItemValue(object? item)
        {
            if (item is null) return null;
            var property = FiltredProperty;
            if (property != null)
            {
                var value = property.Contains('.') ? GetComplexPropertyValue(item, property) : GetPropertyValue(item, property);
                var c = ValueConverter;
                return c is null ? value : c.Convert(value, typeof(object), null, CultureInfo.CurrentCulture);
            }
            var converter = ValueConverter;
            return converter?.Convert(item, typeof(object), null, CultureInfo.CurrentCulture);
        }

        public static object? GetPropertyValue(object? item, string property)
        {
            if (item is null) return null;
            var type = item.GetType();
            var getter_name = $"{type.FullName}:{property}";
            if (__Getter.TryGetValue(getter_name, out var getter)) return getter.DynamicInvoke(item);
            var property_info = type.GetProperty(property);
            if (property_info?.DeclaringType is null) return item;
            var base_getter_name = $"{property_info.DeclaringType.FullName}:{property}";
            if (__Getter.TryGetValue(base_getter_name, out getter)) return getter.DynamicInvoke(item);
            var delegate_tpe = typeof(Func<,>).MakeGenericType(property_info.DeclaringType, property_info.PropertyType);
            getter = Delegate.CreateDelegate(delegate_tpe, property_info.GetGetMethod()!);
            __Getter[getter_name] = getter;
            __Getter[base_getter_name] = getter;
            return getter.DynamicInvoke(item);
        }

        public static object? GetComplexPropertyValue(object? item, string PropertyPath)
        {
            var properties = PropertyPath.Split('.');
            for (var i = 0; item != null && i < properties.Length; i++)
                item = GetPropertyValue(item, properties[i]);
            return item;
        }
    }

    public class RangeCollectionFilterItem : CollectionViewFilterItem
    {
        #region Min - минимум фильтра

        /// <summary>Свойство минимального фильтруемого значения</summary>
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register(
                nameof(Min),
                typeof(IComparable),
                typeof(RangeCollectionFilterItem),
                new PropertyMetadata(default(IComparable), RefreshSource));

        /// <summary>Свойство минимального фильтруемого значения</summary>
        public IComparable? Min { get => (IComparable)GetValue(MinProperty); set => SetValue(MinProperty, value); }

        #endregion

        #region MinInclude - включать нижний предел в выборку

        /// <summary>Включать нижний предел в выборку</summary>
        public static readonly DependencyProperty MinIncludeProperty =
            DependencyProperty.Register(
                nameof(MinInclude),
                typeof(bool),
                typeof(RangeCollectionFilterItem),
                new PropertyMetadata(true, RefreshSource));

        /// <summary>Включать нижний предел в выборку</summary>
        public bool MinInclude { get => (bool)GetValue(MinIncludeProperty); set => SetValue(MinIncludeProperty, value); }

        #endregion

        #region Max - максимум фильтра

        /// <summary>Свойство максимума фильтра</summary>
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register(
                nameof(Max),
                typeof(IComparable),
                typeof(RangeCollectionFilterItem),
                new PropertyMetadata(default(IComparable), RefreshSource));

        /// <summary>Свойство максимума фильтра</summary>
        public IComparable? Max { get => (IComparable)GetValue(MaxProperty); set => SetValue(MaxProperty, value); }

        #endregion

        #region MaxInclude - включать верхний предел в выборку

        /// <summary>Включать верхний предел в выборку</summary>
        public static readonly DependencyProperty MaxIncludeProperty =
            DependencyProperty.Register(
                nameof(MaxInclude),
                typeof(bool),
                typeof(RangeCollectionFilterItem),
                new PropertyMetadata(true, RefreshSource));

        /// <summary>Включать верхний предел в выборку</summary>
        public bool MaxInclude { get => (bool)GetValue(MaxIncludeProperty); set => SetValue(MaxIncludeProperty, value); }

        #endregion

        /// <inheritdoc />
        protected override Freezable CreateInstanceCore()
        {
            var filter = new RangeCollectionFilterItem();
            filter.SetSource(_Source);
            return filter;
        }

        /// <inheritdoc />
        protected override void OnFilter(object Sender, FilterEventArgs E)
        {
            if (!E.Accepted || !Enabled) return;
            var value = GetItemValue(E.Item);
            if (Min is { } min)
            {
                if (MinInclude)
                {
                    if (min.CompareTo(value) > 0) E.Accepted = false;
                }
                else if (min.CompareTo(value) >= 0) E.Accepted = false;
            }

            if (Max is { } max)
            {
                if (MaxInclude)
                {
                    if (max.CompareTo(value) < 0) E.Accepted = false;
                }
                else if (max.CompareTo(value) <= 0) E.Accepted = false;
            }
        }
    }

    public class GroupsCollectionFilterItem : CollectionViewFilterItem
    {
        public ObservableCollection<GroupCollectionFilterItem> Groups { get; } = new();

        private IEnumerable? _ViewSource;

        private void CheckView(IEnumerable? ViewSource)
        {
            if (ReferenceEquals(_ViewSource, ViewSource)) return;
            { if (_ViewSource is INotifyCollectionChanged notify_collection) notify_collection.CollectionChanged -= OnItemsChanged; }
            _ViewSource = ViewSource;
            if (_ViewSource is null) return;
            UpdateGroups();
            { if (_ViewSource is INotifyCollectionChanged notify_collection) notify_collection.CollectionChanged += OnItemsChanged; }
        }

        private void OnItemsChanged(object? Sender, NotifyCollectionChangedEventArgs E)
        {
            switch (E.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (E.NewItems is { } added) foreach (var item in added) AddItem(item);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (E.OldItems is { } old) foreach (var item in old) RemoveItem(item);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (E.OldItems is { } removed) foreach (var item in removed) RemoveItem(item);
                    if (E.NewItems is { } @new) foreach (var item in @new) AddItem(item);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    UpdateGroups();
                    break;
            }
        }

        private void ClearGroups() => Groups.Clear();

        public void UpdateGroups()
        {
            ClearGroups();
            if (_ViewSource is not { } view) return;
            foreach (var item in view) AddItem(item);
        }

        private void AddItem(object item)
        {
            var value = GetItemValue(item);
            if (value is null) return;
            var group = Groups.FirstOrDefault(g => Equals(g.Key, value));
            if (group is null)
            {
                group = new GroupCollectionFilterItem(value);
                if (value is IComparable comparable && Groups.Count > 0)
                {
                    int i;
                    for (i = 0; i < Groups.Count; i++)
                    {
                        if (comparable.CompareTo(Groups[i].Key) >= 0) continue;
                        Groups.Insert(i, group);
                        i = -1;
                        break;
                    }
                    if (i > 0) Groups.Add(group);
                }
                else
                    Groups.Add(group);
                group.EnabledChanged += GroupEnableChanged;
            }
            group.Items.Add(item);
        }

        private void GroupEnableChanged(object? Sender, EventArgs E) => RefreshSource(this, default);

        private void RemoveItem(object item)
        {
            var value = GetItemValue(item);
            if (value is null) return;
            var group = Groups.FirstOrDefault(g => Equals(g.Key, value));
            if (group?.Items.Remove(item) == false || group?.Items.Count != 0) return;
            group.EnabledChanged -= GroupEnableChanged;
            Groups.Remove(group);
        }

        /// <inheritdoc />
        protected override Freezable CreateInstanceCore()
        {
            var filter = new GroupsCollectionFilterItem();
            filter.SetSource(_Source);
            return filter;
        }

        /// <inheritdoc />
        protected override void OnFilter(object Sender, FilterEventArgs E)
        {
            CheckView(((CollectionViewSource)Sender).Source as IEnumerable);
            if (!Enabled || !E.Accepted || !Groups.Any(g => g.Enabled)) return;
            var value = GetItemValue(E.Item);
            var group = Groups.FirstOrDefault(g => Equals(g.Key, value));
            if (group is null) return;
            E.Accepted = group.Enabled;
        }
    }

    public class GroupCollectionFilterItem : DependencyObject
    {
        private bool _Enabled = true;

        public bool Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled == value) return;
                _Enabled = value;
                EnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? EnabledChanged;

        public ObservableCollection<object> Items { get; } = new();

        public object Key { get; }

        public GroupCollectionFilterItem(object key) => Key = key;
    }

    public class PropertyFiltersItem : CollectionViewFilterItem, ICollection<PropertyFilterItem>, INotifyCollectionChanged
    {
        #region CollectionItemType : Type

        /// <summary></summary>
        public static readonly DependencyProperty CollectionItemTypeProperty =
            DependencyProperty.Register(
                nameof(CollectionItemType),
                typeof(Type),
                typeof(PropertyFiltersItem),
                new PropertyMetadata(default(Type), OnCollectionItemTypeChanged));

        private static void OnCollectionItemTypeChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
        {
            if (D is not PropertyFiltersItem filters_item) return;
            foreach (var filter in filters_item)
                filter.ItemType = (Type)E.NewValue;
        }

        /// <summary></summary>
        public Type CollectionItemType { get => (Type)GetValue(CollectionItemTypeProperty); set => SetValue(CollectionItemTypeProperty, value); }

        #endregion

        private readonly ObservableCollection<PropertyFilterItem> _Filters = new();

        public ICommand AddNewFilterCommand { get; }
        public ICommand RemoveCommand { get; }

        public PropertyFiltersItem()
        {
            _Filters.CollectionChanged += OnFiltersCollection_Changed;

            AddNewFilterCommand = new LambdaCommand(OnAddNewCommandExecuted);
            RemoveCommand = new LambdaCommand(OnRemoveCommandExecuted);
            _Filters.Add(new PropertyFilterItem());
        }

        private void OnAddNewCommandExecuted(object? Obj) => _Filters.Add(Obj as PropertyFilterItem ?? new PropertyFilterItem());

        private void OnRemoveCommandExecuted(object? Obj)
        {
            if (Obj is PropertyFilterItem filter) Remove(filter); else Clear();
        }

        private void OnFiltersCollection_Changed(object? Sender, NotifyCollectionChangedEventArgs E)
        {
            switch (E.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (E.NewItems is { } added)
                        foreach (PropertyFilterItem? item in added)
                        {
                            item!.ItemType = CollectionItemType;
                            item.PropertyChanged += OnPropertyFilterItemChanged;
                        }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (E.OldItems is { } removed)
                        foreach (PropertyFilterItem? item in removed)
                            item!.PropertyChanged -= OnPropertyFilterItemChanged;
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (E.OldItems is { } old)
                        foreach (PropertyFilterItem? item in old)
                            item!.PropertyChanged -= OnPropertyFilterItemChanged;
                    if (E.NewItems is { } @new)
                        foreach (PropertyFilterItem? item in @new)
                        {
                            item!.ItemType = CollectionItemType;
                            item.PropertyChanged += OnPropertyFilterItemChanged;
                        }
                    break;
            }
        }

        private void OnPropertyFilterItemChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(PropertyFilterItem.Enabled):
                case nameof(PropertyFilterItem.Property):
                case nameof(PropertyFilterItem.Value):
                case nameof(PropertyFilterItem.CanBeNull):
                case nameof(PropertyFilterItem.Comparison):
                case nameof(PropertyFilterItem.ItemType):
                    RefreshSource();
                    break;
            }
        }

        protected override Freezable CreateInstanceCore() => throw new NotSupportedException();

        protected override void OnFilter(object Sender, FilterEventArgs E)
        {
            if (!E.Accepted) return;
            if (_Filters.Any(f => !f.Filter(E.Item)))
                E.Accepted = false;

        }


        #region ICollection<PropertyFilterItem>

        /// <inheritdoc />
        IEnumerator<PropertyFilterItem> IEnumerable<PropertyFilterItem>.GetEnumerator() => _Filters.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Filters).GetEnumerator();

        /// <inheritdoc />
        public void Add(PropertyFilterItem item) => _Filters.Add(item);

        /// <inheritdoc />
        public void Clear()
        {
            foreach (var filter in _Filters) filter.PropertyChanged -= OnPropertyFilterItemChanged;
            _Filters.Clear();
        }

        /// <inheritdoc />
        bool ICollection<PropertyFilterItem>.Contains(PropertyFilterItem item) => _Filters.Contains(item);

        /// <inheritdoc />
        void ICollection<PropertyFilterItem>.CopyTo(PropertyFilterItem[] array, int index) => _Filters.CopyTo(array, index);

        /// <inheritdoc />
        public bool Remove(PropertyFilterItem item) => _Filters.Remove(item);

        /// <inheritdoc />
        int ICollection<PropertyFilterItem>.Count => _Filters.Count;

        /// <inheritdoc />
        bool ICollection<PropertyFilterItem>.IsReadOnly => false;

        #endregion

        #region INotifyCollectionChanged

        /// <inheritdoc />
        event NotifyCollectionChangedEventHandler? INotifyCollectionChanged.CollectionChanged
        {
            add => _Filters.CollectionChanged += value;
            remove => _Filters.CollectionChanged -= value;
        }

        #endregion
    }

    public enum ComparisonType : byte
    {
        [Description("<")]
        Less,
        [Description("<=")]
        LessOrEqual,
        [Description("==")]
        Equal,
        [Description(">=")]
        GreaterOrEqual,
        [Description(">")]
        Greater,
        [Description("!=")]
        NotEqual
    }

    public class PropertyFilterItem : ViewModel
    {
        private bool _Enabled;
        private PropertyInfo[]? _PropertyChain;

        public bool Enabled { get => _Enabled; set => Set(ref _Enabled, value); }

        private string? _Property;
        public string? Property
        {
            get => _Property;
            set
            {
                if (!Set(ref _Property, value)) return;
                _PropertyChain = null;
                var type = _ItemType;
                if (type is null || string.IsNullOrWhiteSpace(value)) return;
                var properties = value.Split('.');
                var property_chain = new List<PropertyInfo>();

                foreach (var property_name in properties.WhereNot(string.IsNullOrWhiteSpace))
                {
                    var property_info = type.GetProperty(property_name, BindingFlags.Instance | BindingFlags.Public);
                    if (property_info is null) return;
                    property_chain.Add(property_info);
                    type = property_info.PropertyType;
                }

                _PropertyChain = property_chain.ToArray();
            }
        }

        public Type? PropertyType
        {
            get
            {
                var property_type = _PropertyChain is not {Length: > 0} chain
                    ? null
                    : chain[chain.Length-1].PropertyType;
                if (property_type?.IsGenericType != true) return property_type;
                var type_argument = property_type.GetGenericArguments()[0];
                return property_type == typeof(Nullable<>).MakeGenericType(type_argument) ? type_argument : property_type;
            }
        }

        public IEnumerable<PropertyInfo>? PropertyInfos => _ItemType?.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        public IEnumerable<PropertyDescription> Properties => GetProperties(_ItemType, null);//, $"({_ItemType?.Name})");

        private static IEnumerable<PropertyDescription> GetProperties(Type? type, string? BaseProperty, string BasePropertyDisplayName = "")
        {
            if (type is null) yield break;
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var display_name = property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                if (string.IsNullOrWhiteSpace(display_name)) continue;

                var property_type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                if (Type.GetTypeCode(property_type) != TypeCode.Object)
                    yield return new PropertyDescription(property.Name, display_name, BaseProperty, BasePropertyDisplayName);
                else
                    foreach (var sub_property in GetProperties(property_type, $"{BaseProperty}.{property.Name}", $"{BasePropertyDisplayName}.{display_name}"))
                        yield return sub_property;
            }
        }

        private Type? _ItemType;
        public Type? ItemType { get => _ItemType; set => SetValue(ref _ItemType, value).Update(nameof(PropertyInfos), nameof(Properties)); }

        private ComparisonType _Comparison = ComparisonType.Equal;
        public ComparisonType Comparison { get => _Comparison; set => Set(ref _Comparison, value); }

        private object? _Value;
        public object? Value
        {
            get => _Value;
            set
            {
                value = ChangeValueType(value);
                if (Equals(_Value, value)) return;
                _Value = value;
                ValueType = _Value?.GetType();
                OnPropertyChanged();
            }
        }

        private Type? _ValueType;

        public Type? ValueType { get => _ValueType; set => Set(ref _ValueType, value); }

        private object? ChangeValueType(object? value)
        {
            if (value is null) return null;
            var property_type = PropertyType;
            if (_PropertyChain is null || _PropertyChain.Length <= 0 || property_type is null) return value;
            try
            {
                return Convert.ChangeType(value, property_type);
            }
            catch
            {
                return value;
            }
        }

        private bool _CanBeNull = true;
        public bool CanBeNull { get => _CanBeNull; set => Set(ref _CanBeNull, value); }

        public bool Filter(object item)
        {
            if (!_Enabled || string.IsNullOrWhiteSpace(_Property) || _Value is null || !PropertyType?.IsAssignableFrom(_ValueType) == true) return true;

            var value = CollectionViewFilterItem.GetComplexPropertyValue(item, _Property);
            if (value is null) return _CanBeNull;
            if (value is not IComparable comparable) return Equals(value, _Value);

            return value.GetType().IsInstanceOfType(_Value) && _Comparison switch
            {
                ComparisonType.Less => comparable.CompareTo(_Value) < 0,
                ComparisonType.LessOrEqual => comparable.CompareTo(_Value) <= 0,
                ComparisonType.Equal => comparable.CompareTo(_Value) == 0,
                ComparisonType.GreaterOrEqual => comparable.CompareTo(_Value) >= 0,
                ComparisonType.Greater => comparable.CompareTo(_Value) > 0,
                ComparisonType.NotEqual => comparable.CompareTo(_Value) != 0,
                _ => throw new InvalidOperationException()
            };
        }
    }

    public readonly struct PropertyDescription : IEquatable<PropertyDescription>
    {
        public string Name { get; }
        public string DisplayName { get; }
        public string? BaseProperty { get; }
        public string BasePropertyDisplayName { get; }
        public string Path => $"{BaseProperty}.{Name}".TrimStart('.');
        public string DisplayPath => string.IsNullOrEmpty(BasePropertyDisplayName) ? DisplayName : $"{BasePropertyDisplayName}.{DisplayName}";

        public PropertyDescription(string Name, string DisplayName, string? BaseProperty, string BasePropertyDisplayName)
        {
            this.Name = Name;
            this.DisplayName = DisplayName;
            this.BaseProperty = BaseProperty;
            this.BasePropertyDisplayName = BasePropertyDisplayName.TrimStart('.');
        }

        bool IEquatable<PropertyDescription>.Equals(PropertyDescription other) => Name == other.Name && DisplayName == other.DisplayName && BaseProperty == other.BaseProperty && BasePropertyDisplayName == other.BasePropertyDisplayName;

        public override bool Equals(object? obj) => obj is PropertyDescription other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Name, DisplayName, BaseProperty, BasePropertyDisplayName);

        public static bool operator ==(PropertyDescription left, PropertyDescription right) => left.Equals(right);
        public static bool operator !=(PropertyDescription left, PropertyDescription right) => !left.Equals(right);
    }
}