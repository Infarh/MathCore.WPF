using System;

namespace MathCore.WPF.TeX
{
    /// <summary>Atom representing other atom with atoms optionally over and under it</summary>
    internal class UnderOverAtom : Atom
    {
        private static Box ChangeWidth(Box box, double maxWidth)
        {
            if(box != null && Math.Abs(maxWidth - box.Width) > TexUtilities.FloatPrecision)
                return new HorizontalBox(box, maxWidth, TexAlignment.Center);
            return box;
        }

        public Atom BaseAtom { get; }

        public Atom UnderAtom { get; }

        public Atom OverAtom { get; }

        // Kern between base and under atom.
        public double UnderSpace { get; set; }

        // Kern between base and over atom.
        public double OverSpace { get; set; }

        public TexUnit UnderSpaceUnit { get; set; }
        public TexUnit OverSpaceUnit { get; set; }

        public bool UnderScriptSmaller { get; set; }

        public bool OverScriptSmaller { get; set; }

        public UnderOverAtom(Atom baseAtom, Atom underOver, TexUnit underOverUnit, double underOverSpace,
            bool underOverScriptSize, bool over)
        {
            SpaceAtom.CheckUnit(underOverUnit);

            BaseAtom = baseAtom;

            if(over)
            {
                UnderAtom = null;
                UnderSpace = 0;
                UnderSpaceUnit = 0;
                UnderScriptSmaller = false;
                OverAtom = underOver;
                OverSpaceUnit = underOverUnit;
                OverSpace = underOverSpace;
                OverScriptSmaller = underOverScriptSize;
            }
            else
            {
                UnderAtom = underOver;
                UnderSpaceUnit = underOverUnit;
                UnderSpace = underOverSpace;
                UnderScriptSmaller = underOverScriptSize;
                OverSpace = 0;
                OverAtom = null;
                OverSpaceUnit = 0;
                OverScriptSmaller = false;
            }
        }

        public UnderOverAtom(Atom baseAtom, Atom under, TexUnit underUnit, double underSpace, bool underScriptSize,
            Atom over, TexUnit overUnit, double overSpace, bool overScriptSize)
        {
            SpaceAtom.CheckUnit(underUnit);
            SpaceAtom.CheckUnit(overUnit);

            BaseAtom = baseAtom;
            UnderAtom = under;
            UnderSpaceUnit = underUnit;
            UnderSpace = underSpace;
            UnderScriptSmaller = underScriptSize;
            OverAtom = over;
            OverSpaceUnit = overUnit;
            OverSpace = overSpace;
            OverScriptSmaller = overScriptSize;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            // Create box for base atom.
            var baseBox = BaseAtom is null ? StrutBox.Empty : BaseAtom.CreateBox(environment);

            // Create boxes for over and under atoms.
            Box overBox = null, underBox = null;
            var maxWidth = baseBox.Width;

            if(OverAtom != null)
            {
                overBox = OverAtom.CreateBox(OverScriptSmaller ? environment.GetSubscriptStyle() : environment);
                maxWidth = Math.Max(maxWidth, overBox.Width);
            }

            if(UnderAtom != null)
            {
                underBox = UnderAtom.CreateBox(UnderScriptSmaller ? environment.GetSubscriptStyle() : environment);
                maxWidth = Math.Max(maxWidth, underBox.Width);
            }

            // Create result box.
            var resultBox = new VerticalBox();

            environment.LastFontId = baseBox.GetLastFontId();

            // Create and add box for over atom.
            if(OverAtom != null)
            {
                resultBox.Add(ChangeWidth(overBox, maxWidth));
                resultBox.Add(new SpaceAtom(OverSpaceUnit, 0, OverSpace, 0).CreateBox(environment));
            }

            // Add box for base atom.
            resultBox.Add(ChangeWidth(baseBox, maxWidth));

            var totalHeight = resultBox.Height + resultBox.Depth - baseBox.Depth;

            // Create and add box for under atom.
            if(UnderAtom != null)
            {
                resultBox.Add(new SpaceAtom(OverSpaceUnit, 0, UnderSpace, 0).CreateBox(environment));
                resultBox.Add(ChangeWidth(underBox, maxWidth));
            }

            resultBox.Depth = resultBox.Height + resultBox.Depth - totalHeight;
            resultBox.Height = totalHeight;

            return resultBox;
        }
    }
}