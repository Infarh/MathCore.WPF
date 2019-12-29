using System.Windows;
using System.Windows.Media;

// Box representing horizontal line.
namespace MathCore.WPF.TeX
{
    internal class HorizontalRule : Box
    {
        public HorizontalRule(double thickness, double width, double shift)
        {
            Width = width;
            Height = thickness;
            Shift = shift;
        }

        public override void Draw(DrawingContext drawingContext, double scale, double x, double y) => drawingContext.DrawRectangle(Brushes.Black, null, new Rect(
            x * scale, (y - Height) * scale, Width * scale, Height * scale));

        public override int GetLastFontId() => TexFontUtilities.NoFontId;
    }
}
