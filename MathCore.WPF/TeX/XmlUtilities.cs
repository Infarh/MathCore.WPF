using System;
using System.Globalization;
using System.Xml.Linq;

namespace MathCore.WPF.TeX
{
    internal static class XmlUtilities
    {
        public static bool AttributeBooleanValue(this XElement element, string attributeName, bool? defaultValue = null)
        {
            var attribute = element.Attribute(attributeName);
            if(attribute != null) return bool.Parse(attribute.Value);
            if(defaultValue != null)
                return defaultValue.Value;
            throw new InvalidOperationException();
        }

        public static int AttributeInt32Value(this XElement element, string attributeName, int? defaultValue = null)
        {
            var attribute = element.Attribute(attributeName);
            if(attribute != null) return int.Parse(attribute.Value);
            if(defaultValue != null)
                return defaultValue.Value;
            throw new InvalidOperationException();
        }

        public static double AttributeDoubleValue(this XElement element, string attributeName, double? defaultValue = null)
        {
            var attribute = element.Attribute(attributeName);
            if(attribute == null)
            {
                if(defaultValue != null)
                    return defaultValue.Value;
                throw new InvalidOperationException();
            }
            var nfi = new CultureInfo("en-US", false).NumberFormat;
            return double.Parse(attribute.Value, nfi);
        }

        public static string AttributeValue(this XElement element, string attributeName, string defaultValue = null)
        {
            var attribute = element.Attribute(attributeName);
            if(attribute != null) return attribute.Value;
            if(defaultValue != null)
                return defaultValue;
            throw new InvalidOperationException();
        }
    }
}
