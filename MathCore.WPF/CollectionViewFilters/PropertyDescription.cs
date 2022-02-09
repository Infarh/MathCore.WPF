namespace MathCore.WPF;

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