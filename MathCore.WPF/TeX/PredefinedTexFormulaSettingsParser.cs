using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MathCore.WPF.TeX
{
    /// <summary>Parses settings for predefined formulas from XML file</summary>
    internal class TexPredefinedFormulaSettingsParser
    {
        private static void AddToMap(IEnumerable<XElement> mapList, string[] table)
        {
            foreach(var map in mapList)
            {
                var character = map.AttributeValue("char");
                var symbol = map.AttributeValue("symbol");
                Debug.Assert(character != null);
                Debug.Assert(symbol != null);
                Debug.Assert(character.Length == 1);
                table[character[0]] = symbol;
            }
        }

        private readonly XElement rootElement;

        public TexPredefinedFormulaSettingsParser()
        {
            var doc = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream($"{TexUtilities.ResourcesStylesNamespace}TexFormulaSettings.xml"));
            rootElement = doc.Root;
        }

        public string[] GetSymbolMappings()
        {
            var mappings = new string[TexFontInfo.CharCodesCount];
            var charToSymbol = rootElement.Element("CharacterToSymbolMappings");
            if(charToSymbol != null)
                AddToMap(charToSymbol.Elements("Map"), mappings);
            return mappings;
        }

        public string[] GetDelimiterMappings()
        {
            var mappings = new string[TexFontInfo.CharCodesCount];
            var charToDelimiter = rootElement.Element("CharacterToDelimiterMappings");
            if(charToDelimiter != null)
                AddToMap(charToDelimiter.Elements("Map"), mappings);
            return mappings;
        }

        public HashSet<string> GetTextStyles()
        {
            var result = new HashSet<string>();

            var textStyles = rootElement.Element("TextStyles");
            if(textStyles == null) return result;
            foreach(var name in textStyles.Elements("TextStyle").Select(textStyleElement => textStyleElement.AttributeValue("name")))
            {
                Debug.Assert(name != null);
                result.Add(name);
            }

            return result;
        }
    }
}
