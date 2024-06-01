namespace MathCore.WPF.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class ColumnWidthAttribute : Attribute
{
    public bool Auto { get; set; }

    public bool Adaptive { get; set; }

    public double Width { get; set; } = double.NaN;
}
