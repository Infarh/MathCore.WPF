using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MathCore.WPF;

/// <summary>Прибор-адорнер для изменения размера элемента с угловыми маркерами</summary>
public class ResizingAdorner : Adorner
{
    /// <summary>Верхний левый маркер</summary>
    private readonly Thumb _TopLeft;

    /// <summary>Верхний правый маркер</summary>
    private readonly Thumb _TopRight;

    /// <summary>Нижний левый маркер</summary>
    private readonly Thumb _BottomLeft;

    /// <summary>Нижний правый маркер</summary>
    private readonly Thumb _BottomRight;

    /// <summary>Коллекция визуальных дочерних элементов адорнера</summary>
    private readonly VisualCollection _VisualChildren;

    /// <summary>Количество визуальных дочерних элементов</summary>
    protected override int VisualChildrenCount => _VisualChildren.Count;

    /// <summary>Инициализирует новый экземпляр <see cref="ResizingAdorner"/> для заданного элемента</summary>
    /// <param name="AdornedElement">Элемент, к которому применяется адорнер</param>
    public ResizingAdorner(UIElement AdornedElement) : base(AdornedElement)
    {
        _VisualChildren = new(this);
        BuildAdornerCorner(ref _TopLeft, Cursors.SizeNWSE);
        BuildAdornerCorner(ref _TopRight, Cursors.SizeNESW);
        BuildAdornerCorner(ref _BottomLeft, Cursors.SizeNESW);
        BuildAdornerCorner(ref _BottomRight, Cursors.SizeNWSE);

        _BottomLeft.DragDelta  += HandleBottomLeft;
        _BottomRight.DragDelta += HandleBottomRight;
        _TopLeft.DragDelta     += HandleTopLeft;
        _TopRight.DragDelta    += HandleTopRight;
    }

    /// <summary>Обработчик изменения размера с нижнего правого угла</summary>
    private void HandleBottomRight(object sender, DragDeltaEventArgs args)
    {
        if (AdornedElement is not FrameworkElement element || sender is not Thumb thumb) return;
        // Убедиться, что Width и Height инициализированы после изменения размера // кратко по делу
        EnforceSize(element);

        // Изменить размер на значение, на которое пользователь перетянул мышь, с учётом минимального размера маркера // кратко по делу
        element.Width  = Math.Max(element.Width + args.HorizontalChange, thumb.DesiredSize.Width);
        element.Height = Math.Max(args.VerticalChange + element.Height, thumb.DesiredSize.Height);
    }

    /// <summary>Обработчик изменения размера с верхнего правого угла</summary>
    private void HandleTopRight(object sender, DragDeltaEventArgs args)
    {
        if (AdornedElement is not FrameworkElement element || sender is not Thumb thumb) return;
        // Убедиться, что Width и Height инициализированы после изменения размера // кратко по делу
        EnforceSize(element);

        // Изменить ширину в соответствии с горизонтальным смещением // кратко по делу
        element.Width = Math.Max(element.Width + args.HorizontalChange, thumb.DesiredSize.Width);
        // Вычислить новое положение и высоту для верхнего маркера // кратко по делу

        var height_old = element.Height;
        var height_new = Math.Max(element.Height - args.VerticalChange, thumb.DesiredSize.Height);
        var top_old    = Canvas.GetTop(element);
        element.Height = height_new;
        Canvas.SetTop(element, top_old - (height_new - height_old));
    }

    /// <summary>Обработчик изменения размера с верхнего левого угла</summary>
    private void HandleTopLeft(object sender, DragDeltaEventArgs args)
    {
        if (AdornedElement is not FrameworkElement element || sender is not Thumb thumb) return;

        // Убедиться, что Width и Height инициализированы после изменения размера // кратко по делу
        EnforceSize(element);

        // Изменить ширину и сдвинуть элемент по X при уменьшении слева // кратко по делу
        var width_old = element.Width;
        var width_new = Math.Max(element.Width - args.HorizontalChange, thumb.DesiredSize.Width);
        var left_old  = Canvas.GetLeft(element);
        element.Width = width_new;
        Canvas.SetLeft(element, left_old - (width_new - width_old));

        // Изменить высоту и сдвинуть элемент по Y при уменьшении сверху // кратко по делу
        var height_old = element.Height;
        var height_new = Math.Max(element.Height - args.VerticalChange, thumb.DesiredSize.Height);
        var top_old    = Canvas.GetTop(element);
        element.Height = height_new;
        Canvas.SetTop(element, top_old - (height_new - height_old));
    }

