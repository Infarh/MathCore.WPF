namespace MathCore.WPF.TeX;

/// <summary>Atom representing base atom with accent above it</summary>
internal class AccentedAtom : Atom
{
    /// <summary>Atom over which accent symbol is placed</summary>
    public Atom BaseAtom { get; }

    /// <summary>Atom representing accent symbol to place over base atom</summary>
    public SymbolAtom AccentAtom { get; }

    public AccentedAtom(Atom BaseAtom, string AccentName)
    {
        this.BaseAtom = BaseAtom;
        AccentAtom    = SymbolAtom.GetAtom(AccentName);

        if (AccentAtom.Type != TexAtomType.Accent)
            throw new ArgumentException(@"The specified symbol name is not an accent.", nameof(AccentName));
    }

    public AccentedAtom(Atom BaseAtom, TexFormula accent)
    {
        if (accent.RootAtom is not SymbolAtom root_symbol)
            throw new ArgumentException(@"The formula for the accent is not a single symbol.", nameof(accent));

        AccentAtom = root_symbol;

        if (AccentAtom.Type != TexAtomType.Accent)
            throw new ArgumentException(@"The specified symbol name is not an accent.", nameof(accent));
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        var tex_font = environment.TexFont;
        var style    = environment.Style;

        // Create box for base atom.
        var base_box = BaseAtom is null ? StrutBox.Empty : BaseAtom.CreateBox(environment.GetCrampedStyle());
        var skew     = 0d;
        if (BaseAtom is CharSymbol symbol)
            skew = tex_font.GetSkew(symbol.GetCharFont(tex_font), style);

        // Find character of best scale for accent symbol.
        var accent_char = tex_font.GetCharInfo(AccentAtom.Name, style);
        while (tex_font.HasNextLarger(accent_char))
        {
            var next_larger_char = tex_font.GetNextLargerCharInfo(accent_char, style);
            if (next_larger_char.Metrics.Width > base_box.Width)
                break;

            accent_char = next_larger_char;
        }

        var result_box = new VerticalBox();

        // Create and add box for accent symbol.
        Box accent_box;
        var accent_italic_width = accent_char.Metrics.Italic;
        if (accent_italic_width > TexUtilities.FloatPrecision)
        {
            accent_box = new HorizontalBox(new CharBox(environment, accent_char));
            accent_box.Add(new StrutBox(accent_italic_width, 0, 0, 0));
        }
        else
            accent_box = new CharBox(environment, accent_char);

        result_box.Add(accent_box);

        var delta = Math.Min(base_box.Height, tex_font.GetXHeight(style, accent_char.FontId));
        result_box.Add(new StrutBox(0, -delta, 0, 0));

        // Centre and add box for base atom. Centre base box and accent box with respect to each other.
        var box_widths_diff = (base_box.Width - accent_box.Width) / 2;
        accent_box.Shift = skew + Math.Max(box_widths_diff, 0);
        if (box_widths_diff < 0)
            base_box = new HorizontalBox(base_box, accent_box.Width, TexAlignment.Center);
        result_box.Add(base_box);

        // Adjust height and depth of result box.
        var depth         = base_box.Depth;
        var total_height  = result_box.Height + result_box.Depth;
        result_box.Depth  = depth;
        result_box.Height = total_height - depth;

        return result_box;
    }
}