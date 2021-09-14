using System.Windows;
using System.Windows.Input;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF
{
    public static class UI
    {
        public static readonly DependencyProperty InputBindingProperty =
            DependencyProperty.RegisterAttached(
                "InputBinding",
                typeof(InputBinding),
                typeof(UI),
                new PropertyMetadata(default(InputBinding), OnInputBindingChanged));

        private static void OnInputBindingChanged(DependencyObject D, DependencyPropertyChangedEventArgs E) => ((UIElement)D).InputBindings.Add((InputBinding)E.NewValue);

        public static void SetInputBinding([NotNull] DependencyObject element, InputBinding value) => element.SetValue(InputBindingProperty, value);

        public static InputBinding GetInputBinding([NotNull] DependencyObject element) => (InputBinding)element.GetValue(InputBindingProperty);

        #region Attached property HotKeys : GlobalHotKeysCollection - Глобальные горячие клавиши

        /// <summary>Глобальные горячие клавиши</summary>
        public static readonly DependencyProperty HotKeysProperty =
            DependencyProperty.RegisterAttached(
                "ShadowHotKeys",
                typeof(GlobalHotKeysCollection),
                typeof(UI),
                new PropertyMetadata(default(GlobalHotKeysCollection)));

        /// <summary>Глобальные горячие клавиши</summary>
        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static void SetHotKeys(DependencyObject d, GlobalHotKeysCollection value) => d.SetValue(HotKeysProperty, value);

        /// <summary>Глобальные горячие клавиши</summary>
        public static GlobalHotKeysCollection GetHotKeys(DependencyObject element)
        {
            if (element.GetValue(HotKeysProperty) is not GlobalHotKeysCollection collection)
                SetHotKeys(element, collection = new GlobalHotKeysCollection());
            return collection;
        }

        #endregion
    }
}