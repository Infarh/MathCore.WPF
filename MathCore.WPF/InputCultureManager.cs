using System.Globalization;
using System.Windows.Input;

namespace MathCore.WPF;

/// <summary>Менеджер текущей культуры ввода.</summary>
public class InputCultureManager
{
    /// <summary>Текущий экземпляр менеджера культуры ввода.</summary>
    public static InputCultureManager Current { get; } = new();

    /// <summary>Текущая культура ввода.</summary>
    private CultureInfo _Culture = InputLanguageManager.Current.CurrentInputLanguage;

    /// <summary>Возвращает или задает текущую культуру ввода.</summary>
    public CultureInfo Culture
    {
        get => _Culture;
        set => InputLanguageManager.Current.CurrentInputLanguage = value;
    }

    /// <summary>Событие, возникающее при изменении текущей культуры ввода.</summary>
    public event EventHandler? CultureChanged;

    /// <summary>Конструктор менеджера культуры ввода.</summary>
    private InputCultureManager() 
        => InputLanguageManager.Current.InputLanguageChanged += OnInputLanguageChanged;

    /// <summary>Обработчик события изменения текущей культуры ввода.</summary>
    /// <param name="Sender">Источник события.</param>
    /// <param name="E">Аргументы события.</param>
    private void OnInputLanguageChanged(object Sender, InputLanguageEventArgs E)
    {
        // Обновляем текущую культуру ввода.
        _Culture = E.NewLanguage;
        // Вызываем событие CultureChanged.
        CultureChanged?.Invoke(this, EventArgs.Empty);
    }
}