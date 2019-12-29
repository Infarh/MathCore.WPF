namespace MathCore.WPF.TeX
{
    internal sealed class TexFormulaHelper
    {
        public TexFormulaParser FormulaParser;

        public TexFormula Formula { get; }

        public TexFormulaHelper(TexFormula formula)
        {
            FormulaParser = new TexFormulaParser();
            Formula = formula;
        }

        public void SetFixedTypes(TexAtomType leftType, TexAtomType rightType) => Formula.RootAtom = new TypedAtom(Formula.RootAtom, leftType, rightType);

        public void CenterOnAxis() => Formula.RootAtom = new VerticalCenteredAtom(Formula.RootAtom);

        public void AddAccent(string formula, string accentName) => AddAccent(FormulaParser.Parse(formula), accentName);

        public void AddAccent(TexFormula baseAtom, string accentName) => Add(new AccentedAtom(baseAtom?.RootAtom, accentName));

        public void AddAccent(TexFormula baseAtom, TexFormula accent) => Add(new AccentedAtom(baseAtom?.RootAtom, accent));

        public void AddEmbraced(string formula, char leftChar, char rightChar) =>
            AddEmbraced(FormulaParser.Parse(formula), leftChar, rightChar);

        public void AddEmbraced(TexFormula formula, char leftChar, char rightChar) =>
            AddEmbraced(formula, TexFormulaParser.GetDelimeterMapping(leftChar), TexFormulaParser.GetDelimeterMapping(rightChar));

        public void AddEmbraced(string formula, string leftSymbol, string rightSymbol) => AddEmbraced(FormulaParser.Parse(formula), leftSymbol, rightSymbol);

        public void AddEmbraced(TexFormula formula, string leftSymbol, string rightSymbol) => Add(new FencedAtom(formula?.RootAtom, TexFormulaParser.GetDelimiterSymbol(leftSymbol),
            TexFormulaParser.GetDelimiterSymbol(rightSymbol)));

        public void AddFraction(string numerator, string denominator, bool drawLine) => AddFraction(FormulaParser.Parse(numerator), FormulaParser.Parse(denominator), drawLine);

        public void AddFraction(string numerator, TexFormula denominator, bool drawLine) => AddFraction(FormulaParser.Parse(numerator), denominator, drawLine);

        public void AddFraction(string numerator, string denominator, bool drawLine, TexAlignment numeratorAlignment,
            TexAlignment denominatorAlignment) => AddFraction(FormulaParser.Parse(numerator), FormulaParser.Parse(denominator), drawLine, numeratorAlignment,
                denominatorAlignment);

        public void AddFraction(TexFormula numerator, string denominator, bool drawLine) => AddFraction(numerator, FormulaParser.Parse(denominator), drawLine);

        public void AddFraction(TexFormula numerator, TexFormula denominator, bool drawLine) => Add(new FractionAtom(numerator?.RootAtom,
            denominator?.RootAtom, drawLine));

        public void AddFraction(TexFormula numerator, TexFormula denominator, bool drawLine,
            TexAlignment numeratorAlignment, TexAlignment denominatorAlignment) => Add(new FractionAtom(numerator?.RootAtom,
                denominator?.RootAtom, drawLine, numeratorAlignment, denominatorAlignment));

        public void AddRadical(string baseFormula, string nthRoot) => AddRadical(FormulaParser.Parse(baseFormula), FormulaParser.Parse(nthRoot));

        public void AddRadical(string baseFormula, TexFormula nthRoot) => AddRadical(FormulaParser.Parse(baseFormula), nthRoot);

        public void AddRadical(string baseFormula) => AddRadical(FormulaParser.Parse(baseFormula));

        public void AddRadical(TexFormula baseFormula, string degreeFormula) => AddRadical(baseFormula, FormulaParser.Parse(degreeFormula));

        public void AddRadical(TexFormula baseFormula) => AddRadical(baseFormula, (TexFormula)null);

        public void AddRadical(TexFormula baseFormula, TexFormula degreeFormula) => Add(new Radical(baseFormula?.RootAtom,
            degreeFormula?.RootAtom));

        public void AddOperator(string operatorFormula, string lowerLimitFormula, string upperLimitFormula) => AddOperator(FormulaParser.Parse(operatorFormula), FormulaParser.Parse(lowerLimitFormula),
            FormulaParser.Parse(upperLimitFormula));

        public void AddOperator(string operatorFormula, string lowerLimitFormula, string upperLimitFormula,
            bool useVerticalLimits) => AddOperator(FormulaParser.Parse(operatorFormula), FormulaParser.Parse(lowerLimitFormula),
                FormulaParser.Parse(upperLimitFormula), useVerticalLimits);

        public void AddOperator(TexFormula operatorFormula, TexFormula lowerLimitFormula, TexFormula upperLimitFormula) => Add(new BigOperatorAtom(operatorFormula?.RootAtom,
            lowerLimitFormula?.RootAtom,
            upperLimitFormula?.RootAtom));

        public void AddOperator(TexFormula operatorFormula, TexFormula lowerLimitFormula, TexFormula upperLimitFormula,
            bool useVerticalLimits) => Add(new BigOperatorAtom(operatorFormula?.RootAtom,
                lowerLimitFormula?.RootAtom,
                upperLimitFormula?.RootAtom, useVerticalLimits));

        public void AddPhantom(string formula) => AddPhantom(FormulaParser.Parse(formula));

        public void AddPhantom(string formula, bool useWidth, bool useHeight, bool useDepth) => AddPhantom(FormulaParser.Parse(formula), useWidth, useHeight, useDepth);

        public void AddPhantom(TexFormula formula) => Add(new PhantomAtom(formula?.RootAtom));

        public void AddPhantom(TexFormula phantom, bool useWidth, bool useHeight, bool useDepth) => Add(new PhantomAtom(phantom?.RootAtom, useWidth, useHeight, useDepth));

        public void AddStrut(TexUnit unit, double width, double height, double depth) => Add(new SpaceAtom(unit, width, height, depth));

        public void AddStrut(TexUnit widthUnit, double width, TexUnit heightUnit, double height, TexUnit depthUnit,
            double depth) => Add(new SpaceAtom(widthUnit, width, heightUnit, height, depthUnit, depth));

        public void AddSymbol(string name) => Add(SymbolAtom.GetAtom(name));

        public void AddSymbol(string name, TexAtomType type) => Add(new SymbolAtom(SymbolAtom.GetAtom(name), type));

        public void Add(string formula) => Add(FormulaParser.Parse(formula));

        public void Add(TexFormula formula) => Formula.Add(formula);

        public void Add(Atom atom) => Formula.Add(atom);

        public void PutAccentOver(string accentName) => Formula.RootAtom = new AccentedAtom(Formula.RootAtom, accentName);

        public void PutDelimiterOver(TexDelimeter delimiter)
        {
            var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Over];
            Formula.RootAtom = new OverUnderDelimiter(Formula.RootAtom, null, SymbolAtom.GetAtom(name),
                TexUnit.Ex, 0, true);
        }

        public void PutDelimiterOver(TexDelimeter delimiter, string superscriptFormula, TexUnit kernUnit, double kern) => PutDelimiterOver(delimiter, FormulaParser.Parse(superscriptFormula), kernUnit, kern);

        public void PutDelimiterOver(TexDelimeter delimiter, TexFormula superscriptFormula, TexUnit kernUnit, double kern)
        {
            var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Over];
            Formula.RootAtom = new OverUnderDelimiter(Formula.RootAtom,
                superscriptFormula?.RootAtom, SymbolAtom.GetAtom(name), kernUnit, kern,
                true);
        }

        public void PutDelimiterUnder(TexDelimeter delimiter)
        {
            var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Under];
            Formula.RootAtom = new OverUnderDelimiter(Formula.RootAtom, null, SymbolAtom.GetAtom(name),
                TexUnit.Ex, 0, false);
        }

        public void PutDelimiterUnder(TexDelimeter delimiter, string subscriptFormula, TexUnit kernUnit, double kern) => PutDelimiterUnder(delimiter, FormulaParser.Parse(subscriptFormula), kernUnit, kern);

        public void PutDelimiterUnder(TexDelimeter delimiter, TexFormula subscriptName, TexUnit kernUnit, double kern)
        {
            var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Under];
            Formula.RootAtom = new OverUnderDelimiter(Formula.RootAtom,
                subscriptName?.RootAtom, SymbolAtom.GetAtom(name), kernUnit, kern, false);
        }

        public void PutOver(TexFormula overFormula, TexUnit overUnit, double overSpace, bool overScriptSize)
        {
            Formula.RootAtom = new UnderOverAtom(Formula.RootAtom,
                overFormula?.RootAtom, overUnit, overSpace, overScriptSize, true);
        }

        public void PutOver(string overFormula, TexUnit overUnit, double overSpace, bool overScriptSize) => PutOver(overFormula == null ? null : FormulaParser.Parse(overFormula), overUnit, overSpace, overScriptSize);

        public void PutUnder(string underFormula, TexUnit underUnit, double underSpace, bool underScriptSize) => PutUnder(underFormula == null ? null : FormulaParser.Parse(underFormula), underUnit, underSpace,
            underScriptSize);

        public void PutUnder(TexFormula underFormula, TexUnit underUnit, double underSpace, bool underScriptSize)
        {
            Formula.RootAtom = new UnderOverAtom(Formula.RootAtom,
                underFormula?.RootAtom, underUnit, underSpace, underScriptSize, false);
        }

        public void PutUnderAndOver(string underFormula, TexUnit underUnit, double underSpace, bool underScriptSize,
            string over, TexUnit overUnit, double overSpace, bool overScriptSize) => PutUnderAndOver(underFormula == null ? null : FormulaParser.Parse(underFormula), underUnit, underSpace,
                underScriptSize, over == null ? null : FormulaParser.Parse(over), overUnit, overSpace, overScriptSize);

        public void PutUnderAndOver(TexFormula underFormula, TexUnit underUnit, double underSpace, bool underScriptSize,
            TexFormula over, TexUnit overUnit, double overSpace, bool overScriptSize) => Formula.RootAtom = new UnderOverAtom(Formula.RootAtom, underFormula?.RootAtom, underUnit, underSpace, underScriptSize, over?.RootAtom,
                overUnit, overSpace, overScriptSize);
    }
}
