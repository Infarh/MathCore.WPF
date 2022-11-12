namespace MathCore.WPF.TeX;

/// <summary>Represents glueElement for holding together boxes</summary>
internal class Glue
{
    private static readonly List<Glue> __GlueTypes;
    private static readonly int[,,] __GlueRules;

    static Glue()
    {
        var parser = new GlueSettingsParser();
        __GlueTypes = parser.GetGlueTypes();
        __GlueRules = parser.GetGlueRules();
    }

    public double Space { get; }

    public double Stretch { get; }

    public double Shrink { get; }

    public string Name { get; private set; }

    public static Box CreateBox(TexAtomType LeftAtomType, TexAtomType RightAtomType, TexEnvironment environment)
    {
        LeftAtomType  = LeftAtomType > TexAtomType.Inner ? TexAtomType.Ordinary : LeftAtomType;
        RightAtomType = RightAtomType > TexAtomType.Inner ? TexAtomType.Ordinary : RightAtomType;
        var glue_type = __GlueRules[(int)LeftAtomType, (int)RightAtomType, (int)environment.Style / 2];
        return __GlueTypes[glue_type].CreateBox(environment);
    }

    public Glue(double space, double stretch, double shrink, string name)
    {
        Space   = space;
        Stretch = stretch;
        Shrink  = shrink;
        Name    = name;
    }

    private Box CreateBox(TexEnvironment environment)
    {
        var tex_font = environment.TexFont;
        var quad    = tex_font.GetQuad(tex_font.GetMuFontId(), environment.Style);
        return new GlueBox((Space / 18.0f) * quad, (Stretch / 18.0f) * quad, (Shrink / 18.0f) * quad);
    }
}