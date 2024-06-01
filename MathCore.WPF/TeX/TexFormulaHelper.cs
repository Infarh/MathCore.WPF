namespace MathCore.WPF.TeX;

internal sealed class TexFormulaHelper
{
    public TexFormulaParser FormulaParser;

    public TexFormula Formula { get; }

    public TexFormulaHelper(TexFormula formula)
    {
        FormulaParser = new();
        Formula       = formula;
    }

    public void SetFixedTypes(TexAtomType LeftType, TexAtomType RightType) => Formula.RootAtom = new TypedAtom(Formula.RootAtom, LeftType, RightType);

    public void CenterOnAxis() => Formula.RootAtom = new VerticalCenteredAtom(Formula.RootAtom);

    public void AddAccent(string formula, string AccentName) => AddAccent(FormulaParser.Parse(formula), AccentName);

    public void AddAccent(TexFormula BaseAtom, string AccentName) => Add(new AccentedAtom(BaseAtom?.RootAtom, AccentName));

    public void AddAccent(TexFormula BaseAtom, TexFormula accent) => Add(new AccentedAtom(BaseAtom?.RootAtom, accent));

    public void AddEmbraced(string formula, char LeftChar, char RightChar) =>
        AddEmbraced(FormulaParser.Parse(formula), LeftChar, RightChar);

    public void AddEmbraced(TexFormula formula, char LeftChar, char RightChar) =>
        AddEmbraced(formula, TexFormulaParser.GetDelimeterMapping(LeftChar), TexFormulaParser.GetDelimeterMapping(RightChar));

    public void AddEmbraced(string formula, string LeftSymbol, string RightSymbol) => AddEmbraced(FormulaParser.Parse(formula), LeftSymbol, RightSymbol);

    public void AddEmbraced(TexFormula formula, string LeftSymbol, string RightSymbol) => Add(new FencedAtom(formula?.RootAtom, TexFormulaParser.GetDelimiterSymbol(LeftSymbol),
        TexFormulaParser.GetDelimiterSymbol(RightSymbol)));

    public void AddFraction(string numerator, string denominator, bool DrawLine) => AddFraction(FormulaParser.Parse(numerator), FormulaParser.Parse(denominator), DrawLine);

    public void AddFraction(string numerator, TexFormula denominator, bool DrawLine) => AddFraction(FormulaParser.Parse(numerator), denominator, DrawLine);

    public void AddFraction(string numerator, string denominator, bool DrawLine, TexAlignment NumeratorAlignment,
        TexAlignment DenominatorAlignment) => AddFraction(FormulaParser.Parse(numerator), FormulaParser.Parse(denominator), DrawLine, NumeratorAlignment,
        DenominatorAlignment);

    public void AddFraction(TexFormula numerator, string denominator, bool DrawLine) => AddFraction(numerator, FormulaParser.Parse(denominator), DrawLine);

    public void AddFraction(TexFormula numerator, TexFormula denominator, bool DrawLine) => Add(new FractionAtom(numerator?.RootAtom,
        denominator?.RootAtom, DrawLine));

    public void AddFraction(TexFormula numerator, TexFormula denominator, bool DrawLine,
        TexAlignment NumeratorAlignment, TexAlignment DenominatorAlignment) => Add(new FractionAtom(numerator?.RootAtom,
        denominator?.RootAtom, DrawLine, NumeratorAlignment, DenominatorAlignment));

    public void AddRadical(string BaseFormula, string NthRoot) => AddRadical(FormulaParser.Parse(BaseFormula), FormulaParser.Parse(NthRoot));

    public void AddRadical(string BaseFormula, TexFormula NthRoot) => AddRadical(FormulaParser.Parse(BaseFormula), NthRoot);

    public void AddRadical(string BaseFormula) => AddRadical(FormulaParser.Parse(BaseFormula));

    public void AddRadical(TexFormula BaseFormula, string DegreeFormula) => AddRadical(BaseFormula, FormulaParser.Parse(DegreeFormula));

    public void AddRadical(TexFormula BaseFormula) => AddRadical(BaseFormula, (TexFormula)null);

    public void AddRadical(TexFormula BaseFormula, TexFormula DegreeFormula) => Add(new Radical(BaseFormula?.RootAtom,
        DegreeFormula?.RootAtom));

    public void AddOperator(string OperatorFormula, string LowerLimitFormula, string UpperLimitFormula) => AddOperator(FormulaParser.Parse(OperatorFormula), FormulaParser.Parse(LowerLimitFormula),
        FormulaParser.Parse(UpperLimitFormula));

    public void AddOperator(string OperatorFormula, string LowerLimitFormula, string UpperLimitFormula,
        bool UseVerticalLimits) => AddOperator(FormulaParser.Parse(OperatorFormula), FormulaParser.Parse(LowerLimitFormula),
        FormulaParser.Parse(UpperLimitFormula), UseVerticalLimits);

    public void AddOperator(TexFormula OperatorFormula, TexFormula LowerLimitFormula, TexFormula UpperLimitFormula) => Add(new BigOperatorAtom(OperatorFormula?.RootAtom,
        LowerLimitFormula?.RootAtom,
        UpperLimitFormula?.RootAtom));

    public void AddOperator(TexFormula OperatorFormula, TexFormula LowerLimitFormula, TexFormula UpperLimitFormula,
        bool UseVerticalLimits) => Add(new BigOperatorAtom(OperatorFormula?.RootAtom,
        LowerLimitFormula?.RootAtom,
        UpperLimitFormula?.RootAtom, UseVerticalLimits));

