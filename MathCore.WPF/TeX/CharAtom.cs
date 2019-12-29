namespace MathCore.WPF.TeX
{
    /// <summary>Atom representing single character in specific text style</summary>
    internal class CharAtom : CharSymbol
    {
        public char Character { get; }

        /// <summary>Null means default text style</summary>
        public string TextStyle { get; }

        public CharAtom(char character, string textStyle = null)
        {
            Character = character;
            TextStyle = textStyle;
        }

        public override Box CreateBox(TexEnvironment environment) =>
            new CharBox(environment, GetCharInfo(environment.TexFont, environment.Style));

        private CharInfo GetCharInfo(ITeXFont texFont, TexStyle style) =>
            TextStyle == null
                ? texFont.GetDefaultCharInfo(Character, style)
                : texFont.GetCharInfo(Character, TextStyle, style);

        public override CharFont GetCharFont(ITeXFont texFont) => GetCharInfo(texFont, TexStyle.Display).GetCharacterFont();
    }
}
