using System;
using System.Diagnostics;

namespace MathCore.WPF.TeX
{
    /// <summary>Atom representing big operator with optional limits</summary>
    internal class BigOperatorAtom : Atom
    {
        /// <summary>Centre specified box in new box of specified width, if necessary</summary>
        /// <param name="box"></param>
        /// <param name="maxWidth"></param>
        /// <returns></returns>
        private static Box ChangeWidth(Box box, double maxWidth)
        {
            return Math.Abs(maxWidth - box.Width) > TexUtilities.FloatPrecision
                ? new HorizontalBox(box, maxWidth, TexAlignment.Center)
                : box;
        }

        /// <summary>Atom representing big operator</summary>
        public Atom BaseAtom { get; }

        /// <summary>Atoms representing lower and upper limits</summary>
        public Atom LowerLimitAtom { get; }

        public Atom UpperLimitAtom { get; }

        /// <summary>True if limits should be drawn over and under the base atom; false if they should be drawn as scripts</summary>
        public bool? UseVerticalLimits { get; }

        public BigOperatorAtom(Atom baseAtom, Atom lowerLimitAtom, Atom upperLimitAtom, bool? useVerticalLimits = null)
            : this(baseAtom, lowerLimitAtom, upperLimitAtom)
        {
            UseVerticalLimits = useVerticalLimits;
        }

        public BigOperatorAtom(Atom baseAtom, Atom lowerLimitAtom, Atom upperLimitAtom)
        {
            Type = TexAtomType.BigOperator;
            BaseAtom = baseAtom;
            LowerLimitAtom = lowerLimitAtom;
            UpperLimitAtom = upperLimitAtom;
            UseVerticalLimits = null;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            var texFont = environment.TexFont;
            var style = environment.Style;

            if((UseVerticalLimits.HasValue && !UseVerticalLimits.Value) ||
                (!UseVerticalLimits.HasValue && style >= TexStyle.Text))
                // Attach atoms for limits as scripts.
                return new ScriptsAtom(BaseAtom, LowerLimitAtom, UpperLimitAtom).CreateBox(environment);

            // Create box for base atom.
            Box baseBox;
            double delta;

            if((BaseAtom as SymbolAtom)?.Type == TexAtomType.BigOperator)
            {
                // Find character of best scale for operator symbol.
                var opChar = texFont.GetCharInfo(((SymbolAtom)BaseAtom).Name, style);
                if(style < TexStyle.Text && texFont.HasNextLarger(opChar))
                    opChar = texFont.GetNextLargerCharInfo(opChar, style);
                var charBox = new CharBox(environment, opChar);
                charBox.Shift = -(charBox.Height + charBox.Depth) / 2 -
                                environment.TexFont.GetAxisHeight(environment.Style);
                baseBox = new HorizontalBox(charBox);

                delta = opChar.Metrics.Italic;
                if(delta > TexUtilities.FloatPrecision)
                    baseBox.Add(new StrutBox(delta, 0, 0, 0));
            }
            else
            {
                baseBox = new HorizontalBox(BaseAtom?.CreateBox(environment) ?? StrutBox.Empty);
                delta = 0;
            }

            // Create boxes for upper and lower limits.
            var upperLimitBox = UpperLimitAtom?.CreateBox(environment.GetSuperscriptStyle());
            var lowerLimitBox = LowerLimitAtom?.CreateBox(environment.GetSubscriptStyle());

            // Make all component boxes equally wide.
            var maxWidth = Math.Max(Math.Max(baseBox.Width, upperLimitBox?.Width ?? 0),
                lowerLimitBox?.Width ?? 0);
            baseBox = ChangeWidth(baseBox, maxWidth);
            if(upperLimitBox != null)
                upperLimitBox = ChangeWidth(upperLimitBox, maxWidth);
            if(lowerLimitBox != null)
                lowerLimitBox = ChangeWidth(lowerLimitBox, maxWidth);

            var resultBox = new VerticalBox();
            var opSpacing5 = texFont.GetBigOpSpacing5(style);
            var kern = 0d;

            // Create and add box for upper limit.
            if(UpperLimitAtom != null)
            {
                resultBox.Add(new StrutBox(0, opSpacing5, 0, 0));
                Debug.Assert(upperLimitBox != null, "upperLimitBox != null");
                upperLimitBox.Shift = delta / 2;
                resultBox.Add(upperLimitBox);
                kern = Math.Max(texFont.GetBigOpSpacing1(style), texFont.GetBigOpSpacing3(style) -
                                                                 upperLimitBox.Depth);
                resultBox.Add(new StrutBox(0, kern, 0, 0));
            }

            // Add box for base atom.
            resultBox.Add(baseBox);

            // Create and add box for lower limit.
            if(LowerLimitAtom != null)
            {
                Debug.Assert(lowerLimitBox != null, "lowerLimitBox != null");
                resultBox.Add(new StrutBox(0, Math.Max(texFont.GetBigOpSpacing2(style), texFont.GetBigOpSpacing4(style) -
                                                                                        lowerLimitBox.Height), 0, 0));
                lowerLimitBox.Shift = -delta / 2;
                resultBox.Add(lowerLimitBox);
                resultBox.Add(new StrutBox(0, opSpacing5, 0, 0));
            }

            // Adjust height and depth of result box.
            var baseBoxHeight = baseBox.Height;
            var totalHeight = resultBox.Height + resultBox.Depth;
            if(upperLimitBox != null)
                baseBoxHeight += opSpacing5 + kern + upperLimitBox.Height + upperLimitBox.Depth;
            resultBox.Height = baseBoxHeight;
            resultBox.Depth = totalHeight - baseBoxHeight;

            return resultBox;
        }
    }
}
