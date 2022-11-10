using System.Globalization;
using System.Windows.Input;

namespace MathCore.WPF;

public class InputCultureManager
{
    public static InputCultureManager Current { get; } = new();

    private CultureInfo _Culture = InputLanguageManager.Current.CurrentInputLanguage;
    public CultureInfo Culture
    {
        get => _Culture;
        set => InputLanguageManager.Current.CurrentInputLanguage = value;
    }

    public event EventHandler CultureChanged;

    private InputCultureManager() => InputLanguageManager.Current.InputLanguageChanged += OnInputLanguageChanged;

    private void OnInputLanguageChanged(object Sender, InputLanguageEventArgs E)
    {
        _Culture = E.NewLanguage;
        CultureChanged?.Invoke(this, EventArgs.Empty);
    }
}