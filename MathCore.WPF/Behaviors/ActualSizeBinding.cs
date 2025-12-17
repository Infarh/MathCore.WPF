using System.ComponentModel;
using System.Windows;

using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для двухсторонней привязки фактических размеров элемента</summary>
public class ActualSizeBinding : Behavior<FrameworkElement>
{
    #region ActualWidth : double - Ширина элемента

    /// <summary>Ширина элемента</summary>
    public static readonly DependencyProperty ActualWidthProperty =
        DependencyProperty.Register(
            nameof(ActualWidth),
            typeof(double),
            typeof(ActualSizeBinding),
            new FrameworkPropertyMetadata(
                default(double),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnWidthChanged));

    /// <summary>Обработчик изменения ширины элемента</summary>
    /// <param name="D">Объект зависимости</param>
    /// <param name="E">Аргументы изменения свойства</param>
    private static void OnWidthChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        if(D is not ActualSizeBinding { AssociatedObject: var element } || E.NewValue is not double width || element.ActualWidth == width) return;
        element.Width = width;
    }

    /// <summary>Ширина элемента</summary>
    //[Category("")]
    [Description("Ширина элемента")]
    public double ActualWidth
    {
        get => (double)GetValue(ActualWidthProperty);
        set => SetValue(ActualWidthProperty, value);
    }

    #endregion

    #region ActualHeight : double - Высота элемента

    /// <summary>Высота элемента</summary>
    public static readonly DependencyProperty ActualHeightProperty =
        DependencyProperty.Register(
            nameof(ActualHeight),
            typeof(double),
            typeof(ActualSizeBinding),
            new FrameworkPropertyMetadata(
                default(double),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnHeightChanged));

    /// <summary>Обработчик изменения высоты элемента</summary>
    /// <param name="D">Объект зависимости</param>
    /// <param name="E">Аргументы изменения свойства</param>
    private static void OnHeightChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        if(D is not ActualSizeBinding { AssociatedObject: var element } || E.NewValue is not double height || element.ActualHeight == height) return;
        element.Height = height;
    }

    /// <summary>Высота элемента</summary>
    //[Category("")]
    [Description("Высота элемента")]
    public double ActualHeight { get => (double)GetValue(ActualHeightProperty); set => SetValue(ActualHeightProperty, value); }

    #endregion

    /// <summary>Вызывается при присоединении поведения к элементу</summary>
    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.SizeChanged += OnElementSizeChanged;

        //BindingOperations.SetBinding(this, ActualWidthProperty, new Binding(nameof(FrameworkElement.ActualWidth))
        //{
        //    Source = AssociatedObject,
        //    Mode = BindingMode.OneWay
        //});

        //BindingOperations.SetBinding(this, ActualHeightProperty, new Binding(nameof(FrameworkElement.ActualHeight))
        //{
        //    Source = AssociatedObject,
        //    Mode = BindingMode.OneWay
        //});
    }

    /// <summary>Вызывается при отсоединении поведения от элемента</summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.SizeChanged -= OnElementSizeChanged;
    }

    /// <summary>Обработчик изменения размера элемента</summary>
    /// <param name="Sender">Источник события</param>
    /// <param name="E">Аргументы события изменения размера</param>
    private void OnElementSizeChanged(object Sender, SizeChangedEventArgs E)
    {
        var (width, height) = E.NewSize;
        if (E.WidthChanged)
            ActualWidth = width;
        if (E.HeightChanged)
            ActualHeight = height;
    }
}