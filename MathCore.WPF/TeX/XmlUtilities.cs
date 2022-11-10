using System.Globalization;
using System.Xml.Linq;

namespace MathCore.WPF.TeX;

internal static class XmlUtilities
{
    public static bool AttributeBooleanValue(this XElement element, string AttributeName, bool? DefaultValue = null)
    {
        var attribute = element.Attribute(AttributeName);
        if(attribute != null) return bool.Parse(attribute.Value);
        if(DefaultValue != null)
            return DefaultValue.Value;
        throw new InvalidOperationException();
    }

    public static int AttributeInt32Value(this XElement element, string AttributeName, int? DefaultValue = null)
    {
        var attribute = element.Attribute(AttributeName);
        if(attribute != null) return int.Parse(attribute.Value);
        if(DefaultValue != null)
            return DefaultValue.Value;
        throw new InvalidOperationException();
    }

    public static double AttributeDoubleValue(this XElement element, string AttributeName, double? DefaultValue = null)
    {
        var attribute = element.Attribute(AttributeName);
        if(attribute is null)
        {
            if(DefaultValue != null)
                return DefaultValue.Value;
            throw new InvalidOperationException();
        }
        var nfi = new CultureInfo("en-US", false).NumberFormat;
        return double.Parse(attribute.Value, nfi);
    }

    public static string AttributeValue(this XElement element, string AttributeName, string DefaultValue = null)
    {
        var attribute = element.Attribute(AttributeName);
        if(attribute != null) return attribute.Value;
        if(DefaultValue != null)
            return DefaultValue;
        throw new InvalidOperationException();
    }
}