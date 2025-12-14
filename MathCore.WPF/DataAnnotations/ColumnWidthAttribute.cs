namespace MathCore.WPF.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ColumnWidthAttribute : Attribute
{
    public bool Auto { get; set; }

    public bool Adaptive { get; set; }

    public double Width { get; set; } = double.NaN;
}
