namespace MathCore.WPF.TeX
{
    /// <summary>Atom representing other atom vertically centered with respect to axis</summary>
    internal class VerticalCenteredAtom : Atom
    {
        public Atom Atom { get; }

        public VerticalCenteredAtom(Atom atom) => Atom = atom;

        public override Box CreateBox(TexEnvironment environment)
        {
            var box = Atom.CreateBox(environment);

            // Centre box relative to horizontal axis.
            var total_height = box.Height + box.Depth;
            var axis = environment.TexFont.GetAxisHeight(environment.Style);
            box.Shift = -(total_height / 2) - axis;

            return new HorizontalBox(box);
        }
    }
}