using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

// Parse definitions of symbols from XML files.
namespace MathCore.WPF.TeX
{
    internal class TexSymbolParser
    {
        private static readonly Dictionary<string, TexAtomType> typeMappings;

        static TexSymbolParser()
        {
            typeMappings = new Dictionary<string, TexAtomType>();

            SetTypeMappings();
        }

        private static void SetTypeMappings()
        {
            typeMappings.Add("ord", TexAtomType.Ordinary);
            typeMappings.Add("op", TexAtomType.BigOperator);
            typeMappings.Add("bin", TexAtomType.BinaryOperator);
            typeMappings.Add("rel", TexAtomType.Relation);
            typeMappings.Add("open", TexAtomType.Opening);
            typeMappings.Add("close", TexAtomType.Closing);
            typeMappings.Add("punct", TexAtomType.Punctuation);
            typeMappings.Add("acc", TexAtomType.Accent);
        }

        private readonly XElement rootElement;

        public TexSymbolParser()
        {
            var doc = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream(
                $"{TexUtilities.ResourcesStylesNamespace}TexSymbols.xml"));
            rootElement = doc.Root;
        }

        public Dictionary<string, SymbolAtom> GetSymbols()
        {
            var result = new Dictionary<string, SymbolAtom>();

            foreach(var symbolElement in rootElement.Elements("Symbol"))
            {
                var symbolName = symbolElement.AttributeValue("name");
                var symbolType = symbolElement.AttributeValue("type");
                var symbolIsDelimeter = symbolElement.AttributeBooleanValue("del", false);

                result.Add(symbolName, new SymbolAtom(symbolName, typeMappings[symbolType],
                    symbolIsDelimeter));
            }

            return result;
        }
    }
}
