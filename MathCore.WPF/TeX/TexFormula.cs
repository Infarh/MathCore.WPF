using System.Diagnostics;

namespace MathCore.WPF.TeX;

/// <summary>Represents mathematical formula that can be rendered</summary>
public sealed class TexFormula
{
    public TexFormula(List<TexFormula> FormulaList)
    {
        Debug.Assert(FormulaList != null);

        if(FormulaList.Count == 1)
            Add(FormulaList[0]);
        else
            RootAtom = new RowAtom(FormulaList);
    }

    public string TextStyle { get; set; }

    internal Atom? RootAtom { get; set; }

    public TexFormula(TexFormula formula)
    {
        Debug.Assert(formula != null);

        Add(formula);
    }

    public TexFormula() { }

    public TexRenderer GetRenderer(TexStyle style, double scale)
    {
        var environment = new TexEnvironment(style, new DefaultTexFont(scale));
        return new(CreateBox(environment), scale);
    }

    public void Add(TexFormula formula)
    {
        Debug.Assert(formula != null);
        Debug.Assert(formula.RootAtom != null);

        Add(formula.RootAtom is RowAtom ? new RowAtom(formula.RootAtom) : formula.RootAtom);
    }

    internal void Add(Atom atom)
    {
        Debug.Assert(atom != null);
        if(RootAtom is null)
            RootAtom = atom;
        else
        {
            if(RootAtom is not RowAtom)
                RootAtom = new RowAtom(RootAtom);
            ((RowAtom)RootAtom).Add(atom);
        }
    }

    //public void SetForeground(Brush brush)
    //{
    //    if(RootAtom is StyledAtom)
    //        ((StyledAtom)(RootAtom = ((StyledAtom)RootAtom).Clone())).Foreground = brush;
    //    else
    //        RootAtom = new StyledAtom(RootAtom, null, brush);
    //}

    //public void SetBackground(Brush brush)
    //{
    //    if(RootAtom is StyledAtom)
    //        ((StyledAtom)(RootAtom = ((StyledAtom)RootAtom).Clone())).Background = brush;
    //    else
    //        RootAtom = new StyledAtom(RootAtom, brush, null);
    //}

    internal Box CreateBox(TexEnvironment environment)
    {
        var root = RootAtom;
        return root is null ? StrutBox.Empty : root.CreateBox(environment);
    }
}