using System.Text;

// TODO: put all error strings into resources
namespace MathCore.WPF.TeX;

public class TexFormulaParser
{
    // Special characters for parsing
    private const char __EscapeChar = '\\';

    private const char __LeftGroupChar = '{';
    private const char __RightGroupChar = '}';
    private const char __LeftBracketChar = '[';
    private const char __RightBracketChar = ']';

    private const char __SubScriptChar = '_';
    private const char __SuperScriptChar = '^';
    private const char __PrimeChar = '\'';

    // Information used for parsing
    private static HashSet<string> __Commands;
    private static string[] __Symbols;
    private static string[] __Delimiters;
    private static HashSet<string> __TextStyles;
    private static readonly Dictionary<string, TexFormula> __PredefinedFormulas;

    private static readonly string[][] __DelimiterNames =
    {
        new[] { "lbrace", "rbrace" },
        new[] { "lsqbrack", "rsqbrack" },
        new[] { "lbrack", "rbrack" },
        new[] { "downarrow", "downarrow" },
        new[] { "uparrow", "uparrow" },
        new[] { "updownarrow", "updownarrow" },
        new[] { "Downarrow", "Downarrow" },
        new[] { "Uparrow", "Uparrow" },
        new[] { "Updownarrow", "Updownarrow" },
        new[] { "vert", "vert" },
        new[] { "Vert", "Vert" }
    };

    /// <summary>True if parser has been initialized</summary>
    private static bool __IsInitialized;

    internal static string[][] DelimiterNames => __DelimiterNames;

    static TexFormulaParser()
    {
        __IsInitialized = false;

        __PredefinedFormulas = [];
        Initialize();
    }

    private static void Initialize()
    {
        __Commands = ["frac", "sqrt"];

        var formula_settings_parser = new TexPredefinedFormulaSettingsParser();
        __Symbols    = formula_settings_parser.GetSymbolMappings();
        __Delimiters = formula_settings_parser.GetDelimiterMappings();
        __TextStyles = formula_settings_parser.GetTextStyles();

        __IsInitialized = true;

        var predefined_formulas_parser = new TexPredefinedFormulaParser();
        predefined_formulas_parser.Parse(__PredefinedFormulas);
    }

    internal static TexFormula? GetFormula(string name)
    {
        var f = __PredefinedFormulas.GetValue(name);
        return f is null ? null : new TexFormula(f);
    }

    internal static string GetDelimeterMapping(char character)
    {
        if (character < 0 || character >= __Delimiters.Length)
            throw new DelimiterMappingNotFoundException(character);

        return __Delimiters[character];
    }

    internal static SymbolAtom? GetDelimiterSymbol(string name)
    {
        var result = SymbolAtom.GetAtom(name);
        return !result.IsDelimeter ? null : result;
    }

    private static bool IsSymbol(char c) => c is not (>= '0' and <= '9' or >= 'a' and <= 'z' or >= 'A' and <= 'Z');

    private static bool IsWhiteSpace(char ch) => ch is ' ' or '\t' or '\n' or '\r';

    public TexFormulaParser()
    {
        if (!__IsInitialized)
            throw new InvalidOperationException("Parser has not yet been initialized.");
    }

    public TexFormula Parse(string value)
    {
        var formula  = new TexFormula();
        var position = 0;
        while (position < value.Length)
        {
            var ch = value[position];
            if (IsWhiteSpace(ch))
                position++;
            else
                switch (ch)
                {
                    case __EscapeChar:
                        ProcessEscapeSequence(formula, value, ref position);
                        break;
                    case __LeftGroupChar:
                        formula.Add(
                            AttachScripts(
                                formula,
                                value,
                                ref position,
                                Parse(
                                        ReadGroup(
                                            formula,
                                            value,
                                            ref position,
                                            __LeftGroupChar,
                                            __RightGroupChar))
                                   .RootAtom));
                        break;
                    case __RightGroupChar:
                        throw new TexParseException(
                            $"Found a closing '{__RightGroupChar}' without an opening '{__LeftGroupChar}'!");
                    case __SuperScriptChar:
                    case __SubScriptChar:
                    case __PrimeChar:
                        if (position == 0)
                            throw new TexParseException(
                                $"Every script needs a base: \"{__SuperScriptChar}\", \"{__SubScriptChar}\" and \"{__PrimeChar}\" can't be the first character!");

                        throw new TexParseException("Double scripts found! Try using more braces.");
                    default:
                        var scripts_atom = AttachScripts(
                            formula,
                            value,
                            ref position,
                            ConvertCharacter(formula, value, ref position, ch));
                        formula.Add(scripts_atom);
                        break;
                }
        }

        return formula;
    }

