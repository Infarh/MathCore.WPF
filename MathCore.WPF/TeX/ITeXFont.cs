// Font that specifies how TexFormula objects are rendered.
namespace MathCore.WPF.TeX;

internal interface ITeXFont
{
    double Size { get; }

    ITeXFont DeriveFont(double NewSize);

    ExtensionChar GetExtension(CharInfo CharInfo, TexStyle style);

    CharFont? GetLigature(CharFont LeftChar, CharFont RightChar);

    CharInfo GetNextLargerCharInfo(CharInfo CharInfo, TexStyle style);

    CharInfo GetDefaultCharInfo(char character, TexStyle style);

    CharInfo GetCharInfo(char character, string TextStyle, TexStyle style);

    CharInfo GetCharInfo(CharFont CharFont, TexStyle style);

    CharInfo GetCharInfo(string name, TexStyle style);

    double GetKern(CharFont LeftChar, CharFont RightChar, TexStyle style);

    double GetQuad(int FontId, TexStyle style);

    double GetSkew(CharFont CharFont, TexStyle style);

    bool HasSpace(int FontId);

    bool HasNextLarger(CharInfo CharInfo);

    bool IsExtensionChar(CharInfo CharInfo);

    int GetMuFontId();

    double GetXHeight(TexStyle style, int FontId);

    double GetSpace(TexStyle style);

    double GetAxisHeight(TexStyle style);

    double GetBigOpSpacing1(TexStyle style);

    double GetBigOpSpacing2(TexStyle style);

    double GetBigOpSpacing3(TexStyle style);

    double GetBigOpSpacing4(TexStyle style);

    double GetBigOpSpacing5(TexStyle style);

    double GetSub1(TexStyle style);

    double GetSub2(TexStyle style);

    double GetSubDrop(TexStyle style);

    double GetSup1(TexStyle style);

    double GetSup2(TexStyle style);

    double GetSup3(TexStyle style);

    double GetSupDrop(TexStyle style);

    double GetNum1(TexStyle style);

    double GetNum2(TexStyle style);

    double GetNum3(TexStyle style);

    double GetDenom1(TexStyle style);

    double GetDenom2(TexStyle style);

    double GetDefaultLineThickness(TexStyle style);
}