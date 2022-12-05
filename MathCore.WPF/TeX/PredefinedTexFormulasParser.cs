using System.Diagnostics;
using System.Reflection;
using System.Windows.Media;
using System.Xml.Linq;
// ReSharper disable UnusedParameter.Global

// Parses definitions of predefined formulas from XML file.
namespace MathCore.WPF.TeX;

internal class TexPredefinedFormulaParser
{
    private static readonly Dictionary<string, Type> __TypeMappings;
    private static readonly Dictionary<string, ArgumentValueParser> __ArgValueParsers;
    private static readonly Dictionary<string, ActionParser?> __ActionParsers;
    //private static TexFormulaParser __FormulaParser;

    static TexPredefinedFormulaParser()
    {
        __TypeMappings    = new Dictionary<string, Type>();
        __ArgValueParsers = new Dictionary<string, ArgumentValueParser>();
        __ActionParsers   = new Dictionary<string, ActionParser>();
        //__FormulaParser   = new TexFormulaParser();

        __TypeMappings.Add("Formula", typeof(TexFormula));
        __TypeMappings.Add("string", typeof(string));
        __TypeMappings.Add("double", typeof(double));
        __TypeMappings.Add("int", typeof(int));
        __TypeMappings.Add("bool", typeof(bool));
        __TypeMappings.Add("char", typeof(char));
        __TypeMappings.Add("Color", typeof(Color));
        __TypeMappings.Add("Unit", typeof(TexUnit));
        __TypeMappings.Add("AtomType", typeof(TexAtomType));

        __ActionParsers.Add("CreateFormula", new CreateTeXFormulaParser());
        __ActionParsers.Add("MethodInvocation", new MethodInvocationParser());
        __ActionParsers.Add("Return", new ReturnParser());

        __ArgValueParsers.Add("TeXFormula", new TeXFormulaValueParser());
        __ArgValueParsers.Add("string", new StringValueParser());
        __ArgValueParsers.Add("double", new DoubleValueParser());
        __ArgValueParsers.Add("int", new IntValueParser());
        __ArgValueParsers.Add("bool", new BooleanValueParser());
        __ArgValueParsers.Add("char", new CharValueParser());
        __ArgValueParsers.Add("Color", new ColorConstantValueParser());
        __ArgValueParsers.Add("Unit", new EnumParser(typeof(TexUnit)));
        __ArgValueParsers.Add("AtomType", new EnumParser(typeof(TexAtomType)));
    }

    private static Type[] GetArgumentTypes(IEnumerable<XElement> args)
    {
        var result = new List<Type>();

        foreach(var type in args.Select(arg => arg.AttributeValue("type")).Select(t => __TypeMappings[t]))
        {
            Debug.Assert(type != null);
            result.Add(type);
        }

        return result.ToArray();
    }

    private static object[] GetArgumentValues(Dictionary<string, TexFormula> TempFormulas, IEnumerable<XElement> args)
    {
        var result = new List<object>();
        foreach(var cur_arg in args)
        {
            var type_name = cur_arg.AttributeValue("type");
            var value    = cur_arg.AttributeValue("value");

            var parser = __ArgValueParsers[type_name];
            parser.TempFormulas = TempFormulas;
            result.Add(parser.Parse(value, type_name));
        }

        return result.ToArray();
    }

    private readonly XElement _RootElement;

    public TexPredefinedFormulaParser()
    {
        var doc = XDocument.Load(Assembly.GetExecutingAssembly()
           .GetManifestResourceStream($"{TexUtilities.ResourcesStylesNamespace}PredefinedTexFormulas.xml")!);
        _RootElement = doc.Root;
    }

    public void Parse(Dictionary<string, TexFormula> PredefinedTeXFormulas)
    {
        if(!_RootElement.AttributeBooleanValue("enabled", true)) return;
        foreach(var formula_element in _RootElement.Elements("TeXFormula"))
        {
            if(!formula_element.AttributeBooleanValue("enabled", true)) continue;
            var formula_name = formula_element.AttributeValue("name");
            PredefinedTeXFormulas.Add(formula_name, ParseFormula(formula_name, formula_element));
        }
    }

