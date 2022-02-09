using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Markup;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF;

[MarkupExtensionReturnType(typeof(PropertyFilterItem))]
public class PropertyFilterItem : ViewModel
{
    private bool _Enabled;

    private PropertyInfo[]? _PropertyChain;

    public bool Enabled
    {
        get => _Enabled;
        set => Set(ref _Enabled, value);
    }

    private string? _Property;

    public string? Property
    {
        get => _Property;
        set
        {
            if (!Set(ref _Property, value)) return;

            _PropertyChain = null;
            if (_ItemType is not { } type || string.IsNullOrWhiteSpace(value))
                return;

            var properties = value.Split('.');
            var property_chain = new List<PropertyInfo>();

            foreach (var property_name in properties)
                if (!string.IsNullOrWhiteSpace(property_name))
                {
                    const BindingFlags instance = BindingFlags.Instance | BindingFlags.Public;
                    if (type.GetProperty(property_name, instance) is not { } property_info)
                        return;

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
            var property_type = _PropertyChain is { Length: > 0 } chain
                ? chain[^1].PropertyType
                : null;

            if (property_type?.IsGenericType != true)
                return property_type;

            var type_argument = property_type.GetGenericArguments()[0];
            return property_type == typeof(Nullable<>).MakeGenericType(type_argument)
                ? type_argument
                : property_type;
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

    public Type? ItemType
    {
        get => _ItemType;
        set => SetValue(ref _ItemType, value).Update(nameof(PropertyInfos), nameof(Properties));
    }

    private ComparisonType _Comparison = ComparisonType.Equal;

    public ComparisonType Comparison
    {
        get => _Comparison;
        set => Set(ref _Comparison, value);
    }

    private object? _Value;

    public object? Value
    {
        get => _Value;
        set
        {
            value = ChangeValueType(value);
            if (Equals(_Value, value)) 
                return;

            _Value = value;
            ValueType = _Value?.GetType();
            OnPropertyChanged();
        }
    }

    private Type? _ValueType;

    public Type? ValueType
    {
        get => _ValueType; 
        set => Set(ref _ValueType, value);
    }

    private object? ChangeValueType(object? value)
    {
        if (value is null) return null;
        if (PropertyType is not { } property_type || _PropertyChain is not { Length: > 0 }) 
            return value;

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

    public bool CanBeNull
    {
        get => _CanBeNull;
        set => Set(ref _CanBeNull, value);
    }

    public bool Filter(object item)
    {
        if (!_Enabled 
            || string.IsNullOrWhiteSpace(_Property) 
            || _Value is null 
            || !PropertyType?.IsAssignableFrom(_ValueType) == true)
            return true;

        if (CollectionViewFilterItem.GetComplexPropertyValue(item, _Property) is not { } value) 
            return _CanBeNull;

        if (value is not IComparable comparable)
            return Equals(value, _Value);

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