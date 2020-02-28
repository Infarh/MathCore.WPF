using System;
using System.Collections.Generic;
using System.Text;

// TODO: put all error strings into resources
namespace MathCore.WPF.TeX
{
    public class TexFormulaParser
    {
        // Special characters for parsing
        private const char escapeChar = '\\';

        private const char leftGroupChar = '{';
        private const char rightGroupChar = '}';
        private const char leftBracketChar = '[';
        private const char rightBracketChar = ']';

        private const char subScriptChar = '_';
        private const char superScriptChar = '^';
        private const char primeChar = '\'';

        // Information used for parsing
        private static HashSet<string> commands;
        private static string[] symbols;
        private static string[] delimeters;
        private static HashSet<string> textStyles;
        private static readonly Dictionary<string, TexFormula> _PredefinedFormulas;

        private static readonly string[][] delimiterNames =
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
        private static bool isInitialized;

        internal static string[][] DelimiterNames => delimiterNames;

        static TexFormulaParser()
        {
            isInitialized = false;

            _PredefinedFormulas = new Dictionary<string, TexFormula>();
            Initialize();
        }

        private static void Initialize()
        {
            commands = new HashSet<string> { "frac", "sqrt" };

            var formulaSettingsParser = new TexPredefinedFormulaSettingsParser();
            symbols = formulaSettingsParser.GetSymbolMappings();
            delimeters = formulaSettingsParser.GetDelimiterMappings();
            textStyles = formulaSettingsParser.GetTextStyles();

            isInitialized = true;

            var predefinedFormulasParser = new TexPredefinedFormulaParser();
            predefinedFormulasParser.Parse(_PredefinedFormulas);
        }

        internal static TexFormula GetFormula(string name)
        {
            var f = _PredefinedFormulas.GetValue(name);
            return f is null ? null : new TexFormula(f);
        }

        internal static string GetDelimeterMapping(char character)
        {
            if(character < 0 || character >= delimeters.Length)
                throw new DelimiterMappingNotFoundException(character);
            return delimeters[character];

        }

        internal static SymbolAtom GetDelimiterSymbol(string name)
        {
            var result = SymbolAtom.GetAtom(name);
            return !result.IsDelimeter ? null : result;
        }

        private static bool IsSymbol(char c) => !((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'));

        private static bool IsWhiteSpace(char ch) => ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';

        public TexFormulaParser()
        {
            if(!isInitialized)
                throw new InvalidOperationException("Parser has not yet been initialized.");
        }

        public TexFormula Parse(string value)
        {
            var formula = new TexFormula();
            var position = 0;
            while(position < value.Length)
            {
                var ch = value[position];
                if(IsWhiteSpace(ch))
                    position++;
                else
                    switch(ch)
                    {
                        case escapeChar:
                            ProcessEscapeSequence(formula, value, ref position);
                            break;
                        case leftGroupChar:
                            formula.Add(AttachScripts(formula, value, ref position,
                                Parse(ReadGroup(formula, value, ref position,
                                    leftGroupChar, rightGroupChar)).RootAtom));
                            break;
                        case rightGroupChar:
                            throw new TexParseException(
                                $"Found a closing '{rightGroupChar}' without an opening '{leftGroupChar}'!");
                        case superScriptChar:
                        case subScriptChar:
                        case primeChar:
                            if(position == 0)
                                throw new TexParseException(
                                    $"Every script needs a base: \"{superScriptChar}\", \"{subScriptChar}\" and \"{primeChar}\" can't be the first character!");
                            throw new TexParseException("Double scripts found! Try using more braces.");
                        default:
                            var scriptsAtom = AttachScripts(formula, value, ref position,
                                ConvertCharacter(formula, value, ref position, ch));
                            formula.Add(scriptsAtom);
                            break;
                    }
            }

            return formula;
        }

        private string ReadGroup(TexFormula formula, string value, ref int position, char openChar, char closeChar)
        {
            if(position == value.Length || value[position] != openChar)
                throw new TexParseException($"missing '{openChar}'!");

            var result = new StringBuilder();
            var group = 0;
            position++;
            while(position < value.Length && !(value[position] == closeChar && group == 0))
            {
                if(value[position] == openChar)
                    group++;
                else if(value[position] == closeChar)
                    group--;
                result.Append(value[position]);
                position++;
            }

            if(position == value.Length)
                // Reached end of formula but group has not been closed.
                throw new TexParseException($"Illegal end,  missing '{closeChar}'!");

            position++;
            return result.ToString();
        }

        private TexFormula ReadScript(TexFormula formula, string value, ref int position)
        {
            if(position == value.Length)
                throw new TexParseException("illegal end, missing script!");

            SkipWhiteSpace(value, ref position);
            var ch = value[position];
            if(ch == leftGroupChar)
                return Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar));
            position++;
            return Parse(ch.ToString());
        }

