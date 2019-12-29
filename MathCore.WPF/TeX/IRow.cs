// Atom consisting of child atoms displayed in horizontal row with glueElement between them.
namespace MathCore.WPF.TeX
{
    internal interface IRow
    {
        // Dummy atom representing atom just before first child atom.
        DummyAtom PreviousAtom { set; }
    }
}
