namespace MathCore.WPF.TeX;

/// <summary>Atom representing radical (nth-root) construction</summary>
internal class Radical : Atom
{
    private const string __SqrtSymbol = "sqrt";

    private const double __Scale = 0.55;

    public Atom BaseAtom { get; }

    public Atom? DegreeAtom { get; }

    public Radical(Atom BaseAtom, Atom? DegreeAtom = null)
    {
        this.BaseAtom   = BaseAtom;
        this.DegreeAtom = DegreeAtom;
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        var tex_font = environment.TexFont;
        var style    = environment.Style;

        // Calculate minimum clearance amount.
        var default_rule_thickness = tex_font.GetDefaultLineThickness(style);
        var clearance = style < TexStyle.Text ? tex_font.GetXHeight(style, tex_font.GetCharInfo(__SqrtSymbol, style).FontId) : default_rule_thickness;
        clearance = default_rule_thickness + Math.Abs(clearance) / 4;

        // Create box for base atom, in cramped style.
        var base_box = BaseAtom.CreateBox(environment.GetCrampedStyle());

        // Create box for radical sign.
        var total_height = base_box.Height + base_box.Depth;
        var radical_sign_box = DelimiterFactory.CreateBox(__SqrtSymbol, total_height + clearance + default_rule_thickness,
            environment);

        // Add half of excess height to clearance.
        var delta = radical_sign_box.Depth - (total_height + clearance);
        clearance += delta / 2;

        // Create box for square-root containing base box.
        radical_sign_box.Shift = -(base_box.Height + clearance);
        var over_bar = new OverBar(base_box, clearance, radical_sign_box.Height)
        {
            Shift = -(base_box.Height + clearance + default_rule_thickness)
        };
        var radical_container_box = new HorizontalBox(radical_sign_box);
        radical_container_box.Add(over_bar);

        // If atom is simple radical, just return square-root box.
        if (DegreeAtom is null)
            return radical_container_box;

        // Atom is complex radical (nth-root).

        // Create box for root atom.
        var root_box     = DegreeAtom.CreateBox(environment.GetRootStyle());
        var bottom_shift = __Scale * (radical_container_box.Height + radical_container_box.Depth);
        root_box.Shift = radical_container_box.Depth - root_box.Depth - bottom_shift;

        // Create result box.
        var result_box = new HorizontalBox();

        // Add box for negative kern.
        var negative_kern = new SpaceAtom(TexUnit.Mu, -10, 0, 0).CreateBox(environment);
        var x_pos         = root_box.Width + negative_kern.Width;
        if (x_pos < 0)
            result_box.Add(new StrutBox(-x_pos, 0, 0, 0));

        result_box.Add(root_box);
        result_box.Add(negative_kern);
        result_box.Add(radical_container_box);

        return result_box;
    }
}