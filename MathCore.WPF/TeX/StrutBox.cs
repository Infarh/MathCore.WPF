using System.Windows.Media;

// Box representing whitespace.
namespace MathCore.WPF.TeX
{
    internal class StrutBox : Box
    {
        private static readonly StrutBox emptyStrutBox = new(0, 0, 0, 0);

        public static StrutBox Empty => emptyStrutBox;

        public StrutBox(double width, double height, double depth, double shift)
        {
            Width = width;
            Height = height;
            Depth = depth;
            Shift = shift;
        }

        public override void Draw(DrawingContext drawingContext, double scale, double x, double y)
        {
        }

        public override int GetLastFontId() => TexFontUtilities.NoFontId;
    }
}