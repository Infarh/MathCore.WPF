namespace MathCore.WPF.TeX;

/// <summary>Atom representing base atom surrounded by delimeters</summary>
internal class FencedAtom : Atom
{
    private const int __DelimiterFactor = 901;
    private const double __DelimiterShortfall = 0.5;

    public Atom BaseAtom { get; }

    private SymbolAtom? LeftDelimiter { get; }

    private SymbolAtom? RightDelimiter { get; }

    private static void CentreBox(Box box, double axis)
    {
        var total_height = box.Height + box.Depth;
        box.Shift = -(total_height / 2 - box.Height) - axis;
    }

    public FencedAtom(Atom? BaseAtom, SymbolAtom LeftDelimiter, SymbolAtom RightDelimiter)
    {
        this.BaseAtom       = BaseAtom ?? new RowAtom();
        this.LeftDelimiter  = LeftDelimiter;
        this.RightDelimiter = RightDelimiter;
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        var tex_font = environment.TexFont;
        var style   = environment.Style;

        // Create box for base atom.
        var base_box = BaseAtom.CreateBox(environment);

        // Create result box.
        var result_box = new HorizontalBox();
        var axis      = tex_font.GetAxisHeight(style);
        var delta     = Math.Max(base_box.Height - axis, base_box.Depth + axis);
        var min_height = Math.Max((delta / 500) * __DelimiterFactor, 2 * delta - __DelimiterShortfall);

        // Create and add box for left delimeter.
        if(LeftDelimiter != null)
        {
            var left_delimiter_box = DelimiterFactory.CreateBox(LeftDelimiter.Name, min_height, environment);
            CentreBox(left_delimiter_box, axis);
            result_box.Add(left_delimiter_box);
        }

        // add glueElement between left delimeter and base Atom, unless base Atom is whitespace.
        if(BaseAtom is not SpaceAtom)
            result_box.Add(Glue.CreateBox(TexAtomType.Opening, BaseAtom.GetLeftType(), environment));

        // add box for base Atom.
        result_box.Add(base_box);

        // add glueElement between right delimeter and base Atom, unless base Atom is whitespace.
        if(BaseAtom is not SpaceAtom)
            result_box.Add(Glue.CreateBox(BaseAtom.GetRightType(), TexAtomType.Closing, environment));

        // Create and add box for right delimeter.
        if(RightDelimiter is null) return result_box;

        var right_delimeter_box = DelimiterFactory.CreateBox(RightDelimiter.Name, min_height, environment);
        CentreBox(right_delimeter_box, axis);
        result_box.Add(right_delimeter_box);

        return result_box;
    }

    public override TexAtomType GetLeftType() => TexAtomType.Opening;

    public override TexAtomType GetRightType() => TexAtomType.Closing;
}