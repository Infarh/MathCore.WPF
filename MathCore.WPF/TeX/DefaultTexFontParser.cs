using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Media;
using System.Xml.Linq;

namespace MathCore.WPF.TeX
{
    /// <summary>Parses information for DefaultTeXFont settings from XML file</summary>
    internal class DefaultTexFontParser
    {
        private const int fontIdCount = 4;
        private const string fontsDirectory = "Fonts/";

        private static readonly Dictionary<string, int> rangeTypeMappings;
        private static readonly Dictionary<string, ICharChildParser> charChildParsers;

        static DefaultTexFontParser()
        {
            rangeTypeMappings = new Dictionary<string, int>();
            charChildParsers = new Dictionary<string, ICharChildParser>();

            SetRangeTypeMappings();
            SetCharChildParsers();
        }

        private static void SetRangeTypeMappings()
        {
            rangeTypeMappings.Add("numbers", (int)TexCharKind.Numbers);
            rangeTypeMappings.Add("capitals", (int)TexCharKind.Capitals);
            rangeTypeMappings.Add("small", (int)TexCharKind.Small);
        }

        private static void SetCharChildParsers()
        {
            charChildParsers.Add("Kern", new KernParser());
            charChildParsers.Add("Lig", new LigParser());
            charChildParsers.Add("NextLarger", new NextLargerParser());
            charChildParsers.Add("Extension", new ExtensionParser());
        }

        private Dictionary<string, CharFont[]> parsedTextStyles;

        private readonly XElement rootElement;

        public DefaultTexFontParser()
        {
            var doc = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream($"{TexUtilities.ResourcesStylesNamespace}DefaultTexFont.xml"));
            rootElement = doc.Root;
            ParseTextStyleMappings();
        }

        public TexFontInfo[] GetFontDescriptions()
        {
            var result = new TexFontInfo[fontIdCount];

            var fontDescriptions = rootElement.Element("FontDescriptions");
            if(fontDescriptions != null)
            {
                foreach(var fontElement in fontDescriptions.Elements("Font"))
                {
                    var fontName = fontElement.AttributeValue("name");
                    var fontId = fontElement.AttributeInt32Value("id");
                    var space = fontElement.AttributeDoubleValue("space");
                    var xHeight = fontElement.AttributeDoubleValue("xHeight");
                    var quad = fontElement.AttributeDoubleValue("quad");
                    var skewChar = fontElement.AttributeInt32Value("skewChar", -1);

                    var font = CreateFont(fontName);
                    var fontInfo = new TexFontInfo(fontId, font, xHeight, space, quad);
                    if(skewChar != -1)
                        fontInfo.SkewCharacter = (char)skewChar;

                    foreach(var charElement in fontElement.Elements("Char"))
                        ProcessCharElement(charElement, fontInfo);

                    if(result[fontId] != null)
                        throw new InvalidOperationException($"Multiple entries for font with ID {fontId}.");
                    result[fontId] = fontInfo;
                }
            }

            return result;
        }

        private static void ProcessCharElement(XElement charElement, TexFontInfo fontInfo)
        {
            var character = (char)charElement.AttributeInt32Value("code");

            var metrics = new double[4];
            metrics[TexFontUtilities.MetricsWidth] = charElement.AttributeDoubleValue("width", 0d);
            metrics[TexFontUtilities.MetricsHeight] = charElement.AttributeDoubleValue("height", 0d);
            metrics[TexFontUtilities.MetricsDepth] = charElement.AttributeDoubleValue("depth", 0d);
            metrics[TexFontUtilities.MetricsItalic] = charElement.AttributeDoubleValue("italic", 0d);
            fontInfo.SetMetrics(character, metrics);

            foreach(var childElement in charElement.Elements())
            {
                var parser = charChildParsers[childElement.Name.ToString()];
                if(parser is null)
                    throw new InvalidOperationException("Unknown element type.");
                parser.Parse(childElement, character, fontInfo);
            }
        }

        public Dictionary<string, CharFont> GetSymbolMappings()
        {
            var result = new Dictionary<string, CharFont>();

            var symbolMappingsElement = rootElement.Element("SymbolMappings");
            if(symbolMappingsElement is null)
                throw new InvalidOperationException("Cannot find SymbolMappings element.");

            foreach(var mappingElement in symbolMappingsElement.Elements("SymbolMapping"))
            {
                var symbolName = mappingElement.AttributeValue("name");
                var character = mappingElement.AttributeInt32Value("ch");
                var fontId = mappingElement.AttributeInt32Value("fontId");

                result.Add(symbolName, new CharFont((char)character, fontId));
            }

            if(!result.ContainsKey("sqrt"))
                throw new InvalidOperationException("Cannot find SymbolMap element for 'sqrt'.");

            return result;
        }

        public string[] GetDefaultTextStyleMappings()
        {
            var result = new string[3];

            var defaultTextStyleMappings = rootElement.Element("DefaultTextStyleMapping");
            if(defaultTextStyleMappings is null)
                throw new InvalidOperationException("Cannot find DefaultTextStyleMapping element.");

            foreach(var mappingElement in defaultTextStyleMappings.Elements("MapStyle"))
            {
                var code = mappingElement.AttributeValue("code");
                var codeMapping = rangeTypeMappings[code];

                var textStyleName = mappingElement.AttributeValue("textStyle");
                //var textStyleMapping = parsedTextStyles[textStyleName];

                var charFonts = parsedTextStyles[textStyleName];
                Debug.Assert(charFonts[codeMapping] != null);

                result[codeMapping] = textStyleName;
            }

            return result;
        }

