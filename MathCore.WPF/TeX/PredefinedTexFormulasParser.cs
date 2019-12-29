using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Xml.Linq;

// Parses definitions of predefined formulas from XML file.
namespace MathCore.WPF.TeX
{
    internal class TexPredefinedFormulaParser
    {
        private static readonly Dictionary<string, Type> typeMappings;
        private static readonly Dictionary<string, ArgumentValueParser> argValueParsers;
        private static readonly Dictionary<string, ActionParser> actionParsers;
        private static TexFormulaParser formulaParser;

        static TexPredefinedFormulaParser()
        {
            typeMappings = new Dictionary<string, Type>();
            argValueParsers = new Dictionary<string, ArgumentValueParser>();
            actionParsers = new Dictionary<string, ActionParser>();
            formulaParser = new TexFormulaParser();

            typeMappings.Add("Formula", typeof(TexFormula));
            typeMappings.Add("string", typeof(string));
            typeMappings.Add("double", typeof(double));
            typeMappings.Add("int", typeof(int));
            typeMappings.Add("bool", typeof(bool));
            typeMappings.Add("char", typeof(char));
            typeMappings.Add("Color", typeof(Color));
            typeMappings.Add("Unit", typeof(TexUnit));
            typeMappings.Add("AtomType", typeof(TexAtomType));

            actionParsers.Add("CreateFormula", new CreateTeXFormulaParser());
            actionParsers.Add("MethodInvocation", new MethodInvocationParser());
            actionParsers.Add("Return", new ReturnParser());

            argValueParsers.Add("TeXFormula", new TeXFormulaValueParser());
            argValueParsers.Add("string", new StringValueParser());
            argValueParsers.Add("double", new DoubleValueParser());
            argValueParsers.Add("int", new IntValueParser());
            argValueParsers.Add("bool", new BooleanValueParser());
            argValueParsers.Add("char", new CharValueParser());
            argValueParsers.Add("Color", new ColorConstantValueParser());
            argValueParsers.Add("Unit", new EnumParser(typeof(TexUnit)));
            argValueParsers.Add("AtomType", new EnumParser(typeof(TexAtomType)));
        }

        private static Type[] GetArgumentTypes(IEnumerable<XElement> args)
        {
            var result = new List<Type>();

            foreach(var type in args.Select(arg => arg.AttributeValue("type")).Select(t => typeMappings[t]))
            {
                Debug.Assert(type != null);
                result.Add(type);
            }

            return result.ToArray();
        }

        private static object[] GetArgumentValues(Dictionary<string, TexFormula> tempFormulas, IEnumerable<XElement> args)
        {
            var result = new List<object>();
            foreach(var curArg in args)
            {
                var typeName = curArg.AttributeValue("type");
                var value = curArg.AttributeValue("value");

                var parser = argValueParsers[typeName];
                parser.TempFormulas = tempFormulas;
                result.Add(parser.Parse(value, typeName));
            }

            return result.ToArray();
        }

        private readonly XElement rootElement;

        public TexPredefinedFormulaParser()
        {
            var doc = XDocument.Load(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"{TexUtilities.ResourcesStylesNamespace}PredefinedTexFormulas.xml"));
            rootElement = doc.Root;
        }

        public void Parse(Dictionary<string, TexFormula> predefinedTeXFormulas)
        {
            if(!rootElement.AttributeBooleanValue("enabled", true)) return;
            foreach(var formulaElement in rootElement.Elements("TeXFormula"))
            {
                if(!formulaElement.AttributeBooleanValue("enabled", true)) continue;
                var formulaName = formulaElement.AttributeValue("name");
                predefinedTeXFormulas.Add(formulaName, ParseFormula(formulaName, formulaElement));
            }
        }

        public static TexFormula ParseFormula(string formulaName, XElement formulaElement)
        {
            var tempFormulas = new Dictionary<string, TexFormula>();
            foreach(var element in formulaElement.Elements())
            {
                var parser = actionParsers[element.Name.ToString()];
                if(parser == null)
                    continue;

                parser.TempFormulas = tempFormulas;
                parser.Parse(element);
                if(parser is ReturnParser)
                    return ((ReturnParser)parser).Result;
            }
            return null;
        }

        public sealed class MethodInvocationParser : ActionParser
        {
            public override void Parse(XElement element)
            {
                var methodName = element.AttributeValue("name");
                var objectName = element.AttributeValue("formula");
                var args = element.Elements("Argument");

                var formula = TempFormulas[objectName];
                Debug.Assert(formula != null);

                var argTypes = GetArgumentTypes(args);
                var argValues = GetArgumentValues(TempFormulas, args);

                var helper = new TexFormulaHelper(formula);
                typeof(TexFormulaHelper).GetMethod(methodName, argTypes).Invoke(helper, argValues);
            }
        }

        public sealed class CreateTeXFormulaParser : ActionParser
        {
            public override void Parse(XElement element)
            {
                var name = element.AttributeValue("name");
                var args = element.Elements("Argument");

                var argTypes = GetArgumentTypes(args);
                var argValues = GetArgumentValues(TempFormulas, args);

                Debug.Assert(argValues.Length == 1);
                var parser = new TexFormulaParser();
                var formula = parser.Parse((string)argValues[0]);

                TempFormulas.Add(name, formula);
            }
        }

        public sealed class ReturnParser : ActionParser
        {
            public TexFormula Result { get; private set; }

            public override void Parse(XElement element)
            {
                var name = element.AttributeValue("name");
                var result = TempFormulas[name];
                Debug.Assert(result != null);
                Result = result;
            }
        }

        public sealed class DoubleValueParser : ArgumentValueParser
        {
            public override object Parse(string value, string type) => double.Parse(value);
        }

        public sealed class CharValueParser : ArgumentValueParser
        {
            public override object Parse(string value, string type)
            {
                Debug.Assert(value.Length == 1);
                return value[0];
            }
        }

        public sealed class BooleanValueParser : ArgumentValueParser
        {
            public override object Parse(string value, string type) => bool.Parse(value);
        }

        public sealed class IntValueParser : ArgumentValueParser
        {
            public override object Parse(string value, string type) => int.Parse(value);
        }

        public sealed class StringValueParser : ArgumentValueParser
        {
            public override object Parse(string value, string type) => value;
        }

        public sealed class TeXFormulaValueParser : ArgumentValueParser
        {
            public override object Parse(string value, string type)
            {
                if(value == null)
                    return null;

                var formula = TempFormulas[value];
                Debug.Assert(formula != null);
                return formula;
            }
        }

        public class ColorConstantValueParser : ArgumentValueParser
        {
            public override object Parse(string value, string type) => typeof(Color).GetField(value).GetValue(null);
        }

        public sealed class EnumParser : ArgumentValueParser
        {
            private readonly Type enumType;

            public EnumParser(Type enumType) { this.enumType = enumType; }

            public override object Parse(string value, string type) => Enum.Parse(enumType, value);
        }

        public abstract class ActionParser : ParserBase
        {
            public abstract void Parse(XElement element);
        }

        public abstract class ArgumentValueParser : ParserBase
        {
            public abstract object Parse(string value, string type);
        }

        public abstract class ParserBase
        {
            public Dictionary<string, TexFormula> TempFormulas { get; set; }
        }
    }
}
