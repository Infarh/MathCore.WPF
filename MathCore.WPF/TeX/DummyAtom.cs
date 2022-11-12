namespace MathCore.WPF.TeX;

/// <summary>Dummy atom representing atom whose type can change or which can be replaced by a ligature</summary>
internal sealed class DummyAtom : Atom
{
    public DummyAtom PreviousAtom { set { if(Atom is IRow row) row.PreviousAtom = value; } }

    public Atom Atom { get; private set; }

    public bool IsTextSymbol { get; set; }

    public bool IsCharSymbol => Atom is CharSymbol;

    public bool IsKern => Atom is SpaceAtom;

    public DummyAtom(Atom atom)
    {
        Type         = TexAtomType.None;
        Atom         = atom;
        IsTextSymbol = false;
    }

    public void SetLigature(FixedCharAtom LigatureAtom)
    {
        Atom         = LigatureAtom;
        Type         = TexAtomType.None;
        IsTextSymbol = false;
    }

    public CharFont GetCharFont(ITeXFont TexFont) => ((CharSymbol)Atom).GetCharFont(TexFont);

    public override Box CreateBox(TexEnvironment environment)
    {
        if(IsTextSymbol)
            ((CharSymbol)Atom).IsTextSymbol = true;
        var result_box = Atom.CreateBox(environment);
        //TODO: Разобраться с надстрочными знаками!
        if(Atom.Type == TexAtomType.Accent)
            result_box.Width = 0;
        if(IsTextSymbol)
            ((CharSymbol)Atom).IsTextSymbol = false;
        return result_box;
    }

    public override TexAtomType GetLeftType() => Type == TexAtomType.None ? Atom.GetLeftType() : Type;

    public override TexAtomType GetRightType() => Type == TexAtomType.None ? Atom.GetRightType() : Type;
}