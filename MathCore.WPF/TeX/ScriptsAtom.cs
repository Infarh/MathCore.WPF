using System;

namespace MathCore.WPF.TeX
{
    /// <summary>Atom representing scripts to attach to other atom</summary>
    internal class ScriptsAtom : Atom
    {
        private static readonly SpaceAtom scriptSpaceAtom = new SpaceAtom(TexUnit.Point, 0.5, 0, 0);

        public Atom BaseAtom { get; }

        public Atom SubscriptAtom { get; }

        public Atom SuperscriptAtom { get; }

        public ScriptsAtom(Atom baseAtom, Atom subscriptAtom, Atom superscriptAtom)
        {
            BaseAtom = baseAtom;
            SubscriptAtom = subscriptAtom;
            SuperscriptAtom = superscriptAtom;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            var texFont = environment.TexFont;
            var style = environment.Style;

            // Create box for base atom.
            var baseBox = (BaseAtom is null ? StrutBox.Empty : BaseAtom.CreateBox(environment));
            if(SubscriptAtom is null && SuperscriptAtom is null)
                return baseBox;

            // Create result box.
            var resultBox = new HorizontalBox(baseBox);

            // Get last font used or default Mu font.
            var lastFontId = baseBox.GetLastFontId();
            if(lastFontId == TexFontUtilities.NoFontId)
                lastFontId = texFont.GetMuFontId();

            var subscriptStyle = environment.GetSubscriptStyle();
            var superscriptStyle = environment.GetSuperscriptStyle();

            // Set delta value and preliminary shift-up and shift-down amounts depending on type of base atom.
            var delta = 0d;
            double shiftUp, shiftDown;

            if(BaseAtom is AccentedAtom)
            {
                var accentedBox = ((AccentedAtom)BaseAtom).BaseAtom.CreateBox(environment.GetCrampedStyle());
                shiftUp = accentedBox.Height - texFont.GetSupDrop(superscriptStyle.Style);
                shiftDown = accentedBox.Depth + texFont.GetSubDrop(subscriptStyle.Style);
            }
            else if(BaseAtom is SymbolAtom && BaseAtom.Type == TexAtomType.BigOperator)
            {
                var charInfo = texFont.GetCharInfo(((SymbolAtom)BaseAtom).Name, style);
                if(style < TexStyle.Text && texFont.HasNextLarger(charInfo))
                    charInfo = texFont.GetNextLargerCharInfo(charInfo, style);
                var charBox = new CharBox(environment, charInfo);

                charBox.Shift = -(charBox.Height + charBox.Depth) / 2 - environment.TexFont.GetAxisHeight(
                    environment.Style);
                resultBox = new HorizontalBox(charBox);

                delta = charInfo.Metrics.Italic;
                if(delta > TexUtilities.FloatPrecision && SubscriptAtom is null)
                    resultBox.Add(new StrutBox(delta, 0, 0, 0));

                shiftUp = resultBox.Height - texFont.GetSupDrop(superscriptStyle.Style);
                shiftDown = resultBox.Depth + texFont.GetSubDrop(subscriptStyle.Style);
            }
            else if(BaseAtom is CharSymbol)
            {
                var charFont = ((CharSymbol)BaseAtom).GetCharFont(texFont);
                if(!((CharSymbol)BaseAtom).IsTextSymbol || !texFont.HasSpace(charFont.FontId))
                    delta = texFont.GetCharInfo(charFont, style).Metrics.Italic;
                if(delta > TexUtilities.FloatPrecision && SubscriptAtom is null)
                {
                    resultBox.Add(new StrutBox(delta, 0, 0, 0));
                    delta = 0;
                }

                shiftUp = 0;
                shiftDown = 0;
            }
            else
            {
                shiftUp = baseBox.Height - texFont.GetSupDrop(superscriptStyle.Style);
                shiftDown = baseBox.Depth + texFont.GetSubDrop(subscriptStyle.Style);
            }

            Box superscriptBox = null;
            Box superscriptContainerBox = null;
            Box subscriptBox = null;
            Box subscriptContainerBox = null;

            if(SuperscriptAtom != null)
            {
                // Create box for superscript atom.
                superscriptBox = SuperscriptAtom.CreateBox(superscriptStyle);
                superscriptContainerBox = new HorizontalBox(superscriptBox);

                // Add box for script space.
                superscriptContainerBox.Add(scriptSpaceAtom.CreateBox(environment));

                // Adjust shift-up amount.
                double p;
                if(style == TexStyle.Display)
                    p = texFont.GetSup1(style);
                else if(environment.GetCrampedStyle().Style == style)
                    p = texFont.GetSup3(style);
                else
                    p = texFont.GetSup2(style);
                shiftUp = Math.Max(Math.Max(shiftUp, p), superscriptBox.Depth + Math.Abs(texFont.GetXHeight(
                    style, lastFontId)) / 4);
            }

            if(SubscriptAtom != null)
            {
                // Create box for subscript atom.
                subscriptBox = SubscriptAtom.CreateBox(subscriptStyle);
                subscriptContainerBox = new HorizontalBox(subscriptBox);

                // Add box for script space.
                subscriptContainerBox.Add(scriptSpaceAtom.CreateBox(environment));
            }

            // Check if only superscript is set.
            if(subscriptBox is null)
            {
                superscriptContainerBox.Shift = -shiftUp;
                resultBox.Add(superscriptContainerBox);
                return resultBox;
            }

            // Check if only subscript is set.
            if(superscriptBox is null)
            {
                subscriptBox.Shift = Math.Max(Math.Max(shiftDown, texFont.GetSub1(style)), subscriptBox.Height - 4 *
                                                                                           Math.Abs(texFont.GetXHeight(style, lastFontId)) / 5);
                resultBox.Add(subscriptContainerBox);
                return resultBox;
            }

            // Adjust shift-down amount.
            shiftDown = Math.Max(shiftDown, texFont.GetSub2(style));

            // Reposition both subscript and superscript.
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);
            // Space between subscript and superscript.
            var scriptsInterSpace = shiftUp - superscriptBox.Depth + shiftDown - subscriptBox.Height;
            if(scriptsInterSpace < 4 * defaultLineThickness)
            {
                shiftUp += 4 * defaultLineThickness - scriptsInterSpace;

                // Position bottom of superscript at least 4/5 of X-height above baseline.
                var psi = 0.8 * Math.Abs(texFont.GetXHeight(style, lastFontId)) - (shiftUp - superscriptBox.Depth);
                if(psi > 0)
                {
                    shiftUp += psi;
                    shiftDown -= psi;
                }
            }
            scriptsInterSpace = shiftUp - superscriptBox.Depth + shiftDown - subscriptBox.Height;

            // Create box containing both superscript and subscript.
            var scriptsBox = new VerticalBox();
            superscriptContainerBox.Shift = delta;
            scriptsBox.Add(superscriptContainerBox);
            scriptsBox.Add(new StrutBox(0, scriptsInterSpace, 0, 0));
            scriptsBox.Add(subscriptContainerBox);
            scriptsBox.Height = shiftUp + superscriptBox.Height;
            scriptsBox.Depth = shiftDown + subscriptBox.Depth;
            resultBox.Add(scriptsBox);

            return resultBox;
        }

        public override TexAtomType GetLeftType() => BaseAtom.GetLeftType();

        public override TexAtomType GetRightType() => BaseAtom.GetRightType();
    }
}