using System.Windows.Media;

namespace MathCore.WPF.TeX
{
    /// <summary>Specifies current graphical parameters used to create boxes</summary>
    internal class TexEnvironment
    {
        /// <summary>ID of font that was last used</summary>
        private int lastFontId = TexFontUtilities.NoFontId;

        public TexStyle Style { get; private set; }

        public ITeXFont TexFont { get; }

        public Brush Background { get; set; }

        public Brush Foreground { get; set; }
        public int LastFontId
        {
            get { return lastFontId == TexFontUtilities.NoFontId ? TexFont.GetMuFontId() : lastFontId; }
            set { lastFontId = value; }
        }

        public TexEnvironment(TexStyle style, ITeXFont texFont) : this(style, texFont, null, null) { }

        private TexEnvironment(TexStyle style, ITeXFont texFont, Brush background, Brush foreground)
        {
            if(style == TexStyle.Display || style == TexStyle.Text ||
                style == TexStyle.Script || style == TexStyle.ScriptScript)
                Style = style;
            else
                Style = TexStyle.Display;

            TexFont = texFont;
            Background = background;
            Foreground = foreground;
        }

        public TexEnvironment GetCrampedStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = (int)Style % 2 == 1 ? Style : Style + 1;
            return newEnvironment;
        }

        public TexEnvironment GetNumeratorStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = Style + 2 - 2 * ((int)Style / 6);
            return newEnvironment;
        }

        public TexEnvironment GetDenominatorStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = (TexStyle)(2 * ((int)Style / 2) + 1 + 2 - 2 * ((int)Style / 6));
            return newEnvironment;
        }

        public TexEnvironment GetRootStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = TexStyle.ScriptScript;
            return newEnvironment;
        }

        public TexEnvironment GetSubscriptStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = (TexStyle)(2 * ((int)Style / 4) + 4 + 1);
            return newEnvironment;
        }

        public TexEnvironment GetSuperscriptStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = (TexStyle)(2 * ((int)Style / 4) + 4 + ((int)Style % 2));
            return newEnvironment;
        }

        public TexEnvironment Clone() => new TexEnvironment(Style, TexFont, Background, Foreground);

        public void Reset()
        {
            Background = null;
            Foreground = null;
        }
    }
}