using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace MathCore.WPF;

/// <summary>Водяной знак для поля ввода</summary>
public static class Watermark
{
    #region AttachedProperties 

    /// <summary>Прозрачность водяного знака</summary>
    public static readonly DependencyProperty OpacityProperty =
        DependencyProperty.RegisterAttached(
            "Opacity",
            typeof(double),
            typeof(Watermark),
            new FrameworkPropertyMetadata(0.5,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnWatermarkOpacityChanged),
            v => (double)v >= 0 && (double)v <= 1);

    /// <summary>Задать透明ность водяного знака</summary>
    /// <param name="element">Объект, которому устанавливается прозрачность водяного знака</param>
    /// <param name="value">Значение прозрачности водяного знака</param>
    public static void SetOpacity(DependencyObject element, double value) => element.SetValue(OpacityProperty, value);

    /// <summary>Получить значение прозрачности водяного знака</summary>
    /// <param name="element">Элемент, прозрачность водяного знака которого надо получить</param>
    /// <returns>Значение прозрачности водяного знака</returns>
    public static double GetOpacity(DependencyObject element) => (double)element.GetValue(OpacityProperty);

    /// <summary>Значение водяного знака</summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.RegisterAttached(
            "Value",
            typeof(object),
            typeof(Watermark),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender
                | FrameworkPropertyMetadataOptions.AffectsArrange
                | FrameworkPropertyMetadataOptions.AffectsMeasure,
                OnWatermarkPropertyAttached));


    /// <summary>Задать значение водяного знака</summary>
    /// <param name="element">Элемент, которому задаётся значение водяного знака</param>
    /// <param name="value">Значение водяного знака</param>
    public static void SetValue(DependencyObject element, object? value) => element.SetValue(ValueProperty, value);

    /// <summary>Получить значение водяного знака</summary>
    /// <param name="element">Элемент, значение водяного знака которого надо получить</param>
    /// <returns>Значение водяного знака</returns>
    public static object? GetValue(DependencyObject element) => element.GetValue(ValueProperty);


    public static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.RegisterAttached(
            "Foreground",
            typeof(Brush),
            typeof(Watermark),
            new FrameworkPropertyMetadata(Brushes.Black,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnWatermarkPropertyAttached));

    public static void SetForeground(DependencyObject element, Brush? value) => element.SetValue(ForegroundProperty, value);

    public static Brush? GetForeground(DependencyObject element) => (Brush?)element.GetValue(ForegroundProperty);

    #region VerticalAlignment attached dependency property : VerticalAlignment

    /// <summary>Прилогаемое свойство <see cref="Watermark"/>.<see cref="VerticalAlignmentProperty"/> типа <see cref="VerticalAlignment"/></summary>
    public static readonly DependencyProperty VerticalAlignmentProperty =
        DependencyProperty.RegisterAttached(
            "VerticalAlignment",
            typeof(VerticalAlignment),
            typeof(Watermark),
            new(VerticalAlignment.Center));


    /// <summary>Установка значения <see cref="value"/> типа <see cref="VerticalAlignment"/> свйоству <see cref="Watermark"/>.<see cref="VerticalAlignmentProperty"/> целевого объекта <see cref="element"/></summary>
    /// <param name="element">Объект <see cref="DependencyObject"/>, значение <see cref="Watermark"/>.<see cref="VerticalAlignmentProperty"/> которого надо установить</param>
    /// <param name="value">Устанавливаемое значение <see cref="Watermark"/>.<see cref="VerticalAlignmentProperty"/> типа <see cref="VerticalAlignment"/></param>
    public static void SetVerticalAligment(DependencyObject element, VerticalAlignment value) => element.SetValue(VerticalAlignmentProperty, value);

    /// <summary>Получение значения типа <see cref="VerticalAlignment"/> свойства <see cref="Watermark"/>.<see cref="VerticalAlignmentProperty"/> у целевого объекта <see cref="element"/></summary>
    /// <param name="element">Объект <see cref="DependencyObject"/>, значение <see cref="Watermark"/>.<see cref="VerticalAlignmentProperty"/> которого надо получить</param>
    /// <returns>Значение свойства <see cref="Watermark"/>.<see cref="VerticalAlignmentProperty"/> типа <see cref="VerticalAlignment"/> целевого объекта <see cref="element"/></returns>
    public static VerticalAlignment GetVerticalAligment(DependencyObject element) => (VerticalAlignment)element.GetValue(VerticalAlignmentProperty);

    #endregion

    #region HorizontalAlignment attached dependency property : HorizontalAlignment

    /// <summary>Прилогаемое свойство <see cref="Watermark"/>.<see cref="HorizontalAlignmentProperty"/> типа <see cref="HorizontalAlignment"/></summary>
    public static readonly DependencyProperty HorizontalAlignmentProperty =
        DependencyProperty.RegisterAttached(
            "HorizontalAlignment",
            typeof(HorizontalAlignment),
            typeof(Watermark),
            new(HorizontalAlignment.Left));


    /// <summary>Установка значения <see cref="value"/> типа <see cref="HorizontalAlignment"/> свйоству <see cref="Watermark"/>.<see cref="HorizontalAlignmentProperty"/> целевого объекта <see cref="element"/></summary>
    /// <param name="element">Объект <see cref="DependencyObject"/>, значение <see cref="Watermark"/>.<see cref="HorizontalAlignmentProperty"/> которого надо установить</param>
    /// <param name="value">Устанавливаемое значение <see cref="Watermark"/>.<see cref="HorizontalAlignmentProperty"/> типа <see cref="HorizontalAlignment"/></param>
    public static void SetHorizontalAlignment(DependencyObject element, HorizontalAlignment value) => element.SetValue(HorizontalAlignmentProperty, value);

    /// <summary>Получение значения типа <see cref="HorizontalAlignment"/> свойства <see cref="Watermark"/>.<see cref="HorizontalAlignmentProperty"/> у целевого объекта <see cref="element"/></summary>
    /// <param name="element">Объект <see cref="DependencyObject"/>, значение <see cref="Watermark"/>.<see cref="HorizontalAlignmentProperty"/> которого надо получить</param>
    /// <returns>Значение свойства <see cref="Watermark"/>.<see cref="HorizontalAlignmentProperty"/> типа <see cref="HorizontalAlignment"/> целевого объекта <see cref="element"/></returns>
    public static HorizontalAlignment GetHorizontalAlignment(DependencyObject element) => (HorizontalAlignment)element.GetValue(HorizontalAlignmentProperty);

    #endregion

    #region FontSize attached dependency property : double

    /// <summary>Прилогаемое свойство <see cref="Watermark"/>.<see cref="FontSizeProperty"/> типа <see cref="double"/></summary>
    public static readonly DependencyProperty FontSizeProperty =
        DependencyProperty.RegisterAttached(
            "FontSize",
            typeof(double),
            typeof(Watermark),
            new(SystemFonts.MessageFontSize));


    /// <summary>Установка значения <see cref="value"/> типа <see cref="double"/> свйоству <see cref="Watermark"/>.<see cref="FontSizeProperty"/> целевого объекта <see cref="element"/></summary>
    /// <param name="element">Объект <see cref="DependencyObject"/>, значение <see cref="Watermark"/>.<see cref="FontSizeProperty"/> которого надо установить</param>
    /// <param name="value">Устанавливаемое значение <see cref="Watermark"/>.<see cref="FontSizeProperty"/> типа <see cref="double"/></param>
    public static void SetFontSize(DependencyObject element, double value) => element.SetValue(FontSizeProperty, value);

    /// <summary>Получение значения типа <see cref="double"/> свойства <see cref="Watermark"/>.<see cref="FontSizeProperty"/> у целевого объекта <see cref="element"/></summary>
    /// <param name="element">Объект <see cref="DependencyObject"/>, значение <see cref="Watermark"/>.<see cref="FontSizeProperty"/> которого надо получить</param>
    /// <returns>Значение свойства <see cref="Watermark"/>.<see cref="FontSizeProperty"/> типа <see cref="double"/> целевого объекта <see cref="element"/></returns>
    public static double GetFontSize(DependencyObject element) => (double)element.GetValue(FontSizeProperty);

    #endregion

    #endregion

    /// <summary>Словарь объектов ItemsControls, которым установлен водяной знак</summary>
    private static readonly Dictionary<object, ItemsControl> __ItemsControlsDictionary = [];

    private static readonly List<Control> __AttachedControlsList = new(50);

    /// <summary>Обработчик события изменения водяного знака</summary>
    /// <param name="d"><see cref="DependencyObject"/> - источник события</param>
    /// <param name="e"><see cref="DependencyPropertyChangedEventArgs"/> - аргумент события изменения водяного знака</param>
    private static void OnWatermarkOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Control control) return; // защита от некорректного использования

        OnWatermarkPropertyAttached(control, e);

        var layer = AdornerLayer.GetAdornerLayer(control);

        // Графический слой может отсутствовать, если элемент больше не в визуальном дереве
        var adorners = layer?.GetAdorners(control);
        var a        = adorners?.OfType<WatermarkAdorner>().FirstOrDefault();
        if (a is null) return;
        a.Opacity = (double)e.NewValue;
    }

    /// <summary>Обработчик события изменения водяного знака</summary>
    /// <param name="d"><see cref="DependencyObject"/> - источник события</param>
    /// <param name="e"><see cref="DependencyPropertyChangedEventArgs"/> - аргумент события изменения водяного знака</param>
    private static void OnWatermarkPropertyAttached(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Control control) return; // защищаемся от присвоения свойств не контролам
        SetEvents(control);
    }

    private static void SetEvents(Control control)
    {
        if (__AttachedControlsList.Contains(control)) return;
        __AttachedControlsList.Add(control);

        control.Loaded += OnLoaded;
        //control.Unloaded += OnUnloaded;

        switch (control)
        {
            case TextBox text_box:
                control.GotKeyboardFocus  += OnGotKeyboardFocus;
                control.LostKeyboardFocus += OnLoaded;
                text_box.TextChanged      += OnContentChanged;
                break;
            case PasswordBox password_box:
                control.GotKeyboardFocus     += OnGotKeyboardFocus;
                control.LostKeyboardFocus    += OnLoaded;
                password_box.PasswordChanged += OnContentChanged;
                break;
            case ComboBox box:
                box.GotKeyboardFocus  += OnGotKeyboardFocus;
                box.LostKeyboardFocus += OnLoaded;
                box.SelectionChanged  += OnContentChanged;
                break;
            default:
            {
                if (control is not ItemsControl items_control) return;
                items_control.ItemContainerGenerator.ItemsChanged += OnItemsChanged;
                __ItemsControlsDictionary.Add(items_control.ItemContainerGenerator, items_control);

                var property = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, items_control.GetType());
                property.AddValueChanged(items_control, OnItemsSourceChanged);
                break;
            }
        }
    }

    /// <summary>Обработчик события изменения содержимого, влияющего на видимость водяного знака</summary>
    /// <param name="sender">Объект - источник событий</param>
    /// <param name="e"><see cref="RoutedEventArgs"/> - аргумент события</param>
    private static void OnContentChanged(object sender, RoutedEventArgs? e)
    {
        var control = (Control)sender;
        (ShouldShowWatermark(control) ? (Action<Control>)ShowWatermark : RemoveWatermark)(control);
    }

    /// <summary>Обработчик события изменения фокуса ввода клавиатуры</summary>
    /// <param name="sender">Объект - источник событий</param>
    /// <param name="e"><see cref="RoutedEventArgs"/> - аргумент события</param>
    private static void OnGotKeyboardFocus(object sender, RoutedEventArgs? e)
    {
        var control = (Control)sender;
        if (ShouldShowWatermark(control)) RemoveWatermark(control);
    }

    /// <summary>Обработчик события загрузки компонента</summary>
    /// <param name="sender">Источник события</param>
    /// <param name="e"><see cref="RoutedEventArgs"/> - аргумент события</param>
    private static void OnLoaded(object sender, RoutedEventArgs? e)
    {
        var control = (Control)sender;
        if (ShouldShowWatermark(control)) ShowWatermark(control);
    }

    /// <summary>Обработчик события изменения значения свойства Источника элементов</summary>
    /// <param name="sender">Источник события</param>
    /// <param name="e">A <see cref="EventArgs"/> - аргумент события</param>
    private static void OnItemsSourceChanged(object sender, EventArgs? e)
    {
        var control = (ItemsControl)sender;
        (control.ItemsSource is null || ShouldShowWatermark(control) ? (Action<Control>)ShowWatermark : RemoveWatermark)(control);
    }

    /// <summary>Обработчик события изменения элементов объекта</summary>
    /// <param name="sender">Источник события</param>
    /// <param name="e"><see cref="ItemsChangedEventArgs"/> - аргумент события</param>
    private static void OnItemsChanged(object sender, ItemsChangedEventArgs? e)
    {
        if (__ItemsControlsDictionary.TryGetValue(sender, out var control))
            (ShouldShowWatermark(control) ? (Action<Control>)ShowWatermark : RemoveWatermark)(control);
    }

    /// <summary>Удалить водяной знак элемента</summary>
    /// <param name="control">Элемент, водяной знак у которого надо удалить</param>
    private static void RemoveWatermark(UIElement control)
    {
        var layer = AdornerLayer.GetAdornerLayer(control);

        // Графический слой может отсутствовать, если элемент больше не в визуальном дереве
        if (layer is null) return;

        var adorners = layer.GetAdorners(control);
        if (adorners is null || adorners.Length == 0) return;

        foreach (var adorner in adorners)
        {
            adorner.Visibility = Visibility.Hidden; // скрываем на случай, если кто-то ещё держит ссылку
            layer.Remove(adorner);
        }
    }

    /// <summary>Показать водяной знак для компонента</summary>
    /// <param name="control">Компонент, для которого надо показать водяной знак</param>
    private static void ShowWatermark(Control control)
    {
        if (control is null) throw new ArgumentNullException(nameof(control));

        var layer = AdornerLayer.GetAdornerLayer(control);

        // Графический слой может отсутствовать, если элемент больше не в визуальном дереве
        if (layer is null) return;

        var watermark_adorners = (layer.GetAdorners(control) ?? Enumerable.Empty<Adorner>())
           .OfType<WatermarkAdorner>()
           .ToArray();

        if (watermark_adorners.Length == 0)
        {
            var value = GetValue(control);
            if (value is null) return; // если нет значения водяного знака, нет смысла его показывать
            layer.Add(new WatermarkAdorner(control, value));
        }
        else
        {
            foreach (var adorner in watermark_adorners)
                adorner.UpdateLayout();
        }
    }

    /// <summary>Проверка необходимости показать водяной знак компонента</summary>
    /// <param name="control"><see cref="Control"/> - компонент, для которого надо проверить видимость</param>
    /// <returns>Истина, если компонент удовлетворяет условию отображения водяного знака</returns>
    private static bool ShouldShowWatermark(Control? control) => control switch
    {
        ComboBox combo_box => combo_box.SelectedItem is null && string.IsNullOrEmpty(combo_box.Text), // для редактируемого ComboBox учитываем текст
        TextBox text_box => string.IsNullOrEmpty(text_box.Text),
        PasswordBox password_box => string.IsNullOrEmpty(password_box.Password),
        ItemsControl items_control => items_control.Items.Count == 0,
        _ => false
    };

    /// <summary>Слой водяного знака</summary>
    private class WatermarkAdorner : Adorner
    {
        #region Закрытые поля

        /// <summary><see cref="ContentPresenter"/> - объект, содержащий водяной знак</summary>
        private readonly ContentPresenter _ContentPresenter;

        #endregion

        #region Конструктор

        /// <summary>Инициализация нового <see cref="WatermarkAdorner"/></summary>
        /// <param name="control"><see cref="UIElement"/> - компонент, которому назначается водяной знак</param>
        /// <param name="watermark">Значение водяного знака</param>
        public WatermarkAdorner(UIElement control, object? watermark)
            : base(control)
        {
            if (control is null) throw new ArgumentNullException(nameof(control));

            // Запрещаем взаимодействие с подсказкой, чтобы не мешать вводу
            IsHitTestVisible = false;

            _ContentPresenter = new();

            if (watermark is UIElement watermark_element)
            {
                _ContentPresenter.Content = watermark_element;
            }
            else if (watermark is not null)
            {
                _ContentPresenter.Content = new TextBlock
                {
                    Text              = watermark.ToString(),
                    Margin            = new(4, 0, 4, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
            }

            _ContentPresenter.SetBinding(ContentPresenter.ContentProperty, new Binding
            {
                Path                = new("(0)", ValueProperty),
                Source              = control,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            _ContentPresenter.SetBinding(VerticalAlignmentProperty, new Binding
            {
                Path                = new("(0)", VerticalAlignmentProperty),
                Source              = control,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            _ContentPresenter.SetBinding(HorizontalAlignmentProperty, new Binding
            {
                Path                = new("(0)", HorizontalAlignmentProperty),
                Source              = control,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            _ContentPresenter.SetBinding(TextElement.ForegroundProperty, new Binding
            {
                Path                = new("(0)", ForegroundProperty),
                Source              = control,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            _ContentPresenter.SetBinding(TextElement.FontSizeProperty, new Binding
            {
                Path                = new("(0)", FontSizeProperty),
                Source              = control,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            _ContentPresenter.Opacity = GetOpacity(Control);
            _ContentPresenter.SetBinding(OpacityProperty, new Binding
            {
                Path                = new("(0)", OpacityProperty),
                Source              = control,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            _ContentPresenter.Margin = new(
                Control.Margin.Left + Control.Padding.Left,
                Control.Margin.Top + Control.Padding.Top,
                Control.Margin.Right + Control.Padding.Right,
                Control.Margin.Bottom + Control.Padding.Bottom);

            // Если компонент управляет коллекцией элементов и это не ComboBox, размещаем водяной знак по центру
            if (Control is ItemsControl && Control is not ComboBox)
            {
                _ContentPresenter.VerticalAlignment   = VerticalAlignment.Center;
                _ContentPresenter.HorizontalAlignment = HorizontalAlignment.Center;
            }

            // Создаём связь свойств водяного знака и компонента по свойству видимости 
            SetBinding(VisibilityProperty, new Binding("IsVisible")
            {
                Source    = control,
                Converter = new BooleanToVisibilityConverter()
            });

            if (watermark is FrameworkElement binding_item)
            {
                binding_item.SetBinding(TextElement.ForegroundProperty, new Binding
                {
                    Path                = new("(0)", ForegroundProperty),
                    Source              = control,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>Число дочерних слоёв <see cref="ContainerVisual"/></summary>
        protected override int VisualChildrenCount => 1;

        #endregion

        #region Private Properties

        /// <summary>Компонент, который надо отобразить</summary>
        private Control Control => (Control)AdornedElement;

        #endregion

        #region Protected Overrides

        /// <summary>Возвращает дочерний <see cref="Visual"/> по индексу</summary>
        /// <param name="index">Индекс дочернего <see cref="Visual"/></param>
        /// <returns>Дочерний <see cref="Visual"/></returns>
        protected override Visual GetVisualChild(int index) => _ContentPresenter;

        /// <summary>Реализует измерение слоя водяного знака</summary>
        /// <param name="constraint">Необходимый размер</param>
        /// <returns><see cref="Size"/> - размер нужного для отображения слоя</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            _ContentPresenter.Measure(Control.RenderSize);
            return Control.RenderSize;
        }

        /// <summary>Размещение дочерних элементов</summary>
        /// <param name="FinalSize">Итоговая область</param>
        /// <returns>Реальный используемый размер</returns>
        protected override Size ArrangeOverride(Size FinalSize)
        {
            _ContentPresenter.Arrange(new(FinalSize));
            return FinalSize;
        }

        #endregion
    }
}