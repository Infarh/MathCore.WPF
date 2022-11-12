namespace MathCore.WPF.TeX;

/// <summary>Atom representing single character in specific text style</summary>
internal class CharAtom : CharSymbol
{
    public char Character { get; }

    /// <summary>Null means default text style</summary>
    public string TextStyle { get; }

    public CharAtom(char character, string TextStyle = null)
    {
        Character      = character;
        this.TextStyle = TextStyle;
    }

    public override Box CreateBox(TexEnvironment environment) =>
        new CharBox(environment, GetCharInfo(environment.TexFont, environment.Style));

    private CharInfo GetCharInfo(ITeXFont TexFont, TexStyle style) =>
        TextStyle is null
            ? TexFont.GetDefaultCharInfo(Character, style)
            : TexFont.GetCharInfo(Character, TextStyle, style);

    public override CharFont GetCharFont(ITeXFont TexFont) => GetCharInfo(TexFont, TexStyle.Display).GetCharacterFont();
}