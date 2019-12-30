using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using MathCore.Annotations;

namespace MathCore.WPF
{
    /// <summary>Водяной знак для поля ввода</summary>
    public static class Watermark
    {
        #region AttachedProperties 

        /// <summary>Прозпачность возяного знака</summary>
        [NotNull]
        public static readonly DependencyProperty OpacityProperty =
                    DependencyProperty.RegisterAttached(
                        "Opacity",
                        typeof(double),
                        typeof(Watermark),
                        new FrameworkPropertyMetadata(0.5,
                            FrameworkPropertyMetadataOptions.AffectsRender,
                            OnWatermarkOpacityChanged),
                            v => (double)v >= 0 && (double)v <= 1);

        /// <summary>Задать прозрачность возяного знака</summary>
        /// <param name="element">Объект, которому устанавливается прозрачность водяного знака</param>
        /// <param name="value">Значение прозрачности водяного знака</param>
        public static void SetOpacity([NotNull] DependencyObject element, double value) => element.SetValue(OpacityProperty, value);

        /// <summary>Получить значение прозрачности водяного знака</summary>
        /// <param name="element">Элемент, прозрачность водяного знака которого надо получить</param>
        /// <returns>Значение прозрачности водяного знака</returns>
        public static double GetOpacity([NotNull] DependencyObject element) => (double)element.GetValue(OpacityProperty);

        /// <summary>Значение водяного знака</summary>
        [NotNull]
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
        public static void SetValue([NotNull] DependencyObject element, [CanBeNull] object value) => element.SetValue(ValueProperty, value);

        /// <summary>Получить значение водяного знака</summary>
        /// <param name="element">Элемент, значение водяного знака которого надо получить</param>
        /// <returns>Значение водяного знака</returns>
        [CanBeNull] public static object GetValue([NotNull] DependencyObject element) => element.GetValue(ValueProperty);


        [NotNull]
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.RegisterAttached(
                "Foreground",
                typeof(Brush),
                typeof(Watermark),
                    new FrameworkPropertyMetadata(Brushes.Black,
                        FrameworkPropertyMetadataOptions.AffectsRender,
                        OnWatermarkPropertyAttached));

        public static void SetForeground([NotNull] DependencyObject element, [CanBeNull] Brush value) => element.SetValue(ForegroundProperty, value);

        [CanBeNull]
        public static Brush GetForeground([NotNull] DependencyObject element) => (Brush)element.GetValue(ForegroundProperty);

        #region VerticalAlignment attached dependency property : VerticalAlignment

