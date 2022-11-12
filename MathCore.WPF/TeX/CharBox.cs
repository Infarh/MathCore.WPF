using System.Windows;
using System.Windows.Media;
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleOther
// ReSharper disable ArgumentsStyleNamedExpression

// Box representing single character.
namespace MathCore.WPF.TeX;

internal class CharBox : Box
{
    public CharInfo Character { get; }

    public CharBox(TexEnvironment environment, CharInfo CharInfo)
        : base(environment)
    {
        Character = CharInfo;
        Width     = CharInfo.Metrics.Width;
        Height    = CharInfo.Metrics.Height;
        Depth     = CharInfo.Metrics.Depth;
    }

    public override void Draw(DrawingContext Context, double scale, double x, double y)
    {
        // Draw character at given position.
        var typeface    = Character.Font;
        var glyph_index = typeface.CharacterToGlyphMap[Character.Character];

        var glyph_run = new GlyphRun(
            glyphTypeface: typeface,
            bidiLevel: 0,
            isSideways: false,
            renderingEmSize: Character.Size * scale,
#if NET47 || NET48 || NET5_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            pixelsPerDip: 96,
#endif
            glyphIndices: new[] { glyph_index },
            baselineOrigin: new Point(x * scale, y * scale),
            advanceWidths: new[] { typeface.AdvanceWidths[glyph_index] },
            glyphOffsets: null,
            characters: null,
            deviceFontName: null,
            clusterMap: null,
            caretStops: null,
            language: null);



        Context.DrawGlyphRun(Foreground ?? Brushes.Black, glyph_run);
    }

    public override int GetLastFontId() => Character.FontId;
}