using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Windows;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Extensions;

public class Localizing : BindingExtension
{
    public sealed class Manager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private ResourceManager? _SourceManager;

        public ResourceManager? SourceManager
        {
            get => _SourceManager;
            set
            {
                _SourceManager = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SourceManager)));
            }
        }

        public string? Get(string? key, string? StringFormat = null)
        {
            if (_SourceManager is null || string.IsNullOrWhiteSpace(key)) return key;
            var localized_value = _SourceManager.GetString(key) ?? $":{key}:";
            return string.IsNullOrEmpty(StringFormat)
                ? localized_value
                : string.Format(StringFormat, localized_value);
        }
    }

    public enum CharCases
    {
        Default,
        Lower,
        Upper
    }

    public static readonly Manager ActiveManager = new();

    public string? Key { get; set; }

    public CharCases CharCase { get; set; }

    public Localizing()
    {
        Source = ActiveManager;
        Path = new PropertyPath(nameof(ActiveManager.SourceManager));
    }

    public Localizing(string key)
    {
        Key = key;
        Source = ActiveManager;
        Path = new PropertyPath(nameof(ActiveManager.SourceManager));
    }

    public override string ToString() => 
        Convert(ActiveManager.SourceManager, null, Key, Thread.CurrentThread.CurrentCulture) as string ?? string.Empty;

    public override object Convert(object? v, Type? t, object? p, CultureInfo? c)
    {
        var key = Key;
        var localized_value = v is not ResourceManager resource_manager || string.IsNullOrEmpty(key)
            ? $":{key}:"
            : resource_manager.GetString(key) ?? $":{key}:";

        return CharCase switch
        {
            CharCases.Lower => localized_value.ToLower(),
            CharCases.Upper => localized_value.ToUpper(),
            _ => localized_value
        };
    }
}