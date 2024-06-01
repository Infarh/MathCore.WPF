using System.Collections;

// Atom representing horizontal row of other atoms, separated by glue.
namespace MathCore.WPF.TeX;

internal class RowAtom : Atom, IRow
{
    // Set of atom types that make previous atom of BinaryOperator type change to Ordinary type.
    private static readonly BitArray __BinaryOperatorChangeSet;

    // Set of atom types that may need kern, or together with previous atom, be replaced by ligature.
    private static readonly BitArray __LigatureKernChangeSet;

    static RowAtom()
    {
        __BinaryOperatorChangeSet = new(16);
        __BinaryOperatorChangeSet.Set((int)TexAtomType.BinaryOperator, true);
        __BinaryOperatorChangeSet.Set((int)TexAtomType.BigOperator, true);
        __BinaryOperatorChangeSet.Set((int)TexAtomType.Relation, true);
        __BinaryOperatorChangeSet.Set((int)TexAtomType.Opening, true);
        __BinaryOperatorChangeSet.Set((int)TexAtomType.Punctuation, true);

        __LigatureKernChangeSet = new(16);
        __LigatureKernChangeSet.Set((int)TexAtomType.Ordinary, true);
        __LigatureKernChangeSet.Set((int)TexAtomType.BigOperator, true);
        __LigatureKernChangeSet.Set((int)TexAtomType.BinaryOperator, true);
        __LigatureKernChangeSet.Set((int)TexAtomType.Relation, true);
        __LigatureKernChangeSet.Set((int)TexAtomType.Opening, true);
        __LigatureKernChangeSet.Set((int)TexAtomType.Closing, true);
        __LigatureKernChangeSet.Set((int)TexAtomType.Punctuation, true);
    }

    public DummyAtom PreviousAtom { get; set; }
    public List<Atom> Elements { get; }
    public RowAtom() => Elements = [];

    public RowAtom(List<TexFormula> FormulaList)
        : this() =>
        Elements.AddRange(FormulaList.Where(f => f.RootAtom != null).Select(f => f.RootAtom));

    public RowAtom(Atom? BaseAtom)
        : this()
    {
        switch (BaseAtom)
        {
            case null:         return;
            case RowAtom atom: Elements.AddRange(atom.Elements); break;
            default:           Elements.Add(BaseAtom); break;
        }
    }

    public void Add(Atom? atom) { if(atom != null) Elements.Add(atom); }

    private static void ChangeAtomToOrdinary(DummyAtom CurrentAtom, DummyAtom? PreviousAtom, Atom? NextAtom)
    {
        var type = CurrentAtom.GetLeftType();
        if(type == TexAtomType.BinaryOperator && (PreviousAtom is null || __BinaryOperatorChangeSet[(int)PreviousAtom.GetRightType()]))
            CurrentAtom.Type = TexAtomType.Ordinary;
        else if(NextAtom != null && CurrentAtom.GetRightType() == TexAtomType.BinaryOperator)
        {
            var next_type = NextAtom.GetLeftType();
            if(next_type is TexAtomType.Relation or TexAtomType.Closing or TexAtomType.Punctuation)
                CurrentAtom.Type = TexAtomType.Ordinary;
        }
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        var tex_font = environment.TexFont;

        // Create result box.
        var result = new HorizontalBox(environment.Foreground, environment.Background);

        // Create and add box for each atom in row.
        for(var i = 0; i < Elements.Count; i++)
        {
            var current_atom = new DummyAtom(Elements[i]);

            // Change atom type to Ordinary, if required.
            var has_next_atom = i < Elements.Count - 1;
            var next_atom    = has_next_atom ? Elements[i + 1] : null;
            ChangeAtomToOrdinary(current_atom, PreviousAtom, next_atom);

            // Check if atom is part of ligature or should be kerned.
            var kern = 0d;
            if(has_next_atom && current_atom.GetRightType() == TexAtomType.Ordinary && current_atom.IsCharSymbol)
                if(next_atom is CharSymbol symbol && __LigatureKernChangeSet[(int)symbol.GetLeftType()])
                {
                    current_atom.IsTextSymbol = true;
                    var left_atom_char_font  = current_atom.GetCharFont(tex_font);
                    var right_atom_char_font = symbol.GetCharFont(tex_font);
                    var ligature_char_font   = tex_font.GetLigature(left_atom_char_font, right_atom_char_font);
                    if(ligature_char_font is null)
                        // Atom should be kerned.
                        kern = tex_font.GetKern(left_atom_char_font, right_atom_char_font, environment.Style);
                    else
                    {
                        // Atom is part of ligature.
                        current_atom.SetLigature(new(ligature_char_font));
                        i++;
                    }
                }

            // Create and add glue box, unless atom is first of row or previous/current atom is kern.
            if(i != 0 && PreviousAtom is { IsKern: false } && !current_atom.IsKern)
                result.Add(Glue.CreateBox(PreviousAtom.GetRightType(), current_atom.GetLeftType(), environment));

            // Create and add box for atom.
            current_atom.PreviousAtom = PreviousAtom;
            var cur_box = current_atom.CreateBox(environment);
            result.Add(cur_box);
            environment.LastFontId = cur_box.GetLastFontId();

            // Insert kern, if required.
            if(kern > TexUtilities.FloatPrecision)
                result.Add(new StrutBox(0, kern, 0, 0));

            if(!current_atom.IsKern)
                PreviousAtom = current_atom;
        }

        // Reset previous atom.
        PreviousAtom = null;

        return result;
    }

    public override TexAtomType GetLeftType() => Elements.Count == 0 ? Type : Elements.First().GetLeftType();

    public override TexAtomType GetRightType() => Elements.Count == 0 ? Type : Elements.Last().GetRightType();
}