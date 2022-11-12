// Atom representing single character that can be marked as text symbol.
namespace MathCore.WPF.TeX;

internal abstract class CharSymbol : Atom
{
    protected CharSymbol() => IsTextSymbol = false;

    public bool IsTextSymbol
    {
        get;
        set;
    }

    public abstract CharFont GetCharFont(ITeXFont TexFont);
}