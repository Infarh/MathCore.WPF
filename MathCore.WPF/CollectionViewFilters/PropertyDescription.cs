namespace MathCore.WPF;

public readonly struct PropertyDescription(string Name, string DisplayName, string? BaseProperty, string BasePropertyDisplayName) : IEquatable<PropertyDescription>
{
    public string Name { get; } = Name;

    public string DisplayName { get; } = DisplayName;

    public string? BaseProperty { get; } = BaseProperty;

    public string BasePropertyDisplayName { get; } = BasePropertyDisplayName.TrimStart('.');

    public string Path => $"{BaseProperty}.{Name}".TrimStart('.');
    
    public string DisplayPath => BasePropertyDisplayName is { Length: > 0 } ? $"{BasePropertyDisplayName}.{DisplayName}" : DisplayName;

    bool IEquatable<PropertyDescription>.Equals(PropertyDescription other) => Name == other.Name && DisplayName == other.DisplayName && BaseProperty == other.BaseProperty && BasePropertyDisplayName == other.BasePropertyDisplayName;

    public override bool Equals(object? obj) => obj is PropertyDescription other && Equals(other);

    public override int GetHashCode() => HashBuilder.New(Name)
       .Append(DisplayName)
       .Append(BaseProperty)
       .Append(BasePropertyDisplayName);

    public static bool operator ==(PropertyDescription left, PropertyDescription right) => Equals(left, right);
    public static bool operator !=(PropertyDescription left, PropertyDescription right) => !(left == right);
}