    public static TexFormula? ParseFormula(string FormulaName, XElement FormulaElement)
    {
        var temp_formulas = new Dictionary<string, TexFormula>();
        foreach(var element in FormulaElement.Elements())
        {
            var parser = __ActionParsers[element.Name.ToString()];
            if(parser is null)
                continue;

            parser.TempFormulas = temp_formulas;
            parser.Parse(element);
            if(parser is ReturnParser return_parser)
                return return_parser.Result;
        }
        return null;
    }

    public sealed class MethodInvocationParser : ActionParser
    {
        public override void Parse(XElement element)
        {
            var method_name = element.AttributeValue("name");
            var object_name = element.AttributeValue("formula");
            var args       = (element.Elements("Argument") ?? throw new InvalidOperationException("Отсутствует элемент Arguments")).ToArray();

            var formula = TempFormulas[object_name];
            Debug.Assert(formula != null);

            var arg_types  = GetArgumentTypes(args);
            var arg_values = GetArgumentValues(TempFormulas, args);

            var helper = new TexFormulaHelper(formula);
            typeof(TexFormulaHelper).GetMethod(method_name, arg_types).Invoke(helper, arg_values);
        }
    }

    public sealed class CreateTeXFormulaParser : ActionParser
    {
        public override void Parse(XElement element)
        {
            var name = element.AttributeValue("name");
            var args = element.Elements("Argument") ?? throw new InvalidOperationException("Отсутствует элемент Arguments");

            //var arg_types  = GetArgumentTypes(args);
            var arg_values = GetArgumentValues(TempFormulas, args);

            Debug.Assert(arg_values.Length == 1);
            var parser  = new TexFormulaParser();
            var formula = parser.Parse((string)arg_values[0]);

            TempFormulas.Add(name, formula);
        }
    }

    public sealed class ReturnParser : ActionParser
    {
        public TexFormula Result { get; private set; }

        public override void Parse(XElement element)
        {
            var name   = element.AttributeValue("name");
            var result = TempFormulas[name];
            Debug.Assert(result != null);
            Result = result;
        }
    }

    public sealed class DoubleValueParser : ArgumentValueParser
    {
        public override object Parse(string? value, string type) => double.Parse(value.NotNull());
    }

    public sealed class CharValueParser : ArgumentValueParser
    {
        public override object Parse(string? value, string type)
        {
            Debug.Assert(value.Length == 1);
            return value[0];
        }
    }

    public sealed class BooleanValueParser : ArgumentValueParser
    {
        public override object Parse(string? value, string type) => bool.Parse(value.NotNull());
    }

    public sealed class IntValueParser : ArgumentValueParser
    {
        public override object Parse(string? value, string type) => int.Parse(value.NotNull());
    }

    public sealed class StringValueParser : ArgumentValueParser
    {
        public override object? Parse(string? value, string type) => value;
    }

    public sealed class TeXFormulaValueParser : ArgumentValueParser
    {
        public override object? Parse(string? value, string type)
        {
            if(value is null)
                return null;

            var formula = TempFormulas[value];
            Debug.Assert(formula != null);
            return formula;
        }
    }

    public class ColorConstantValueParser : ArgumentValueParser
    {
        public override object? Parse(string? value, string type) => typeof(Color).GetField(value.NotNull()).GetValue(null);
    }

    public sealed class EnumParser : ArgumentValueParser
    {
        private readonly Type _EnumType;

        public EnumParser(Type EnumType) => this._EnumType = EnumType;

        public override object Parse(string? value, string type) => Enum.Parse(_EnumType, value.NotNull());
    }

    public abstract class ActionParser : ParserBase
    {
        public abstract void Parse(XElement element);
    }

    public abstract class ArgumentValueParser : ParserBase
    {
        public abstract object? Parse(string? value, string type);
    }

    public abstract class ParserBase
    {
        public Dictionary<string, TexFormula> TempFormulas { get; set; }
    }
}