using System.Windows.Media;

namespace MathCore.WPF.TeX;

/// <summary>Box containing horizontal stack of child boxes</summary>
internal sealed class HorizontalBox : Box
{
    private double _ChildBoxesTotalWidth;

    public HorizontalBox(Box box, double width, TexAlignment alignment)
        : this()
    {
        var extra_width = width - box.Width;
        switch(alignment)
        {
            case TexAlignment.Center:
                var strut_box = new StrutBox(extra_width / 2, 0, 0, 0);
                Add(strut_box);
                Add(box);
                Add(strut_box);
                break;
            case TexAlignment.Left:
                Add(box);
                Add(new StrutBox(extra_width, 0, 0, 0));
                break;
            case TexAlignment.Right:
                Add(new StrutBox(extra_width, 0, 0, 0));
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

        _ChildBoxesTotalWidth += box.Width;
        Width                =  Math.Max(Width, _ChildBoxesTotalWidth);
        Height               =  Math.Max(Children.Count == 0 ? double.NegativeInfinity : Height, box.Height - box.Shift);
        Depth                =  Math.Max(Children.Count == 0 ? double.NegativeInfinity : Depth, box.Depth + box.Shift);
    }

    public override void Draw(DrawingContext Context, double scale, double x, double y)
    {
        base.Draw(Context, scale, x, y);

        var cur_x = x;
        foreach(var box in Children)
        {
            box.Draw(Context, scale, cur_x, y + box.Shift);
            cur_x += box.Width;
        }
    }

    public override int GetLastFontId()
    {
        var font_id = TexFontUtilities.NoFontId;
        foreach(var child in Children)
        {
            font_id = child.GetLastFontId();
            if(font_id == TexFontUtilities.NoFontId)
                break;
        }
        return font_id;
    }
}