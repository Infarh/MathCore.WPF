using System;

namespace MathCore.WPF.TeX
{
    /// <summary>Atom representing radical (nth-root) construction</summary>
    internal class Radical : Atom
    {
        private const string sqrtSymbol = "sqrt";

        private const double scale = 0.55;

        public Atom BaseAtom { get; }

        public Atom DegreeAtom { get; }

        public Radical(Atom baseAtom, Atom degreeAtom = null)
        {
            BaseAtom = baseAtom;
            DegreeAtom = degreeAtom;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            var texFont = environment.TexFont;
            var style = environment.Style;

            // Calculate minimum clearance amount.
            var defaultRuleThickness = texFont.GetDefaultLineThickness(style);
            var clearance = style < TexStyle.Text ? texFont.GetXHeight(style, texFont.GetCharInfo(sqrtSymbol, style).FontId) : defaultRuleThickness;
            clearance = defaultRuleThickness + Math.Abs(clearance) / 4;

            // Create box for base atom, in cramped style.
            var baseBox = BaseAtom.CreateBox(environment.GetCrampedStyle());

            // Create box for radical sign.
            var totalHeight = baseBox.Height + baseBox.Depth;
            var radicalSignBox = DelimiterFactory.CreateBox(sqrtSymbol, totalHeight + clearance + defaultRuleThickness,
                environment);

            // Add half of excess height to clearance.
            var delta = radicalSignBox.Depth - (totalHeight + clearance);
            clearance += delta / 2;

            // Create box for square-root containing base box.
            radicalSignBox.Shift = -(baseBox.Height + clearance);
            var overBar = new OverBar(baseBox, clearance, radicalSignBox.Height);
            overBar.Shift = -(baseBox.Height + clearance + defaultRuleThickness);
            var radicalContainerBox = new HorizontalBox(radicalSignBox);
            radicalContainerBox.Add(overBar);

            // If atom is simple radical, just return square-root box.
            if(DegreeAtom == null)
                return radicalContainerBox;

            // Atom is complex radical (nth-root).

            // Create box for root atom.
            var rootBox = DegreeAtom.CreateBox(environment.GetRootStyle());
            var bottomShift = scale * (radicalContainerBox.Height + radicalContainerBox.Depth);
            rootBox.Shift = radicalContainerBox.Depth - rootBox.Depth - bottomShift;

            // Create result box.
            var resultBox = new HorizontalBox();

            // Add box for negative kern.
            var negativeKern = new SpaceAtom(TexUnit.Mu, -10, 0, 0).CreateBox(environment);
            var xPos = rootBox.Width + negativeKern.Width;
            if(xPos < 0)
                resultBox.Add(new StrutBox(-xPos, 0, 0, 0));

            resultBox.Add(rootBox);
            resultBox.Add(negativeKern);
            resultBox.Add(radicalContainerBox);

            return resultBox;
        }
    }
}
