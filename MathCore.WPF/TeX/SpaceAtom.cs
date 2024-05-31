

// Atom representing whitespace.
namespace MathCore.WPF.TeX;

internal class SpaceAtom : Atom
{
    // Collection of unit conversion functions.
    private static readonly UnitConversion[] __UnitConversions = new[]
    {
        e => e.TexFont.GetXHeight(e.Style, e.LastFontId),
        e => e.TexFont.GetXHeight(e.Style, e.LastFontId),
        e => 1.0 / e.TexFont.Size,
        e => TexFontUtilities.PixelsPerPoint / e.TexFont.Size,
        e => 12 * TexFontUtilities.PixelsPerPoint / e.TexFont.Size,
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

    public SpaceAtom(TexUnit WidthUnit, double width, TexUnit HeightUnit, double height, TexUnit DepthUnit, double depth)
    {
        CheckUnit(WidthUnit);
        CheckUnit(HeightUnit);
        CheckUnit(DepthUnit);

        _IsHardSpace = false;
        _WidthUnit   = WidthUnit;
        _HeightUnit  = HeightUnit;
        _DepthUnit   = DepthUnit;
        _Width       = width;
        _Height      = height;
        _Depth       = depth;
    }

    public SpaceAtom(TexUnit unit, double width, double height, double depth)
    {
        CheckUnit(unit);

        _IsHardSpace = false;
        _WidthUnit   = unit;
        _HeightUnit  = unit;
        _DepthUnit   = unit;
        _Width       = width;
        _Height      = height;
        _Depth       = depth;
    }

    public SpaceAtom() => _IsHardSpace = true;

    public override Box CreateBox(TexEnvironment environment) =>
        _IsHardSpace
            ? new(environment.TexFont.GetSpace(environment.Style), 0, 0, 0)
            : new StrutBox(_Width * GetConversionFactor(_WidthUnit, environment), _Height * GetConversionFactor(
                _HeightUnit, environment), _Depth * GetConversionFactor(_DepthUnit, environment), 0);

    private static double GetConversionFactor(TexUnit unit, TexEnvironment environment) => __UnitConversions[(int)unit](environment);
}