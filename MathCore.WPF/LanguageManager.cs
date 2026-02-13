using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace MathCore.WPF;

/// <summary>Менеджер переключения языка ввода</summary>
public class LanguageManager : DependencyObject
{
    #region InputCulture

    /// <summary>Свойство зависимости для текущей культуры ввода</summary>
    public static readonly DependencyProperty InputCultureProperty =
        DependencyProperty.Register(
            nameof(InputCulture),
            typeof(CultureInfo),
            typeof(LanguageManager),
            new(InputLanguageManager.Current.CurrentInputLanguage, (s, e) => ChangeCulture((CultureInfo)e.NewValue)));


    /// <summary>Текущая культура ввода</summary>
    public CultureInfo InputCulture { get => (CultureInfo)GetValue(InputCultureProperty); set => SetValue(InputCultureProperty, value); }

    #endregion

    #region Singleton

    private static volatile LanguageManager __Manager;

    private static readonly object __ManagerSyncRoot = new();

    /// <summary>Текущий экземпляр менеджера языка ввода</summary>
    public static LanguageManager Current
    {
        get
        {
            if (__Manager != null) return __Manager;
            lock (__ManagerSyncRoot)
            {
                if (__Manager != null) return __Manager;
                return __Manager = new();
            }
        }
    }

    #endregion

    private LanguageManager() => InputLanguageManager.Current.InputLanguageChanged += OnLanguageChanged;

    private void OnLanguageChanged(object Sender, InputLanguageEventArgs E) => InputCulture = E.NewLanguage;

    /// <summary>Изменить текущую культуру ввода</summary>
    /// <param name="culture">Новая культура ввода</param>
    private static void ChangeCulture(CultureInfo culture) => InputLanguageManager.Current.CurrentInputLanguage = culture;
}