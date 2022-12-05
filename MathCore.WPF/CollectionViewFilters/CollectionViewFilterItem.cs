using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

// ReSharper disable InconsistentNaming

namespace MathCore.WPF;

public class CollectionViewFilterItem : Freezable
{
    /// <summary>Обновить представление</summary>
    protected static void RefreshSource(DependencyObject? s, DependencyPropertyChangedEventArgs e) => ((CollectionViewFilterItem)s)?.RefreshSource();

    #region ValueConverter : IValueConverter - конвертер фильтруемого значения

    /// <summary>Конвертер фильтруемого значения</summary>
    public static readonly DependencyProperty ValueConverterProperty =
        DependencyProperty.Register(
            nameof(ValueConverter),
            typeof(IValueConverter),
            typeof(CollectionViewFilterItem),
            new PropertyMetadata(default(IValueConverter)));

    /// <summary>Конвертер фильтруемого значения</summary>
    public IValueConverter? ValueConverter
    {
        get => (IValueConverter)GetValue(ValueConverterProperty); 
        set => SetValue(ValueConverterProperty, value);
    }

    #endregion

    #region FiltredProperty : string - имя фильтруемого свойства объекта

    /// <summary>Имя фильтруемого свойства объекта</summary>
    public static readonly DependencyProperty FiltredPropertyProperty =
        DependencyProperty.Register(
            nameof(FiltredProperty),
            typeof(string),
            typeof(CollectionViewFilterItem),
            new PropertyMetadata(default(string)));

    /// <summary>Имя фильтруемого свойства объекта</summary>
    public string? FiltredProperty
    {
        get => (string)GetValue(FiltredPropertyProperty);
        set => SetValue(FiltredPropertyProperty, value);
    }

    #endregion

    #region FiltredValue : object - Значение фильтруемого свойства

    /// <summary>Значение фильтруемого свойства</summary>
    public static readonly DependencyProperty FiltredValueProperty =
        DependencyProperty.Register(
            nameof(FiltredValue),
            typeof(object),
            typeof(CollectionViewFilterItem),
            new PropertyMetadata(default, RefreshSource));

    /// <summary>Значение фильтруемого свойства</summary>
    public object? FiltredValue
    {
        get => GetValue(FiltredValueProperty);
        set => SetValue(FiltredValueProperty, value);
    }

    #endregion

    #region Enabled : bool - активность фильтра

    /// <summary>Активность фильтра</summary>
    public static readonly DependencyProperty EnabledProperty =
        DependencyProperty.Register(
            nameof(Enabled),
            typeof(bool),
            typeof(CollectionViewFilterItem),
            new PropertyMetadata(true, RefreshSource));

    /// <summary>Активность фильтра</summary>
    public bool Enabled
    {
        get => (bool)GetValue(EnabledProperty); 
        set => SetValue(EnabledProperty, value);
    }

    #endregion

    protected CollectionViewSource? _Source;

    public virtual void SetSource(CollectionViewSource? source)
    {
        if (_Source != null) _Source.Filter -= OnFilter;
        _Source = source;
        if (source != null) source.Filter += OnFilter;
    }

    protected virtual void OnFilter(object Sender, FilterEventArgs E)
    {
        if (!E.Accepted || !Enabled) return;
        switch (FiltredValue)
        {
            default: return;

            case string { Length: > 0 } str:
            {
                var string_value = GetItemValue(E.Item) switch
                {
                    string s => s,
                    { } v => v.ToString(),
                    _ => null
                };
                if (string_value?.IndexOf(str, StringComparison.InvariantCultureIgnoreCase) < 0)
                    E.Accepted = false;
                return;
            }

            case { } obj:
                var value = GetItemValue(E.Item);
                if (!Equals(value, obj))
                    E.Accepted = false;
                break;
        }
    }

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

    protected async Task<object?> GetItemValueAsync(object? item)
    {
        if (item is null) return null;
        var property = FiltredProperty;
        if (property != null)
        {
            var value = property.IndexOf('.') >= 0
                ? await GetComplexPropertyValueAsync(item, property).ConfigureAwait(false)
                : await GetPropertyValueAsync(item, property).ConfigureAwait(false);
            var c = ValueConverter;
            return c is null ? value : c.Convert(value, typeof(object), null, CultureInfo.CurrentCulture);
        }

        var converter = ValueConverter;
        return converter?.Convert(item, typeof(object), null, CultureInfo.CurrentCulture);
    }