    /// <summary>Обработчик изменения размера с нижнего левого угла</summary>
    private void HandleBottomLeft(object sender, DragDeltaEventArgs args)
    {
        if (AdornedElement is not FrameworkElement element || sender is not Thumb thumb) return;

        // Убедиться, что Width и Height инициализированы после изменения размера // кратко по делу
        EnforceSize(element);

        // Изменить высоту в соответствии с вертикальным смещением // кратко по делу
        element.Height = Math.Max(args.VerticalChange + element.Height, thumb.DesiredSize.Height);

        // Изменить ширину и сдвинуть элемент по X при уменьшении слева // кратко по делу
        var width_old = element.Width;
        var width_new = Math.Max(element.Width - args.HorizontalChange, thumb.DesiredSize.Width);
        var left_old  = Canvas.GetLeft(element);
        element.Width = width_new;
        Canvas.SetLeft(element, left_old - (width_new - width_old));
    }

    /// <summary>Разместить маркеры-адорнеры по углам выделенного элемента</summary>
    /// <param name="FinalSize">Окончательный размер, выделенный системой для адорнера</param>
    /// <returns>Возвращает фактический используемый размер</returns>
    protected override Size ArrangeOverride(Size FinalSize)
    {
        // desiredWidth и desiredHeight это размеры элемента, к которому применяется адорнер // кратко по делу
        var size_width     = AdornedElement.DesiredSize.Width;
        var desired_height = AdornedElement.DesiredSize.Height;
        // adornerWidth и adornerHeight используются для размещения маркеров // кратко по делу
        var adorner_width  = DesiredSize.Width;
        var adorner_height = DesiredSize.Height;

        _TopLeft.Arrange(new(-adorner_width / 2, -adorner_height / 2, adorner_width, adorner_height));
        _TopRight.Arrange(new(size_width - adorner_width / 2, -adorner_height / 2, adorner_width, adorner_height));
        _BottomLeft.Arrange(new(-adorner_width / 2, desired_height - adorner_height / 2, adorner_width, adorner_height));
        _BottomRight.Arrange(new(size_width - adorner_width / 2, desired_height - adorner_height / 2, adorner_width, adorner_height));

        // Возвращаем итоговый размер // кратко по делу
        return FinalSize;
    }

    /// <summary>Создаёт маркер-угол и добавляет его в визуальную коллекцию</summary>
    /// <param name="thumb">Переменная, куда будет присвоен созданный маркер</param>
    /// <param name="cursor">Курсор, используемый для маркера</param>
    private void BuildAdornerCorner(ref Thumb thumb, Cursor cursor)
    {
        if (thumb != null) return;
        // Задаём некоторые визуальные характеристики маркера // кратко по делу
        _VisualChildren.Add(thumb = new()
        {
            Cursor     = cursor,
            Height     = 10,
            Width      = 10,
            Opacity    = 0.40,
            Background = new SolidColorBrush(Colors.MediumBlue)
        });
    }

    /// <summary>Гарантирует инициализацию Width и Height и ограничивает максимальные размеры</summary>
    /// <param name="element">Элемент, для которого нужно установить размеры</param>
    private static void EnforceSize(FrameworkElement element)
    {
        if (element.Width.Equals(double.NaN))
            element.Width = element.DesiredSize.Width;
        if (element.Height.Equals(double.NaN))
            element.Height = element.DesiredSize.Height;

        if (element.Parent is not FrameworkElement parent) return;
        element.MaxHeight = parent.ActualHeight;
        element.MaxWidth  = parent.ActualWidth;
    }

    /// <summary>Возвращает визуального дочернего по индексу</summary>
    /// <param name="index">Индекс визуального дочернего элемента</param>
    /// <returns>Визуальный дочерний элемент</returns>
    protected override Visual GetVisualChild(int index) => _VisualChildren[index];

    ///// <inheritdoc />
    //protected override void OnRender(DrawingContext drawing)
    //{
    //    var element_rect = new Rect(this.AdornedElement.DesiredSize);

    //    var brush = new SolidColorBrush(Colors.Green) { Opacity = 0.2 };
    //    var pen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
    //    var radius = 5.0;

    //    drawing.DrawEllipse(brush, pen, element_rect.TopLeft, radius, radius);
    //    drawing.DrawEllipse(brush, pen, element_rect.TopRight, radius, radius);
    //    drawing.DrawEllipse(brush, pen, element_rect.BottomLeft, radius, radius);
    //    drawing.DrawEllipse(brush, pen, element_rect.BottomRight, radius, radius);
    //}
}