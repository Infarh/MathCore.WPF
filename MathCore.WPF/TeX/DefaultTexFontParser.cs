using System.Diagnostics;
using System.Reflection;
using System.Windows.Media;
using System.Xml.Linq;

namespace MathCore.WPF.TeX;

/// <summary>Parses information for DefaultTeXFont settings from XML file</summary>
internal class DefaultTexFontParser
{
    private const int __FontIdCount = 4;
    private const string __FontsDirectory = "Fonts/";

    private static readonly Dictionary<string, int> __RangeTypeMappings;
    private static readonly Dictionary<string, ICharChildParser> __CharChildParsers;

    static DefaultTexFontParser()
    {
        __RangeTypeMappings = [];
        __CharChildParsers  = [];

        SetRangeTypeMappings();
        SetCharChildParsers();
    }

    private static void SetRangeTypeMappings()
    {
        __RangeTypeMappings.Add("numbers", (int)TexCharKind.Numbers);
        __RangeTypeMappings.Add("capitals", (int)TexCharKind.Capitals);
        __RangeTypeMappings.Add("small", (int)TexCharKind.Small);
    }

    private static void SetCharChildParsers()
    {
        __CharChildParsers.Add("Kern", new KernParser());
        __CharChildParsers.Add("Lig", new LigParser());
        __CharChildParsers.Add("NextLarger", new NextLargerParser());
        __CharChildParsers.Add("Extension", new ExtensionParser());
    }

    private Dictionary<string, CharFont[]> _ParsedTextStyles;

    private readonly XElement _RootElement;