    public static object? GetPropertyValue(object? item, string property)
    {
        if (item is null) 
            return null;

        var type = item.GetType();
        var getter_name = $"{type.FullName}:{property}";
        if (__Getter.TryGetValue(getter_name, out var getter)) 
            return getter.DynamicInvoke(item);

        var property_info = type.GetProperty(property);
        if (property_info?.DeclaringType is null) 
            return item;

        var base_getter_name = $"{property_info.DeclaringType.FullName}:{property}";
        if (__Getter.TryGetValue(base_getter_name, out getter)) 
            return getter.DynamicInvoke(item);

        var delegate_tpe = typeof(Func<,>).MakeGenericType(property_info.DeclaringType, property_info.PropertyType);
        getter = Delegate.CreateDelegate(delegate_tpe, property_info.GetGetMethod()!);
        __Getter[getter_name] = getter;
        __Getter[base_getter_name] = getter;
        return getter.DynamicInvoke(item);
    }

    public static async Task<object?> GetPropertyValueAsync(object? item, string property)
    {
        if (item is null) 
            return null;

        var type = item.GetType();
        var getter_name = $"{type.FullName}:{property}";
        if (__Getter.TryGetValue(getter_name, out var getter))
            return await getter.Async(item, (d, v) => d.DynamicInvoke(v)).ConfigureAwait(false);

        var property_info = type.GetProperty(property);
        if (property_info?.DeclaringType is null || !property_info.CanRead)
            return item;

        var base_getter_name = $"{property_info.DeclaringType.FullName}:{property}";
        if (__Getter.TryGetValue(base_getter_name, out getter))
            return await getter.Async(item, (d, v) => d.DynamicInvoke(v)).ConfigureAwait(false);

        var delegate_tpe = typeof(Func<,>).MakeGenericType(property_info.DeclaringType, property_info.PropertyType);
        getter = Delegate.CreateDelegate(delegate_tpe, property_info.GetGetMethod()!);

        __Getter[getter_name] = getter;
        __Getter[base_getter_name] = getter;
        return await getter.Async(item, (d, v) => d.DynamicInvoke(v)).ConfigureAwait(false);
    }

    public static object? GetComplexPropertyValue(object? item, string PropertyPath)
    {
        var properties = PropertyPath.Split('.');
        for (var i = 0; item != null && i < properties.Length; i++)
            item = GetPropertyValue(item, properties[i]);
        return item;
    }

    public static async Task<object?> GetComplexPropertyValueAsync(object? item, string PropertyPath)
    {
        var properties = PropertyPath.Split('.');
        for (var i = 0; item != null && i < properties.Length; i++)
            item = await GetPropertyValueAsync(item, properties[i]).ConfigureAwait(false);
        return item;
    }

    protected override Freezable CreateInstanceCore() => new CollectionViewFilterItem
    {
        Enabled = Enabled,
        FiltredValue = FiltredValue,
        FiltredProperty = FiltredProperty,
        ValueConverter = ValueConverter
    };
}

public class CollectionViewFilterItem<TCriteria> : ReadOnlyObservableCollection<object?>
{
    private readonly ObservableCollection<object?> _InternalCollection;

    private bool _Enabled;

    public bool Enabled
    {
        get => _Enabled;
        set
        {
            if (_Enabled == value) return;
            _Enabled = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Enabled)));
            EnabledChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool _ExistInView;

    public bool ExistInView
    {
        get => _ExistInView;
        internal set
        {
            if (_ExistInView == value) return;
            _ExistInView = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(ExistInView)));
        }
    }

    public event EventHandler? EnabledChanged;

    public TCriteria? Key { get; }

    private CollectionViewFilterItem(ObservableCollection<object?> collection) : base(collection) => _InternalCollection = collection;
    public CollectionViewFilterItem(TCriteria? key) : this(new ObservableCollection<object?>()) => Key = key;

    public CollectionViewFilterItem(TCriteria? key, IEnumerable items) : this(new ObservableCollection<object?>(items.Cast<object?>())) => Key = key;

    internal void Add(object? value) { if (!_InternalCollection.Contains(value)) _InternalCollection.Add(value); }

    internal bool Remove(object? value) => _InternalCollection.Remove(value);
}