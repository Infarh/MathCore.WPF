namespace MathCore.WPF.TeX;

/// <summary>Atom representing scripts to attach to other atom</summary>
internal class ScriptsAtom : Atom
{
    private static readonly SpaceAtom __ScriptSpaceAtom = new(TexUnit.Point, 0.5, 0, 0);

    public Atom? BaseAtom { get; }

    public Atom? SubscriptAtom { get; }

    public Atom? SuperscriptAtom { get; }

    public ScriptsAtom(Atom BaseAtom, Atom? SubscriptAtom, Atom? SuperscriptAtom)
    {
        this.BaseAtom        = BaseAtom;
        this.SubscriptAtom   = SubscriptAtom;
        this.SuperscriptAtom = SuperscriptAtom;
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        var tex_font = environment.TexFont;
        var style    = environment.Style;

        // Create box for base atom.
        var base_box = (BaseAtom is null ? StrutBox.Empty : BaseAtom.CreateBox(environment));
        if (SubscriptAtom is null && SuperscriptAtom is null)
            return base_box;

        // Create result box.
        var result_box = new HorizontalBox(base_box);

        // Get last font used or default Mu font.
        var last_font_id = base_box.GetLastFontId();
        if (last_font_id == TexFontUtilities.NoFontId)
            last_font_id = tex_font.GetMuFontId();

        var subscript_style   = environment.GetSubscriptStyle();
        var superscript_style = environment.GetSuperscriptStyle();

        // Set delta value and preliminary shift-up and shift-down amounts depending on type of base atom.
        var    delta = 0d;
        double shift_up, shift_down;

        switch (BaseAtom)
        {
            case AccentedAtom atom:
            {
                var accented_box = atom.BaseAtom.CreateBox(environment.GetCrampedStyle());
                shift_up   = accented_box.Height - tex_font.GetSupDrop(superscript_style.Style);
                shift_down = accented_box.Depth + tex_font.GetSubDrop(subscript_style.Style);
                break;
            }
            case SymbolAtom { Type: TexAtomType.BigOperator } symbol_atom:
            {
                var char_info = tex_font.GetCharInfo(symbol_atom.Name, style);
                if (style < TexStyle.Text && tex_font.HasNextLarger(char_info))
                    char_info = tex_font.GetNextLargerCharInfo(char_info, style);
                var char_box = new CharBox(environment, char_info);

                char_box.Shift = -(char_box.Height + char_box.Depth) / 2
                    - environment.TexFont.GetAxisHeight(
                        environment.Style);
                result_box = new HorizontalBox(char_box);

                delta = char_info.Metrics.Italic;
                if (delta > TexUtilities.FloatPrecision && SubscriptAtom is null)
                    result_box.Add(new StrutBox(delta, 0, 0, 0));

                shift_up   = result_box.Height - tex_font.GetSupDrop(superscript_style.Style);
                shift_down = result_box.Depth + tex_font.GetSubDrop(subscript_style.Style);
                break;
            }
            case CharSymbol symbol:
            {
                var char_font = symbol.GetCharFont(tex_font);
                if (!symbol.IsTextSymbol || !tex_font.HasSpace(char_font.FontId))
                    delta = tex_font.GetCharInfo(char_font, style).Metrics.Italic;
                if (delta > TexUtilities.FloatPrecision && SubscriptAtom is null)
                {
                    result_box.Add(new StrutBox(delta, 0, 0, 0));
                    delta = 0;
                }

                shift_up   = 0;
                shift_down = 0;
                break;
            }
            default:
                shift_up   = base_box.Height - tex_font.GetSupDrop(superscript_style.Style);
                shift_down = base_box.Depth + tex_font.GetSubDrop(subscript_style.Style);
                break;
        }

        Box superscript_box           = null;
        Box superscript_container_box = null;
        Box subscript_box             = null;
        Box subscript_container_box   = null;

        if (SuperscriptAtom != null)
        {
            // Create box for superscript atom.
            superscript_box           = SuperscriptAtom.CreateBox(superscript_style);
            superscript_container_box = new HorizontalBox(superscript_box);

            // Add box for script space.
            superscript_container_box.Add(__ScriptSpaceAtom.CreateBox(environment));

            // Adjust shift-up amount.
            double p;
            if (style == TexStyle.Display)
                p = tex_font.GetSup1(style);
            else if (environment.GetCrampedStyle().Style == style)
                p = tex_font.GetSup3(style);
            else
                p = tex_font.GetSup2(style);
            shift_up = Math.Max(
                Math.Max(shift_up, p),
                superscript_box.Depth
                + Math.Abs(
                    tex_font.GetXHeight(
                        style,
                        last_font_id))
                / 4);
        }

        if (SubscriptAtom != null)
        {
            // Create box for subscript atom.
            subscript_box           = SubscriptAtom.CreateBox(subscript_style);
            subscript_container_box = new HorizontalBox(subscript_box);

            // Add box for script space.
            subscript_container_box.Add(__ScriptSpaceAtom.CreateBox(environment));
        }

        // Check if only superscript is set.
        if (subscript_box is null)
        {
            superscript_container_box.Shift = -shift_up;
            result_box.Add(superscript_container_box);
            return result_box;
        }

        // Check if only subscript is set.
        if (superscript_box is null)
        {
            subscript_box.Shift = Math.Max(
                Math.Max(shift_down, tex_font.GetSub1(style)),
                subscript_box.Height - 4 * Math.Abs(tex_font.GetXHeight(style, last_font_id)) / 5);
            result_box.Add(subscript_container_box);
            return result_box;
        }

        // Adjust shift-down amount.
        shift_down = Math.Max(shift_down, tex_font.GetSub2(style));

        // Reposition both subscript and superscript.
        var default_line_thickness = tex_font.GetDefaultLineThickness(style);
        // Space between subscript and superscript.
        var scripts_inter_space = shift_up - superscript_box.Depth + shift_down - subscript_box.Height;
        if (scripts_inter_space < 4 * default_line_thickness)
        {
            shift_up += 4 * default_line_thickness - scripts_inter_space;

            // Position bottom of superscript at least 4/5 of X-height above baseline.
            var psi = 0.8 * Math.Abs(tex_font.GetXHeight(style, last_font_id)) - (shift_up - superscript_box.Depth);
            if (psi > 0)
            {
                shift_up   += psi;
                shift_down -= psi;
            }
        }

        scripts_inter_space = shift_up - superscript_box.Depth + shift_down - subscript_box.Height;

        // Create box containing both superscript and subscript.
        var scripts_box = new VerticalBox();
        superscript_container_box.Shift = delta;
        scripts_box.Add(superscript_container_box);
        scripts_box.Add(new StrutBox(0, scripts_inter_space, 0, 0));
        scripts_box.Add(subscript_container_box);
        scripts_box.Height = shift_up + superscript_box.Height;
        scripts_box.Depth  = shift_down + subscript_box.Depth;
        result_box.Add(scripts_box);

        return result_box;
    }

    public override TexAtomType GetLeftType() => BaseAtom.GetLeftType();

    public override TexAtomType GetRightType() => BaseAtom.GetRightType();
}