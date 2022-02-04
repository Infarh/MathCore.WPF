using System;
using System.Windows.Media;

namespace MathCore.WPF.TeX
{
    /// <summary>Box containing horizontal stack of child boxes</summary>
    internal sealed class HorizontalBox : Box
    {
        private double childBoxesTotalWidth;

        public HorizontalBox(Box box, double width, TexAlignment alignment)
            : this()
        {
            var extraWidth = width - box.Width;
            switch(alignment)
            {
                case TexAlignment.Center:
                    var strutBox = new StrutBox(extraWidth / 2, 0, 0, 0);
                    Add(strutBox);
                    Add(box);
                    Add(strutBox);
                    break;
                case TexAlignment.Left:
                    Add(box);
                    Add(new StrutBox(extraWidth, 0, 0, 0));
                    break;
                case TexAlignment.Right:
                    Add(new StrutBox(extraWidth, 0, 0, 0));
                    Add(box);
                    break;
            }
        }

        public HorizontalBox(Box box) : this() => Add(box);

        public HorizontalBox(Brush foreground, Brush background) : base(foreground, background) { }

        public HorizontalBox() { }

        public override void Add(Box box)
        {
            base.Add(box);

            childBoxesTotalWidth += box.Width;
            Width = Math.Max(Width, childBoxesTotalWidth);
            Height = Math.Max(Children.Count == 0 ? double.NegativeInfinity : Height, box.Height - box.Shift);
            Depth = Math.Max(Children.Count == 0 ? double.NegativeInfinity : Depth, box.Depth + box.Shift);
        }

        public override void Draw(DrawingContext Context, double scale, double x, double y)
        {
            base.Draw(Context, scale, x, y);

            var curX = x;
            foreach(var box in Children)
            {
                box.Draw(Context, scale, curX, y + box.Shift);
                curX += box.Width;
            }
        }

        public override int GetLastFontId()
        {
            var fontId = TexFontUtilities.NoFontId;
            foreach(var child in Children)
            {
                fontId = child.GetLastFontId();
                if(fontId == TexFontUtilities.NoFontId)
                    break;
            }
            return fontId;
        }
    }
}