using System;
using System.Diagnostics;
using System.Windows.Markup;
using System.Xml;

namespace MathCore.WPF
{
    /// <summary>Генератор содержимого по указанному файлу разметки XAML</summary>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    public class XAML : MarkupExtension
    {
        /// <summary>Указатель на источник разметки</summary>
        public string? URI { get; set; }

        /// <summary>Инициализация нового генератора разметки</summary>
        public XAML() { }

        /// <summary>Инициализация нового генератора разметки</summary>
        /// <param name="URI">Указатель на источник разметки</param>
        public XAML(string? URI) => this.URI = URI;

        /// <inheritdoc />
        public override object? ProvideValue(IServiceProvider ServiceProvider)
        {
            if (string.IsNullOrWhiteSpace(URI)) return null;
            try
            {
                using var reader = XmlReader.Create(URI);
                return XamlReader.Load(reader);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
            }
            return null;
        }
    }
}
