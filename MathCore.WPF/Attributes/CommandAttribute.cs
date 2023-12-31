using System.Windows.Markup;

namespace MathCore.WPF.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute(string Name) : Attribute
{
    public CommandAttribute() : this(null) { }

    [ConstructorArgument(nameof(Name))]
    public string? Name { get; set; } = Name;
}