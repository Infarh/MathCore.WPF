using System.IO;
using System.Windows.Markup;

namespace MathCore.WPF.Converters.Reflection;

/// <summary>Возвращает время создания файла сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyTime))]
public class AssemblyTime : AssemblyConverter
{
    /// <summary>Возвращает время создания файла сборки</summary>
    public AssemblyTime() : base(a =>
    {
        var location = a.Location;
        return string.IsNullOrEmpty(location) ? null : (object?)new FileInfo(location).CreationTime;
    })
    { }
}