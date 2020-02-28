using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace System.Windows
{
    public static class ContentControlExtensions
    {
        public static void DeserializeContentFrom([NotNull] this ContentControl control, [NotNull] XmlReader reader) => control.Content = XamlReader.Load(reader);
        public static void DeserializeContentFrom([NotNull] this ContentControl control, TextReader reader) => control.DeserializeContentFrom(XmlReader.Create(reader));
        public static void DeserializeContentFrom([NotNull] this ContentControl control, [NotNull] Stream stream) => control.Content = XamlReader.Load(stream);

        public static void SerializeContentTo([NotNull] this ContentControl control, [NotNull] XmlWriter writer) => XamlWriter.Save(control.Content, writer);
        public static void SerializeContentTo([NotNull] this ContentControl control, [NotNull] TextWriter writer) => XamlWriter.Save(control.Content, writer);
        public static void SerializeContentTo([NotNull] this ContentControl control, [NotNull] Stream stream) => XamlWriter.Save(control.Content, stream);
    }
}