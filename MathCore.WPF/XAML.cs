using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF;

/// <summary>Генератор содержимого по указанному файлу разметки XAML</summary>
/// <remarks>Инициализация нового генератора разметки</remarks>
/// <param name="URI">Указатель на источник разметки</param>
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedMember.Global
public class XAML(string? URI) : MarkupExtension
{
    /// <summary>Указатель на источник разметки</summary>
    public string? URI { get; set; } = URI;

    /// <summary>Инициализация нового генератора разметки</summary>
    public XAML() : this(null) { }

    /// <inheritdoc />
    public override object? ProvideValue(IServiceProvider ServiceProvider) => new Binding(nameof(XAMLContentValue.Content)) { Source = new XAMLContentValue(URI) };
}