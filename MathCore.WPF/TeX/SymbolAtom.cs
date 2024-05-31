using System.Collections;

namespace MathCore.WPF.TeX;

/// <summary>Atom representing symbol (non-alphanumeric character)</summary>
internal class SymbolAtom : CharSymbol
{
    /// <summary>Dictionary of definitions of all symbols, keyed by name</summary>
    private static readonly Dictionary<string, SymbolAtom> __Symbols;

    /// <summary>Set of all valid symbol types</summary>
    private static readonly BitArray __ValidSymbolTypes;

    static SymbolAtom()
    {
        __Symbols = new TexSymbolParser().GetSymbols();

        __ValidSymbolTypes = new(16);
        __ValidSymbolTypes.Set((int)TexAtomType.Ordinary, true);
        __ValidSymbolTypes.Set((int)TexAtomType.BigOperator, true);
        __ValidSymbolTypes.Set((int)TexAtomType.BinaryOperator, true);
        __ValidSymbolTypes.Set((int)TexAtomType.Relation, true);
        __ValidSymbolTypes.Set((int)TexAtomType.Opening, true);
        __ValidSymbolTypes.Set((int)TexAtomType.Closing, true);
        __ValidSymbolTypes.Set((int)TexAtomType.Punctuation, true);
        __ValidSymbolTypes.Set((int)TexAtomType.Accent, true);
    }

    public static SymbolAtom GetAtom(string name)
    {
        if(!__Symbols.ContainsKey(name))
            throw new SymbolNotFoundException(name);
        return __Symbols[name];
    }

    public bool IsDelimeter { get; }

    public string Name { get; }

    public SymbolAtom(SymbolAtom SymbolAtom, TexAtomType type)
    {
        if(!__ValidSymbolTypes[(int)type])
            throw new ArgumentException(@"The specified type is not a valid symbol type.", nameof(type));
        Type        = type;
        Name        = SymbolAtom.Name;
        IsDelimeter = SymbolAtom.IsDelimeter;
    }

    public SymbolAtom(string name, TexAtomType type, bool IsDelimeter)
    {
        Type             = type;
        Name             = name;
        this.IsDelimeter = IsDelimeter;
    }

    public override Box CreateBox(TexEnvironment environment) => new CharBox(environment, environment.TexFont.GetCharInfo(Name, environment.Style));

    public override CharFont GetCharFont(ITeXFont TexFont) => TexFont.GetCharInfo(Name, TexStyle.Display).GetCharacterFont();
}