    public DefaultTexFontParser()
    {
        var doc = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream($"{TexUtilities.ResourcesStylesNamespace}DefaultTexFont.xml"));
        _RootElement = doc.Root;
        ParseTextStyleMappings();
    }

    public TexFontInfo[] GetFontDescriptions()
    {
        var result = new TexFontInfo[__FontIdCount];

        var font_descriptions = _RootElement.Element("FontDescriptions");
        if (font_descriptions == null) return result;
        foreach(var font_element in font_descriptions.Elements("Font"))
        {
            var font_name = font_element.AttributeValue("name");
            var font_id   = font_element.AttributeInt32Value("id");
            var space    = font_element.AttributeDoubleValue("space");
            var x_height  = font_element.AttributeDoubleValue("xHeight");
            var quad     = font_element.AttributeDoubleValue("quad");
            var skew_char = font_element.AttributeInt32Value("skewChar", -1);

            var font     = CreateFont(font_name);
            var font_info = new TexFontInfo(font_id, font, x_height, space, quad);
            if(skew_char != -1)
                font_info.SkewCharacter = (char)skew_char;

            foreach(var char_element in font_element.Elements("Char"))
                ProcessCharElement(char_element, font_info);

            if(result[font_id] != null)
                throw new InvalidOperationException($"Multiple entries for font with ID {font_id}.");
            result[font_id] = font_info;
        }

        return result;
    }

    private static void ProcessCharElement(XElement CharElement, TexFontInfo FontInfo)
    {
        var character = (char)CharElement.AttributeInt32Value("code");

        var metrics = new double[4];
        metrics[TexFontUtilities.MetricsWidth]  = CharElement.AttributeDoubleValue("width", 0d);
        metrics[TexFontUtilities.MetricsHeight] = CharElement.AttributeDoubleValue("height", 0d);
        metrics[TexFontUtilities.MetricsDepth]  = CharElement.AttributeDoubleValue("depth", 0d);
        metrics[TexFontUtilities.MetricsItalic] = CharElement.AttributeDoubleValue("italic", 0d);
        FontInfo.SetMetrics(character, metrics);

        foreach(var child_element in CharElement.Elements())
        {
            var parser = __CharChildParsers[child_element.Name.ToString()];
            if(parser is null)
                throw new InvalidOperationException("Unknown element type.");
            parser.Parse(child_element, character, FontInfo);
        }
    }

    public Dictionary<string, CharFont> GetSymbolMappings()
    {
        var result = new Dictionary<string, CharFont>();

        var symbol_mappings_element = _RootElement.Element("SymbolMappings");
        if(symbol_mappings_element is null)
            throw new InvalidOperationException("Cannot find SymbolMappings element.");

        foreach(var mapping_element in symbol_mappings_element.Elements("SymbolMapping"))
        {
            var symbol_name = mapping_element.AttributeValue("name");
            var character  = mapping_element.AttributeInt32Value("ch");
            var font_id     = mapping_element.AttributeInt32Value("fontId");

            result.Add(symbol_name, new((char)character, font_id));
        }

        if(!result.ContainsKey("sqrt"))
            throw new InvalidOperationException("Cannot find SymbolMap element for 'sqrt'.");

        return result;
    }

    public string[] GetDefaultTextStyleMappings()
    {
        var result = new string[3];

        var default_text_style_mappings = _RootElement.Element("DefaultTextStyleMapping");
        if(default_text_style_mappings is null)
            throw new InvalidOperationException("Cannot find DefaultTextStyleMapping element.");

        foreach(var mapping_element in default_text_style_mappings.Elements("MapStyle"))
        {
            var code        = mapping_element.AttributeValue("code");
            var code_mapping = __RangeTypeMappings[code];

            var text_style_name = mapping_element.AttributeValue("textStyle");
            //var textStyleMapping = parsedTextStyles[textStyleName];

            var char_fonts = _ParsedTextStyles[text_style_name];
            Debug.Assert(char_fonts[code_mapping] != null);

            result[code_mapping] = text_style_name;
        }

        return result;
    }

    public Dictionary<string, double> GetParameters()
    {
        var parameters = _RootElement.Element("Parameters");
        if(parameters is null)
            throw new InvalidOperationException("Cannot find Parameters element.");

        return parameters.Attributes()
           .ToDictionary(attribute => attribute.Name.ToString(), attribute => parameters.AttributeDoubleValue(attribute.Name.ToString()));
    }

    public Dictionary<string, object> GetGeneralSettings()
    {
        var result = new Dictionary<string, object>();

        var general_settings = _RootElement.Element("GeneralSettings");
        if(general_settings is null)
            throw new InvalidOperationException("Cannot find GeneralSettings element.");

        result.Add("mufontid", general_settings.AttributeInt32Value("mufontid"));
        result.Add("spacefontid", general_settings.AttributeInt32Value("spacefontid"));
        result.Add("scriptfactor", general_settings.AttributeDoubleValue("scriptfactor"));
        result.Add("scriptscriptfactor", general_settings.AttributeDoubleValue("scriptscriptfactor"));

        return result;
    }

    public Dictionary<string, CharFont[]> GetTextStyleMappings() => _ParsedTextStyles;

    private void ParseTextStyleMappings()
    {
        _ParsedTextStyles = [];

        var text_style_mappings = _RootElement.Element("TextStyleMappings");
        if(text_style_mappings is null)
            throw new InvalidOperationException("Cannot find TextStyleMappings element.");

        foreach(var mapping_element in text_style_mappings.Elements("TextStyleMapping"))
        {
            var text_style_name = mapping_element.AttributeValue("name");
            var char_fonts     = new CharFont[3];
            foreach(var map_range_element in mapping_element.Elements("MapRange"))
            {
                var font_id      = map_range_element.AttributeInt32Value("fontId");
                var character   = map_range_element.AttributeInt32Value("start");
                var code        = map_range_element.AttributeValue("code");
                var code_mapping = __RangeTypeMappings[code];

                char_fonts[code_mapping] = new((char)character, font_id);
            }
            _ParsedTextStyles.Add(text_style_name, char_fonts);
        }
    }

    private static GlyphTypeface CreateFont(string name)
    {
        using var memory_package  = new MemoryPackage();
        using var font_stream     = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{TexUtilities.ResourcesFontsNamespace}{name}");
        var       typeface_source = memory_package.CreatePart(font_stream);

        GlyphTypeface glyph_typeface = new(typeface_source);

        memory_package.DeletePart(typeface_source);

        return glyph_typeface;
    }

    public sealed class ExtensionParser : ICharChildParser
    {
        public void Parse(XElement element, char character, TexFontInfo FontInfo)
        {
            var extension_chars = new int[4];
            extension_chars[TexFontUtilities.ExtensionRepeat] = element.AttributeInt32Value("rep");
            extension_chars[TexFontUtilities.ExtensionTop] = element.AttributeInt32Value("top",
                (int)TexCharKind.None);
            extension_chars[TexFontUtilities.ExtensionMiddle] = element.AttributeInt32Value("mid",
                (int)TexCharKind.None);
            extension_chars[TexFontUtilities.ExtensionBottom] = element.AttributeInt32Value("bot",
                (int)TexCharKind.None);

            FontInfo.SetExtensions(character, extension_chars);
        }
    }

    public sealed class KernParser : ICharChildParser
    {
        public void Parse(XElement element, char character, TexFontInfo FontInfo) => FontInfo.AddKern(character, (char)element.AttributeInt32Value("code"),
            element.AttributeDoubleValue("val"));
    }

    public sealed class LigParser : ICharChildParser
    {
        public void Parse(XElement element, char character, TexFontInfo FontInfo) => FontInfo.AddLigature(character, (char)element.AttributeInt32Value("code"),
            (char)element.AttributeInt32Value("ligCode"));
    }

    public sealed class NextLargerParser : ICharChildParser
    {
        public void Parse(XElement element, char character, TexFontInfo FontInfo) => FontInfo.SetNextLarger(character, (char)element.AttributeInt32Value("code"),
            element.AttributeInt32Value("fontId"));
    }

    public interface ICharChildParser
    {
        void Parse(XElement element, char character, TexFontInfo FontInfo);
    }
}