namespace MathCore.WPF.TeX;

/// <summary>Atom representing other atom with atoms optionally over and under it</summary>
internal class UnderOverAtom : Atom
{
    private static Box? ChangeWidth(Box? box, double MaxWidth)
    {
        if(box != null && Math.Abs(MaxWidth - box.Width) > TexUtilities.FloatPrecision)
            return new HorizontalBox(box, MaxWidth, TexAlignment.Center);
        return box;
    }

    public Atom? BaseAtom { get; }

    public Atom? UnderAtom { get; }

    public Atom? OverAtom { get; }

    // Kern between base and under atom.
    public double UnderSpace { get; set; }

    // Kern between base and over atom.
    public double OverSpace { get; set; }

    public TexUnit UnderSpaceUnit { get; set; }
    public TexUnit OverSpaceUnit { get; set; }

    public bool UnderScriptSmaller { get; set; }

    public bool OverScriptSmaller { get; set; }

    public UnderOverAtom(
        Atom BaseAtom, 
        Atom? UnderOver, 
        TexUnit UnderOverUnit,
        double UnderOverSpace,
        bool UnderOverScriptSize,
        bool over)
    {
        SpaceAtom.CheckUnit(UnderOverUnit);

        this.BaseAtom = BaseAtom;

        if(over)
        {
            UnderAtom          = null;
            UnderSpace         = 0;
            UnderSpaceUnit     = 0;
            UnderScriptSmaller = false;
            OverAtom           = UnderOver;
            OverSpaceUnit      = UnderOverUnit;
            OverSpace          = UnderOverSpace;
            OverScriptSmaller  = UnderOverScriptSize;
        }
        else
        {
            UnderAtom          = UnderOver;
            UnderSpaceUnit     = UnderOverUnit;
            UnderSpace         = UnderOverSpace;
            UnderScriptSmaller = UnderOverScriptSize;
            OverSpace          = 0;
            OverAtom           = null;
            OverSpaceUnit      = 0;
            OverScriptSmaller  = false;
        }
    }

    public UnderOverAtom(
        Atom BaseAtom, 
        Atom under, 
        TexUnit UnderUnit,
        double UnderSpace,
        bool UnderScriptSize,
        Atom over,
        TexUnit OverUnit,
        double OverSpace, 
        bool OverScriptSize)
    {
        SpaceAtom.CheckUnit(UnderUnit);
        SpaceAtom.CheckUnit(OverUnit);

        this.BaseAtom      = BaseAtom;
        UnderAtom          = under;
        UnderSpaceUnit     = UnderUnit;
        this.UnderSpace    = UnderSpace;
        UnderScriptSmaller = UnderScriptSize;
        OverAtom           = over;
        OverSpaceUnit      = OverUnit;
        this.OverSpace     = OverSpace;
        OverScriptSmaller  = OverScriptSize;
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        // Create box for base atom.
        var base_box = BaseAtom is null ? StrutBox.Empty : BaseAtom.CreateBox(environment);

        // Create boxes for over and under atoms.
        Box over_box  = null, under_box = null;
        var max_width = base_box.Width;

        if(OverAtom != null)
        {
            over_box  = OverAtom.CreateBox(OverScriptSmaller ? environment.GetSubscriptStyle() : environment);
            max_width = Math.Max(max_width, over_box.Width);
        }

        if(UnderAtom != null)
        {
            under_box = UnderAtom.CreateBox(UnderScriptSmaller ? environment.GetSubscriptStyle() : environment);
            max_width = Math.Max(max_width, under_box.Width);
        }

        // Create result box.
        var result_box = new VerticalBox();

        environment.LastFontId = base_box.GetLastFontId();

        // Create and add box for over atom.
        if(OverAtom != null)
        {
            result_box.Add(ChangeWidth(over_box, max_width));
            result_box.Add(new SpaceAtom(OverSpaceUnit, 0, OverSpace, 0).CreateBox(environment));
        }

        // Add box for base atom.
        result_box.Add(ChangeWidth(base_box, max_width));

        var total_height = result_box.Height + result_box.Depth - base_box.Depth;

        // Create and add box for under atom.
        if(UnderAtom != null)
        {
            result_box.Add(new SpaceAtom(OverSpaceUnit, 0, UnderSpace, 0).CreateBox(environment));
            result_box.Add(ChangeWidth(under_box, max_width));
        }

        result_box.Depth  = result_box.Height + result_box.Depth - total_height;
        result_box.Height = total_height;

        return result_box;
    }
}