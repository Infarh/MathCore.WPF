using System.Windows;
using System.Windows.Media;

// ReSharper disable once CheckNamespace
namespace MathCore.WPF;

public static class ModernWindow
{
    #region Attached property HeaderContent : object - Содержимое заголовка

    /// <summary>Содержимое заголовка</summary>
    [AttachedPropertyBrowsableForType(typeof(Window))]
    public static void SetHeaderContent(DependencyObject d, object value) => d.SetValue(HeaderContentProperty, value);

    /// <summary>Содержимое заголовка</summary>
    public static object GetHeaderContent(DependencyObject d) => (object)d.GetValue(HeaderContentProperty);

    /// <summary>Содержимое заголовка</summary>
    public static readonly DependencyProperty HeaderContentProperty =
        DependencyProperty.RegisterAttached(
            "HeaderContent",
            typeof(object),
            typeof(ModernWindow),
            new PropertyMetadata(default(object)));

    #endregion

    #region Attached property HeaderHeight : double - Высота строки заголовка окна

    /// <summary>Высота строки заголовка окна</summary>
    [AttachedPropertyBrowsableForType(typeof(Window))]
    public static void SetHeaderHeight(DependencyObject d, double value) => d.SetValue(HeaderHeightProperty, value);

    /// <summary>Высота строки заголовка окна</summary>
    public static double GetHeaderHeight(DependencyObject d) => (double)d.GetValue(HeaderHeightProperty);

    /// <summary>Высота строки заголовка окна</summary>
    public static readonly DependencyProperty HeaderHeightProperty =
        DependencyProperty.RegisterAttached(
            "HeaderHeight",
            typeof(double),
            typeof(ModernWindow),
            new PropertyMetadata(30d, null, (d, v) => (double)v > 0 ? v : 0), v => (double)v >= 0);

    #endregion

    #region Attached property HeaderActiveBrush : Brush - Кисть заголовка активного окна

    /// <summary>Кисть заголовка активного окна</summary>
    [AttachedPropertyBrowsableForType(typeof(Window))]
    public static void SetHeaderActiveBrush(DependencyObject d, Brush value) => d.SetValue(HeaderActiveBrushProperty, value);

    /// <summary>Кисть заголовка активного окна</summary>
    public static Brush GetHeaderActiveBrush(DependencyObject d) => (Brush)d.GetValue(HeaderActiveBrushProperty);

    /// <summary>Кисть заголовка активного окна</summary>
    public static readonly DependencyProperty HeaderActiveBrushProperty =
        DependencyProperty.RegisterAttached(
            "HeaderActiveBrush",
            typeof(Brush),
            typeof(ModernWindow),
            new PropertyMetadata(default(Brush)));

    #endregion

    #region Attached property HeaderInActiveBrush : Brush - Кисть заголовка не активного окна

    /// <summary>Кисть заголовка не активного окна</summary>
    [AttachedPropertyBrowsableForType(typeof(Window))]
    public static void SetHeaderInActiveBrush(DependencyObject d, Brush value) => d.SetValue(HeaderInActiveBrushProperty, value);

    /// <summary>Кисть заголовка не активного окна</summary>
    public static Brush GetHeaderInActiveBrush(DependencyObject d) => (Brush)d.GetValue(HeaderInActiveBrushProperty);

    /// <summary>Кисть заголовка не активного окна</summary>
    public static readonly DependencyProperty HeaderInActiveBrushProperty =
        DependencyProperty.RegisterAttached(
            "HeaderInActiveBrush",
            typeof(Brush),
            typeof(ModernWindow),
            new PropertyMetadata(default(Brush)));

    #endregion

    #region Attached property HeaderButtonTopMostVisibility : Visibility - Видимость кнопки - поверх всех окон

