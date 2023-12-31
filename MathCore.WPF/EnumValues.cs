using System.Windows.Markup;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace MathCore.WPF;

public class EnumValues : MarkupExtension
{
    private Type? _Type;
    public Type? Type
    {
        get => _Type;
        set
        {
            if (value is { IsEnum: false }) throw new ArgumentException("Тип не является перечислением", nameof(value));
            _Type = value;
        }
    }

    public EnumValues() { }

    public EnumValues(Type type) => Type = type;

    public override object? ProvideValue(IServiceProvider sp) => _Type?.GetEnumValues();
}