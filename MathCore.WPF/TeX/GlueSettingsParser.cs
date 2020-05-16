using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

// Parses information about glue settings from XML file.
namespace MathCore.WPF.TeX
{
    internal class GlueSettingsParser
    {
        private static readonly Dictionary<string, TexAtomType> typeMappings;
        private static readonly Dictionary<string, TexStyle> styleMappings;

        static GlueSettingsParser()
        {
            typeMappings = new Dictionary<string, TexAtomType>();
            styleMappings = new Dictionary<string, TexStyle>();

            SetTypeMappings();
            SetStyleMappings();
        }

        private static Glue CreateGlue(XElement type, string name)
        {
            var names = new[] { "space", "stretch", "shrink" };
            var values = new double[names.Length];
            for(var i = 0; i < names.Length; i++)
                values[i] = type.AttributeDoubleValue(names[i], 0d);
            return new Glue(values[0], values[1], values[2], name);
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
            typeMappings.Add("inner", TexAtomType.Inner);
        }

        private static void SetStyleMappings()
        {
            styleMappings.Add("display", (int)TexStyle.Display / 2);
            styleMappings.Add("text", (TexStyle)((int)TexStyle.Text / 2));
            styleMappings.Add("script", (TexStyle)((int)TexStyle.Script / 2));
            styleMappings.Add("script_script", (TexStyle)((int)TexStyle.ScriptScript / 2));
        }

        private List<Glue> glueTypes;
        private Dictionary<string, int> glueTypeMappings;

        private readonly XElement rootElement;

        public GlueSettingsParser()
        {
            var doc = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream(
                $"{TexUtilities.ResourcesStylesNamespace}GlueSettings.xml"));
            rootElement = doc.Root;
            ParseGlueTypes();
        }

        public List<Glue> GetGlueTypes() => glueTypes;

        public int[,,] GetGlueRules()
        {
            var rules = new int[typeMappings.Count, typeMappings.Count, styleMappings.Count];

            var GlobalElement = rootElement.Element("GlueTable");
            if(GlobalElement is null) return rules;
            foreach(var element in GlobalElement.Elements("Glue"))
            {
                var leftType = typeMappings[element.AttributeValue("lefttype")];
                var rightType = typeMappings[element.AttributeValue("righttype")];
                var glueType = glueTypeMappings[element.AttributeValue("gluetype")];

                foreach(var style in element.Elements("Style").Select(e => e.AttributeValue("name")))
                    rules[(int)leftType, (int)rightType, (int)styleMappings[style]] = glueType;
            }

            return rules;
        }

        private void ParseGlueTypes()
        {
            glueTypes = new List<Glue>();
            glueTypeMappings = new Dictionary<string, int>();

            var defaultIndex = -1;
            var index = 0;

            var GlobalElement = rootElement.Element("GlueTypes");
            if(GlobalElement != null)
            {
                foreach(var element in GlobalElement.Elements("GlueType"))
                {
                    var name = element.AttributeValue("name");
                    var glue = CreateGlue(element, name);
                    if(name.Equals("default", StringComparison.InvariantCultureIgnoreCase))
                        defaultIndex = index;
                    glueTypes.Add(glue);
                    index++;
                }
            }

            // Create default glue type if it does not exist.
            if(defaultIndex < 0)
            {
                defaultIndex = index;
                glueTypes.Add(new Glue(0, 0, 0, "default"));
            }

            // Insure that default glue type is first in list.
            if(defaultIndex > 0)
            {
                var tempGlueType = glueTypes[defaultIndex];
                glueTypes[defaultIndex] = glueTypes[0];
                glueTypes[0] = tempGlueType;
            }

            // Create dictionary of reverse mappings.
            for(var i = 0; i < glueTypes.Count; i++)
                glueTypeMappings.Add(glueTypes[i].Name, i);
        }
    }
}