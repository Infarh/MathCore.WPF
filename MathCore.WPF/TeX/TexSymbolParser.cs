using System.Reflection;
using System.Xml.Linq;

// Parse definitions of symbols from XML files.
namespace MathCore.WPF.TeX;

internal class TexSymbolParser
{
    private static readonly Dictionary<string, TexAtomType> __TypeMappings;

    static TexSymbolParser()
    {
        __TypeMappings = [];

        SetTypeMappings();
    }

    private static void SetTypeMappings()
    {
        __TypeMappings.Add("ord", TexAtomType.Ordinary);
        __TypeMappings.Add("op", TexAtomType.BigOperator);
        __TypeMappings.Add("bin", TexAtomType.BinaryOperator);
        __TypeMappings.Add("rel", TexAtomType.Relation);
        __TypeMappings.Add("open", TexAtomType.Opening);
        __TypeMappings.Add("close", TexAtomType.Closing);
        __TypeMappings.Add("punct", TexAtomType.Punctuation);
        __TypeMappings.Add("acc", TexAtomType.Accent);
    }

    private readonly XElement _RootElement;

    public TexSymbolParser()
    {
        var doc = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream(
            $"{TexUtilities.ResourcesStylesNamespace}TexSymbols.xml"));
        _RootElement = doc.Root;
    }

    public Dictionary<string, SymbolAtom> GetSymbols()
    {
        var result = new Dictionary<string, SymbolAtom>();

        foreach(var symbol_element in _RootElement.Elements("Symbol"))
        {
            var symbol_name        = symbol_element.AttributeValue("name");
            var symbol_type        = symbol_element.AttributeValue("type");
            var symbol_is_delimeter = symbol_element.AttributeBooleanValue("del", false);

            result.Add(symbol_name, new(symbol_name, __TypeMappings[symbol_type],
                symbol_is_delimeter));
        }

        return result;
    }
}