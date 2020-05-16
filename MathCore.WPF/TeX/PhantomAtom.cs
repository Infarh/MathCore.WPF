namespace MathCore.WPF.TeX
{
    /// <summary>Atom representing other atom that is not rendered</summary>
    internal class PhantomAtom : Atom, IRow
    {
        private readonly bool useWidth;
        private readonly bool useHeight;
        private readonly bool useDepth;

        public DummyAtom PreviousAtom { get { return RowAtom.PreviousAtom; } set { RowAtom.PreviousAtom = value; } }

        public RowAtom RowAtom { get; }

        public PhantomAtom(Atom baseAtom, bool useWidth = true, bool useHeight = true, bool useDepth = true)
        {
            RowAtom = baseAtom is null ? new RowAtom() : new RowAtom(baseAtom);
            this.useWidth = useWidth;
            this.useHeight = useHeight;
            this.useDepth = useDepth;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            var resultBox = RowAtom.CreateBox(environment);
            return new StrutBox((useWidth ? resultBox.Width : 0), (useHeight ? resultBox.Height : 0),
                (useDepth ? resultBox.Depth : 0), resultBox.Shift);
        }

        public override TexAtomType GetLeftType() => RowAtom.GetLeftType();

        public override TexAtomType GetRightType() => RowAtom.GetRightType();
    }
}