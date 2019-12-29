using System.Windows;
using System.Windows.Media;

namespace MathCore.WPF.TeX
{
    public class TexRenderer
    {
        public Box Box { get; set; }

        public double Scale { get; }

        public Size RenderSize => new Size(Box.Width * Scale, Box.TotalHeight * Scale);

        public double Baseline => Box.Height / Box.TotalHeight * Scale;
        internal TexRenderer(Box box, double scale) { Box = box; Scale = scale; }

        public void Render(DrawingContext drawingContext, double x, double y)
        {
            var box = Box;
            var scale = Scale;
            box.Draw(drawingContext, scale, x / scale, y / scale + box.Height);
        }
    }
}
