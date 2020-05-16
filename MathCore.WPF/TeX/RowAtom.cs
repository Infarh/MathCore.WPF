using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Atom representing horizontal row of other atoms, separated by glue.
namespace MathCore.WPF.TeX
{
    internal class RowAtom : Atom, IRow
    {
        // Set of atom types that make previous atom of BinaryOperator type change to Ordinary type.
        private static readonly BitArray binaryOperatorChangeSet;

        // Set of atom types that may need kern, or together with previous atom, be replaced by ligature.
        private static readonly BitArray ligatureKernChangeSet;

        static RowAtom()
        {
            binaryOperatorChangeSet = new BitArray(16);
            binaryOperatorChangeSet.Set((int)TexAtomType.BinaryOperator, true);
            binaryOperatorChangeSet.Set((int)TexAtomType.BigOperator, true);
            binaryOperatorChangeSet.Set((int)TexAtomType.Relation, true);
            binaryOperatorChangeSet.Set((int)TexAtomType.Opening, true);
            binaryOperatorChangeSet.Set((int)TexAtomType.Punctuation, true);

            ligatureKernChangeSet = new BitArray(16);
            ligatureKernChangeSet.Set((int)TexAtomType.Ordinary, true);
            ligatureKernChangeSet.Set((int)TexAtomType.BigOperator, true);
            ligatureKernChangeSet.Set((int)TexAtomType.BinaryOperator, true);
            ligatureKernChangeSet.Set((int)TexAtomType.Relation, true);
            ligatureKernChangeSet.Set((int)TexAtomType.Opening, true);
            ligatureKernChangeSet.Set((int)TexAtomType.Closing, true);
            ligatureKernChangeSet.Set((int)TexAtomType.Punctuation, true);
        }

        public DummyAtom PreviousAtom { get; set; }
        public List<Atom> Elements { get; }
        public RowAtom() { Elements = new List<Atom>(); }

        public RowAtom(List<TexFormula> formulaList)
            : this()
        {
            Elements.AddRange(formulaList.Where(f => f.RootAtom != null).Select(f => f.RootAtom));
        }

        public RowAtom(Atom baseAtom)
            : this()
        {
            if(baseAtom is null) return;
            var atom = baseAtom as RowAtom;
            if(atom != null)
                Elements.AddRange(atom.Elements);
            else
                Elements.Add(baseAtom);
        }

        public void Add(Atom atom) { if(atom != null) Elements.Add(atom); }

        private void ChangeAtomToOrdinary(DummyAtom currentAtom, DummyAtom previousAtom, Atom nextAtom)
        {
            var type = currentAtom.GetLeftType();
            if(type == TexAtomType.BinaryOperator && (previousAtom is null || binaryOperatorChangeSet[(int)previousAtom.GetRightType()]))
                currentAtom.Type = TexAtomType.Ordinary;
            else if(nextAtom != null && currentAtom.GetRightType() == TexAtomType.BinaryOperator)
            {
                var nextType = nextAtom.GetLeftType();
                if(nextType == TexAtomType.Relation || nextType == TexAtomType.Closing ||
                    nextType == TexAtomType.Punctuation)
                    currentAtom.Type = TexAtomType.Ordinary;
            }
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            var texFont = environment.TexFont;

            // Create result box.
            var result = new HorizontalBox(environment.Foreground, environment.Background);

            // Create and add box for each atom in row.
            for(var i = 0; i < Elements.Count; i++)
            {
                var current_atom = new DummyAtom(Elements[i]);

                // Change atom type to Ordinary, if required.
                var hasNextAtom = i < Elements.Count - 1;
                var nextAtom = hasNextAtom ? Elements[i + 1] : null;
                ChangeAtomToOrdinary(current_atom, PreviousAtom, nextAtom);

                // Check if atom is part of ligature or should be kerned.
                var kern = 0d;
                if(hasNextAtom && current_atom.GetRightType() == TexAtomType.Ordinary && current_atom.IsCharSymbol)
                {
                    if(nextAtom is CharSymbol && ligatureKernChangeSet[(int)nextAtom.GetLeftType()])
                    {
                        current_atom.IsTextSymbol = true;
                        var leftAtomCharFont = current_atom.GetCharFont(texFont);
                        var rightAtomCharFont = ((CharSymbol)nextAtom).GetCharFont(texFont);
                        var ligatureCharFont = texFont.GetLigature(leftAtomCharFont, rightAtomCharFont);
                        if(ligatureCharFont is null)
                        {
                            // Atom should be kerned.
                            kern = texFont.GetKern(leftAtomCharFont, rightAtomCharFont, environment.Style);
                        }
                        else
                        {
                            // Atom is part of ligature.
                            current_atom.SetLigature(new FixedCharAtom(ligatureCharFont));
                            i++;
                        }
                    }
                }

                // Create and add glue box, unless atom is first of row or previous/current atom is kern.
                if(i != 0 && PreviousAtom != null && !PreviousAtom.IsKern && !current_atom.IsKern)
                    result.Add(Glue.CreateBox(PreviousAtom.GetRightType(), current_atom.GetLeftType(), environment));

                // Create and add box for atom.
                current_atom.PreviousAtom = PreviousAtom;
                var curBox = current_atom.CreateBox(environment);
                result.Add(curBox);
                environment.LastFontId = curBox.GetLastFontId();

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
}