    private static string ReadGroup(TexFormula formula, string value, ref int position, char OpenChar, char CloseChar)
    {
        if (position == value.Length || value[position] != OpenChar)
            throw new TexParseException($"missing '{OpenChar}'!");

        var result = new StringBuilder();
        var group  = 0;
        position++;
        while (position < value.Length && !(value[position] == CloseChar && group == 0))
        {
            if (value[position] == OpenChar)
                group++;
            else if (value[position] == CloseChar)
                group--;
            result.Append(value[position]);
            position++;
        }

        if (position == value.Length)
            // Reached end of formula but group has not been closed.
            throw new TexParseException($"Illegal end,  missing '{CloseChar}'!");

        position++;
        return result.ToString();
    }

    private TexFormula ReadScript(TexFormula formula, string value, ref int position)
    {
        if (position == value.Length)
            throw new TexParseException("illegal end, missing script!");

        SkipWhiteSpace(value, ref position);
        var ch = value[position];
        if (ch == __LeftGroupChar)
            return Parse(ReadGroup(formula, value, ref position, __LeftGroupChar, __RightGroupChar));

        position++;
        return Parse(ch.ToString());
    }

    private Atom ProcessCommand(TexFormula formula, string value, ref int position, string command)
    {
        SkipWhiteSpace(value, ref position);

        switch (command)
        {
            case "frac":
                // Command is fraction.

                var numerator_formula = Parse(ReadGroup(formula, value, ref position, __LeftGroupChar, __RightGroupChar));
                SkipWhiteSpace(value, ref position);
                var denominator_formula = Parse(ReadGroup(formula, value, ref position, __LeftGroupChar, __RightGroupChar));
                if (numerator_formula.RootAtom is null || denominator_formula.RootAtom is null)
                    throw new TexParseException("Both numerator and denominator of a fraction can't be empty!");

                return new FractionAtom(numerator_formula.RootAtom, denominator_formula.RootAtom, true);
            case "sqrt":
                // Command is radical.

                SkipWhiteSpace(value, ref position);
                if (position == value.Length)
                    throw new TexParseException("illegal end!");

                TexFormula? degree_formula = null;
                if (value[position] != __LeftBracketChar)
                    return new Radical(
                        Parse(ReadGroup(formula, value, ref position, __LeftGroupChar, __RightGroupChar))
                           .RootAtom,
                        degree_formula?.RootAtom);

                // Degree of radical- is specified.
                degree_formula = Parse(ReadGroup(formula, value, ref position, __LeftBracketChar, __RightBracketChar));
                SkipWhiteSpace(value, ref position);

                return new Radical(
                    Parse(ReadGroup(formula, value, ref position, __LeftGroupChar, __RightGroupChar))
                       .RootAtom,
                    degree_formula.RootAtom);
        }

        throw new TexParseException("Invalid command.");
    }

