using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;

namespace MathCore.WPF.TeX;

/// <summary>Parses settings for predefined formulas from XML file</summary>
internal class TexPredefinedFormulaSettingsParser
{
    private static void AddToMap(IEnumerable<XElement> MapList, string[] table)
    {
        foreach(var map in MapList)
        {
            var character = map.AttributeValue("char");
            var symbol    = map.AttributeValue("symbol");
            Debug.Assert(character != null);
            Debug.Assert(symbol != null);
            Debug.Assert(character.Length == 1);
            table[character[0]] = symbol;
        }
    }

    private readonly XElement _RootElement;

    public TexPredefinedFormulaSettingsParser()
    {
        var doc = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream($"{TexUtilities.ResourcesStylesNamespace}TexFormulaSettings.xml"));
        _RootElement = doc.Root;
    }

    public string[] GetSymbolMappings()
    {
        var mappings     = new string[TexFontInfo.CharCodesCount];
        var char_to_symbol = _RootElement.Element("CharacterToSymbolMappings");
        if(char_to_symbol != null)
            AddToMap(char_to_symbol.Elements("Map"), mappings);
        return mappings;
    }

    public string[] GetDelimiterMappings()
    {
        var mappings        = new string[TexFontInfo.CharCodesCount];
        var char_to_delimiter = _RootElement.Element("CharacterToDelimiterMappings");
        if(char_to_delimiter != null)
            AddToMap(char_to_delimiter.Elements("Map"), mappings);
        return mappings;
    }

    public HashSet<string> GetTextStyles()
    {
        var result = new HashSet<string>();

        var text_styles = _RootElement.Element("TextStyles");
        if(text_styles is null) return result;
        foreach(var name in text_styles.Elements("TextStyle").Select(TextStyleElement => TextStyleElement.AttributeValue("name")))
        {
            Debug.Assert(name != null);
            result.Add(name);
        }

        return result;
    }
}