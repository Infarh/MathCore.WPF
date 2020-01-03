using System;
using System.Windows.Media;

namespace MathCore.WPF.TeX
{
    /// <summary>Box containing vertical stack of child boxes</summary>
    internal class VerticalBox : Box
    {
        private double leftMostPos = double.MaxValue;
        private double rightMostPos = double.MinValue;

        public VerticalBox(Box box, double rest, TexAlignment alignment)
        {
            Add(box);
            switch(alignment)
            {
                case TexAlignment.Center:
                    var strutBox = new StrutBox(0, rest / 2, 0, 0);
                    base.Add(0, strutBox);
                    Height += rest / 2;
                    Depth += rest / 2;
                    base.Add(strutBox);
                    break;
                case TexAlignment.Top:
                    Depth += rest;
                    base.Add(new StrutBox(0, rest, 0, 0));
                    break;
                case TexAlignment.Bottom:
                    Height += rest;
                    base.Add(0, new StrutBox(0, rest, 0, 0));
                    break;
            }
        }

        public VerticalBox() { }

        public override void Add(Box box)
        {
            base.Add(box);

            if(Children.Count == 1)
            {
                Height = box.Height;
                Depth = box.Depth;
            }
            else
                Depth += box.Height + box.Depth;
            RecalculateWidth(box);
        }

        public override void Add(int position, Box box)
        {
            base.Add(position, box);

            if(position == 0)
            {
                Depth += box.Depth + Height;
                Height = box.Height;
            }
            else
                Depth += box.Height + box.Depth;
            RecalculateWidth(box);
        }

        private void RecalculateWidth(Box box)
        {
            leftMostPos = Math.Min(leftMostPos, box.Shift);
            rightMostPos = Math.Max(rightMostPos, box.Shift + (box.Width > 0 ? box.Width : 0));
            Width = rightMostPos - leftMostPos;
        }

        public override void Draw(DrawingContext drawingContext, double scale, double x, double y)
        {
            base.Draw(drawingContext, scale, x, y);

            var curY = y - Height;
            foreach(var child in Children)
            {
                curY += child.Height;
                child.Draw(drawingContext, scale, x + child.Shift - leftMostPos, curY);
                curY += child.Depth;
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