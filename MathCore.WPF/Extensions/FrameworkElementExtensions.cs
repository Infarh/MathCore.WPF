using System.IO;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Windows
{
    /// <summary>Класс методов-расширений для класса FrameworkElement</summary>
    public static class FrameworkElementExtensions
    {
        public static void RegisterForNotification([NotNull] this FrameworkElement element, string PropertyName, PropertyChangedCallback callback)
        {
            var binding = new Binding(PropertyName) { Source = element };
            var prop = DependencyProperty.RegisterAttached($"ListenAttached{PropertyName}",
                    typeof(object),
                    element.GetType(),
                    new PropertyMetadata(callback));

            element.SetBinding(prop, binding);
        }

        public static void Serialize([NotNull] this UIElement element, [NotNull] Stream stream) => XamlWriter.Save(element, stream);

        public static void Serialize([NotNull] this UIElement element, [NotNull] TextWriter writer) => XamlWriter.Save(element, writer);

        public static void Serialize([NotNull] this UIElement element, [NotNull] XmlWriter writer) => XamlWriter.Save(element, writer);

        [NotNull]
        public static string SerializeToStr([NotNull] this UIElement element)
        {
            var result = new StringBuilder();
            var writer_settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };

            var writer = XmlWriter.Create(result, writer_settings);
            var manager = new XamlDesignerSerializationManager(writer) { XamlWriterMode = XamlWriterMode.Expression };
            XamlWriter.Save(element, manager);
            return result.ToString();
        }

        [NotNull] public static XDocument SerializeToXml([NotNull] this UIElement element) => XDocument.Parse(element.SerializeToStr());

        //public static XmlDocument Serialize(this UIElement element)
        //{
        //    var output = new StringBuilder();
        //    XamlWriter.Save(element, new XamlDesignerSerializationManager(XmlWriter.Create(output, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true })) { XamlWriterMode = XamlWriterMode.Expression });
        //    var xml = new XmlDocument();
        //    xml.LoadXml(output.ToString());
        //    return xml;
        //}
    }
}