    /// <summary>Видимость кнопки - поверх всех окон</summary>
    [AttachedPropertyBrowsableForType(typeof(Window))]
    public static void SetHeaderButtonTopMostVisibility(DependencyObject d, Visibility value) => d.SetValue(HeaderButtonTopMostVisibilityProperty, value);

    /// <summary>Видимость кнопки - поверх всех окон</summary>
    public static Visibility GetHeaderButtonTopMostVisibility(DependencyObject d) => (Visibility)d.GetValue(HeaderButtonTopMostVisibilityProperty);

    /// <summary>Видимость кнопки - поверх всех окон</summary>
    public static readonly DependencyProperty HeaderButtonTopMostVisibilityProperty =
        DependencyProperty.RegisterAttached(
            "HeaderButtonTopMostVisibility",
            typeof(Visibility),
            typeof(ModernWindow),
            new PropertyMetadata(Visibility.Visible));

    #endregion

    #region Attached property HeaderButtonMinimizeVisibility : Visibility - Видимость кнопки - минимизации

    /// <summary>Видимость кнопки - минимизации</summary>
    [AttachedPropertyBrowsableForType(typeof(Window))]
    public static void SetHeaderButtonMinimizeVisibility(DependencyObject d, Visibility value) => d.SetValue(HeaderButtonMinimizeVisibilityProperty, value);

    /// <summary>Видимость кнопки - минимизации</summary>
    public static Visibility GetHeaderButtonMinimizeVisibility(DependencyObject d) => (Visibility)d.GetValue(HeaderButtonMinimizeVisibilityProperty);

    /// <summary>Видимость кнопки - минимизации</summary>
    public static readonly DependencyProperty HeaderButtonMinimizeVisibilityProperty =
        DependencyProperty.RegisterAttached(
            "HeaderButtonMinimizeVisibility",
            typeof(Visibility),
            typeof(ModernWindow),
            new PropertyMetadata(Visibility.Visible));

    #endregion

    #region Attached property HeaderButtonMaximizeVisibility : Visibility - Видимость кнопки - максимизации

    /// <summary>Видимость кнопки - максимизации</summary>
    [AttachedPropertyBrowsableForType(typeof(Window))]
    public static void SetHeaderButtonMaximizeVisibility(DependencyObject d, Visibility value) => d.SetValue(HeaderButtonMaximizeVisibilityProperty, value);

    /// <summary>Видимость кнопки - максимизации</summary>
    public static Visibility GetHeaderButtonMaximizeVisibility(DependencyObject d) => (Visibility)d.GetValue(HeaderButtonMaximizeVisibilityProperty);

    /// <summary>Видимость кнопки - максимизации</summary>
    public static readonly DependencyProperty HeaderButtonMaximizeVisibilityProperty =
        DependencyProperty.RegisterAttached(
            "HeaderButtonMaximizeVisibility",
            typeof(Visibility),
            typeof(ModernWindow),
            new PropertyMetadata(Visibility.Visible));

    #endregion

    #region Attached property HeaderButtonCloseVisibility : Visibility - Видимость кнопки - закрытия

    /// <summary>Видимость кнопки - закрытия</summary>
    [AttachedPropertyBrowsableForType(typeof(Window))]
    public static void SetHeaderButtonCloseVisibility(DependencyObject d, Visibility value) => d.SetValue(HeaderButtonCloseVisibilityProperty, value);

    /// <summary>Видимость кнопки - закрытия</summary>
    public static Visibility GetHeaderButtonCloseVisibility(DependencyObject d) => (Visibility)d.GetValue(HeaderButtonCloseVisibilityProperty);

    /// <summary>Видимость кнопки - закрытия</summary>
    public static readonly DependencyProperty HeaderButtonCloseVisibilityProperty =
        DependencyProperty.RegisterAttached(
            "HeaderButtonCloseVisibility",
            typeof(Visibility),
            typeof(ModernWindow),
            new PropertyMetadata(Visibility.Visible));

    #endregion

