namespace MathCore.WPF.TeX
{
    /// <summary>Atom representing other atom with horizontal rule above it</summary>
    internal class OverlinedAtom : Atom
    {
        public Atom BaseAtom { get; }

        public OverlinedAtom(Atom baseAtom)
        {
            Type = TexAtomType.Ordinary;
            BaseAtom = baseAtom;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            // Create box for base atom, in cramped style.
            var baseBox = BaseAtom is null ? StrutBox.Empty : BaseAtom.CreateBox(environment.GetCrampedStyle());

            // Create result box.
            var defaultLineThickness = environment.TexFont.GetDefaultLineThickness(environment.Style);
            var resultBox = new OverBar(baseBox, 3 * defaultLineThickness, defaultLineThickness);

            // Adjust height and depth of result box.
            resultBox.Height = baseBox.Height + 5 * defaultLineThickness;
            resultBox.Depth = baseBox.Depth;

            return resultBox;
        }
    }
}