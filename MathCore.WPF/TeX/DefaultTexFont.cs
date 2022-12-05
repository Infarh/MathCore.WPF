namespace MathCore.WPF.TeX;

/// <summary>Default implementation of ITeXFont that reads all font information from XML file</summary>
internal class DefaultTexFont : ITeXFont
{
    private static readonly Dictionary<string, double> __Parameters;
    private static readonly Dictionary<string, object> __GeneralSettings;
    private static readonly Dictionary<string, CharFont[]> __TextStyleMappings;
    private static readonly Dictionary<string, CharFont> __SymbolMappings;
    private static readonly string[] __DefaultTextStyleMappings;
    private static readonly TexFontInfo[] __FontInfoList;

    static DefaultTexFont()
    {
        try
        {
            var parser = new DefaultTexFontParser();
            __Parameters               = parser.GetParameters();
            __GeneralSettings          = parser.GetGeneralSettings();
            __TextStyleMappings        = parser.GetTextStyleMappings();
            __DefaultTextStyleMappings = parser.GetDefaultTextStyleMappings();
            __SymbolMappings           = parser.GetSymbolMappings();
            __FontInfoList             = parser.GetFontDescriptions();

            // Check that Mu font exists.
            var mu_font_id = (int)__GeneralSettings["mufontid"];
            if(mu_font_id < 0 || mu_font_id >= __FontInfoList.Length || __FontInfoList[mu_font_id] is null)
                throw new InvalidOperationException("ID of Mu font is invalid.");
        } catch(Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static double GetParameter(string name) => __Parameters[name];

    private static double GetSizeFactor(TexStyle style) => style switch
    {
        < TexStyle.Script       => 1d,
        < TexStyle.ScriptScript => (double)__GeneralSettings["scriptfactor"],
        _                       => (double)__GeneralSettings["scriptscriptfactor"]
    };

    public double Size { get; }
    public DefaultTexFont(double size) => Size = size;

    public ITeXFont DeriveFont(double NewSize) => new DefaultTexFont(NewSize);

    public ExtensionChar GetExtension(CharInfo CharInfo, TexStyle style)
    {
        var size_factor = GetSizeFactor(style);

        // Create character for each part of extension.
        var font_info  = __FontInfoList[CharInfo.FontId];
        var extension = font_info.GetExtension(CharInfo.Character);
        var parts     = new CharInfo[extension.Length];
        for(var i = 0; i < extension.Length; i++)
            parts[i] = extension[i] == (int)TexCharKind.None
                ? null
                : new(
                    (char)extension[i],
                    CharInfo.Font,
                    size_factor,
                    CharInfo.FontId,
                    GetMetrics(new CharFont((char)extension[i], CharInfo.FontId), size_factor));

        return new ExtensionChar(parts[TexFontUtilities.ExtensionTop], parts[TexFontUtilities.ExtensionMiddle],
            parts[TexFontUtilities.ExtensionBottom], parts[TexFontUtilities.ExtensionRepeat]);
    }

    public CharFont? GetLigature(CharFont LeftCharFont, CharFont RightCharFont) =>
        LeftCharFont.FontId == RightCharFont.FontId
            ? __FontInfoList[LeftCharFont.FontId].GetLigature(LeftCharFont.Character, RightCharFont.Character)
            : null;

    public CharInfo GetNextLargerCharInfo(CharInfo CharInfo, TexStyle style)
    {
        var char_font = __FontInfoList[CharInfo.FontId].GetNextLarger(CharInfo.Character);
        return new CharInfo(char_font.Character, __FontInfoList[char_font.FontId].Font, GetSizeFactor(style), char_font.FontId,
            GetMetrics(char_font, GetSizeFactor(style)));
    }

    public CharInfo GetDefaultCharInfo(char character, TexStyle style) =>
        character switch
        {
            >= '0' and <= '9' => GetCharInfo(character, __DefaultTextStyleMappings[(int)TexCharKind.Numbers], style),
            >= 'a' and <= 'z' => GetCharInfo(character, __DefaultTextStyleMappings[(int)TexCharKind.Small], style),
            _                 => GetCharInfo(character, __DefaultTextStyleMappings[(int)TexCharKind.Capitals], style)
        };

    private CharInfo GetCharInfo(char character, CharFont[] CharFont, TexStyle style)
    {
        TexCharKind char_kind;
        int         char_index_offset;
        switch (character)
        {
            case >= '0' and <= '9':
                char_kind         = TexCharKind.Numbers;
                char_index_offset = character - '0';
                break;
            case >= 'a' and <= 'z':
                char_kind         = TexCharKind.Small;
                char_index_offset = character - 'a';
                break;
            default:
                char_kind         = TexCharKind.Capitals;
                char_index_offset = character - 'A';
                break;
        }

        return CharFont[(int)char_kind] is null
            ? GetDefaultCharInfo(character, style)
            : GetCharInfo(new CharFont((char)(CharFont[(int)char_kind].Character + char_index_offset),
                CharFont[(int)char_kind].FontId), style);
    }

    public CharInfo GetCharInfo(char character, string TextStyle, TexStyle style)
    {
        var mapping = __TextStyleMappings[TextStyle];
        if(mapping is null)
            throw new TextStyleMappingNotFoundException(TextStyle);
        return GetCharInfo(character, mapping, style);
    }

    public CharInfo GetCharInfo(CharFont CharFont, TexStyle style)
    {
        var size = GetSizeFactor(style);
        return new CharInfo(CharFont.Character, __FontInfoList[CharFont.FontId].Font, size, CharFont.FontId, GetMetrics(CharFont, size));
    }

    public CharInfo GetCharInfo(string SymbolName, TexStyle style)
    {
        var mapping = __SymbolMappings[SymbolName];
        if(mapping is null)
            throw new SymbolMappingNotFoundException(SymbolName);
        return GetCharInfo(mapping, style);
    }

    public double GetKern(CharFont LeftCharFont, CharFont RightCharFont, TexStyle style) =>
        LeftCharFont.FontId == RightCharFont.FontId
            ? __FontInfoList[LeftCharFont.FontId].GetKern(LeftCharFont.Character, RightCharFont.Character,
                GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint)
            : 0;

    public double GetQuad(int FontId, TexStyle style) => __FontInfoList[FontId].GetQuad(GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint);

    public double GetSkew(CharFont CharFont, TexStyle style)
    {
        var skew_char = __FontInfoList[CharFont.FontId].SkewCharacter;
        return skew_char == 1 ? 0 : GetKern(CharFont, new CharFont(skew_char, CharFont.FontId), style);
    }

    public bool HasSpace(int FontId) => __FontInfoList[FontId].HasSpace();

    public bool HasNextLarger(CharInfo CharInfo) =>
        __FontInfoList[CharInfo.FontId].GetNextLarger(CharInfo.Character) != null;

    public bool IsExtensionChar(CharInfo CharInfo) =>
        __FontInfoList[CharInfo.FontId].GetExtension(CharInfo.Character) != null;

    public int GetMuFontId() => (int)__GeneralSettings["mufontid"];

    public double GetXHeight(TexStyle style, int FontCode) =>
        __FontInfoList[FontCode].GetXHeight(GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint);

    public double GetSpace(TexStyle style) =>
        __FontInfoList[(int)__GeneralSettings["spacefontid"]].GetSpace(GetSizeFactor(style)
            * TexFontUtilities.PixelsPerPoint);

    public double GetAxisHeight(TexStyle style) => GetParameter("axisheight") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetBigOpSpacing1(TexStyle style) => GetParameter("bigopspacing1") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetBigOpSpacing2(TexStyle style) => GetParameter("bigopspacing2") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetBigOpSpacing3(TexStyle style) => GetParameter("bigopspacing3") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetBigOpSpacing4(TexStyle style) => GetParameter("bigopspacing4") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetBigOpSpacing5(TexStyle style) => GetParameter("bigopspacing5") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetSub1(TexStyle style) => GetParameter("sub1") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetSub2(TexStyle style) => GetParameter("sub2") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetSubDrop(TexStyle style) => GetParameter("subdrop") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetSup1(TexStyle style) => GetParameter("sup1") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetSup2(TexStyle style) => GetParameter("sup2") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetSup3(TexStyle style) => GetParameter("sup3") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetSupDrop(TexStyle style) => GetParameter("supdrop") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetNum1(TexStyle style) => GetParameter("num1") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetNum2(TexStyle style) => GetParameter("num2") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetNum3(TexStyle style) => GetParameter("num3") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetDenom1(TexStyle style) => GetParameter("denom1") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetDenom2(TexStyle style) => GetParameter("denom2") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    public double GetDefaultLineThickness(TexStyle style) => GetParameter("defaultrulethickness") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;

    private static TexFontMetrics GetMetrics(CharFont CharFont, double size)
    {
        var metrics = __FontInfoList[CharFont.FontId].GetMetrics(CharFont.Character);
        return new TexFontMetrics(metrics[TexFontUtilities.MetricsWidth], metrics[TexFontUtilities.MetricsHeight],
            metrics[TexFontUtilities.MetricsDepth], metrics[TexFontUtilities.MetricsItalic],
            size * TexFontUtilities.PixelsPerPoint);
    }
}

internal enum TexCharKind
{
    None = -1,
    Numbers = 0,
    Capitals = 1,
    Small = 2
}