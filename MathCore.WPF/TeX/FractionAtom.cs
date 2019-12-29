namespace MathCore.WPF.TeX
{
    /// <summary>Atom representing fraction, with or without separation line</summary>
    internal sealed class FractionAtom : Atom
    {
        private static TexAlignment CheckAlignment(TexAlignment alignment)
        {
            if(alignment == TexAlignment.Left || alignment == TexAlignment.Right)
                return alignment;
            return TexAlignment.Center;
        }

        private readonly TexAlignment numeratorAlignment;
        private readonly TexAlignment denominatorAlignment;

        private readonly double lineThickness;
        private readonly TexUnit lineThicknessUnit;

        private readonly bool useDefaultThickness;
        private double? lineRelativeThickness = null;

        public Atom Numerator { get; }

        public Atom Denominator { get; }

        /*
                public FractionAtom(Atom numerator, Atom denominator, double relativeThickness,
                    TexAlignment numeratorAlignment, TexAlignment denominatorAlignment)
                    : this(numerator, denominator, true, numeratorAlignment, denominatorAlignment)
                {
                    lineRelativeThickness = relativeThickness;
                }
        */

        public FractionAtom(Atom numerator, Atom denominator, bool drawLine,
            TexAlignment numeratorAlignment, TexAlignment denominatorAlignment)
            : this(numerator, denominator, drawLine)
        {
            this.numeratorAlignment = CheckAlignment(numeratorAlignment);
            this.denominatorAlignment = CheckAlignment(denominatorAlignment);
        }

        public FractionAtom(Atom numerator, Atom denominator, bool drawLine)
            : this(numerator, denominator, drawLine, TexUnit.Pixel, 0d)
        {
        }

        public FractionAtom(Atom numerator, Atom denominator, TexUnit unit, double thickness,
            TexAlignment numeratorAlignment, TexAlignment denominatorAlignment)
            : this(numerator, denominator, unit, thickness)
        {
            this.numeratorAlignment = CheckAlignment(numeratorAlignment);
            this.denominatorAlignment = CheckAlignment(denominatorAlignment);
        }

        public FractionAtom(Atom numerator, Atom denominator, TexUnit unit, double thickness)
            : this(numerator, denominator, false, unit, thickness)
        {
        }

        private FractionAtom(Atom numerator, Atom denominator, bool useDefaultThickness, TexUnit unit, double thickness)
        {
            SpaceAtom.CheckUnit(unit);

            Type = TexAtomType.Inner;
            Numerator = numerator;
            Denominator = denominator;
            numeratorAlignment = TexAlignment.Center;
            denominatorAlignment = TexAlignment.Center;
            this.useDefaultThickness = useDefaultThickness;
            lineThicknessUnit = unit;
            lineThickness = thickness;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            var texFont = environment.TexFont;
            var style = environment.Style;

            // set thickness to default if default value should be used
            double lineHeight;
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);
            if(useDefaultThickness)
                lineHeight = lineRelativeThickness * defaultLineThickness ?? defaultLineThickness;
            else
                lineHeight = new SpaceAtom(lineThicknessUnit, 0, lineThickness, 0).CreateBox(environment).Height;

            // Create boxes for numerator and demoninator atoms, and make them of equal width.
            var numeratorBox = Numerator == null ? StrutBox.Empty :
                Numerator.CreateBox(environment.GetNumeratorStyle());
            var denominatorBox = Denominator == null ? StrutBox.Empty :
                Denominator.CreateBox(environment.GetDenominatorStyle());

            if(numeratorBox.Width < denominatorBox.Width)
                numeratorBox = new HorizontalBox(numeratorBox, denominatorBox.Width, numeratorAlignment);
            else
                denominatorBox = new HorizontalBox(denominatorBox, numeratorBox.Width, denominatorAlignment);

            // Calculate preliminary shift-up and shift-down amounts.
            double shiftUp, shiftDown;
            if(style < TexStyle.Text)
            {
                shiftUp = texFont.GetNum1(style);
                shiftDown = texFont.GetDenom1(style);
            }
            else
            {
                shiftDown = texFont.GetDenom2(style);
                shiftUp = lineHeight > 0 ? texFont.GetNum2(style) : texFont.GetNum3(style);
            }

            // Create result box.
            var resultBox = new VerticalBox();

            // add box for numerator.
            resultBox.Add(numeratorBox);

            // Calculate clearance and adjust shift amounts.
            var axis = texFont.GetAxisHeight(style);

            if(lineHeight > 0)
            {
                // Draw fraction line.

                // Calculate clearance amount.
                double clearance;
                if(style < TexStyle.Text)
                    clearance = 3 * lineHeight;
                else
                    clearance = lineHeight;

                // Adjust shift amounts.
                var delta = lineHeight / 2;
                var kern1 = shiftUp - numeratorBox.Depth - (axis + delta);
                var kern2 = axis - delta - (denominatorBox.Height - shiftDown);
                var delta1 = clearance - kern1;
                var delta2 = clearance - kern2;
                if(delta1 > 0)
                {
                    shiftUp += delta1;
                    kern1 += delta1;
                }
                if(delta2 > 0)
                {
                    shiftDown += delta2;
                    kern2 += delta2;
                }

                resultBox.Add(new StrutBox(0, kern1, 0, 0));
                resultBox.Add(new HorizontalRule(lineHeight, numeratorBox.Width, 0));
                resultBox.Add(new StrutBox(0, kern2, 0, 0));
            }
            else
            {
                // Do not draw fraction line.

                // Calculate clearance amount.
                double clearance;
                if(style < TexStyle.Text)
                    clearance = 7 * defaultLineThickness;
                else
                    clearance = 3 * defaultLineThickness;

                // Adjust shift amounts.
                var kern = shiftUp - numeratorBox.Depth - (denominatorBox.Height - shiftDown);
                var delta = (clearance - kern) / 2;
                if(delta > 0)
                {
                    shiftUp += delta;
                    shiftDown += delta;
                    kern += 2 * delta;
                }

                resultBox.Add(new StrutBox(0, kern, 0, 0));
            }

            // add box for denominator.
            resultBox.Add(denominatorBox);

            // Adjust height and depth of result box.
            resultBox.Height = shiftUp + numeratorBox.Height;
            resultBox.Depth = shiftDown + denominatorBox.Depth;

            return resultBox;
        }
    }
}
