using System.Windows;
using System.Windows.Input;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF;

public static class UI
{
    /// <summary>Коллекция горячих клавиш, ассоциированных с элементом</summary>
    public static readonly DependencyProperty InputBindingProperty =
        DependencyProperty.RegisterAttached(
            "InputBinding",
            typeof(InputBinding),
            typeof(UI),
            new PropertyMetadata(default(InputBinding), OnInputBindingChanged));

    /// <summary>Обработчик события изменения значения свойства <see cref="InputBindingProperty"/></summary>
    /// <param name="D">Элемент, с которым ассоциирована коллекция горячих клавиш</param>
    /// <param name="E">Аргумент события, содержащий изменившееся значение свойства</param>
    private static void OnInputBindingChanged(DependencyObject D, DependencyPropertyChangedEventArgs E) => ((UIElement)D).InputBindings.Add((InputBinding)E.NewValue);

    /// <summary>Установка значения свойства коллекции ассоциированных с элементом клавиш</summary>
    /// <param name="element"></param>
    /// <param name="value"></param>
    public static void SetInputBinding(DependencyObject element, InputBinding value) => element.SetValue(InputBindingProperty, value);

    public static InputBinding GetInputBinding(DependencyObject element) => (InputBinding)element.GetValue(InputBindingProperty);

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
    public static void SetHotKeys(DependencyObject d, GlobalHotKeysCollection value) => d.SetValue(HotKeysProperty, value ?? throw new ArgumentNullException(nameof(value)));

    /// <summary>Глобальные горячие клавиши</summary>
    public static GlobalHotKeysCollection GetHotKeys(DependencyObject element)
    {
        if (element.GetValue(HotKeysProperty) is GlobalHotKeysCollection collection) 
            return collection;

        collection = [];
        if (element is FrameworkElement framework_element)
            framework_element.Unloaded += (e, _) =>
            {
                if(((DependencyObject)e).GetValue(HotKeysProperty) is GlobalHotKeysCollection keys)
                    keys.Dispose();
            };
        SetHotKeys(element, collection);
        return collection;
    }

    #endregion
}