        public Dictionary<string, double> GetParameters()
        {
            var parameters = rootElement.Element("Parameters");
            if(parameters is null)
                throw new InvalidOperationException("Cannot find Parameters element.");

            return parameters.Attributes()
                .ToDictionary(attribute => attribute.Name.ToString(), attribute => parameters.AttributeDoubleValue(attribute.Name.ToString()));
        }

        public Dictionary<string, object> GetGeneralSettings()
        {
            var result = new Dictionary<string, object>();

            var generalSettings = rootElement.Element("GeneralSettings");
            if(generalSettings is null)
                throw new InvalidOperationException("Cannot find GeneralSettings element.");

            result.Add("mufontid", generalSettings.AttributeInt32Value("mufontid"));
            result.Add("spacefontid", generalSettings.AttributeInt32Value("spacefontid"));
            result.Add("scriptfactor", generalSettings.AttributeDoubleValue("scriptfactor"));
            result.Add("scriptscriptfactor", generalSettings.AttributeDoubleValue("scriptscriptfactor"));

            return result;
        }

        public Dictionary<string, CharFont[]> GetTextStyleMappings() => parsedTextStyles;

        private void ParseTextStyleMappings()
        {
            parsedTextStyles = new Dictionary<string, CharFont[]>();

            var textStyleMappings = rootElement.Element("TextStyleMappings");
            if(textStyleMappings is null)
                throw new InvalidOperationException("Cannot find TextStyleMappings element.");

            foreach(var mappingElement in textStyleMappings.Elements("TextStyleMapping"))
            {
                var textStyleName = mappingElement.AttributeValue("name");
                var charFonts = new CharFont[3];
                foreach(var mapRangeElement in mappingElement.Elements("MapRange"))
                {
                    var fontId = mapRangeElement.AttributeInt32Value("fontId");
                    var character = mapRangeElement.AttributeInt32Value("start");
                    var code = mapRangeElement.AttributeValue("code");
                    var codeMapping = rangeTypeMappings[code];

                    charFonts[codeMapping] = new CharFont((char)character, fontId);
                }
                parsedTextStyles.Add(textStyleName, charFonts);
            }
        }

        private GlyphTypeface CreateFont(string name)
        {
            GlyphTypeface glyphTypeface;
            using(var memoryPackage = new MemoryPackage())
            {
                using(var fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{TexUtilities.ResourcesFontsNamespace}{name}"))
                {
                    var typefaceSource = memoryPackage.CreatePart(fontStream);

                    glyphTypeface = new GlyphTypeface(typefaceSource);

                    memoryPackage.DeletePart(typefaceSource);
                }
            }
            return glyphTypeface;
        }

        public sealed class ExtensionParser : ICharChildParser
        {
            public void Parse(XElement element, char character, TexFontInfo fontInfo)
            {
                var extensionChars = new int[4];
                extensionChars[TexFontUtilities.ExtensionRepeat] = element.AttributeInt32Value("rep");
                extensionChars[TexFontUtilities.ExtensionTop] = element.AttributeInt32Value("top",
                    (int)TexCharKind.None);
                extensionChars[TexFontUtilities.ExtensionMiddle] = element.AttributeInt32Value("mid",
                    (int)TexCharKind.None);
                extensionChars[TexFontUtilities.ExtensionBottom] = element.AttributeInt32Value("bot",
                    (int)TexCharKind.None);

                fontInfo.SetExtensions(character, extensionChars);
            }
        }

        public sealed class KernParser : ICharChildParser
        {
            public void Parse(XElement element, char character, TexFontInfo fontInfo) => fontInfo.AddKern(character, (char)element.AttributeInt32Value("code"),
                element.AttributeDoubleValue("val"));
        }

        public sealed class LigParser : ICharChildParser
        {
            public void Parse(XElement element, char character, TexFontInfo fontInfo) => fontInfo.AddLigature(character, (char)element.AttributeInt32Value("code"),
                (char)element.AttributeInt32Value("ligCode"));
        }

        public sealed class NextLargerParser : ICharChildParser
        {
            public void Parse(XElement element, char character, TexFontInfo fontInfo) => fontInfo.SetNextLarger(character, (char)element.AttributeInt32Value("code"),
                element.AttributeInt32Value("fontId"));
        }

        public interface ICharChildParser
        {
            void Parse(XElement element, char character, TexFontInfo fontInfo);
        }
    }

    sealed class MemoryPackage : IDisposable
    {
        private static int packageCounter;

        private readonly Uri packageUri = new Uri("payload://memorypackage" + Interlocked.Increment(ref packageCounter), UriKind.Absolute);
        private readonly Package package = Package.Open(new MemoryStream(), FileMode.Create);
        private int partCounter;

        public MemoryPackage()
        {
            PackageStore.AddPackage(this.packageUri, this.package);
        }

        public Uri CreatePart(Stream stream, string contentType = "application/octet-stream")
        {
            var partUri = new Uri("/stream" + ++partCounter, UriKind.Relative);

            var part = package.CreatePart(partUri, contentType);

            Debug.Assert(part != null, "part != null");
            using(var partStream = part.GetStream())
                CopyStream(stream, partStream);

            // Each packUri must be globally unique because WPF might perform some caching based on it.
            return PackUriHelper.Create(this.packageUri, partUri);
        }

        public void DeletePart(Uri packUri) => package.DeletePart(PackUriHelper.GetPartUri(packUri));

        public void Dispose()
        {
            PackageStore.RemovePackage(packageUri);
            package.Close();
        }

        private static void CopyStream(Stream source, Stream destination)
        {
            const int bufferSize = 4096;

            var buffer = new byte[bufferSize];
            int read;
            while((read = source.Read(buffer, 0, buffer.Length)) != 0)
                destination.Write(buffer, 0, read);
        }
    }
}