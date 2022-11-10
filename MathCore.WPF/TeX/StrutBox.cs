using System.Windows.Media;

// Box representing whitespace.
namespace MathCore.WPF.TeX;

internal class StrutBox : Box
{
    private static readonly StrutBox __EmptyStrutBox = new(0, 0, 0, 0);

    public static StrutBox Empty => __EmptyStrutBox;

    public StrutBox(double width, double height, double depth, double shift)
    {
        Width  = width;
        Height = height;
        Depth  = depth;
        Shift  = shift;
    }

    public override void Draw(DrawingContext Context, double scale, double x, double y)
    {
    }

    public override int GetLastFontId() => TexFontUtilities.NoFontId;
}