namespace MathCore.WPF.TeX;

/// <summary>Atom representing other atom with horizontal rule above it</summary>
internal class OverlinedAtom : Atom
{
    public Atom BaseAtom { get; }

    public OverlinedAtom(Atom BaseAtom)
    {
        Type          = TexAtomType.Ordinary;
        this.BaseAtom = BaseAtom;
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        // Create box for base atom, in cramped style.
        var base_box = BaseAtom is null ? StrutBox.Empty : BaseAtom.CreateBox(environment.GetCrampedStyle());

        // Create result box.
        var default_line_thickness = environment.TexFont.GetDefaultLineThickness(environment.Style);
        var result_box = new OverBar(base_box, 3 * default_line_thickness, default_line_thickness)
        {
            Height = base_box.Height + 5 * default_line_thickness,
            Depth  = base_box.Depth
        };

        // Adjust height and depth of result box.

        return result_box;
    }
}