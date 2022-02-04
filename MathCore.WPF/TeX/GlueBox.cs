using System.Windows.Media;

// Box representing glue.
namespace MathCore.WPF.TeX
{
    internal class GlueBox : Box
    {
        public GlueBox(double space, double stretch, double shrink)
        {
            Width = space;
            Stretch = stretch;
            Shrink = shrink;
        }

        public double Stretch
        {
            get;
            private set;
        }

        public double Shrink
        {
            get;
            private set;
        }

        public override void Draw(DrawingContext Context, double scale, double x, double y)
        {
        }

        public override int GetLastFontId() => TexFontUtilities.NoFontId;
    }
}