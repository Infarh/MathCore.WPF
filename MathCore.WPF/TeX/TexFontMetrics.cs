namespace MathCore.WPF.TeX;

/// <summary>Specifies font metrics for single character</summary>
internal class TexFontMetrics
{
    public double Width { get; set; }

    public double Height { get; set; }

    public double Depth { get; set; }

    public double Italic { get; set; }

    public TexFontMetrics(double width, double height, double depth, double ItalicWidth, double scale)
    {
        Width  = width * scale;
        Height = height * scale;
        Depth  = depth * scale;
        Italic = ItalicWidth * scale;
    }
}