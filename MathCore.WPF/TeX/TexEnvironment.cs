using System.Windows.Media;

namespace MathCore.WPF.TeX;

/// <summary>Specifies current graphical parameters used to create boxes</summary>
internal class TexEnvironment
{
    /// <summary>ID of font that was last used</summary>
    private int _LastFontId = TexFontUtilities.NoFontId;

    public TexStyle Style { get; private set; }

    public ITeXFont TexFont { get; }

    public Brush Background { get; set; }

    public Brush Foreground { get; set; }
    public int LastFontId
    {
        get => _LastFontId == TexFontUtilities.NoFontId ? TexFont.GetMuFontId() : _LastFontId;
        set => _LastFontId = value;
    }

    public TexEnvironment(TexStyle style, ITeXFont TexFont) : this(style, TexFont, null, null) { }

    private TexEnvironment(TexStyle style, ITeXFont TexFont, Brush background, Brush foreground)
    {
        if(style is TexStyle.Display or TexStyle.Text or TexStyle.Script or TexStyle.ScriptScript)
            Style = style;
        else
            Style = TexStyle.Display;

        this.TexFont    = TexFont;
        Background = background;
        Foreground = foreground;
    }

    public TexEnvironment GetCrampedStyle()
    {
        var new_environment = Clone();
        new_environment.Style = (int)Style % 2 == 1 ? Style : Style + 1;
        return new_environment;
    }

    public TexEnvironment GetNumeratorStyle()
    {
        var new_environment = Clone();
        new_environment.Style = Style + 2 - 2 * ((int)Style / 6);
        return new_environment;
    }

    public TexEnvironment GetDenominatorStyle()
    {
        var new_environment = Clone();
        new_environment.Style = (TexStyle)(2 * ((int)Style / 2) + 1 + 2 - 2 * ((int)Style / 6));
        return new_environment;
    }

    public TexEnvironment GetRootStyle()
    {
        var new_environment = Clone();
        new_environment.Style = TexStyle.ScriptScript;
        return new_environment;
    }

    public TexEnvironment GetSubscriptStyle()
    {
        var new_environment = Clone();
        new_environment.Style = (TexStyle)(2 * ((int)Style / 4) + 4 + 1);
        return new_environment;
    }

    public TexEnvironment GetSuperscriptStyle()
    {
        var new_environment = Clone();
        new_environment.Style = (TexStyle)(2 * ((int)Style / 4) + 4 + ((int)Style % 2));
        return new_environment;
    }

    public TexEnvironment Clone() => new(Style, TexFont, Background, Foreground);

    public void Reset()
    {
        Background = null;
        Foreground = null;
    }
}