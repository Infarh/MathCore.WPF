using System;

// Atom representing other atom with delimeter and script atoms over or under it.
namespace MathCore.WPF.TeX
{
    internal class OverUnderDelimiter : Atom
    {
        private static double GetMaxWidth(Box baseBox, Box delimeterBox, Box scriptBox)
        {
            var maxWidth = Math.Max(baseBox.Width, delimeterBox.Height + delimeterBox.Depth);
            if(scriptBox != null)
                maxWidth = Math.Max(maxWidth, scriptBox.Width);
            return maxWidth;
        }

        public Atom BaseAtom { get; }

        private Atom Script { get; }

        private SymbolAtom Symbol { get; }

        /// <summary>Kern between delimeter symbol and script</summary>
        private SpaceAtom Kern { get; }

        /// <summary> True to place delimeter symbol Over base; false to place delimeter symbol under base</summary>
        public bool Over { get; set; }

        public OverUnderDelimiter(Atom baseAtom, Atom script, SymbolAtom symbol, TexUnit kernUnit, double kern, bool over)
        {
            Type = TexAtomType.Inner;
            BaseAtom = baseAtom;
            Script = script;
            Symbol = symbol;
            Kern = new SpaceAtom(kernUnit, 0, kern, 0);
            Over = over;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            // Create boxes for base, delimeter, and script atoms.
            var baseBox = BaseAtom == null ? StrutBox.Empty : BaseAtom.CreateBox(environment);
            var delimeterBox = DelimiterFactory.CreateBox(Symbol.Name, baseBox.Width, environment);
            var scriptBox = Script?.CreateBox(Over ? environment.GetSuperscriptStyle() : environment.GetSubscriptStyle());

            // Create centered horizontal box if any box is smaller than maximum width.
            var maxWidth = GetMaxWidth(baseBox, delimeterBox, scriptBox);
            if(Math.Abs(maxWidth - baseBox.Width) > TexUtilities.FloatPrecision)
                baseBox = new HorizontalBox(baseBox, maxWidth, TexAlignment.Center);
            if(Math.Abs(maxWidth - delimeterBox.Height - delimeterBox.Depth) > TexUtilities.FloatPrecision)
                delimeterBox = new VerticalBox(delimeterBox, maxWidth, TexAlignment.Center);
            if(scriptBox != null && Math.Abs(maxWidth - scriptBox.Width) > TexUtilities.FloatPrecision)
                scriptBox = new HorizontalBox(scriptBox, maxWidth, TexAlignment.Center);

            return new OverUnderBox(baseBox, delimeterBox, scriptBox, Kern.CreateBox(environment).Height, Over);
        }
    }
}
