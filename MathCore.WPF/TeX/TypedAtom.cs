namespace MathCore.WPF.TeX
{
    /// <summary>Atom representing other atom with custom left and right types</summary>
    internal sealed class TypedAtom : Atom
    {
        public Atom Atom { get; }

        public TexAtomType LeftType { get; }

        public TexAtomType RightType { get; }

        public TypedAtom(Atom atom, TexAtomType leftType, TexAtomType rightType)
        {
            Atom = atom;
            LeftType = leftType;
            RightType = rightType;
        }

        public override Box CreateBox(TexEnvironment environment) => Atom.CreateBox(environment);

        public override TexAtomType GetLeftType() => LeftType;

        public override TexAtomType GetRightType() => RightType;
    }
}
