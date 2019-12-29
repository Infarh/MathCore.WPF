using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MathCore.WPF.Commands;

namespace MathCore.WPF.Dialogs
{
    public class ShowWindow : Dialog
    {

        #region IsModal property : bool

        public static readonly DependencyProperty IsModalProperty =
            DependencyProperty.Register(
                nameof(IsModal),
                typeof(bool),
                typeof(ShowWindow),
                new PropertyMetadata(default(bool)));

        public bool IsModal
        {
            get => (bool)GetValue(IsModalProperty);
            set => SetValue(IsModalProperty, value);
        }

        #endregion

        #region DialogResult property : bool

        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.Register(
                nameof(DialogResult),
                typeof(bool?),
                typeof(ShowWindow),
                new PropertyMetadata(default(bool?)));

        public bool? DialogResult
        {
            get => (bool?)GetValue(DialogResultProperty);
            set => SetValue(DialogResultProperty, value);
        }

        #endregion

        #region Style property : bool

        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register(
                nameof(Style),
                typeof(Style),
                typeof(ShowWindow),
                new PropertyMetadata(default(Style)));/*,
                s => s == null
                     || s is Style
                        && ((Style)s).TargetType == null
                            || ((Style)s).TargetType == typeof(Window)
                            || ((Style)s).TargetType.IsSubclassOf(typeof(Window)));*/

        public Style Style
        {
            get => (Style)GetValue(StyleProperty);
            set => SetValue(StyleProperty, value);
        }

        #endregion

        #region Template property : ControlTemplate

        //public static readonly DependencyProperty TemplateProperty =
        //DependencyProperty.Register(
        //    nameof(Template),
        //    typeof(ControlTemplate),
        //    typeof(ShowWindow),
        //    new PropertyMetadata(default(ControlTemplate)),
        //    t => t == null
        //         || t is ControlTemplate
        //         && ((ControlTemplate)t).TargetType == null
        //            || ((ControlTemplate)t).TargetType == typeof(Window)
        //            || ((ControlTemplate)t).TargetType.IsSubclassOf(typeof(Window)));

        //public ControlTemplate Template
        //{
        //    get { return (ControlTemplate)GetValue(Control.TemplateProperty); }
        //    set { SetValue(Control.TemplateProperty, value); }
        //}

        #endregion

        #region ContentTemplate property : DataTemplate

        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(
                nameof(ContentTemplate),
                typeof(DataTemplate),
                typeof(ShowWindow),
                new PropertyMetadata(default(DataTemplate)));

        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        #endregion

        #region Width property : double

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(
                nameof(Width),
                typeof(double),
                typeof(ShowWindow),
                new PropertyMetadata(default(double)), w => (double)w >= 0);

        public double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        #endregion

        #region Height property : double

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(
                nameof(Height),
                typeof(double),
                typeof(ShowWindow),
                new PropertyMetadata(default(double)), h => (double)h >= 0);

        public double Height
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        #endregion

        #region MaxWidth dependency property : double

        /// <summary>Максимальная ширина окна</summary>
        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register(
                nameof(MaxWidth),
                typeof(double),
                typeof(ShowWindow),
                new PropertyMetadata(default(double)), w => (double)w >= 0);

        /// <summary>Максимальная ширина окна</summary>
        public double MaxWidth
        {
            get => (double)GetValue(MaxWidthProperty);
            set => SetValue(MaxWidthProperty, value);
        }

        #endregion

        #region MaxHeight dependency property : double

        /// <summary>Максимальная высота окна</summary>
        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.Register(
                nameof(MaxHeight),
                typeof(double),
                typeof(ShowWindow),
                new PropertyMetadata(default(double)), h => (double)h >= 0);

        /// <summary>Максимальная высота окна</summary>
        public double MaxHeight
        {
            get => (double)GetValue(MaxHeightProperty);
            set => SetValue(MaxHeightProperty, value);
        }

        #endregion

        #region DataContext property : object

        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register(
                nameof(DataContext),
                typeof(object),
                typeof(ShowWindow),
                new PropertyMetadata(default(object)));

        public object DataContext
        {
            get => (object)GetValue(DataContextProperty);
            set => SetValue(DataContextProperty, value);
        }

        #endregion

        #region Content property : object

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(object),
                typeof(ShowWindow),
                new PropertyMetadata(default(object)));

        public object Content
        {
            get => (object)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        #endregion

        #region Icon dependency property : ImageSource

        /// <summary>Иконка окна</summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(ImageSource),
                typeof(ShowWindow),
                new PropertyMetadata(default(ImageSource)), v => v == null || v is ImageSource);

        /// <summary>Иконка окна</summary>
        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        #endregion

        #region Window readonly dependency property : Window

