namespace MathCore.WPF.TeX;

/// <summary>Atom representing other atom with custom left and right types</summary>
internal sealed class TypedAtom : Atom
{
    public Atom Atom { get; }

    public TexAtomType LeftType { get; }

    public TexAtomType RightType { get; }

    public TypedAtom(Atom atom, TexAtomType LeftType, TexAtomType RightType)
    {
        Atom           = atom;
        this.LeftType  = LeftType;
        this.RightType = RightType;
    }

    public override Box CreateBox(TexEnvironment environment) => Atom.CreateBox(environment);

    public override TexAtomType GetLeftType() => LeftType;

    public override TexAtomType GetRightType() => RightType;
}