    private void ProcessEscapeSequence(TexFormula formula, string value, ref int position)
    {
        var result = new StringBuilder();
        position++;
        while (position < value.Length)
        {
            var ch     = value[position];
            var is_end = position == value.Length - 1;
            if (!char.IsLetter(ch) || is_end)
            {
                // Escape sequence has ended.
                if (is_end)
                {
                    result.Append(ch);
                    position++;
                }

                break;
            }

            result.Append(ch);
            position++;
        }

        var command = result.ToString();

        SymbolAtom symbol_atom = null;

        try
        {
            symbol_atom = SymbolAtom.GetAtom(command);
        }
        catch (SymbolNotFoundException) { }

        var predefined_formula = GetFormula(command);

        if (symbol_atom != null)
            // Symbol was found.
            formula.Add(AttachScripts(formula, value, ref position, symbol_atom));
        else if (predefined_formula != null)
            // Predefined formula was found.
            formula.Add(AttachScripts(formula, value, ref position, predefined_formula.RootAtom));
        else if (command.Equals("nbsp"))
            // Space was found.
            formula.Add(AttachScripts(formula, value, ref position, new SpaceAtom()));
        else if (__TextStyles.Contains(command))
        {
            // Text style was found.

            SkipWhiteSpace(value, ref position);
            var styled_formula = Parse(ReadGroup(formula, value, ref position, __LeftGroupChar, __RightGroupChar));
            styled_formula.TextStyle = command;
            formula.Add(AttachScripts(formula, value, ref position, styled_formula.RootAtom));
        }
        else if (__Commands.Contains(command))
            // Command was found.
            formula.Add(
                AttachScripts(
                    formula,
                    value,
                    ref position,
                    ProcessCommand(
                        formula,
                        value,
                        ref position,
                        command)));
        else
            // Escape sequence is invalid.
            throw new TexParseException("Unknown symbol or command or predefined TeXFormula: '" + command + "'");
    }

    private Atom AttachScripts(TexFormula formula, string value, ref int position, Atom atom)
    {
        SkipWhiteSpace(value, ref position);

        if (position == value.Length)
            return atom;

        // Check for prime marks.
        var primes_row_atom = new RowAtom();
        var i               = position + 1;
        while (i < value.Length)
        {
            if (value[i] == __PrimeChar)
                primes_row_atom.Add(SymbolAtom.GetAtom("prime"));
            else if (!IsWhiteSpace(value[i]))
                break;

            position++;
            i++;
        }

        // Attach prime marks as superscript, if any were found.
        if (primes_row_atom.Elements.Count > 0)
            atom = new ScriptsAtom(atom, null, primes_row_atom);

        TexFormula superscript_formula = null;
        TexFormula subscript_formula   = null;

        var ch = value[position];
        switch (ch)
        {
            case __SuperScriptChar:
                // Attach superscript.
                position++;
                superscript_formula = ReadScript(formula, value, ref position);

                SkipWhiteSpace(value, ref position);
                if (position < value.Length && value[position] == __SubScriptChar)
                {
                    // Attach subscript also.
                    position++;
                    subscript_formula = ReadScript(formula, value, ref position);
                }

                break;
            case __SubScriptChar:
                // Add subscript.
                position++;
                subscript_formula = ReadScript(formula, value, ref position);

                SkipWhiteSpace(value, ref position);
                if (position < value.Length && value[position] == __SuperScriptChar)
                {
                    // Attach superscript also.
                    position++;
                    superscript_formula = ReadScript(formula, value, ref position);
                }

                break;
        }

        if (superscript_formula is null && subscript_formula is null)
            return atom;

        // Check whether to return Big Operator or Scripts.
        if (atom.GetRightType() == TexAtomType.BigOperator)
            return new BigOperatorAtom(
                atom,
                subscript_formula?.RootAtom,
                superscript_formula?.RootAtom);

        return new ScriptsAtom(
            atom,
            subscript_formula?.RootAtom,
            superscript_formula?.RootAtom);
    }

    private static Atom ConvertCharacter(TexFormula formula, string value, ref int position, char character)
    {
        position++;
        if (!IsSymbol(character))
            // Character is alpha-numeric.
            return new CharAtom(character, formula.TextStyle);

        // Character is symbol.
        var symbol_name = __Symbols[character];
        if (symbol_name is null)
            throw new TexParseException($"Unknown character : '{character}'");

        try
        {
            return SymbolAtom.GetAtom(symbol_name);
        }
        catch (SymbolNotFoundException e)
        {
            throw new TexParseException(
                $"The character '{character}' was mapped to an unknown symbol with the name '{symbol_name}'!",
                e);
        }
    }

    private void SkipWhiteSpace(string value, ref int position)
    {
        while (position < value.Length && IsWhiteSpace(value[position]))
            position++;
    }
}