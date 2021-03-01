namespace MathCore.WPF.TeX
{
    /// <summary>Atom representing character that does not depend on text style</summary>
    internal class FixedCharAtom : CharSymbol
    {
        public CharFont CharFont { get; }

        public FixedCharAtom(CharFont charFont) => CharFont = charFont;

        public override CharFont GetCharFont(ITeXFont texFont) => CharFont;

        public override Box CreateBox(TexEnvironment environment) =>
            new CharBox(environment, environment.TexFont.GetCharInfo(CharFont, environment.Style));
    }
}