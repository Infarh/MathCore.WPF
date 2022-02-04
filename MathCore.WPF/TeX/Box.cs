using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

// Represents graphical box that is part of math expression, and can itself contain child boxes.
namespace MathCore.WPF.TeX
{
    public abstract class Box
    {
        private readonly List<Box> children;
        private readonly ReadOnlyCollection<Box> childrenReadOnly;

        public ReadOnlyCollection<Box> Children => childrenReadOnly;

        public Brush Foreground { get; set; }

        public Brush Background { get; set; }

        public double TotalHeight => Height + Depth;

        public double Width { get; set; }

        public double Height { get; set; }

        public double Depth { get; set; }

        public double Shift { get; set; }
        internal Box(TexEnvironment environment) : this(environment.Foreground, environment.Background) { }

        protected Box() : this(null, null) { }

        protected Box(Brush foreground, Brush background)
        {
            children = new List<Box>();
            childrenReadOnly = new ReadOnlyCollection<Box>(children);
            Foreground = foreground;
            Background = background;
        }

        public virtual void Draw(DrawingContext Context, double scale, double x, double y)
        {
            if(Background is null) return;
            // Fill background of box with color.
            Context.DrawRectangle(Background, null, new Rect(x * scale, (y - Height) * scale,
                Width * scale, (Height + Depth) * scale));
        }

        public virtual void Add(Box box) => children.Add(box);

        public virtual void Add(int position, Box box) => children.Insert(position, box);

        public abstract int GetLastFontId();
    }
}