using System.Windows.Media;

namespace MathCore.WPF.TeX
{
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

        public OverUnderBox(Box baseBox, Box delimeterBox, Box scriptBox, double kern, bool over)
        {
            BaseBox = baseBox;
            DelimeterBox = delimeterBox;
            ScriptBox = scriptBox;
            Kern = kern;
            Over = over;

            // Calculate dimensions of box.
            Width = baseBox.Width;
            Height = baseBox.Height + (over ? delimeterBox.Width : 0) +
                          (over && scriptBox != null ? scriptBox.Height + scriptBox.Depth + kern : 0);
            Depth = baseBox.Depth + (over ? 0 : delimeterBox.Width) +
                         (!over && scriptBox is null ? 0 : scriptBox.Height + scriptBox.Depth + kern);
        }

        public override void Draw(DrawingContext drawingContext, double scale, double x, double y)
        {
            BaseBox.Draw(drawingContext, scale, x, y);

            if(Over)
            {
                // Draw delimeter and script boxes over base box.
                var centerY = y - BaseBox.Height - DelimeterBox.Width;
                var translationX = x + DelimeterBox.Width / 2;
                var translationY = centerY + DelimeterBox.Width / 2;

                drawingContext.PushTransform(new TranslateTransform(translationX * scale, translationY * scale));
                drawingContext.PushTransform(new RotateTransform(90));
                DelimeterBox.Draw(drawingContext, scale, -DelimeterBox.Width / 2,
                    -DelimeterBox.Depth + DelimeterBox.Width / 2);
                drawingContext.Pop();
                drawingContext.Pop();

                // Draw script box as superscript.
                ScriptBox?.Draw(drawingContext, scale, x, centerY - Kern - ScriptBox.Depth);
            }
            else
            {
                // Draw delimeter and script boxes under base box.
                var centerY = y + BaseBox.Depth + DelimeterBox.Width;
                var translationX = x + DelimeterBox.Width / 2;
                var translationY = centerY - DelimeterBox.Width / 2;

                drawingContext.PushTransform(new TranslateTransform(translationX * scale, translationY * scale));
                drawingContext.PushTransform(new RotateTransform(90));
                DelimeterBox.Draw(drawingContext, scale, -DelimeterBox.Width / 2,
                    -DelimeterBox.Depth + DelimeterBox.Width / 2);
                drawingContext.Pop();
                drawingContext.Pop();

                // Draw script box as subscript.
                ScriptBox?.Draw(drawingContext, scale, x, centerY + Kern + ScriptBox.Height);
            }

        }

        public override int GetLastFontId() => TexFontUtilities.NoFontId;
    }
}