        /// <summary>Прилогаемое свойство <see cref="Watermark"/>.<see cref="VerticalAlignmentProperty"/> типа <see cref="VerticalAlignment"/></summary>
        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.RegisterAttached(
                "VerticalAlignment",
                typeof(VerticalAlignment),
                typeof(Watermark),
                new PropertyMetadata(VerticalAlignment.Center));


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
                new PropertyMetadata(HorizontalAlignment.Left));


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
                new PropertyMetadata(SystemFonts.MessageFontSize));


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
        [NotNull] private static readonly Dictionary<object, ItemsControl> __ItemsControlsDictionary = new Dictionary<object, ItemsControl>();

        [NotNull] private static readonly List<Control> __AttachedControlsList = new List<Control>(50);

        /// <summary>Обработчик события изменения водяного знака</summary>
        /// <param name="d"><see cref="DependencyObject"/> - источник события</param>
        /// <param name="e"><see cref="DependencyPropertyChangedEventArgs"/> - аргумент события изменения водяного знака</param>
        private static void OnWatermarkOpacityChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnWatermarkPropertyAttached(d, e);
            var control = (Control)d;
            var layer = AdornerLayer.GetAdornerLayer(control);

            // Графический слой может отсутствовать, если элемент больше не в визуальном дереве
            var adorners = layer?.GetAdorners(control);
            var a = adorners?.OfType<WatermarkAdorner>().FirstOrDefault();
            if (a is null) return;
            a.Opacity = (double)e.NewValue;
            //layer.Add(new WatermarkAdorner(control, GetValue(control)));
        }

        /// <summary>Обработчик события изменения водяного знака</summary>
        /// <param name="d"><see cref="DependencyObject"/> - источник события</param>
        /// <param name="e"><see cref="DependencyPropertyChangedEventArgs"/> - аргумент события изменения водяного знака</param>
        private static void OnWatermarkPropertyAttached([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e) => SetEvents((Control)d);//OnContentChanged(d, null);

        private static void SetEvents([NotNull] Control control)
        {
            if (__AttachedControlsList.Contains(control)) return;
            __AttachedControlsList.Add(control);
            control.Loaded += OnLoaded;
            //control.Unloaded += OnUnloaded;

            switch (control)
            {
                case TextBox test_box:
                    control.GotKeyboardFocus += OnGotKeyboardFocus;
                    control.LostKeyboardFocus += OnLoaded;
                    test_box.TextChanged += OnContentChanged;
                    break;
                case PasswordBox password_box:
                    control.GotKeyboardFocus += OnGotKeyboardFocus;
                    control.LostKeyboardFocus += OnLoaded;
                    password_box.PasswordChanged += OnContentChanged;
                    break;
                case ComboBox box:
                    box.GotKeyboardFocus += OnGotKeyboardFocus;
                    box.LostKeyboardFocus += OnLoaded;
                    box.SelectionChanged += OnContentChanged;
                    break;
                default:
                    {
                        if (!(control is ItemsControl items_control)) return;
                        items_control.ItemContainerGenerator.ItemsChanged += OnItemsChanged;
                        __ItemsControlsDictionary.Add(items_control.ItemContainerGenerator, items_control);

                        var property = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, items_control.GetType());
                        property.AddValueChanged(items_control, OnItemsSourceChanged);
                        break;
                    }
            }
        }

        //private static void OnUnloaded(object sender, EventArgs e)
        //{
        //    var control = (Control)sender;
        //    if(!__AttachedControlsList.Contains(control)) return;
        //    __AttachedControlsList.Remove(control);
        //    control.Loaded -= OnLoaded;

        //    if(control is TextBox || control is PasswordBox)
        //    {
        //        control.GotKeyboardFocus -= OnGotKeyboardFocus;
        //        control.LostKeyboardFocus -= OnLoaded;
        //    }
        //    else if(control is ComboBox)
        //    {
        //        control.GotKeyboardFocus -= OnGotKeyboardFocus;
        //        control.LostKeyboardFocus -= OnLoaded;
        //        ((ComboBox)control).SelectionChanged -= OnContentChanged;
        //    }
        //    else
        //    {
        //        var items_control = control as ItemsControl;
        //        if(items_control is null) return;
        //        // for Items property  
        //        items_control.ItemContainerGenerator.ItemsChanged -= OnItemsChanged;
        //        __ItemsControlsDictionary.Remove(items_control.ItemContainerGenerator);

        //        // for ItemsSource property  
        //        var prop = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, items_control.GetType());
        //        prop.RemoveValueChanged(items_control, OnItemsSourceChanged);
        //    }
        //}

        /// <summary>Обработчик события изменения фокуса ввода элемента</summary>
        /// <param name="sender">Объект - источник событий</param>
        /// <param name="e"><see cref="ItemsChangedEventArgs"/> - аргумент события</param>
        private static void OnContentChanged([NotNull] object sender, [CanBeNull] RoutedEventArgs e) => (ShouldShowWatermark((Control)sender) ? (Action<Control>)ShowWatermark : RemoveWatermark)((Control)sender);

        /// <summary>Обработчик события изменения фокуса ввода клавиатуры</summary>
        /// <param name="sender">Объект - источник событий</param>
        /// <param name="e"><see cref="RoutedEventArgs"/> - аргумент события</param>
        private static void OnGotKeyboardFocus([NotNull] object sender, [CanBeNull] RoutedEventArgs e)
        {
            if (ShouldShowWatermark((Control)sender)) RemoveWatermark((Control)sender);
        }

        /// <summary>Обработчик события загрузки компонента</summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e"><see cref="RoutedEventArgs"/> - аргумент события</param>
        private static void OnLoaded([NotNull] object sender, [CanBeNull] RoutedEventArgs e)
        {
            if (ShouldShowWatermark((Control)sender)) ShowWatermark((Control)sender);
        }

        /// <summary>Обработчик события изменения значения свойства Источника элементов</summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">A <see cref="EventArgs"/> - аргумент события</param>
        private static void OnItemsSourceChanged([NotNull] object sender, [CanBeNull] EventArgs e)
        {
            var control = (ItemsControl)sender;
            (control.ItemsSource is null || ShouldShowWatermark(control) ? (Action<Control>)ShowWatermark : RemoveWatermark)(control);
        }

        /// <summary>Обработчик события изменения элементов объекта</summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e"><see cref="ItemsChangedEventArgs"/> - аргумент события</param>
        private static void OnItemsChanged([NotNull] object sender, [CanBeNull] ItemsChangedEventArgs e)
        {
            if (__ItemsControlsDictionary.TryGetValue(sender, out var control))
                (ShouldShowWatermark(control) ? (Action<Control>)ShowWatermark : RemoveWatermark)(control);
        }

        /// <summary>Уделить водяной знак элемента</summary>
        /// <param name="control">Элемент, водяной знак у которого надо удалить</param>
        private static void RemoveWatermark([NotNull] UIElement control)
        {
            var layer = AdornerLayer.GetAdornerLayer(control);

            // Графический слой может отсутствовать, если элемент больше не в визуальном дереве
            if (layer is null) return;
            (layer.GetAdorners(control) ?? Enumerable.Empty<Adorner>())
                .ToArray()
                .Foreach(a =>
                {
                    a.Visibility = Visibility.Hidden;
                    layer.Remove(a);
                });
        }

        /// <summary>Показать водяной знак для компонента</summary>
        /// <param name="control">Компонент, для которого надо показать водяной знак</param>
        private static void ShowWatermark([NotNull] Control control)
        {
            if (control is null) throw new NullReferenceException(nameof(control));

            var layer = AdornerLayer.GetAdornerLayer(control);

            // Графический слой может отсутствовать, если элемент больше не в визуальном дереве
            if (layer is null) return;
            var watermark_adorners = (layer.GetAdorners(control) ?? Enumerable.Empty<Adorner>())
                .OfType<WatermarkAdorner>()
                .ToArray();
            if (watermark_adorners.Length == 0)
                layer.Add(new WatermarkAdorner(control, GetValue(control)));
            else
                watermark_adorners.Foreach(a => a.UpdateLayout());

        }

        /// <summary>Проверка необходимости показать водяной знак компонента</summary>
        /// <param name="control"><see cref="Control"/> - компонент, для которого надо проверить видимость</param>
        /// <returns>Истина, если компонент удовлетворяет условию отображения водяного знака</returns>
        private static bool ShouldShowWatermark([CanBeNull] Control control)
        {
            switch (control)
            {
                case ComboBox combo_box:
                    return combo_box.SelectedItem is null;
                case TextBox text_box:
                    return text_box.Text == string.Empty;
                case PasswordBox password_box:
                    return password_box.Password == string.Empty;
                case ItemsControl items_control:
                    return items_control.Items.Count == 0;
                default:
                    return false;
            }
        }

        /// <summary>Слой водяного знака</summary>
        private class WatermarkAdorner : Adorner
        {
            #region Закрытые поля

            /// <summary><see cref="ContentPresenter"/> - объект, содержащий водяной знак</summary>
            [NotNull] private readonly ContentPresenter _ContentPresenter;

            #endregion

            #region Конструктор

            /// <summary>Инициализация нового <see cref="WatermarkAdorner"/></summary>
            /// <param name="control"><see cref="UIElement"/> - компонент, которому назначается водяной знак</param>
            /// <param name="watermark">Значение водяного знака</param>
            public WatermarkAdorner([NotNull] UIElement control, [CanBeNull] object watermark)
                : base(control)
            {
                if (control is null) throw new ArgumentNullException(nameof(control));
                if (watermark is null) throw new ArgumentNullException(nameof(watermark));
                //ЗАпретить показ подсказок
                IsHitTestVisible = false;

                // Новый компонент, содержащий водяной знак
                _ContentPresenter = new ContentPresenter();
                // Если значение водяного знака - строка
                if (watermark is UIElement)
                    _ContentPresenter.Content = watermark;
                else
                    _ContentPresenter.Content = new TextBlock
                    {
                        Text = watermark.ToString(),
                        Margin = new Thickness(4, 0, 4, 0),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                _ContentPresenter.SetBinding(ContentPresenter.ContentProperty, new Binding
                {
                    Path = new PropertyPath("(0)", ValueProperty),
                    Source = control,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

                _ContentPresenter.SetBinding(VerticalAlignmentProperty, new Binding
                {
                    Path = new PropertyPath("(0)", Watermark.VerticalAlignmentProperty),
                    Source = control,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
                _ContentPresenter.SetBinding(HorizontalAlignmentProperty, new Binding
                {
                    Path = new PropertyPath("(0)", Watermark.HorizontalAlignmentProperty),
                    Source = control,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
                _ContentPresenter.SetBinding(TextElement.ForegroundProperty, new Binding
                {
                    Path = new PropertyPath("(0)", Watermark.ForegroundProperty),
                    Source = control,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
                _ContentPresenter.SetBinding(TextElement.FontSizeProperty, new Binding
                {
                    Path = new PropertyPath("(0)", Watermark.FontSizeProperty),
                    Source = control,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

                _ContentPresenter.Opacity = GetOpacity(Control);
                _ContentPresenter.SetBinding(OpacityProperty, new Binding
                {
                    Path = new PropertyPath("(0)", Watermark.OpacityProperty),
                    Source = control,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

                _ContentPresenter.Margin = new Thickness(
                    Control.Margin.Left + Control.Padding.Left,
                    Control.Margin.Top + Control.Padding.Top,
                    Control.Margin.Right + Control.Padding.Right,
                    Control.Margin.Bottom + Control.Padding.Bottom);

                //Исли компонент контролирует другие компоненты и компонент - не ComboBox
                if (Control is ItemsControl && !(Control is ComboBox))
                {   // размещаем водяной знак по центру
                    _ContentPresenter.VerticalAlignment = VerticalAlignment.Center;
                    _ContentPresenter.HorizontalAlignment = HorizontalAlignment.Center;
                }

                // Создаём связь свойств водяного знака и компонента по свойству видимости 
                SetBinding(VisibilityProperty, new Binding("IsVisible")
                {
                    Source = control,
                    Converter = new BooleanToVisibilityConverter()
                });

                var binding_item = watermark as FrameworkElement ?? this;


                binding_item.SetBinding(TextElement.ForegroundProperty, new Binding
                {
                    Path = new PropertyPath("(0)", ForegroundProperty),
                    Source = control,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
            }

            #endregion

            #region Protected Properties

            /// <summary>Число дочерних слоёв <see cref="ContainerVisual"/></summary>
            protected override int VisualChildrenCount => 1;

            #endregion

            #region Private Properties

            /// <summary>Космонент, который надо отобразить</summary>
            private Control Control => (Control)AdornedElement;

            #endregion

            #region Protected Overrides

            /// <summary>
            /// Возвращает специальный тип дочернего <see cref="Visual"/> для родительского <see cref="ContainerVisual"/>.
            /// </summary>
            /// <param name="index">Индекс дочернего <see cref="Visual"/>. Значение индекса должно быть между 0 и <see cref="VisualChildrenCount"/> - 1</param>
            /// <returns>Дочерний <see cref="Visual"/></returns>
            protected override Visual GetVisualChild(int index) => _ContentPresenter;

            /// <summary> Реализует любое ручное поведение процесса измерения слоя</summary>
            /// <param name="constraint">Необходимый размер</param>
            /// <returns><see cref="Size"/> - размер нужного для отображения слоя</returns>
            protected override Size MeasureOverride(Size constraint)
            {
                // Здесь секрет получения размера слоя, накрывающего весь компонент
                _ContentPresenter.Measure(Control.RenderSize);
                return Control.RenderSize;
            }

            /// <summary>
            /// При переопределении в производном классе размещает дочерние элементы и определяет размер для класса, производного от <see cref="T:System.Windows.FrameworkElement"/>. 
            /// </summary>
            /// <returns>Реальный используемый размер</returns>
            /// <param name="FinalSize">Итоговая область в родительском элементе, которую этот элемент должен использовать для собственного размещения и размещения своих дочерних элементов.</param>
            protected override Size ArrangeOverride(Size FinalSize)
            {
                _ContentPresenter.Arrange(new Rect(FinalSize));
                return FinalSize;
            }

            #endregion
        }
    }
}