    #region Attached property HeaderIconVisibility : Visibility - Видимость иконки

    /// <summary>Видимость иконки</summary>
    [AttachedPropertyBrowsableForType(typeof(Window))]
    public static void SetHeaderIconVisibility(DependencyObject d, Visibility value) => d.SetValue(HeaderIconVisibilityProperty, value);

    /// <summary>Видимость иконки</summary>
    public static Visibility GetHeaderIconVisibility(DependencyObject d) => (Visibility)d.GetValue(HeaderIconVisibilityProperty);

    /// <summary>Видимость иконки</summary>
    public static readonly DependencyProperty HeaderIconVisibilityProperty =
        DependencyProperty.RegisterAttached(
            "HeaderIconVisibility",
            typeof(Visibility),
            typeof(ModernWindow),
            new PropertyMetadata(Visibility.Visible));

    #endregion

    #region Attached property HeaderAdditionalContentVisibility : Visibility - Видимость дополнительного контента в заголовке

    /// <summary>Видимость дополнительного контента в заголовке</summary>
    [AttachedPropertyBrowsableForType(typeof(Window))]
    public static void SetHeaderAdditionalContentVisibility(DependencyObject d, Visibility value) => d.SetValue(HeaderAdditionalContentVisibilityProperty, value);

    /// <summary>Видимость дополнительного контента в заголовке</summary>
    public static Visibility GetHeaderAdditionalContentVisibility(DependencyObject d) => (Visibility)d.GetValue(HeaderAdditionalContentVisibilityProperty);

    /// <summary>Видимость дополнительного контента в заголовке</summary>
    public static readonly DependencyProperty HeaderAdditionalContentVisibilityProperty =
        DependencyProperty.RegisterAttached(
            "HeaderAdditionalContentVisibility",
            typeof(Visibility),
            typeof(ModernWindow),
            new PropertyMetadata(Visibility.Visible));

    #endregion

    #region Attached property HeaderTextBrushActive : Brush - Кисть отрисовки текста заголовка активного окна

    /// <summary>Кисть отрисовки текста заголовка активного окна</summary>
    [AttachedPropertyBrowsableForType(typeof(Window))]
    public static void SetHeaderTextBrushActive(DependencyObject d, Brush value) => d.SetValue(HeaderTextBrushActiveProperty, value);

    /// <summary>Кисть отрисовки текста заголовка активного окна</summary>
    public static Brush GetHeaderTextBrushActive(DependencyObject d) => (Brush)d.GetValue(HeaderTextBrushActiveProperty);

    /// <summary>Кисть отрисовки текста заголовка активного окна</summary>
    public static readonly DependencyProperty HeaderTextBrushActiveProperty =
        DependencyProperty.RegisterAttached(
            "HeaderTextBrushActive",
            typeof(Brush),
            typeof(ModernWindow),
            new PropertyMetadata(default(Brush)));

    #endregion

    #region Attached property HeaderTextBrushInActive : Brush - Кисть отрисовки текста заголовка не активного окна

    /// <summary>Кисть отрисовки текста заголовка не активного окна</summary>
    [AttachedPropertyBrowsableForType(typeof(Window))]
    public static void SetHeaderTextBrushInActive(DependencyObject d, Brush value) => d.SetValue(HeaderTextBrushInActiveProperty, value);

    /// <summary>Кисть отрисовки текста заголовка не активного окна</summary>
    public static Brush GetHeaderTextBrushInActive(DependencyObject d) => (Brush)d.GetValue(HeaderTextBrushInActiveProperty);

    /// <summary>Кисть отрисовки текста заголовка не активного окна</summary>
    public static readonly DependencyProperty HeaderTextBrushInActiveProperty =
        DependencyProperty.RegisterAttached(
            "HeaderTextBrushInActive",
            typeof(Brush),
            typeof(ModernWindow),
            new PropertyMetadata(default(Brush)));

    #endregion
}
