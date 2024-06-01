using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MathCore.WPF;

public static class ControlEx
{
    #region Attached property BrushBackgroundMouseOver : Brush - Кисть, применяемая к фону при наведении мыши

    /// <summary>Кисть, применяемая к фону при наведении мыши</summary>
    public static readonly DependencyProperty BrushBackgroundMouseOverProperty =
        DependencyProperty.RegisterAttached(
            "BrushBackgroundMouseOver",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Кисть, применяемая к фону при наведении мыши</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushBackgroundMouseOver(DependencyObject d, Brush value) => d.SetValue(BrushBackgroundMouseOverProperty, value);

    /// <summary>Кисть, применяемая к фону при наведении мыши</summary>
    public static Brush GetBrushBackgroundMouseOver(DependencyObject d) => (Brush)d.GetValue(BrushBackgroundMouseOverProperty);

    #endregion

    #region Attached property BrushForegroundMouseOver : Brush - Основная кисть при наведении мыши

    /// <summary>Основная кисть при наведении мыши</summary>
    public static readonly DependencyProperty BrushForegroundMouseOverProperty =
        DependencyProperty.RegisterAttached(
            "BrushForegroundMouseOver",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Основная кисть при наведении мыши</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushForegroundMouseOver(DependencyObject d, Brush value) => d.SetValue(BrushForegroundMouseOverProperty, value);

    /// <summary>Основная кисть при наведении мыши</summary>
    public static Brush GetBrushForegroundMouseOver(DependencyObject d) => (Brush)d.GetValue(BrushForegroundMouseOverProperty);

    #endregion

    #region Attached property BrushBorderMouseOver : Brush - Кисть, применяемая к рамке при наведении мыши

    /// <summary>Кисть, применяемая к рамке при наведении мыши</summary>
    public static readonly DependencyProperty BrushBorderMouseOverProperty =
        DependencyProperty.RegisterAttached(
            "BrushBorderMouseOver",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Кисть, применяемая к рамке при наведении мыши</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushBorderMouseOver(DependencyObject d, Brush value) => d.SetValue(BrushBorderMouseOverProperty, value);

    /// <summary>Кисть, применяемая к рамке при наведении мыши</summary>
    public static Brush GetBrushBorderMouseOver(DependencyObject d) => (Brush)d.GetValue(BrushBorderMouseOverProperty);

    #endregion


    #region Attached property BrushBackgroundMousePressed : Brush - Кисть, применяемая к фону при нажатии мыши

    /// <summary>Кисть, применяемая к фону при нажатии мыши</summary>
    public static readonly DependencyProperty BrushBackgroundMousePressedProperty =
        DependencyProperty.RegisterAttached(
            "BrushBackgroundMousePressed",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Кисть, применяемая к фону при нажатии мыши</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushBackgroundMousePressed(DependencyObject d, Brush value) => d.SetValue(BrushBackgroundMousePressedProperty, value);

    /// <summary>Кисть, применяемая к фону при нажатии мыши</summary>
    public static Brush GetBrushBackgroundMousePressed(DependencyObject d) => (Brush)d.GetValue(BrushBackgroundMousePressedProperty);

    #endregion

    #region Attached property BrushForegroundMousePressed : Brush - Основная кисть при нажатии мыши

    /// <summary>Основная кисть при нажатии мыши</summary>
    public static readonly DependencyProperty BrushForegroundMousePressedProperty =
        DependencyProperty.RegisterAttached(
            "BrushForegroundMousePressed",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Основная кисть при нажатии мыши</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushForegroundMousePressed(DependencyObject d, Brush value) => d.SetValue(BrushForegroundMousePressedProperty, value);

    /// <summary>Основная кисть при нажатии мыши</summary>
    public static Brush GetBrushForegroundMousePressed(DependencyObject d) => (Brush)d.GetValue(BrushForegroundMousePressedProperty);

    #endregion

    #region Attached property BrushBorderMousePressed : Brush - Кисть, применяемая к рамке при нажатии мыши

    /// <summary>Кисть, применяемая к рамке при нажатии мыши</summary>
    public static readonly DependencyProperty BrushBorderMousePressedProperty =
        DependencyProperty.RegisterAttached(
            "BrushBorderMousePressed",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Кисть, применяемая к рамке при нажатии мыши</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushBorderMousePressed(DependencyObject d, Brush value) => d.SetValue(BrushBorderMousePressedProperty, value);

    /// <summary>Кисть, применяемая к рамке при нажатии мыши</summary>
    public static Brush GetBrushBorderMousePressed(DependencyObject d) => (Brush)d.GetValue(BrushBorderMousePressedProperty);

    #endregion


    #region Attached property BrushBackgroundDisabled : Brush - Кисть, применяемая к фону при выключении элемента

    /// <summary>Кисть, применяемая к фону при выключении элемента</summary>
    public static readonly DependencyProperty BrushBackgroundDisabledProperty =
        DependencyProperty.RegisterAttached(
            "BrushBackgroundDisabled",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Кисть, применяемая к фону при выключении элемента</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushBackgroundDisabled(DependencyObject d, Brush value) => d.SetValue(BrushBackgroundDisabledProperty, value);

    /// <summary>Кисть, применяемая к фону при выключении элемента</summary>
    public static Brush GetBrushBackgroundDisabled(DependencyObject d) => (Brush)d.GetValue(BrushBackgroundDisabledProperty);

    #endregion

    #region Attached property BrushForegroundDisabled : Brush - Основная кисть при выключении элемента

    /// <summary>Основная кисть при выключении элемента</summary>
    public static readonly DependencyProperty BrushForegroundDisabledProperty =
        DependencyProperty.RegisterAttached(
            "BrushForegroundDisabled",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Основная кисть при выключении элемента</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushForegroundDisabled(DependencyObject d, Brush value) => d.SetValue(BrushForegroundDisabledProperty, value);

    /// <summary>Основная кисть при выключении элемента</summary>
    public static Brush GetBrushForegroundDisabled(DependencyObject d) => (Brush)d.GetValue(BrushForegroundDisabledProperty);

    #endregion

    #region Attached property BrushBorderDisabled : Brush - Кисть, применяемая к рамке при выключении элемента

    /// <summary>Кисть, применяемая к рамке при выключении элемента</summary>
    public static readonly DependencyProperty BrushBorderDisabledProperty =
        DependencyProperty.RegisterAttached(
            "BrushBorderDisabled",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Кисть, применяемая к рамке при выключении элемента</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushBorderDisabled(DependencyObject d, Brush value) => d.SetValue(BrushBorderDisabledProperty, value);

    /// <summary>Кисть, применяемая к рамке при выключении элемента</summary>
    public static Brush GetBrushBorderDisabled(DependencyObject d) => (Brush)d.GetValue(BrushBorderDisabledProperty);

    #endregion


    #region Attached property BrushBackgroundSelected : Brush - Кисть, применяемая к фону при выборе элемента

    /// <summary>Кисть, применяемая к фону при выборе элемента</summary>
    public static readonly DependencyProperty BrushBackgroundSelectedProperty =
        DependencyProperty.RegisterAttached(
            "BrushBackgroundSelected",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Кисть, применяемая к фону при выборе элемента</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushBackgroundSelected(DependencyObject d, Brush value) => d.SetValue(BrushBackgroundSelectedProperty, value);

    /// <summary>Кисть, применяемая к фону при выборе элемента</summary>
    public static Brush GetBrushBackgroundSelected(DependencyObject d) => (Brush)d.GetValue(BrushBackgroundSelectedProperty);

    #endregion

    #region Attached property BrushForegroundSelected : Brush - Основная кисть при выборе элемента

    /// <summary>Основная кисть при выборе элемента</summary>
    public static readonly DependencyProperty BrushForegroundSelectedProperty =
        DependencyProperty.RegisterAttached(
            "BrushForegroundSelected",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Основная кисть при выборе элемента</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushForegroundSelected(DependencyObject d, Brush value) => d.SetValue(BrushForegroundSelectedProperty, value);

    /// <summary>Основная кисть при выборе элемента</summary>
    public static Brush GetBrushForegroundSelected(DependencyObject d) => (Brush)d.GetValue(BrushForegroundSelectedProperty);

    #endregion

    #region Attached property BrushBorderSelected : Brush - Кисть, применяемая к рамке при выборе элемента

    /// <summary>Кисть, применяемая к рамке при выборе элемента</summary>
    public static readonly DependencyProperty BrushBorderSelectedProperty =
        DependencyProperty.RegisterAttached(
            "BrushBorderSelected",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Кисть, применяемая к рамке при выборе элемента</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushBorderSelected(DependencyObject d, Brush value) => d.SetValue(BrushBorderSelectedProperty, value);

    /// <summary>Кисть, применяемая к рамке при выборе элемента</summary>
    public static Brush GetBrushBorderSelected(DependencyObject d) => (Brush)d.GetValue(BrushBorderSelectedProperty);

    #endregion


    #region Attached property BrushBackgroundSelectedMouseOver : Brush - Кисть, применяемая к фону при наведении мыши на выбранный элемент

    /// <summary>Кисть, применяемая к фону при наведении мыши на выбранный элемент</summary>
    public static readonly DependencyProperty BrushBackgroundSelectedMouseOverProperty =
        DependencyProperty.RegisterAttached(
            "BrushBackgroundSelectedMouseOver",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Кисть, применяемая к фону при наведении мыши на выбранный элемент</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushBackgroundSelectedMouseOver(DependencyObject d, Brush value) => d.SetValue(BrushBackgroundSelectedMouseOverProperty, value);

    /// <summary>Кисть, применяемая к фону при наведении мыши на выбранный элемент</summary>
    public static Brush GetBrushBackgroundSelectedMouseOver(DependencyObject d) => (Brush)d.GetValue(BrushBackgroundSelectedMouseOverProperty);

    #endregion

    #region Attached property BrushForegroundSelectedMouseOver : Brush - Основная кисть при наведении мыши на выбранный элемент

    /// <summary>Основная кисть при наведении мыши на выбранный элемент</summary>
    public static readonly DependencyProperty BrushForegroundSelectedMouseOverProperty =
        DependencyProperty.RegisterAttached(
            "BrushForegroundSelectedMouseOver",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Основная кисть при наведении мыши на выбранный элемент</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushForegroundSelectedMouseOver(DependencyObject d, Brush value) => d.SetValue(BrushForegroundSelectedMouseOverProperty, value);

    /// <summary>Основная кисть при наведении мыши на выбранный элемент</summary>
    public static Brush GetBrushForegroundSelectedMouseOver(DependencyObject d) => (Brush)d.GetValue(BrushForegroundSelectedMouseOverProperty);

    #endregion

    #region Attached property BrushBorderSelectedMouseOver : Brush - Кисть, применяемая к рамке при наведении мыши на выбранный элемент

    /// <summary>Кисть, применяемая к рамке при наведении мыши на выбранный элемент</summary>
    public static readonly DependencyProperty BrushBorderSelectedMouseOverProperty =
        DependencyProperty.RegisterAttached(
            "BrushBorderSelectedMouseOver",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Кисть, применяемая к рамке при наведении мыши на выбранный элемент</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushBorderSelectedMouseOver(DependencyObject d, Brush value) => d.SetValue(BrushBorderSelectedMouseOverProperty, value);

    /// <summary>Кисть, применяемая к рамке при наведении мыши на выбранный элемент</summary>
    public static Brush GetBrushBorderSelectedMouseOver(DependencyObject d) => (Brush)d.GetValue(BrushBorderSelectedMouseOverProperty);

    #endregion


    #region Attached property BrushBackgroundSelectedMousePressed : Brush - Кисть, применяемая к фону при нажатии мыши на выбранный элемент

    /// <summary>Кисть, применяемая к фону при нажатии мыши на выбранный элемент</summary>
    public static readonly DependencyProperty BrushBackgroundSelectedMousePressedProperty =
        DependencyProperty.RegisterAttached(
            "BrushBackgroundSelectedMousePressed",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Кисть, применяемая к фону при нажатии мыши на выбранный элемент</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushBackgroundSelectedMousePressed(DependencyObject d, Brush value) => d.SetValue(BrushBackgroundSelectedMousePressedProperty, value);

    /// <summary>Кисть, применяемая к фону при нажатии мыши на выбранный элемент</summary>
    public static Brush GetBrushBackgroundSelectedMousePressed(DependencyObject d) => (Brush)d.GetValue(BrushBackgroundSelectedMousePressedProperty);

    #endregion

    #region Attached property BrushForegroundSelectedMousePressed : Brush - Основная кисть при нажатии мыши на выбранный элемент

    /// <summary>Основная кисть при нажатии мыши на выбранный элемент</summary>
    public static readonly DependencyProperty BrushForegroundSelectedMousePressedProperty =
        DependencyProperty.RegisterAttached(
            "BrushForegroundSelectedMousePressed",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Основная кисть при нажатии мыши на выбранный элемент</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushForegroundSelectedMousePressed(DependencyObject d, Brush value) => d.SetValue(BrushForegroundSelectedMousePressedProperty, value);

    /// <summary>Основная кисть при нажатии мыши на выбранный элемент</summary>
    public static Brush GetBrushForegroundSelectedMousePressed(DependencyObject d) => (Brush)d.GetValue(BrushForegroundSelectedMousePressedProperty);

    #endregion

    #region Attached property BrushBorderSelectedMousePressed : Brush - Кисть, применяемая к рамке при нажатии мыши на выбранный элемент

    /// <summary>Кисть, применяемая к рамке при нажатии мыши на выбранный элемент</summary>
    public static readonly DependencyProperty BrushBorderSelectedMousePressedProperty =
        DependencyProperty.RegisterAttached(
            "BrushBorderSelectedMousePressed",
            typeof(Brush),
            typeof(ControlEx),
            new(default(Brush)));

    /// <summary>Кисть, применяемая к рамке при нажатии мыши на выбранный элемент</summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetBrushBorderSelectedMousePressed(DependencyObject d, Brush value) => d.SetValue(BrushBorderSelectedMousePressedProperty, value);

    /// <summary>Кисть, применяемая к рамке при нажатии мыши на выбранный элемент</summary>
    public static Brush GetBrushBorderSelectedMousePressed(DependencyObject d) => (Brush)d.GetValue(BrushBorderSelectedMousePressedProperty);

    #endregion

}