using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace MathCore.WPF
{
    public class LanguageManager : DependencyObject
    {
        #region InputCulture

        /// <summary></summary>
        public static readonly DependencyProperty InputCultureProperty =
            DependencyProperty.Register(
                nameof(InputCulture),
                typeof(CultureInfo),
                typeof(LanguageManager),
                new PropertyMetadata(InputLanguageManager.Current.CurrentInputLanguage, (s, e) => ((LanguageManager)s).ChangeCulture((CultureInfo)e.NewValue)));


        /// <summary></summary>
        public CultureInfo InputCulture { get => (CultureInfo)GetValue(InputCultureProperty); set => SetValue(InputCultureProperty, value); }

        #endregion

        #region Singleton

        private static volatile LanguageManager __Manager;
        private static readonly object __ManagerSyncRoot = new();

        public static LanguageManager Current
        {
            get
            {
                if (__Manager != null) return __Manager;
                lock (__ManagerSyncRoot)
                {
                    if (__Manager != null) return __Manager;
                    return __Manager = new LanguageManager();
                }
            }
        }

        #endregion

        private LanguageManager() => InputLanguageManager.Current.InputLanguageChanged += OnLanguageChanged;

        private void OnLanguageChanged(object Sender, InputLanguageEventArgs E) => InputCulture = E.NewLanguage;

        private void ChangeCulture(CultureInfo culture) => InputLanguageManager.Current.CurrentInputLanguage = culture;
    }
}