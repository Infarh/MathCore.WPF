using System.Windows.Markup;

namespace MathCore.WPF.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    [ConstructorArgument(nameof(Name))]
    public string? Name { get; set; }

    public CommandAttribute() { }

    public CommandAttribute(string Name) => this.Name = Name;
}