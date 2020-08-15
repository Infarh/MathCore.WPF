using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MathCore.WPF
{
    public class ConfirmButton
    {
        #region Attached property PopupPlacment : PlacementMode - Положение выпадающего меню

        /// <summary>Положение выпадающего меню</summary>
        public static readonly DependencyProperty PopupPlacemenProperty =
            DependencyProperty.RegisterAttached(
                "PopupPlacemen",
                typeof(PlacementMode),
                typeof(ConfirmButton),
                new PropertyMetadata(PlacementMode.Right));

        /// <summary>Положение выпадающего меню</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetPopupPlacemen(DependencyObject element, PlacementMode value) => element.SetValue(PopupPlacemenProperty, value);

        /// <summary>Положение выпадающего меню</summary>
        public static PlacementMode GetPopupPlacemen(DependencyObject element) => (PlacementMode)element.GetValue(PopupPlacemenProperty);

        #endregion

        #region Attached property Animation : PopupAnimation - Анимация открытия

        /// <summary>Анимация открытия</summary>
        public static readonly DependencyProperty AnimationProperty =
            DependencyProperty.RegisterAttached(
                "Animation",
                typeof(PopupAnimation),
                typeof(ConfirmButton),
                new PropertyMetadata(PopupAnimation.Fade));

        /// <summary>Анимация открытия</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetAnimation(DependencyObject element, PopupAnimation value) => element.SetValue(AnimationProperty, value);

        /// <summary>Анимация открытия</summary>
        public static PopupAnimation GetAnimation(DependencyObject element) => (PopupAnimation)element.GetValue(AnimationProperty);

        #endregion

        #region Attached property HorizontalPopupOffset : double - Горизонтальное расстояние выпадающего меню

        /// <summary>Горизонтальное расстояние выпадающего меню</summary>
        public static readonly DependencyProperty HorizontalPopupOffsetProperty =
            DependencyProperty.RegisterAttached(
                "HorizontalPopupOffset",
                typeof(double),
                typeof(ConfirmButton),
                new PropertyMetadata(0d));

        /// <summary>Горизонтальное расстояние выпадающего меню</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetHorizontalPopupOffset(DependencyObject element, double value) => element.SetValue(HorizontalPopupOffsetProperty, value);

        /// <summary>Горизонтальное расстояние выпадающего меню</summary>
        public static double GetHorizontalPopupOffset(DependencyObject element) => (double)element.GetValue(HorizontalPopupOffsetProperty);

        #endregion

        #region Attached property VerticalPopupOffset : double - Вертикальное расстояние выпадающего меню

        /// <summary>Вертикальное расстояние выпадающего меню</summary>
        public static readonly DependencyProperty VerticalPopupOffsetProperty =
            DependencyProperty.RegisterAttached(
                "VerticalPopupOffset",
                typeof(double),
                typeof(ConfirmButton),
                new PropertyMetadata(0d));

        /// <summary>Вертикальное расстояние выпадающего меню</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetVerticalPopupOffset(DependencyObject element, double value) => element.SetValue(VerticalPopupOffsetProperty, value);

        /// <summary>Вертикальное расстояние выпадающего меню</summary>
        public static double GetVerticalPopupOffset(DependencyObject element) => (double)element.GetValue(VerticalPopupOffsetProperty);

        #endregion

        #region Attached property ConfirmButtonContent : object - Содержимое кнопки подтверждения

        /// <summary>Содержимое кнопки подтверждения</summary>
        public static readonly DependencyProperty ConfirmButtonContentProperty =
            DependencyProperty.RegisterAttached(
                "ConfirmButtonContent",
                typeof(object),
                typeof(ConfirmButton),
                new PropertyMetadata(default(object)));

        /// <summary>Содержимое кнопки подтверждения</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetConfirmButtonContent(DependencyObject element, object value) => element.SetValue(ConfirmButtonContentProperty, value);

        /// <summary>Содержимое кнопки подтверждения</summary>
        public static object GetConfirmButtonContent(DependencyObject element) => (object)element.GetValue(ConfirmButtonContentProperty);

        #endregion

        #region Attached property ConfirmButtonMargin : Thickness - Отступы кнопки подтвеждения

        /// <summary>Отступы кнопки подтвеждения</summary>
        public static readonly DependencyProperty ConfirmButtonMarginProperty =
            DependencyProperty.RegisterAttached(
                "ConfirmButtonMargin",
                typeof(Thickness),
                typeof(ConfirmButton),
                new PropertyMetadata(default(Thickness)));

        /// <summary>Отступы кнопки подтвеждения</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetConfirmButtonMargin(DependencyObject element, Thickness value) => element.SetValue(ConfirmButtonMarginProperty, value);

        /// <summary>Отступы кнопки подтвеждения</summary>
        public static Thickness GetConfirmButtonMargin(DependencyObject element) => (Thickness)element.GetValue(ConfirmButtonMarginProperty);

        #endregion

        #region Attached property ConfirmButtonPadding : Thickness - Внутренние отступы кнопки подтверждения

        /// <summary>Внутренние отступы кнопки подтверждения</summary>
        public static readonly DependencyProperty ConfirmButtonPaddingProperty =
            DependencyProperty.RegisterAttached(
                "ConfirmButtonPadding",
                typeof(Thickness),
                typeof(ConfirmButton),
                new PropertyMetadata(default(Thickness)));

        /// <summary>Внутренние отступы кнопки подтверждения</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetConfirmButtonPadding(DependencyObject element, Thickness value) => element.SetValue(ConfirmButtonPaddingProperty, value);

        /// <summary>Внутренние отступы кнопки подтверждения</summary>
        public static Thickness GetConfirmButtonPadding(DependencyObject element) => (Thickness)element.GetValue(ConfirmButtonPaddingProperty);

        #endregion

        #region Attached property ConfirmButtonVerticalAligment : VerticalAlignment - Вертикальное размещение кнопки подтверждения

        /// <summary>Вертикальное размещение кнопки подтверждения</summary>
        public static readonly DependencyProperty ConfirmButtonVerticalAlignmentProperty =
            DependencyProperty.RegisterAttached(
                "ConfirmButtonVerticalAlignment",
                typeof(VerticalAlignment),
                typeof(ConfirmButton),
                new PropertyMetadata(default(VerticalAlignment)));

        /// <summary>Вертикальное размещение кнопки подтверждения</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetConfirmButtonVerticalAlignment(DependencyObject element, VerticalAlignment value) => element.SetValue(ConfirmButtonVerticalAlignmentProperty, value);

        /// <summary>Вертикальное размещение кнопки подтверждения</summary>
        public static VerticalAlignment GetConfirmButtonVerticalAlignment(DependencyObject element) => (VerticalAlignment)element.GetValue(ConfirmButtonVerticalAlignmentProperty);

        #endregion

        #region Attached property ConfirmButtonHorizontalAligment : HorizontalAlignment - Горизонтальное размещение кнопки подтверждения

        /// <summary>Горизонтальное размещение кнопки подтверждения</summary>
        public static readonly DependencyProperty ConfirmButtonHorizontalAligmentProperty =
            DependencyProperty.RegisterAttached(
                "ConfirmButtonHorizontalAligment",
                typeof(HorizontalAlignment),
                typeof(ConfirmButton),
                new PropertyMetadata(default(HorizontalAlignment)));

        /// <summary>Горизонтальное размещение кнопки подтверждения</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetConfirmButtonHorizontalAligment(DependencyObject element, HorizontalAlignment value) => element.SetValue(ConfirmButtonHorizontalAligmentProperty, value);

        /// <summary>Горизонтальное размещение кнопки подтверждения</summary>
        public static HorizontalAlignment GetConfirmButtonHorizontalAligment(DependencyObject element) => (HorizontalAlignment)element.GetValue(ConfirmButtonHorizontalAligmentProperty);

        #endregion

        #region Attached property ConfirmButtonForeground : Brush - Кисть для рисования контента кнопки подтверждения

        /// <summary>Кисть для рисования контента кнопки подтверждения</summary>
        public static readonly DependencyProperty ConfirmButtonForegroundProperty =
            DependencyProperty.RegisterAttached(
                "ConfirmButtonForeground",
                typeof(Brush),
                typeof(ConfirmButton),
                new PropertyMetadata(default(Brush)));

        /// <summary>Кисть для рисования контента кнопки подтверждения</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetConfirmButtonForeground(DependencyObject element, Brush value) => element.SetValue(ConfirmButtonForegroundProperty, value);

        /// <summary>Кисть для рисования контента кнопки подтверждения</summary>
        public static Brush GetConfirmButtonForeground(DependencyObject element) => (Brush)element.GetValue(ConfirmButtonForegroundProperty);

        #endregion

        #region Attached property ConfirmButtonBackground : Brush - Кисть для рисования фона кнопки подтверждения

        /// <summary>Кисть для рисования фона кнопки подтверждения</summary>
        public static readonly DependencyProperty ConfirmButtonBackgroundProperty =
            DependencyProperty.RegisterAttached(
                "ConfirmButtonBackground",
                typeof(Brush),
                typeof(ConfirmButton),
                new PropertyMetadata(default(Brush)));

        /// <summary>Кисть для рисования фона кнопки подтверждения</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetConfirmButtonBackground(DependencyObject element, Brush value) => element.SetValue(ConfirmButtonBackgroundProperty, value);

        /// <summary>Кисть для рисования фона кнопки подтверждения</summary>
        public static Brush GetConfirmButtonBackground(DependencyObject element) => (Brush)element.GetValue(ConfirmButtonBackgroundProperty);

        #endregion

        #region Attached property ConfirmButtonBorderBrush : Brush - Кисть для рисования рамки кнопки подтвеждения

        /// <summary>Кисть для рисования рамки кнопки подтвеждения</summary>
        public static readonly DependencyProperty ConfirmButtonBorderBrushProperty =
            DependencyProperty.RegisterAttached(
                "ConfirmButtonBorderBrush",
                typeof(Brush),
                typeof(ConfirmButton),
                new PropertyMetadata(default(Brush)));

        /// <summary>Кисть для рисования рамки кнопки подтвеждения</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetConfirmButtonBorderBrush(DependencyObject element, Brush value) => element.SetValue(ConfirmButtonBorderBrushProperty, value);

        /// <summary>Кисть для рисования рамки кнопки подтвеждения</summary>
        public static Brush GetConfirmButtonBorderBrush(DependencyObject element) => (Brush)element.GetValue(ConfirmButtonBorderBrushProperty);

        #endregion

        #region Attached property ConfirmButtonBorderThickness : Thickness - Толщина рамки кнопки подтверждения

        /// <summary>Толщина рамки кнопки подтверждения</summary>
        public static readonly DependencyProperty ConfirmButtonBorderThicknessProperty =
            DependencyProperty.RegisterAttached(
                "ConfirmButtonBorderThickness",
                typeof(Thickness),
                typeof(ConfirmButton),
                new PropertyMetadata(default(Thickness)));

        /// <summary>Толщина рамки кнопки подтверждения</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetConfirmButtonBorderThickness(DependencyObject element, Thickness value) => element.SetValue(ConfirmButtonBorderThicknessProperty, value);

        /// <summary>Толщина рамки кнопки подтверждения</summary>
        public static Thickness GetConfirmButtonBorderThickness(DependencyObject element) => (Thickness)element.GetValue(ConfirmButtonBorderThicknessProperty);

        #endregion

        #region Attached property ConfirmButtonCornerRadius : CornerRadius - Радиус скругления рамки кнопки подтверждения

        /// <summary>Радиус скругления рамки кнопки подтверждения</summary>
        public static readonly DependencyProperty ConfirmButtonCornerRadiusProperty =
            DependencyProperty.RegisterAttached(
                "ConfirmButtonCornerRadius",
                typeof(CornerRadius),
                typeof(ConfirmButton),
                new PropertyMetadata(default(CornerRadius)));

        /// <summary>Радиус скругления рамки кнопки подтверждения</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetConfirmButtonCornerRadius(DependencyObject element, CornerRadius value) => element.SetValue(ConfirmButtonCornerRadiusProperty, value);

        /// <summary>Радиус скругления рамки кнопки подтверждения</summary>
        public static CornerRadius GetConfirmButtonCornerRadius(DependencyObject element) => (CornerRadius)element.GetValue(ConfirmButtonCornerRadiusProperty);

        #endregion

        #region Attached property ConfirmButtonFontWeight : FontWeight - Толщина шрифта на кнопке подтверждения

        /// <summary>Толщина шрифта на кнопке подтверждения</summary>
        public static readonly DependencyProperty ConfirmButtonFontWeightProperty =
            DependencyProperty.RegisterAttached(
                "ConfirmButtonFontWeight",
                typeof(FontWeight),
                typeof(ConfirmButton),
                new PropertyMetadata(default(FontWeight)));

        /// <summary>Толщина шрифта на кнопке подтверждения</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetConfirmButtonFontWeight(DependencyObject element, FontWeight value) => element.SetValue(ConfirmButtonFontWeightProperty, value);

        /// <summary>Толщина шрифта на кнопке подтверждения</summary>
        public static FontWeight GetConfirmButtonFontWeight(DependencyObject element) => (FontWeight)element.GetValue(ConfirmButtonFontWeightProperty);

        #endregion

        #region Attached property ConfirmButtonFontSize : double - Размер шрифта на кнопке подтверждения

        /// <summary>Размер шрифта на кнопке подтверждения</summary>
        public static readonly DependencyProperty ConfirmButtonFontSizeProperty =
            DependencyProperty.RegisterAttached(
                "ConfirmButtonFontSize",
                typeof(double),
                typeof(ConfirmButton),
                new PropertyMetadata(12d));

        /// <summary>Размер шрифта на кнопке подтверждения</summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetConfirmButtonFontSize(DependencyObject element, double value) => element.SetValue(ConfirmButtonFontSizeProperty, value);

        /// <summary>Размер шрифта на кнопке подтверждения</summary>
        public static double GetConfirmButtonFontSize(DependencyObject element) => (double)element.GetValue(ConfirmButtonFontSizeProperty);

        #endregion
    }
}
