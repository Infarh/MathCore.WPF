using System.Reflection;
using System.Xml.Linq;

// Parses information about glue settings from XML file.
namespace MathCore.WPF.TeX;

internal class GlueSettingsParser
{
    private static readonly Dictionary<string, TexAtomType> __TypeMappings;
    private static readonly Dictionary<string, TexStyle> __StyleMappings;

    static GlueSettingsParser()
    {
        __TypeMappings  = [];
        __StyleMappings = [];

        SetTypeMappings();
        SetStyleMappings();
    }

    private static Glue CreateGlue(XElement type, string name)
    {
        var names  = new[] { "space", "stretch", "shrink" };
        var values = new double[names.Length];
        for(var i = 0; i < names.Length; i++)
            values[i] = type.AttributeDoubleValue(names[i], 0d);
        return new(values[0], values[1], values[2], name);
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
        __TypeMappings.Add("inner", TexAtomType.Inner);
    }

    private static void SetStyleMappings()
    {
        __StyleMappings.Add("display", (int)TexStyle.Display / 2);
        __StyleMappings.Add("text", (TexStyle)((int)TexStyle.Text / 2));
        __StyleMappings.Add("script", (TexStyle)((int)TexStyle.Script / 2));
        __StyleMappings.Add("script_script", (TexStyle)((int)TexStyle.ScriptScript / 2));
    }

    private List<Glue> _GlueTypes;
    private Dictionary<string, int> _GlueTypeMappings;

    private readonly XElement _RootElement;

    public GlueSettingsParser()
    {
        var doc = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream(
            $"{TexUtilities.ResourcesStylesNamespace}GlueSettings.xml")!);
        _RootElement = doc.Root;
        ParseGlueTypes();
    }

    public List<Glue> GetGlueTypes() => _GlueTypes;

    public int[,,] GetGlueRules()
    {
        var rules = new int[__TypeMappings.Count, __TypeMappings.Count, __StyleMappings.Count];

        var GlobalElement = _RootElement.Element("GlueTable");
        if(GlobalElement is null) return rules;
        foreach(var element in GlobalElement.Elements("Glue"))
        {
            var left_type  = __TypeMappings[element.AttributeValue("lefttype")];
            var right_type = __TypeMappings[element.AttributeValue("righttype")];
            var glue_type  = _GlueTypeMappings[element.AttributeValue("gluetype")];

            foreach(var style in element.Elements("Style").Select(e => e.AttributeValue("name")))
                rules[(int)left_type, (int)right_type, (int)__StyleMappings[style]] = glue_type;
        }

        return rules;
    }

    private void ParseGlueTypes()
    {
        _GlueTypes        = [];
        _GlueTypeMappings = [];

        var default_index = -1;
        var index        = 0;

        if(_RootElement.Element("GlueTypes") is { } global_element)
            foreach(var element in global_element.Elements("GlueType"))
            {
                var name = element.AttributeValue("name");
                var glue = CreateGlue(element, name);
                if(name.Equals("default", StringComparison.InvariantCultureIgnoreCase))
                    default_index = index;
                _GlueTypes.Add(glue);
                index++;
            }

        // Create default glue type if it does not exist.
        if(default_index < 0)
        {
            default_index = index;
            _GlueTypes.Add(new(0, 0, 0, "default"));
        }

        // Insure that default glue type is first in list.
        if(default_index > 0) 
            (_GlueTypes[default_index], _GlueTypes[0]) = (_GlueTypes[0], _GlueTypes[default_index]);

        // Create dictionary of reverse mappings.
        for(var i = 0; i < _GlueTypes.Count; i++)
            _GlueTypeMappings.Add(_GlueTypes[i].Name, i);
    }
}