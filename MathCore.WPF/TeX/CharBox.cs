using System.Windows;
using System.Windows.Media;

// Box representing single character.
namespace MathCore.WPF.TeX
{
    internal class CharBox : Box
    {
        public CharInfo Character { get; }

        public CharBox(TexEnvironment environment, CharInfo charInfo)
            : base(environment)
        {
            Character = charInfo;
            Width = charInfo.Metrics.Width;
            Height = charInfo.Metrics.Height;
            Depth = charInfo.Metrics.Depth;
        }

        public override void Draw(DrawingContext drawingContext, double scale, double x, double y)
        {
            // Draw character at given position.
            var typeface = Character.Font;
            var glyphIndex = typeface.CharacterToGlyphMap[Character.Character];
            var glyphRun = new GlyphRun(typeface, 0, false, Character.Size * scale,
                new[] { glyphIndex }, new Point(x * scale, y * scale),
                new[] { typeface.AdvanceWidths[glyphIndex] }, null, null, null, null, null, null);
            drawingContext.DrawGlyphRun(Foreground ?? Brushes.Black, glyphRun);
        }

        public override int GetLastFontId() => Character.FontId;
    }
}
