

// Atom representing other atom with delimeter and script atoms over or under it.
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace MathCore.WPF.TeX;

internal class OverUnderDelimiter : Atom
{
    private static double GetMaxWidth(Box BaseBox, Box DelimeterBox, Box? ScriptBox)
    {
        var max_width = Math.Max(BaseBox.Width, DelimeterBox.Height + DelimeterBox.Depth);
        if(ScriptBox != null)
            max_width = Math.Max(max_width, ScriptBox.Width);
        return max_width;
    }

    public Atom? BaseAtom { get; }

    private Atom? Script { get; }

    private SymbolAtom Symbol { get; }

    /// <summary>Kern between delimeter symbol and script</summary>
    private SpaceAtom Kern { get; }

    /// <summary> True to place delimeter symbol Over base; false to place delimeter symbol under base</summary>
    public bool Over { get; set; }

    public OverUnderDelimiter(Atom BaseAtom, Atom script, SymbolAtom symbol, TexUnit KernUnit, double kern, bool over)
    {
        Type          = TexAtomType.Inner;
        this.BaseAtom = BaseAtom;
        Script        = script;
        Symbol        = symbol;
        Kern          = new(KernUnit, 0, kern, 0);
        Over          = over;
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        // Create boxes for base, delimeter, and script atoms.
        var base_box      = BaseAtom is null ? StrutBox.Empty : BaseAtom.CreateBox(environment);
        var delimeter_box = DelimiterFactory.CreateBox(Symbol.Name, base_box.Width, environment);
        var script_box    = Script?.CreateBox(Over ? environment.GetSuperscriptStyle() : environment.GetSubscriptStyle());

        // Create centered horizontal box if any box is smaller than maximum width.
        var max_width = GetMaxWidth(base_box, delimeter_box, script_box);
        if(Math.Abs(max_width - base_box.Width) > TexUtilities.FloatPrecision)
            base_box = new HorizontalBox(base_box, max_width, TexAlignment.Center);

        if(Math.Abs(max_width - delimeter_box.Height - delimeter_box.Depth) > TexUtilities.FloatPrecision)
            delimeter_box = new VerticalBox(delimeter_box, max_width, TexAlignment.Center);

        if(script_box != null && Math.Abs(max_width - script_box.Width) > TexUtilities.FloatPrecision)
            script_box = new HorizontalBox(script_box, max_width, TexAlignment.Center);

        return new OverUnderBox(base_box, delimeter_box, script_box, Kern.CreateBox(environment).Height, Over);
    }
}