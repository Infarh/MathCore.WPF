using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MathCore.WPF;

/// <summary>Класс прикрепляемых свойств-зависимости для работы с <see cref="PasswordBox"/></summary>
// ReSharper disable once UnusedMember.Global
public static class PasswordBoxEx
{
    #region AttachProperty

    /// <summary>Прикрепляемое свойство-зависимости, устанавливающее связь для дальнейшей работы с <see cref="PasswordBox"/></summary>
    public static readonly DependencyProperty AttachProperty =
        DependencyProperty.RegisterAttached(
            "Attach",
            typeof(bool),
            typeof(PasswordBoxEx),
            new PropertyMetadata(default(bool), OnAttachChanged));

    /// <summary>Установка значения свойства присоединения</summary>
    /// <param name="o">Объект для которого производится установка значения</param>
    /// <param name="v">Устанавливаемое значение</param>
    [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
    public static void SetAttach(DependencyObject o, bool v) => o.SetValue(AttachProperty, v);

    public static bool GetAttach(DependencyObject dp) => (bool)dp.GetValue(AttachProperty);

    private static void OnAttachChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.OldValue) ((PasswordBox)sender).PasswordChanged -= PasswordChanged;
        if ((bool)e.NewValue) ((PasswordBox)sender).PasswordChanged += PasswordChanged;
    }

    #endregion

    #region PasswordProperty

    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached(
            "Password",
            typeof(string),
            typeof(PasswordBoxEx),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnPasswordPropertyChanged));

    public static void SetPassword(DependencyObject dp, string value) => dp.SetValue(PasswordProperty, value);

    [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
    public static string GetPassword(DependencyObject dp) => (string)dp.GetValue(PasswordProperty);

    private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var password_box = (PasswordBox)sender;
        password_box.PasswordChanged -= PasswordChanged;
        if (!GetIsUpdating(password_box)) password_box.Password = (string)e.NewValue;
        password_box.PasswordChanged += PasswordChanged;
    }

    private static void PasswordChanged(object sender, RoutedEventArgs e)
    {
        var password_box = (PasswordBox)sender;
        SetIsUpdating(password_box, true);
        SetPassword(password_box, password_box.Password);
        SetIsUpdating(password_box, false);
    }

    #endregion

    #region IsUpdatingProperty

    private static readonly DependencyProperty IsUpdatingProperty =
        DependencyProperty.RegisterAttached(
            "IsUpdating",
            typeof(bool),
            typeof(PasswordBoxEx));

    [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
    private static void SetIsUpdating(DependencyObject dp, bool value) => dp.SetValue(IsUpdatingProperty, value);

    private static bool GetIsUpdating(DependencyObject dp) => (bool)dp.GetValue(IsUpdatingProperty);

    #endregion

    #region Attached property WhatermarkText : string - Текст, замещающий пустое пространство при отсутствии ввода пароля

    /// <summary>Текст, замещающий пустое пространство при отсутствии ввода пароля</summary>
    public static readonly DependencyProperty WaterMarkTextProperty =
        DependencyProperty.RegisterAttached(
            "WaterMarkText",
            typeof(string),
            typeof(PasswordBoxEx),
            new PropertyMetadata("Введите пароль"));

    public static void SetWaterMarkText(DependencyObject element, string value) => element.SetValue(WaterMarkTextProperty, value);

    public static string GetWaterMarkText(DependencyObject element) => (string)element.GetValue(WaterMarkTextProperty);

    #endregion

    #region Attached property WatermarkOpacity : double - Прозрачность водяного знака

    /// <summary>Прозрачность водяного знака</summary>
    public static readonly DependencyProperty WatermarkOpacityProperty =
        DependencyProperty.RegisterAttached(
            "WatermarkOpacity",
            typeof(double),
            typeof(PasswordBoxEx),
            new PropertyMetadata(0.8));

    /// <summary>Прозрачность водяного знака</summary>
    [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
    public static void SetWatermarkOpacity(DependencyObject d, double value) => d.SetValue(WatermarkOpacityProperty, value);

    /// <summary>Прозрачность водяного знака</summary>
    public static double GetWatermarkOpacity(DependencyObject d) => (double)d.GetValue(WatermarkOpacityProperty);

    #endregion

    #region Attached property WatermarkTextBrush : Brush - Кисть рисования текста водяного знака

    /// <summary>Кисть рисования текста водяного знака</summary>
    public static readonly DependencyProperty WatermarkTextBrushProperty =
        DependencyProperty.RegisterAttached(
            "WatermarkTextBrush",
            typeof(Brush),
            typeof(PasswordBoxEx),
            new PropertyMetadata(Brushes.DarkGray));

    /// <summary>Кисть рисования текста водяного знака</summary>
    [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
    public static void SetWatermarkTextBrush(DependencyObject d, Brush value) => d.SetValue(WatermarkTextBrushProperty, value);

    /// <summary>Кисть рисования текста водяного знака</summary>
    public static Brush GetWatermarkTextBrush(DependencyObject d) => (Brush)d.GetValue(WatermarkTextBrushProperty);

    #endregion

    #region Attached property WatermarkMargin : Thickness - Внешняя рамка до водяного знака

    /// <summary>Внешняя рамка до водяного знака</summary>
    public static readonly DependencyProperty WatermarkMarginProperty =
        DependencyProperty.RegisterAttached(
            "WatermarkMargin",
            typeof(Thickness),
            typeof(PasswordBoxEx),
            new PropertyMetadata(default(Thickness)));

    /// <summary>Внешняя рамка до водяного знака</summary>
    [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
    public static void SetWatermarkMargin(DependencyObject d, Thickness value) => d.SetValue(WatermarkMarginProperty, value);

    /// <summary>Внешняя рамка до водяного знака</summary>
    public static Thickness GetWatermarkMargin(DependencyObject d) => (Thickness)d.GetValue(WatermarkMarginProperty);

    #endregion
}

/*- ---------------------------------------------------------------------------------- -*/