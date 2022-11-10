namespace MathCore.WPF.TeX;

/// <summary>Atom representing fraction, with or without separation line</summary>
internal sealed class FractionAtom : Atom
{
    private static TexAlignment CheckAlignment(TexAlignment alignment)
    {
        if(alignment is TexAlignment.Left or TexAlignment.Right)
            return alignment;
        return TexAlignment.Center;
    }

    private readonly TexAlignment _NumeratorAlignment;
    private readonly TexAlignment _DenominatorAlignment;

    private readonly double _LineThickness;
    private readonly TexUnit _LineThicknessUnit;

    private readonly bool _UseDefaultThickness;
    private double? _LineRelativeThickness = null;

    public Atom? Numerator { get; }

    public Atom? Denominator { get; }

    /*
            public FractionAtom(Atom numerator, Atom denominator, double relativeThickness,
                TexAlignment numeratorAlignment, TexAlignment denominatorAlignment)
                : this(numerator, denominator, true, numeratorAlignment, denominatorAlignment)
            {
                lineRelativeThickness = relativeThickness;
            }
    */

    public FractionAtom(Atom numerator, Atom denominator, bool DrawLine,
        TexAlignment NumeratorAlignment, TexAlignment DenominatorAlignment)
        : this(numerator, denominator, DrawLine)
    {
        this._NumeratorAlignment   = CheckAlignment(NumeratorAlignment);
        this._DenominatorAlignment = CheckAlignment(DenominatorAlignment);
    }

    public FractionAtom(Atom numerator, Atom denominator, bool DrawLine)
        : this(numerator, denominator, DrawLine, TexUnit.Pixel, 0d)
    {
    }

    public FractionAtom(Atom numerator, Atom denominator, TexUnit unit, double thickness,
        TexAlignment NumeratorAlignment, TexAlignment DenominatorAlignment)
        : this(numerator, denominator, unit, thickness)
    {
        this._NumeratorAlignment   = CheckAlignment(NumeratorAlignment);
        this._DenominatorAlignment = CheckAlignment(DenominatorAlignment);
    }

    public FractionAtom(Atom numerator, Atom denominator, TexUnit unit, double thickness)
        : this(numerator, denominator, false, unit, thickness)
    {
    }

    private FractionAtom(Atom numerator, Atom denominator, bool UseDefaultThickness, TexUnit unit, double thickness)
    {
        SpaceAtom.CheckUnit(unit);

        Type                     = TexAtomType.Inner;
        Numerator                = numerator;
        Denominator              = denominator;
        _NumeratorAlignment       = TexAlignment.Center;
        _DenominatorAlignment     = TexAlignment.Center;
        this._UseDefaultThickness = UseDefaultThickness;
        _LineThicknessUnit        = unit;
        _LineThickness            = thickness;
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        var tex_font = environment.TexFont;
        var style   = environment.Style;

        // set thickness to default if default value should be used
        double line_height;
        var    default_line_thickness = tex_font.GetDefaultLineThickness(style);
        if(_UseDefaultThickness)
            line_height = _LineRelativeThickness * default_line_thickness ?? default_line_thickness;
        else
            line_height = new SpaceAtom(_LineThicknessUnit, 0, _LineThickness, 0).CreateBox(environment).Height;

        // Create boxes for numerator and demoninator atoms, and make them of equal width.
        var numerator_box = Numerator is null ? StrutBox.Empty :
            Numerator.CreateBox(environment.GetNumeratorStyle());
        var denominator_box = Denominator is null ? StrutBox.Empty :
            Denominator.CreateBox(environment.GetDenominatorStyle());

        if(numerator_box.Width < denominator_box.Width)
            numerator_box = new HorizontalBox(numerator_box, denominator_box.Width, _NumeratorAlignment);
        else
            denominator_box = new HorizontalBox(denominator_box, numerator_box.Width, _DenominatorAlignment);

        // Calculate preliminary shift-up and shift-down amounts.
        double shift_up, shift_down;
        if(style < TexStyle.Text)
        {
            shift_up   = tex_font.GetNum1(style);
            shift_down = tex_font.GetDenom1(style);
        }
        else
        {
            shift_down = tex_font.GetDenom2(style);
            shift_up   = line_height > 0 ? tex_font.GetNum2(style) : tex_font.GetNum3(style);
        }

        // Create result box.
        var result_box = new VerticalBox();

        // add box for numerator.
        result_box.Add(numerator_box);

        // Calculate clearance and adjust shift amounts.
        var axis = tex_font.GetAxisHeight(style);

        if(line_height > 0)
        {
            // Draw fraction line.

            // Calculate clearance amount.
            double clearance;
            if(style < TexStyle.Text)
                clearance = 3 * line_height;
            else
                clearance = line_height;

            // Adjust shift amounts.
            var delta  = line_height / 2;
            var kern1  = shift_up - numerator_box.Depth - (axis + delta);
            var kern2  = axis - delta - (denominator_box.Height - shift_down);
            var delta1 = clearance - kern1;
            var delta2 = clearance - kern2;
            if(delta1 > 0)
            {
                shift_up += delta1;
                kern1   += delta1;
            }
            if(delta2 > 0)
            {
                shift_down += delta2;
                kern2     += delta2;
            }

            result_box.Add(new StrutBox(0, kern1, 0, 0));
            result_box.Add(new HorizontalRule(line_height, numerator_box.Width, 0));
            result_box.Add(new StrutBox(0, kern2, 0, 0));
        }
        else
        {
            // Do not draw fraction line.

            // Calculate clearance amount.
            double clearance;
            if(style < TexStyle.Text)
                clearance = 7 * default_line_thickness;
            else
                clearance = 3 * default_line_thickness;

            // Adjust shift amounts.
            var kern  = shift_up - numerator_box.Depth - (denominator_box.Height - shift_down);
            var delta = (clearance - kern) / 2;
            if(delta > 0)
            {
                shift_up   += delta;
                shift_down += delta;
                kern      += 2 * delta;
            }

            result_box.Add(new StrutBox(0, kern, 0, 0));
        }

        // add box for denominator.
        result_box.Add(denominator_box);

        // Adjust height and depth of result box.
        result_box.Height = shift_up + numerator_box.Height;
        result_box.Depth  = shift_down + denominator_box.Depth;

        return result_box;
    }
}