    public void AddPhantom(string formula) => AddPhantom(FormulaParser.Parse(formula));

    public void AddPhantom(string formula, bool UseWidth, bool UseHeight, bool UseDepth) => AddPhantom(FormulaParser.Parse(formula), UseWidth, UseHeight, UseDepth);

    public void AddPhantom(TexFormula formula) => Add(new PhantomAtom(formula?.RootAtom));

    public void AddPhantom(TexFormula phantom, bool UseWidth, bool UseHeight, bool UseDepth) => Add(new PhantomAtom(phantom?.RootAtom, UseWidth, UseHeight, UseDepth));

    public void AddStrut(TexUnit unit, double width, double height, double depth) => Add(new SpaceAtom(unit, width, height, depth));

    public void AddStrut(TexUnit WidthUnit, double width, TexUnit HeightUnit, double height, TexUnit DepthUnit,
        double depth) => Add(new SpaceAtom(WidthUnit, width, HeightUnit, height, DepthUnit, depth));

    public void AddSymbol(string name) => Add(SymbolAtom.GetAtom(name));

    public void AddSymbol(string name, TexAtomType type) => Add(new SymbolAtom(SymbolAtom.GetAtom(name), type));

    public void Add(string formula) => Add(FormulaParser.Parse(formula));

    public void Add(TexFormula formula) => Formula.Add(formula);

    public void Add(Atom atom) => Formula.Add(atom);

    public void PutAccentOver(string AccentName) => Formula.RootAtom = new AccentedAtom(Formula.RootAtom, AccentName);

    public void PutDelimiterOver(TexDelimeter delimiter)
    {
        var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Over];
        Formula.RootAtom = new OverUnderDelimiter(Formula.RootAtom, null, SymbolAtom.GetAtom(name),
            TexUnit.Ex, 0, true);
    }

    public void PutDelimiterOver(TexDelimeter delimiter, string SuperscriptFormula, TexUnit KernUnit, double kern) => PutDelimiterOver(delimiter, FormulaParser.Parse(SuperscriptFormula), KernUnit, kern);

    public void PutDelimiterOver(TexDelimeter delimiter, TexFormula SuperscriptFormula, TexUnit KernUnit, double kern)
    {
        var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Over];
        Formula.RootAtom = new OverUnderDelimiter(Formula.RootAtom,
            SuperscriptFormula?.RootAtom, SymbolAtom.GetAtom(name), KernUnit, kern,
            true);
    }

    public void PutDelimiterUnder(TexDelimeter delimiter)
    {
        var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Under];
        Formula.RootAtom = new OverUnderDelimiter(Formula.RootAtom, null, SymbolAtom.GetAtom(name),
            TexUnit.Ex, 0, false);
    }

    public void PutDelimiterUnder(TexDelimeter delimiter, string SubscriptFormula, TexUnit KernUnit, double kern) => PutDelimiterUnder(delimiter, FormulaParser.Parse(SubscriptFormula), KernUnit, kern);

    public void PutDelimiterUnder(TexDelimeter delimiter, TexFormula SubscriptName, TexUnit KernUnit, double kern)
    {
        var name = TexFormulaParser.DelimiterNames[(int)delimiter][(int)TexDelimeterType.Under];
        Formula.RootAtom = new OverUnderDelimiter(Formula.RootAtom,
            SubscriptName?.RootAtom, SymbolAtom.GetAtom(name), KernUnit, kern, false);
    }

    public void PutOver(TexFormula OverFormula, TexUnit OverUnit, double OverSpace, bool OverScriptSize) =>
        Formula.RootAtom = new UnderOverAtom(Formula.RootAtom,
            OverFormula?.RootAtom, OverUnit, OverSpace, OverScriptSize, true);

    public void PutOver(string OverFormula, TexUnit OverUnit, double OverSpace, bool OverScriptSize) => PutOver(OverFormula is null ? null : FormulaParser.Parse(OverFormula), OverUnit, OverSpace, OverScriptSize);

    public void PutUnder(string UnderFormula, TexUnit UnderUnit, double UnderSpace, bool UnderScriptSize) => PutUnder(UnderFormula is null ? null : FormulaParser.Parse(UnderFormula), UnderUnit, UnderSpace,
        UnderScriptSize);

    public void PutUnder(TexFormula UnderFormula, TexUnit UnderUnit, double UnderSpace, bool UnderScriptSize) =>
        Formula.RootAtom = new UnderOverAtom(Formula.RootAtom,
            UnderFormula?.RootAtom, UnderUnit, UnderSpace, UnderScriptSize, false);

    public void PutUnderAndOver(string UnderFormula, TexUnit UnderUnit, double UnderSpace, bool UnderScriptSize,
        string over, TexUnit OverUnit, double OverSpace, bool OverScriptSize) => PutUnderAndOver(UnderFormula is null ? null : FormulaParser.Parse(UnderFormula), UnderUnit, UnderSpace,
        UnderScriptSize, over is null ? null : FormulaParser.Parse(over), OverUnit, OverSpace, OverScriptSize);

    public void PutUnderAndOver(TexFormula UnderFormula, TexUnit UnderUnit, double UnderSpace, bool UnderScriptSize,
        TexFormula over, TexUnit OverUnit, double OverSpace, bool OverScriptSize) => Formula.RootAtom = new UnderOverAtom(Formula.RootAtom, UnderFormula?.RootAtom, UnderUnit, UnderSpace, UnderScriptSize, over?.RootAtom,
        OverUnit, OverSpace, OverScriptSize);
}