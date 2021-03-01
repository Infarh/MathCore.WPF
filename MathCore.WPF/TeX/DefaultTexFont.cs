using System;
using System.Collections.Generic;

namespace MathCore.WPF.TeX
{
    /// <summary>Default implementation of ITeXFont that reads all font information from XML file</summary>
    internal class DefaultTexFont : ITeXFont
    {
        private static readonly Dictionary<string, double> parameters;
        private static readonly Dictionary<string, object> generalSettings;
        private static readonly Dictionary<string, CharFont[]> textStyleMappings;
        private static readonly Dictionary<string, CharFont> symbolMappings;
        private static readonly string[] defaultTextStyleMappings;
        private static readonly TexFontInfo[] fontInfoList;

        static DefaultTexFont()
        {
            try
            {
                var parser = new DefaultTexFontParser();
                parameters = parser.GetParameters();
                generalSettings = parser.GetGeneralSettings();
                textStyleMappings = parser.GetTextStyleMappings();
                defaultTextStyleMappings = parser.GetDefaultTextStyleMappings();
                symbolMappings = parser.GetSymbolMappings();
                fontInfoList = parser.GetFontDescriptions();

                // Check that Mu font exists.
                var muFontId = (int)generalSettings["mufontid"];
                if(muFontId < 0 || muFontId >= fontInfoList.Length || fontInfoList[muFontId] is null)
                    throw new InvalidOperationException("ID of Mu font is invalid.");
            } catch(Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static double GetParameter(string name) => parameters[name];

        private static double GetSizeFactor(TexStyle style)
        {
            if(style < TexStyle.Script)
                return 1d;
            if(style < TexStyle.ScriptScript)
                return (double)generalSettings["scriptfactor"];
            return (double)generalSettings["scriptscriptfactor"];
        }

        public double Size { get; }
        public DefaultTexFont(double size) => Size = size;

        public ITeXFont DeriveFont(double newSize) => new DefaultTexFont(newSize);

        public ExtensionChar GetExtension(CharInfo charInfo, TexStyle style)
        {
            var sizeFactor = GetSizeFactor(style);

            // Create character for each part of extension.
            var fontInfo = fontInfoList[charInfo.FontId];
            var extension = fontInfo.GetExtension(charInfo.Character);
            var parts = new CharInfo[extension.Length];
            for(var i = 0; i < extension.Length; i++)
            {
                if(extension[i] == (int)TexCharKind.None)
                    parts[i] = null;
                else
                    parts[i] = new CharInfo((char)extension[i], charInfo.Font, sizeFactor, charInfo.FontId,
                        GetMetrics(new CharFont((char)extension[i], charInfo.FontId), sizeFactor));
            }

            return new ExtensionChar(parts[TexFontUtilities.ExtensionTop], parts[TexFontUtilities.ExtensionMiddle],
                parts[TexFontUtilities.ExtensionBottom], parts[TexFontUtilities.ExtensionRepeat]);
        }

        public CharFont GetLigature(CharFont leftCharFont, CharFont rightCharFont) =>
            leftCharFont.FontId == rightCharFont.FontId
                ? fontInfoList[leftCharFont.FontId].GetLigature(leftCharFont.Character, rightCharFont.Character)
                : null;

        public CharInfo GetNextLargerCharInfo(CharInfo charInfo, TexStyle style)
        {
            var charFont = fontInfoList[charInfo.FontId].GetNextLarger(charInfo.Character);
            return new CharInfo(charFont.Character, fontInfoList[charFont.FontId].Font, GetSizeFactor(style), charFont.FontId,
                GetMetrics(charFont, GetSizeFactor(style)));
        }

        public CharInfo GetDefaultCharInfo(char character, TexStyle style)
        {
            if(character >= '0' && character <= '9')
                return GetCharInfo(character, defaultTextStyleMappings[(int)TexCharKind.Numbers], style);
            if(character >= 'a' && character <= 'z')
                return GetCharInfo(character, defaultTextStyleMappings[(int)TexCharKind.Small], style);
            return GetCharInfo(character, defaultTextStyleMappings[(int)TexCharKind.Capitals], style);
        }

        private CharInfo GetCharInfo(char character, CharFont[] charFont, TexStyle style)
        {
            TexCharKind charKind;
            int charIndexOffset;
            if(character >= '0' && character <= '9')
            {
                charKind = TexCharKind.Numbers;
                charIndexOffset = character - '0';
            }
            else if(character >= 'a' && character <= 'z')
            {
                charKind = TexCharKind.Small;
                charIndexOffset = character - 'a';
            }
            else
            {
                charKind = TexCharKind.Capitals;
                charIndexOffset = character - 'A';
            }

            return charFont[(int)charKind] is null
                ? GetDefaultCharInfo(character, style)
                : GetCharInfo(new CharFont((char)(charFont[(int)charKind].Character + charIndexOffset),
                    charFont[(int)charKind].FontId), style);
        }

        public CharInfo GetCharInfo(char character, string textStyle, TexStyle style)
        {
            var mapping = textStyleMappings[textStyle];
            if(mapping is null)
                throw new TextStyleMappingNotFoundException(textStyle);
            return GetCharInfo(character, mapping, style);
        }

        public CharInfo GetCharInfo(CharFont charFont, TexStyle style)
        {
            var size = GetSizeFactor(style);
            return new CharInfo(charFont.Character, fontInfoList[charFont.FontId].Font, size, charFont.FontId, GetMetrics(charFont, size));
        }

        public CharInfo GetCharInfo(string symbolName, TexStyle style)
        {
            var mapping = symbolMappings[symbolName];
            if(mapping is null)
                throw new SymbolMappingNotFoundException(symbolName);
            return GetCharInfo(mapping, style);
        }

        public double GetKern(CharFont leftCharFont, CharFont rightCharFont, TexStyle style) =>
            leftCharFont.FontId == rightCharFont.FontId
                ? fontInfoList[leftCharFont.FontId].GetKern(leftCharFont.Character, rightCharFont.Character,
                    GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint)
                : 0;

        public double GetQuad(int fontId, TexStyle style) => fontInfoList[fontId].GetQuad(GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint);

        public double GetSkew(CharFont charFont, TexStyle style)
        {
            var skewChar = fontInfoList[charFont.FontId].SkewCharacter;
            return skewChar == 1 ? 0 : GetKern(charFont, new CharFont(skewChar, charFont.FontId), style);
        }

        public bool HasSpace(int fontId) => fontInfoList[fontId].HasSpace();

        public bool HasNextLarger(CharInfo charInfo) =>
            fontInfoList[charInfo.FontId].GetNextLarger(charInfo.Character) != null;

        public bool IsExtensionChar(CharInfo charInfo) =>
            fontInfoList[charInfo.FontId].GetExtension(charInfo.Character) != null;

        public int GetMuFontId() => (int)generalSettings["mufontid"];

        public double GetXHeight(TexStyle style, int fontCode) =>
            fontInfoList[fontCode].GetXHeight(GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint);

        public double GetSpace(TexStyle style) =>
            fontInfoList[(int)generalSettings["spacefontid"]].GetSpace(GetSizeFactor(style)
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

        private TexFontMetrics GetMetrics(CharFont charFont, double size)
        {
            var metrics = fontInfoList[charFont.FontId].GetMetrics(charFont.Character);
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
}