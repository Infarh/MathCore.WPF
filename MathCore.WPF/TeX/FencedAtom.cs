using System;

namespace MathCore.WPF.TeX
{
    /// <summary>Atom representing base atom surrounded by delimeters</summary>
    internal class FencedAtom : Atom
    {
        private const int delimeterFactor = 901;
        private const double delimeterShortfall = 0.5;

        public Atom BaseAtom { get; }

        private SymbolAtom LeftDelimeter { get; }

        private SymbolAtom RightDelimeter { get; }

        private static void CentreBox(Box box, double axis)
        {
            var totalHeight = box.Height + box.Depth;
            box.Shift = -(totalHeight / 2 - box.Height) - axis;
        }

        public FencedAtom(Atom baseAtom, SymbolAtom leftDelimeter, SymbolAtom rightDelimeter)
        {
            BaseAtom = baseAtom ?? new RowAtom();
            LeftDelimeter = leftDelimeter;
            RightDelimeter = rightDelimeter;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            var texFont = environment.TexFont;
            var style = environment.Style;

            // Create box for base atom.
            var baseBox = BaseAtom.CreateBox(environment);

            // Create result box.
            var resultBox = new HorizontalBox();
            var axis = texFont.GetAxisHeight(style);
            var delta = Math.Max(baseBox.Height - axis, baseBox.Depth + axis);
            var minHeight = Math.Max((delta / 500) * delimeterFactor, 2 * delta - delimeterShortfall);

            // Create and add box for left delimeter.
            if(LeftDelimeter != null)
            {
                var leftDelimeterBox = DelimiterFactory.CreateBox(LeftDelimeter.Name, minHeight, environment);
                CentreBox(leftDelimeterBox, axis);
                resultBox.Add(leftDelimeterBox);
            }

            // add glueElement between left delimeter and base Atom, unless base Atom is whitespace.
            if(!(BaseAtom is SpaceAtom))
                resultBox.Add(Glue.CreateBox(TexAtomType.Opening, BaseAtom.GetLeftType(), environment));

            // add box for base Atom.
            resultBox.Add(baseBox);

            // add glueElement between right delimeter and base Atom, unless base Atom is whitespace.
            if(!(BaseAtom is SpaceAtom))
                resultBox.Add(Glue.CreateBox(BaseAtom.GetRightType(), TexAtomType.Closing, environment));

            // Create and add box for right delimeter.
            if(RightDelimeter == null) return resultBox;

            var rightDelimeterBox = DelimiterFactory.CreateBox(RightDelimeter.Name, minHeight, environment);
            CentreBox(rightDelimeterBox, axis);
            resultBox.Add(rightDelimeterBox);

            return resultBox;
        }

        public override TexAtomType GetLeftType() => TexAtomType.Opening;

        public override TexAtomType GetRightType() => TexAtomType.Closing;
    }
}
