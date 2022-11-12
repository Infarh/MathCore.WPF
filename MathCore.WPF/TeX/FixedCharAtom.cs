namespace MathCore.WPF.TeX;

/// <summary>Atom representing character that does not depend on text style</summary>
internal class FixedCharAtom : CharSymbol
{
    public CharFont CharFont { get; }

    public FixedCharAtom(CharFont CharFont) => this.CharFont = CharFont;

    public override CharFont GetCharFont(ITeXFont TexFont) => CharFont;

    public override Box CreateBox(TexEnvironment environment) =>
        new CharBox(environment, environment.TexFont.GetCharInfo(CharFont, environment.Style));
}