        private static readonly DependencyPropertyKey WindowPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(Window),
                typeof(Window),
                typeof(ShowWindow),
                new FrameworkPropertyMetadata(default(Window)), v => v == null || v is Window);

        public static readonly DependencyProperty WindowProperty = WindowPropertyKey.DependencyProperty;

        public Window Window
        {
            get => (Window)GetValue(WindowProperty);
            private set => SetValue(WindowPropertyKey, value);
        }

        #endregion

        #region Topmost dependency property : bool

        /// <summary>Окно является окном верхнего уровня</summary>
        public static readonly DependencyProperty TopmostProperty =
            DependencyProperty.Register(
                nameof(Topmost),
                typeof(bool),
                typeof(ShowWindow),
                new PropertyMetadata(default(bool)), v => v == null || v is bool);

        /// <summary>Окно является окном верхнего уровня</summary>
        public bool Topmost
        {
            get => (bool)GetValue(TopmostProperty);
            set => SetValue(TopmostProperty, value);
        }

        #endregion

        #region Owner dependency property : Window

        /// <summary>Окно - владелец</summary>
        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register(
                nameof(Owner),
                typeof(Window),
                typeof(ShowWindow),
                new PropertyMetadata(default(Window)), v => v == null || v is Window);

        /// <summary>Окно - владелец</summary>
        public Window Owner
        {
            get => (Window)GetValue(OwnerProperty);
            set => SetValue(OwnerProperty, value);
        }

        #endregion

        #region WindowState dependency property : WindowState

        /// <summary>Состояние окна</summary>
        public static readonly DependencyProperty WindowStateProperty =
            DependencyProperty.Register(
                nameof(WindowState),
                typeof(WindowState),
                typeof(ShowWindow),
                new PropertyMetadata(default(WindowState)), v => v == null || v is WindowState);

        /// <summary>Состояние окна</summary>
        public WindowState WindowState
        {
            get => (WindowState)GetValue(WindowStateProperty);
            set => SetValue(WindowStateProperty, value);
        }

        #endregion

        #region StartupLocation dependency property : WindowStartupLocation

        /// <summary>Начальное положение окна</summary>
        public static readonly DependencyProperty StartupLocationProperty =
            DependencyProperty.Register(
                nameof(StartupLocation),
                typeof(WindowStartupLocation),
                typeof(ShowWindow),
                new PropertyMetadata(default(WindowStartupLocation)), v => v == null || v is WindowStartupLocation);

        /// <summary>Начальное положение окна</summary>
        public WindowStartupLocation StartupLocation
        {
            get => (WindowStartupLocation)GetValue(StartupLocationProperty);
            set => SetValue(StartupLocationProperty, value);
        }

        #endregion

        #region SizeToContent dependency property : SizeToContent

        /// <summary>Принцип автоматического изменения размеров окна</summary>
        public static readonly DependencyProperty SizeToContentProperty =
            DependencyProperty.Register(
                nameof(SizeToContent),
                typeof(SizeToContent),
                typeof(ShowWindow),
                new PropertyMetadata(default(SizeToContent)), v => v == null || v is SizeToContent);

        /// <summary>Принцип автоматического изменения размеров окна</summary>
        public SizeToContent SizeToContent
        {
            get => (SizeToContent)GetValue(SizeToContentProperty);
            set => SetValue(SizeToContentProperty, value);
        }

        #endregion

        //static ShowWindow()
        //{
        //    Control.TemplateProperty.OverrideMetadata(typeof(ShowWindow), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
        //}

        public ShowWindow()
        {
            _OpenCommand = new LambdaCommand((Action<object>)Open, p => !IsOpened);
        }

        public override void Open(object p)
        {
            if(IsOpened) return;
            lock (_OpenSyncRoot)
            {
                if(IsOpened) return;
                IsOpened = true;
                try
                {
                    OpenDialog(p);
                    if(!IsModal)
                        IsOpened = false;
                } catch(Exception error)
                {
                    IsOpened = false;
                    LastException = error;
                    Window = null;
                    throw new ApplicationException($"Ошибка диалога {GetType()}", error);
                }
            }
        }

        protected override void OpenDialog(object p)
        {
            var window = new Window();
            window.SetBinding(FrameworkElement.StyleProperty, new Binding("Style") { Source = this });
            //window.SetBinding(Control.TemplateProperty, new Binding("Template") { Source = this });
            window.SetBinding(ContentControl.ContentTemplateProperty, new Binding("ContentTemplate") { Source = this });
            window.SetBinding(FrameworkElement.MaxWidthProperty, new Binding("MaxWidth") { Source = this });
            window.SetBinding(FrameworkElement.WidthProperty, new Binding("Width") { Source = this });
            window.SetBinding(FrameworkElement.MaxHeightProperty, new Binding("MaxHeight") { Source = this });
            window.SetBinding(FrameworkElement.HeightProperty, new Binding("Height") { Source = this });
            window.SetBinding(Window.IconProperty, new Binding("Icon") { Source = this });
            if(p == null)
                window.SetBinding(FrameworkElement.DataContextProperty, new Binding("DataContext") { Source = this });
            else
                window.DataContext = p;
            window.SetBinding(Window.TitleProperty, new Binding("Title") { Source = this });
            window.SetBinding(ContentControl.ContentProperty, new Binding("Content") { Source = this });
            window.SetBinding(Window.TopmostProperty, new Binding("Topmost") { Source = this });
            window.SetBinding(Window.WindowStateProperty, new Binding("WindowState") { Source = this });
            window.SetBinding(Window.SizeToContentProperty, new Binding("SizeToContent") { Source = this });

            window.WindowStartupLocation = StartupLocation;
            window.Owner = Owner;
            Window = window;

            if(IsModal)
                DialogResult = window.ShowDialog();
            else
            {
                window.Closed += (s, e) =>
                {
                    DialogResult = ((Window)s).DialogResult;
                    IsOpened = false;
                    Window = null;
                };
                window.Show();
            }
        }
    }
}
