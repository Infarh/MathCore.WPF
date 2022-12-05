using System.Diagnostics;

namespace MathCore.WPF.TeX;

/// <summary>Atom representing big operator with optional limits</summary>
internal class BigOperatorAtom : Atom
{
    /// <summary>Centre specified box in new box of specified width, if necessary</summary>
    /// <param name="box"></param>
    /// <param name="MaxWidth"></param>
    /// <returns></returns>
    private static Box ChangeWidth(Box box, double MaxWidth) =>
        Math.Abs(MaxWidth - box.Width) > TexUtilities.FloatPrecision
            ? new HorizontalBox(box, MaxWidth, TexAlignment.Center)
            : box;

    /// <summary>Atom representing big operator</summary>
    public Atom BaseAtom { get; }

    /// <summary>Atoms representing lower and upper limits</summary>
    public Atom LowerLimitAtom { get; }

    public Atom UpperLimitAtom { get; }

    /// <summary>True if limits should be drawn over and under the base atom; false if they should be drawn as scripts</summary>
    public bool? UseVerticalLimits { get; }

    public BigOperatorAtom(Atom BaseAtom, Atom LowerLimitAtom, Atom UpperLimitAtom, bool? UseVerticalLimits = null)
        : this(BaseAtom, LowerLimitAtom, UpperLimitAtom) =>
        this.UseVerticalLimits = UseVerticalLimits;

    public BigOperatorAtom(Atom BaseAtom, Atom? LowerLimitAtom, Atom? UpperLimitAtom)
    {
        Type                = TexAtomType.BigOperator;
        this.BaseAtom       = BaseAtom;
        this.LowerLimitAtom = LowerLimitAtom;
        this.UpperLimitAtom = UpperLimitAtom;
        UseVerticalLimits   = null;
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        var tex_font = environment.TexFont;
        var style   = environment.Style;

        if((UseVerticalLimits.HasValue && !UseVerticalLimits.Value) ||
           (!UseVerticalLimits.HasValue && style >= TexStyle.Text))
            // Attach atoms for limits as scripts.
            return new ScriptsAtom(BaseAtom, LowerLimitAtom, UpperLimitAtom).CreateBox(environment);

        // Create box for base atom.
        Box    base_box;
        double delta;

        if((BaseAtom as SymbolAtom)?.Type == TexAtomType.BigOperator)
        {
            // Find character of best scale for operator symbol.
            var op_char = tex_font.GetCharInfo(((SymbolAtom)BaseAtom).Name, style);
            if(style < TexStyle.Text && tex_font.HasNextLarger(op_char))
                op_char = tex_font.GetNextLargerCharInfo(op_char, style);
            var char_box = new CharBox(environment, op_char);
            char_box.Shift = -(char_box.Height + char_box.Depth) / 2 -
                environment.TexFont.GetAxisHeight(environment.Style);
            base_box = new HorizontalBox(char_box);

            delta = op_char.Metrics.Italic;
            if(delta > TexUtilities.FloatPrecision)
                base_box.Add(new StrutBox(delta, 0, 0, 0));
        }
        else
        {
            base_box = new HorizontalBox(BaseAtom?.CreateBox(environment) ?? StrutBox.Empty);
            delta   = 0;
        }

        // Create boxes for upper and lower limits.
        var upper_limit_box = UpperLimitAtom?.CreateBox(environment.GetSuperscriptStyle());
        var lower_limit_box = LowerLimitAtom?.CreateBox(environment.GetSubscriptStyle());

        // Make all component boxes equally wide.
        var max_width = Math.Max(Math.Max(base_box.Width, upper_limit_box?.Width ?? 0),
            lower_limit_box?.Width ?? 0);
        base_box = ChangeWidth(base_box, max_width);
        if(upper_limit_box != null)
            upper_limit_box = ChangeWidth(upper_limit_box, max_width);
        if(lower_limit_box != null)
            lower_limit_box = ChangeWidth(lower_limit_box, max_width);

        var result_box  = new VerticalBox();
        var op_spacing5 = tex_font.GetBigOpSpacing5(style);
        var kern       = 0d;

        // Create and add box for upper limit.
        if(UpperLimitAtom != null)
        {
            result_box.Add(new StrutBox(0, op_spacing5, 0, 0));
            Debug.Assert(upper_limit_box != null, "upperLimitBox != null");
            upper_limit_box.Shift = delta / 2;
            result_box.Add(upper_limit_box);
            kern = Math.Max(tex_font.GetBigOpSpacing1(style), tex_font.GetBigOpSpacing3(style) -
                upper_limit_box.Depth);
            result_box.Add(new StrutBox(0, kern, 0, 0));
        }

        // Add box for base atom.
        result_box.Add(base_box);

        // Create and add box for lower limit.
        if(LowerLimitAtom != null)
        {
            Debug.Assert(lower_limit_box != null, "lowerLimitBox != null");
            result_box.Add(new StrutBox(0, Math.Max(tex_font.GetBigOpSpacing2(style), tex_font.GetBigOpSpacing4(style) -
                lower_limit_box.Height), 0, 0));
            lower_limit_box.Shift = -delta / 2;
            result_box.Add(lower_limit_box);
            result_box.Add(new StrutBox(0, op_spacing5, 0, 0));
        }

        // Adjust height and depth of result box.
        var base_box_height = base_box.Height;
        var total_height   = result_box.Height + result_box.Depth;
        if(upper_limit_box != null)
            base_box_height += op_spacing5 + kern + upper_limit_box.Height + upper_limit_box.Depth;
        result_box.Height = base_box_height;
        result_box.Depth  = total_height - base_box_height;

        return result_box;
    }
}