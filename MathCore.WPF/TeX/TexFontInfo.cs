using System.Windows.Media;

namespace MathCore.WPF.TeX;

///<summary>Specifies all information about single font</summary>
internal class TexFontInfo
{
    public const int CharCodesCount = 256;

    private readonly double[][] _Metrics;
    private readonly Dictionary<Tuple<char, char>, char> _Ligatures;
    private readonly Dictionary<Tuple<char, char>, double> _Kerns;
    private readonly CharFont[] _NextLarger;
    private readonly int[][] _Extensions;

    public int FontId { get; }

    public GlyphTypeface Font { get; private set; }

    public double XHeight { get; }

    public double Space { get; }

    public double Quad { get; }

    /// <summary>Skew character (used for positioning accents)</summary>
    public char SkewCharacter { get; set; }

    public TexFontInfo(int FontId, GlyphTypeface font, double XHeight, double space, double quad)
    {
        _Metrics    = new double[CharCodesCount][];
        _Ligatures  = [];
        _Kerns      = [];
        _NextLarger = new CharFont[CharCodesCount];
        _Extensions = new int[CharCodesCount][];

        this.FontId   = FontId;
        Font          = font;
        this.XHeight  = XHeight;
        Space         = space;
        Quad          = quad;
        SkewCharacter = (char)1;
    }

    public void AddKern(char LeftChar, char RightChar, double kern) =>
        _Kerns.Add(Tuple.Create(LeftChar, RightChar), kern);

    public void AddLigature(char LeftChar, char RightChar, char LigatureChar) =>
        _Ligatures.Add(Tuple.Create(LeftChar, RightChar), LigatureChar);

    public bool HasSpace() => Space > TexUtilities.FloatPrecision;

    public void SetExtensions(char character, int[] extensions) =>
        _Extensions[character] = extensions;

    public void SetMetrics(char character, double[] metrics) =>
        _Metrics[character] = metrics;

    public void SetNextLarger(char character, char LargerCharacter, int LargerFont) =>
        _NextLarger[character] = new(LargerCharacter, LargerFont);

    public int[] GetExtension(char character) => _Extensions[character];

    public double GetKern(char LeftChar, char RightChar, double factor) =>
        _Kerns.GetValue(Tuple.Create(LeftChar, RightChar)) * factor;

    public CharFont? GetLigature(char left, char right)
    {
        var key = Tuple.Create(left, right);
        return _Ligatures.ContainsKey(key) 
            ? new CharFont(_Ligatures[key], FontId) 
            : null;
    }

    public CharFont GetNextLarger(char character) => _NextLarger[character];

    public double GetQuad(double factor) => Quad * factor;

    public double GetSpace(double factor) => Space * factor;

    public double GetXHeight(double factor) => XHeight * factor;

    public double[] GetMetrics(char character) => _Metrics[character];
}