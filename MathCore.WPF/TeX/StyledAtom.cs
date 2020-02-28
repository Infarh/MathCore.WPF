using System.Windows.Media;

namespace MathCore.WPF.TeX
{
    /// <summary>Atom specifying graphical style</summary>
    internal sealed class StyledAtom : Atom, IRow
    {
        public DummyAtom PreviousAtom
        {
            get { return RowAtom.PreviousAtom; }
            set { RowAtom.PreviousAtom = value; }
        }

        /// <summary>RowAtom to which colors are applied</summary>
        public RowAtom RowAtom { get; }

        public Brush Background { get; set; }

        public Brush Foreground { get; set; }

        public StyledAtom(Atom atom, Brush backgroundColor, Brush foregroundColor)
        {
            RowAtom = new RowAtom(atom);
            Background = backgroundColor;
            Foreground = foregroundColor;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            var newEnvironment = environment.Clone();
            if(Background != null)
                newEnvironment.Background = Background;
            if(Foreground != null)
                newEnvironment.Foreground = Foreground;
            return RowAtom.CreateBox(newEnvironment);
        }

        public override TexAtomType GetLeftType() => RowAtom.GetLeftType();

        public override TexAtomType GetRightType() => RowAtom.GetRightType();

        public StyledAtom Clone() => new StyledAtom(RowAtom, Background, Foreground);
    }
}