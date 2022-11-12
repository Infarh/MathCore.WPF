

// Atom representing whitespace.
namespace MathCore.WPF.TeX;

internal class SpaceAtom : Atom
{
    // Collection of unit conversion functions.
    private static readonly UnitConversion[] __UnitConversions = new[]
    {
        new UnitConversion(e => e.TexFont.GetXHeight(e.Style, e.LastFontId)),
        new UnitConversion(e => e.TexFont.GetXHeight(e.Style, e.LastFontId)),
        new UnitConversion(e => 1.0 / e.TexFont.Size),
        new UnitConversion(e => TexFontUtilities.PixelsPerPoint / e.TexFont.Size),
        new UnitConversion(e => (12 * TexFontUtilities.PixelsPerPoint) / e.TexFont.Size),
        new UnitConversion(e =>
        {
            var tex_font = e.TexFont;
            return tex_font.GetQuad(tex_font.GetMuFontId(), e.Style) / 18;
        }),
    };

    private delegate double UnitConversion(TexEnvironment environment);

    public static void CheckUnit(TexUnit unit)
    {
        if(unit < 0 || (int)unit >= __UnitConversions.Length)
            throw new InvalidOperationException("No conversion for this unit exists.");
    }

    // True to represent hard space (actual space character).
    private readonly bool _IsHardSpace;

    private readonly double _Width;
    private readonly double _Height;
    private readonly double _Depth;

    private readonly TexUnit _WidthUnit;
    private readonly TexUnit _HeightUnit;
    private readonly TexUnit _DepthUnit;

    public SpaceAtom(TexUnit WidthUnit, double width, TexUnit HeightUnit, double height,
        TexUnit DepthUnit, double depth)
    {
        CheckUnit(WidthUnit);
        CheckUnit(HeightUnit);
        CheckUnit(DepthUnit);

        _IsHardSpace     = false;
        this._WidthUnit  = WidthUnit;
        this._HeightUnit = HeightUnit;
        this._DepthUnit  = DepthUnit;
        this._Width      = width;
        this._Height     = height;
        this._Depth      = depth;
    }

    public SpaceAtom(TexUnit unit, double width, double height, double depth)
    {
        CheckUnit(unit);

        _IsHardSpace = false;
        _WidthUnit   = unit;
        _HeightUnit  = unit;
        _DepthUnit   = unit;
        this._Width  = width;
        this._Height = height;
        this._Depth  = depth;
    }

    public SpaceAtom() => _IsHardSpace = true;

    public override Box CreateBox(TexEnvironment environment)
    {
        if(_IsHardSpace)
            return new StrutBox(environment.TexFont.GetSpace(environment.Style), 0, 0, 0);
        return new StrutBox(_Width * GetConversionFactor(_WidthUnit, environment), _Height * GetConversionFactor(
            _HeightUnit, environment), _Depth * GetConversionFactor(_DepthUnit, environment), 0);
    }

    private double GetConversionFactor(TexUnit unit, TexEnvironment environment) => __UnitConversions[(int)unit](environment);
}