using System.Windows.Media;

namespace MathCore.WPF.TeX;

/// <summary>Box representing other box with delimeter and script box over or under it</summary>
internal sealed class OverUnderBox : Box
{
    public Box BaseBox { get; }

    public Box DelimeterBox { get; }

    public Box ScriptBox { get; }

    /// <summary>Kern between delimeter and Script</summary>
    public double Kern { get; }

    /// <summary>True to draw delimeter and script over base; false to draw under base</summary>
    public bool Over { get; }

    public OverUnderBox(Box BaseBox, Box DelimeterBox, Box ScriptBox, double kern, bool over)
    {
        this.BaseBox      = BaseBox;
        this.DelimeterBox = DelimeterBox;
        this.ScriptBox    = ScriptBox;
        Kern              = kern;
        Over              = over;

        // Calculate dimensions of box.
        Width = BaseBox.Width;
        Height = BaseBox.Height + (over ? DelimeterBox.Width : 0) +
            (over && ScriptBox != null ? ScriptBox.Height + ScriptBox.Depth + kern : 0);
        Depth = BaseBox.Depth + (over ? 0 : DelimeterBox.Width) +
            (!over && ScriptBox is null ? 0 : ScriptBox.Height + ScriptBox.Depth + kern);
    }

    public override void Draw(DrawingContext Context, double scale, double x, double y)
    {
        BaseBox.Draw(Context, scale, x, y);

        if(Over)
        {
            // Draw delimeter and script boxes over base box.
            var center_y      = y - BaseBox.Height - DelimeterBox.Width;
            var translation_x = x + DelimeterBox.Width / 2;
            var translation_y = center_y + DelimeterBox.Width / 2;

            Context.PushTransform(new TranslateTransform(translation_x * scale, translation_y * scale));
            Context.PushTransform(new RotateTransform(90));
            DelimeterBox.Draw(Context, scale, -DelimeterBox.Width / 2,
                -DelimeterBox.Depth + DelimeterBox.Width / 2);
            Context.Pop();
            Context.Pop();

            // Draw script box as superscript.
            ScriptBox?.Draw(Context, scale, x, center_y - Kern - ScriptBox.Depth);
        }
        else
        {
            // Draw delimeter and script boxes under base box.
            var center_y      = y + BaseBox.Depth + DelimeterBox.Width;
            var translation_x = x + DelimeterBox.Width / 2;
            var translation_y = center_y - DelimeterBox.Width / 2;

            Context.PushTransform(new TranslateTransform(translation_x * scale, translation_y * scale));
            Context.PushTransform(new RotateTransform(90));
            DelimeterBox.Draw(Context, scale, -DelimeterBox.Width / 2,
                -DelimeterBox.Depth + DelimeterBox.Width / 2);
            Context.Pop();
            Context.Pop();

            // Draw script box as subscript.
            ScriptBox?.Draw(Context, scale, x, center_y + Kern + ScriptBox.Height);
        }

    }

    public override int GetLastFontId() => TexFontUtilities.NoFontId;
}