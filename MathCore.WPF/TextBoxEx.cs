using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MathCore.WPF;

public static class TextBoxEx
{
    #region Attached property UpdateBindingSourceOnEnter : bool - Обновить привязку при нажатии Enter

    /// <summary>Обновить привязку при нажатии Enter</summary>
    public static readonly DependencyProperty UpdateBindingSourceOnEnterProperty =
        DependencyProperty.RegisterAttached(
            "UpdateBindingSourceOnEnter",
            typeof(bool),
            typeof(TextBoxEx),
            new(default(bool), OnUpdateBindingOnEnterChanged));

    private static void OnUpdateBindingOnEnterChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if(sender is not TextBox text_box || e.NewValue is not bool value) return;

        if (value)
            text_box.KeyDown += OnTextBoxKeyDown;
        else
            text_box.KeyDown -= OnTextBoxKeyDown;
    }

    private static void OnTextBoxKeyDown(object Sender, KeyEventArgs E)
    {
        if(E.Key != Key.Enter) return;

        var text_box = (TextBox)Sender;

        if (text_box.AcceptsReturn && !E.KeyboardDevice.IsKeyDown(Key.LeftCtrl)) 
            return;

        if (BindingOperations.GetBindingExpression(text_box, TextBox.TextProperty) is not { } binding) 
            return;

        if(binding.ParentBinding.UpdateSourceTrigger != UpdateSourceTrigger.Explicit && !E.KeyboardDevice.IsKeyDown(Key.LeftCtrl))
            return;

        binding.UpdateSource();
        E.Handled = true;
    }

    /// <summary>Обновить привязку при нажатии Enter</summary>
    [AttachedPropertyBrowsableForType(typeof(TextBox))]
    public static void SetUpdateBindingSourceOnEnter(DependencyObject d, bool value) => d.SetValue(UpdateBindingSourceOnEnterProperty, value);

    /// <summary>Обновить привязку при нажатии Enter</summary>
    public static bool GetUpdateBindingSourceOnEnter(DependencyObject d) => (bool)d.GetValue(UpdateBindingSourceOnEnterProperty);

    #endregion

    #region Attached property ValidateInputScope : bool - Проверять корректность правил InputScope

    /// <summary>Проверять корректность правил InputScope</summary>
    public static readonly DependencyProperty ValidateInputScopeProperty =
        DependencyProperty.RegisterAttached(
            "ValidateInputScope",
            typeof(bool),
            typeof(TextBoxEx),
            new(default(bool), OnValidateInputScopeChanged));

    private static void OnValidateInputScopeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if(obj is not TextBox text_box || e.NewValue is not bool value) return;

        if (value)
            text_box.PreviewTextInput += ValidateScopes_OnPreviewTextInput;
        else
            text_box.PreviewTextInput -= ValidateScopes_OnPreviewTextInput;
    }

    private static void ValidateScopes_OnPreviewTextInput(object Sender, TextCompositionEventArgs E)
    {
        if(Sender is not TextBox { InputScope: { } input_scope, Text: var current_text } text_box) return;

        var input_text = E.Text;
        var full_text = current_text + input_text;

        foreach (InputScopeName name in input_scope.Names)
            switch (name.NameValue)
            {
                case InputScopeNameValue.Time when !DateTime.TryParse(full_text, text_box.Language.GetSpecificCulture(), DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces, out _):
                case InputScopeNameValue.TimeMinorSec when !int.TryParse(full_text, out var min_sec) && min_sec is not (>= 0 and < 60):
                case InputScopeNameValue.TimeHour when !int.TryParse(input_text, out _):
                case InputScopeNameValue.OneChar when full_text.Length > 1:
                case InputScopeNameValue.FullFilePath when Path.GetInvalidPathChars().Any(c => input_text.Contains(c)):
                case InputScopeNameValue.FileName when Path.GetInvalidFileNameChars().Any(c => input_text.Contains(c)):
                case InputScopeNameValue.DateDayName when full_text.ToLower() is not (
                    "sunday" or "sun" or "su" or 
                    "monday" or "mon" or "mo" or
                    "tuesday" or "tue" or "tu" or
                    "wednesday" or "wed" or "wd" or
                    "thursday" or "thu" or "th" or
                    "friday" or "fri" or "fr" or
                    "saturday" or "sat" or "st" or 
                    "понедельник" or "пон" or "пн" or
                    "вторник" or "втр" or "вт" or
                    "среда" or "срд" or "ср" or
                    "четверг" or "чет" or "чт" or
                    "пятница" or "пят" or "пт" or
                    "суббота" or "суб" or "сб" or
                    "воскресенье" or "вос" or "вс" 
                    ):
                case InputScopeNameValue.DateDay when !int.TryParse(full_text, out var day) && day is not (>= 1 and <= 31):
                case InputScopeNameValue.DateMonth when !int.TryParse(full_text, out var month) && month is not (>= 1 and <= 12):
                case InputScopeNameValue.DateYear when !int.TryParse(full_text, out var year) && year < 1:
                case InputScopeNameValue.Date when !DateTime.TryParse(full_text, text_box.Language.GetSpecificCulture(), DateTimeStyles.AllowWhiteSpaces, out _):
                case InputScopeNameValue.Digits when !input_text.IsInt():
                case InputScopeNameValue.Number when !StringExtensions.IsDouble(full_text):
                    E.Handled = true;
                    return;
            }

        if (full_text.Length > 0 && input_scope.RegularExpression is { Length: > 0 } regex_str && !Regex.IsMatch(full_text, regex_str)) 
            E.Handled = true;
    }

    /// <summary>Проверять корректность правил InputScope</summary>
    [AttachedPropertyBrowsableForType(typeof(TextBox))]
    public static void SetValidateInputScope(DependencyObject d, bool value) => d.SetValue(ValidateInputScopeProperty, value);

    /// <summary>Проверять корректность правил InputScope</summary>
    public static bool GetValidateInputScope(DependencyObject d) => (bool)d.GetValue(ValidateInputScopeProperty);

    #endregion

    #region Attached property MouseWheelIncrement : decimal - Инкремент колёсика мышки

    /// <summary>Инкремент колёсика мышки</summary>
    public static readonly DependencyProperty MouseWheelIncrementProperty =
        DependencyProperty.RegisterAttached(
            "MouseWheelIncrement",
            typeof(decimal),
            typeof(TextBoxEx),
            new(decimal.Zero, OnMouseWheelIncrementChanged));

    private static void OnMouseWheelIncrementChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
    {
        if (e is { OldValue: decimal.Zero, NewValue: not decimal.Zero })
            ((TextBox)s).MouseWheel += OnMouseWheel;
        else if (e is { OldValue: not decimal.Zero, NewValue: decimal.Zero })
            ((TextBox)s).MouseWheel -= OnMouseWheel;
    }

    /// <summary>Инкремент колёсика мышки</summary>
    [AttachedPropertyBrowsableForType(typeof(TextBox))]
    public static void SetMouseWheelIncrement(DependencyObject d, decimal value) => d.SetValue(MouseWheelIncrementProperty, value);

    /// <summary>Инкремент колёсика мышки</summary>
    public static decimal GetMouseWheelIncrement(DependencyObject d) => (decimal)d.GetValue(MouseWheelIncrementProperty);

    #endregion

    #region Attached property MouseWheelIncrementCtrlRatio : decimal - Инкремент колёсика мышки

    /// <summary>Инкремент колёсика мышки</summary>
    public static readonly DependencyProperty MouseWheelIncrementCtrlRatioProperty =
        DependencyProperty.RegisterAttached(
            "MouseWheelIncrementCtrlRatio",
            typeof(decimal),
            typeof(TextBoxEx),
            new(0.1m));

    /// <summary>Инкремент колёсика мышки</summary>
    [AttachedPropertyBrowsableForType(typeof(TextBox))]
    public static void SetMouseWheelIncrementCtrlRatio(DependencyObject d, decimal value) => d.SetValue(MouseWheelIncrementCtrlRatioProperty, value);

    /// <summary>Инкремент колёсика мышки</summary>
    public static decimal GetMouseWheelIncrementCtrlRatio(DependencyObject d) => (decimal)d.GetValue(MouseWheelIncrementCtrlRatioProperty);

    #endregion

    private static void OnMouseWheel(object Sender, MouseWheelEventArgs E)
    {
        if (Sender is not TextBox { Text: var text } text_block) return;
        if (GetMouseWheelIncrement(text_block) is not (not decimal.Zero and var inc)) return;

        var value = decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : 0m;

        var delta = E.Delta / 120m * inc;

        if (Keyboard.IsKeyDown(Key.LeftCtrl) && GetMouseWheelIncrementCtrlRatio(text_block) is not decimal.Zero and not 1m and var ratio)
            delta = Keyboard.IsKeyDown(Key.LeftShift) ? delta / ratio : delta * ratio;

        var new_value = (value + delta);
        var new_value_text = new_value.ToString(CultureInfo.InvariantCulture);
        text_block.Text = new_value_text;
    }
}
