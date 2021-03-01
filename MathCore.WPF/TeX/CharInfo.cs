using System.Windows.Media;

namespace MathCore.WPF.TeX
{
    /// <summary>Single character togeter with information about font and metrics</summary>
    internal class CharInfo
    {
        public char Character { get; set; }

        public GlyphTypeface Font { get; set; }

        public double Size { get; set; }

        public TexFontMetrics Metrics { get; set; }

        public int FontId { get; set; }

        public CharInfo(char character, GlyphTypeface font, double size, int fontId, TexFontMetrics metrics)
        {
            Character = character;
            Font = font;
            Size = size;
            FontId = fontId;
            Metrics = metrics;
        }

        public CharFont GetCharacterFont() => new(Character, FontId);
    }
}