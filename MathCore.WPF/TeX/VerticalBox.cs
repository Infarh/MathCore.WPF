using System.Windows.Media;

namespace MathCore.WPF.TeX;

/// <summary>Box containing vertical stack of child boxes</summary>
internal class VerticalBox : Box
{
    private double _LeftMostPos = double.MaxValue;
    private double _RightMostPos = double.MinValue;

    public VerticalBox(Box box, double rest, TexAlignment alignment)
    {
        Add(box);
        switch(alignment)
        {
            case TexAlignment.Center:
                var strut_box = new StrutBox(0, rest / 2, 0, 0);
                base.Add(0, strut_box);
                Height += rest / 2;
                Depth  += rest / 2;
                base.Add(strut_box);
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
            Depth  = box.Depth;
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
            Depth  += box.Depth + Height;
            Height =  box.Height;
        }
        else
            Depth += box.Height + box.Depth;
        RecalculateWidth(box);
    }

    private void RecalculateWidth(Box box)
    {
        _LeftMostPos  = Math.Min(_LeftMostPos, box.Shift);
        _RightMostPos = Math.Max(_RightMostPos, box.Shift + (box.Width > 0 ? box.Width : 0));
        Width        = _RightMostPos - _LeftMostPos;
    }

    public override void Draw(DrawingContext Context, double scale, double x, double y)
    {
        base.Draw(Context, scale, x, y);

        var cur_y = y - Height;
        foreach(var child in Children)
        {
            cur_y += child.Height;
            child.Draw(Context, scale, x + child.Shift - _LeftMostPos, cur_y);
            cur_y += child.Depth;
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