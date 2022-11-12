namespace MathCore.WPF.TeX;

/// <summary>Box representing other box with horizontal rule above it</summary>
internal sealed class OverBar : VerticalBox
{
    public OverBar(Box box, double kern, double thickness)
    {
        Add(new StrutBox(0, thickness, 0, 0));
        Add(new HorizontalRule(thickness, box.Width, 0));
        Add(new StrutBox(0, kern, 0, 0));
        Add(box);
    }
}