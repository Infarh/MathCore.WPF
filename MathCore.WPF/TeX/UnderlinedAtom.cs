namespace MathCore.WPF.TeX;

/// <summary>Atom representing other atom that is underlined</summary>
internal class UnderlinedAtom : Atom
{
    public Atom BaseAtom { get; }

    public UnderlinedAtom(Atom BaseAtom)
    {
        Type          = TexAtomType.Ordinary;
        this.BaseAtom = BaseAtom;
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        var default_line_thickness = environment.TexFont.GetDefaultLineThickness(environment.Style);

        // Create box for base atom.
        var base_box = BaseAtom is null ? StrutBox.Empty : BaseAtom.CreateBox(environment);

        // Create result box.
        var result_box = new VerticalBox();
        result_box.Add(base_box);
        result_box.Add(new StrutBox(0, 3 * default_line_thickness, 0, 0));
        result_box.Add(new HorizontalRule(default_line_thickness, base_box.Width, 0));

        result_box.Depth  = base_box.Depth + 5 * default_line_thickness;
        result_box.Height = base_box.Height;

        return result_box;
    }
}