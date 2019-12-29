namespace MathCore.WPF.TeX
{
    /// <summary>Atom representing other atom that is underlined</summary>
    internal class UnderlinedAtom : Atom
    {
        public Atom BaseAtom { get; }

        public UnderlinedAtom(Atom baseAtom)
        {
            Type = TexAtomType.Ordinary;
            BaseAtom = baseAtom;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            var defaultLineThickness = environment.TexFont.GetDefaultLineThickness(environment.Style);

            // Create box for base atom.
            var baseBox = BaseAtom == null ? StrutBox.Empty : BaseAtom.CreateBox(environment);

            // Create result box.
            var resultBox = new VerticalBox();
            resultBox.Add(baseBox);
            resultBox.Add(new StrutBox(0, 3 * defaultLineThickness, 0, 0));
            resultBox.Add(new HorizontalRule(defaultLineThickness, baseBox.Width, 0));

            resultBox.Depth = baseBox.Depth + 5 * defaultLineThickness;
            resultBox.Height = baseBox.Height;

            return resultBox;
        }
    }
}