        private Atom ProcessCommand(TexFormula formula, string value, ref int position, string command)
        {
            SkipWhiteSpace(value, ref position);

            switch(command)
            {
                case "frac":
                    // Command is fraction.

                    var numeratorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar));
                    SkipWhiteSpace(value, ref position);
                    var denominatorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar));
                    if(numeratorFormula.RootAtom is null || denominatorFormula.RootAtom is null)
                        throw new TexParseException("Both numerator and denominator of a fraction can't be empty!");

                    return new FractionAtom(numeratorFormula.RootAtom, denominatorFormula.RootAtom, true);
                case "sqrt":
                    // Command is radical.

                    SkipWhiteSpace(value, ref position);
                    if(position == value.Length)
                        throw new TexParseException("illegal end!");

                    TexFormula degreeFormula = null;
                    if(value[position] != leftBracketChar)
                        return new Radical(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar))
                            .RootAtom, degreeFormula?.RootAtom);
                    // Degree of radical- is specified.
                    degreeFormula = Parse(ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar));
                    SkipWhiteSpace(value, ref position);

                    return new Radical(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar))
                        .RootAtom, degreeFormula?.RootAtom);
            }

            throw new TexParseException("Invalid command.");
        }

        private void ProcessEscapeSequence(TexFormula formula, string value, ref int position)
        {
            var result = new StringBuilder();
            position++;
            while(position < value.Length)
            {
                var ch = value[position];
                var isEnd = position == value.Length - 1;
                if(!char.IsLetter(ch) || isEnd)
                {
                    // Escape sequence has ended.
                    if(isEnd)
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

            SymbolAtom symbolAtom = null;

            try
            {
                symbolAtom = SymbolAtom.GetAtom(command);
            } catch(SymbolNotFoundException)
            {
            }

            var predefinedFormula = GetFormula(command);

            if(symbolAtom != null)
            {
                // Symbol was found.

                formula.Add(AttachScripts(formula, value, ref position, symbolAtom));
            }
            else if(predefinedFormula != null)
            {
                // Predefined formula was found.

                formula.Add(AttachScripts(formula, value, ref position, predefinedFormula.RootAtom));
            }
            else if(command.Equals("nbsp"))
            {
                // Space was found.

                formula.Add(AttachScripts(formula, value, ref position, new SpaceAtom()));
            }
            else if(textStyles.Contains(command))
            {
                // Text style was found.

                SkipWhiteSpace(value, ref position);
                var styledFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar));
                styledFormula.TextStyle = command;
                formula.Add(AttachScripts(formula, value, ref position, styledFormula.RootAtom));
            }
            else if(commands.Contains(command))
            {
                // Command was found.

                formula.Add(AttachScripts(formula, value, ref position, ProcessCommand(formula, value, ref position,
                    command)));
            }
            else
            {
                // Escape sequence is invalid.
                throw new TexParseException("Unknown symbol or command or predefined TeXFormula: '" + command + "'");
            }

        }

        private Atom AttachScripts(TexFormula formula, string value, ref int position, Atom atom)
        {
            SkipWhiteSpace(value, ref position);

            if(position == value.Length)
                return atom;

            // Check for prime marks.
            var primesRowAtom = new RowAtom();
            var i = position + 1;
            while(i < value.Length)
            {
                if(value[i] == primeChar)
                    primesRowAtom.Add(SymbolAtom.GetAtom("prime"));
                else if(!IsWhiteSpace(value[i]))
                    break;
                position++;
                i++;
            }

            // Attach prime marks as superscript, if any were found.
            if(primesRowAtom.Elements.Count > 0)
                atom = new ScriptsAtom(atom, null, primesRowAtom);

            TexFormula superscriptFormula = null;
            TexFormula subscriptFormula = null;

            var ch = value[position];
            switch(ch)
            {
                case superScriptChar:
                    // Attach superscript.
                    position++;
                    superscriptFormula = ReadScript(formula, value, ref position);

                    SkipWhiteSpace(value, ref position);
                    if(position < value.Length && value[position] == subScriptChar)
                    {
                        // Attach subscript also.
                        position++;
                        subscriptFormula = ReadScript(formula, value, ref position);
                    }
                    break;
                case subScriptChar:
                    // Add subscript.
                    position++;
                    subscriptFormula = ReadScript(formula, value, ref position);

                    SkipWhiteSpace(value, ref position);
                    if(position < value.Length && value[position] == superScriptChar)
                    {
                        // Attach superscript also.
                        position++;
                        superscriptFormula = ReadScript(formula, value, ref position);
                    }
                    break;
            }

            if(superscriptFormula is null && subscriptFormula is null)
                return atom;

            // Check whether to return Big Operator or Scripts.
            if(atom.GetRightType() == TexAtomType.BigOperator)
                return new BigOperatorAtom(atom, subscriptFormula?.RootAtom,
                    superscriptFormula?.RootAtom);
            return new ScriptsAtom(atom, subscriptFormula?.RootAtom,
                superscriptFormula?.RootAtom);
        }

        private Atom ConvertCharacter(TexFormula formula, string value, ref int position, char character)
        {
            position++;
            if(IsSymbol(character))
            {
                // Character is symbol.
                var symbolName = symbols[character];
                if(symbolName is null)
                    throw new TexParseException($"Unknown character : '{character}'");

                try
                {
                    return SymbolAtom.GetAtom(symbolName);
                } catch(SymbolNotFoundException e)
                {
                    throw new TexParseException(
                        $"The character '{character}' was mapped to an unknown symbol with the name '{symbolName}'!", e);
                }
            }
            // Character is alpha-numeric.
            return new CharAtom(character, formula.TextStyle);
        }

        private void SkipWhiteSpace(string value, ref int position)
        {
            while(position < value.Length && IsWhiteSpace(value[position]))
                position++;
        }
    }
}
