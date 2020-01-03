using System;
using System.Collections;
using System.Collections.Generic;

namespace MathCore.WPF.TeX
{
    /// <summary>Atom representing symbol (non-alphanumeric character)</summary>
    internal class SymbolAtom : CharSymbol
    {
        /// <summary>Dictionary of definitions of all symbols, keyed by name</summary>
        private static readonly Dictionary<string, SymbolAtom> symbols;

        /// <summary>Set of all valid symbol types</summary>
        private static readonly BitArray validSymbolTypes;

        static SymbolAtom()
        {
            symbols = new TexSymbolParser().GetSymbols();

            validSymbolTypes = new BitArray(16);
            validSymbolTypes.Set((int)TexAtomType.Ordinary, true);
            validSymbolTypes.Set((int)TexAtomType.BigOperator, true);
            validSymbolTypes.Set((int)TexAtomType.BinaryOperator, true);
            validSymbolTypes.Set((int)TexAtomType.Relation, true);
            validSymbolTypes.Set((int)TexAtomType.Opening, true);
            validSymbolTypes.Set((int)TexAtomType.Closing, true);
            validSymbolTypes.Set((int)TexAtomType.Punctuation, true);
            validSymbolTypes.Set((int)TexAtomType.Accent, true);
        }

        public static SymbolAtom GetAtom(string name)
        {
            if(!symbols.ContainsKey(name))
                throw new SymbolNotFoundException(name);
            return symbols[name];
        }

        public bool IsDelimeter { get; }

        public string Name { get; }

        public SymbolAtom(SymbolAtom symbolAtom, TexAtomType type)
        {
            if(!validSymbolTypes[(int)type])
                throw new ArgumentException(@"The specified type is not a valid symbol type.", nameof(type));
            Type = type;
            Name = symbolAtom.Name;
            IsDelimeter = symbolAtom.IsDelimeter;
        }

        public SymbolAtom(string name, TexAtomType type, bool isDelimeter)
        {
            Type = type;
            Name = name;
            IsDelimeter = isDelimeter;
        }

        public override Box CreateBox(TexEnvironment environment) => new CharBox(environment, environment.TexFont.GetCharInfo(Name, environment.Style));

        public override CharFont GetCharFont(ITeXFont texFont) => texFont.GetCharInfo(Name, TexStyle.Display).GetCharacterFont();
    }
}