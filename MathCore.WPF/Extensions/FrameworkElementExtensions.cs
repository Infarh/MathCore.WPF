using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Windows;

/// <summary>Класс методов-расширений для класса FrameworkElement</summary>
public static class FrameworkElementExtensions
{
    public static void RegisterForNotification(this FrameworkElement element, string PropertyName, PropertyChangedCallback callback)
    {
        var binding = new Binding(PropertyName) { Source = element };
        var prop = DependencyProperty.RegisterAttached($"ListenAttached{PropertyName}",
            typeof(object),
            element.GetType(),
            new PropertyMetadata(callback));

        element.SetBinding(prop, binding);
    }

    public static void Serialize(this UIElement element, Stream stream) => XamlWriter.Save(element, stream);

    public static void Serialize(this UIElement element, TextWriter writer) => XamlWriter.Save(element, writer);

    public static void Serialize(this UIElement element, XmlWriter writer) => XamlWriter.Save(element, writer);

    public static string SerializeToStr(this UIElement element)
    {
        var result = new StringBuilder();
        var writer_settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };

        var writer = XmlWriter.Create(result, writer_settings);
        var manager = new XamlDesignerSerializationManager(writer) { XamlWriterMode = XamlWriterMode.Expression };
        XamlWriter.Save(element, manager);
        return result.ToString();
    }

    public static XDocument SerializeToXml(this UIElement element) => XDocument.Parse(element.SerializeToStr());

    //public static XmlDocument Serialize(this UIElement element)
    //{
    //    var output = new StringBuilder();
    //    XamlWriter.Save(element, new XamlDesignerSerializationManager(XmlWriter.Create(output, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true })) { XamlWriterMode = XamlWriterMode.Expression });
    //    var xml = new XmlDocument();
    //    xml.LoadXml(output.ToString());
    //    return xml;
    //}

    public static Task<bool> LongMouseButtonPress(this FrameworkElement element, TimeSpan duration, MouseButton button = MouseButton.Left)
    {
        var tcs = new TaskCompletionSource<bool>();

        var timer = new DispatcherTimer { Interval = duration };

        void OnMouseUp(object s, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != button) return;
            timer.Stop();
            if (tcs.Task.Status == TaskStatus.Running)
                tcs.SetResult(false);
        }

        element.PreviewMouseUp += OnMouseUp;

        timer.Tick += (_, _) =>
        {
            timer.Start();
            element.PreviewMouseUp -= OnMouseUp;
            tcs.SetResult(true);
        };

        timer.Start();
        return tcs.Task;
    }

    private class ElementDragManager : IDisposable
    {
        private static Size GetLayoutSize(FrameworkElement element)
        {
            var actual_width = element.ActualWidth;
            var actual_height = element.ActualHeight;
            if (element is Image or MediaElement)
                if (element.Parent is Canvas)
                {
                    actual_width = double.IsNaN(element.Width) ? actual_width : element.Width;
                    actual_height = double.IsNaN(element.Height) ? actual_height : element.Height;
                }
                else
                {
                    actual_width = element.RenderSize.Width;
                    actual_height = element.RenderSize.Height;
                }

            var width = element.Visibility == Visibility.Collapsed ? 0.0 : actual_width;
            var height = element.Visibility == Visibility.Collapsed ? 0.0 : actual_height;
            return new Size(width, height);
        }

        private readonly FrameworkElement _Element;
        private readonly bool _AllowX;
        private readonly bool _AllowY;
        private readonly UIElement? _Root;
        private Point _StartPoint;
        private readonly bool _ConstrainToParent;

        private Transform? _RenderTransform;

        private Transform? RenderTransform
        {
            get => _RenderTransform ??= GetElementTransform(_Element.RenderTransform);
            set
            {
                if (Equals(_RenderTransform, value)) return;
                _RenderTransform = _Element.RenderTransform = value;
            }
        }

        private static Transform? GetElementTransform(Transform? Transform) =>
            Transform switch
            {
                ScaleTransform transform => new ScaleTransform { CenterX = transform.CenterX, CenterY = transform.CenterY, ScaleX = transform.ScaleX, ScaleY = transform.ScaleY },
                RotateTransform transform => new RotateTransform { Angle = transform.Angle, CenterX = transform.CenterX, CenterY = transform.CenterY },
                SkewTransform transform => new SkewTransform { AngleX = transform.AngleX, AngleY = transform.AngleY, CenterX = transform.CenterX, CenterY = transform.CenterY },
                TranslateTransform transform => new TranslateTransform { X = transform.X, Y = transform.Y },
                MatrixTransform transform => new MatrixTransform { Matrix = transform.Matrix },
                TransformGroup transform => new TransformGroup { Children = new TransformCollection(transform.Children.Select(GetElementTransform)) },
                _ => null
            };


        private Rect ElementBounds => new(GetLayoutSize(_Element));

        public ElementDragManager(FrameworkElement element, bool AllowX = true, bool AllowY = true, bool ConstrainToParent = false)
        {
            _Element = element;
            _AllowX = AllowX;
            _AllowY = AllowY;
            _ConstrainToParent = ConstrainToParent;
            _Root = element.FindVisualRoot() as UIElement;
            element.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), false);
        }

        private void OnMouseLeftButtonDown(object Sender, MouseButtonEventArgs E) => StartDrag(E.GetPosition(_Element));

        private void OnMouseLeftButtonUp(object Sender, MouseButtonEventArgs E) => EndDrag();

        private void OnLostMouseCapture(object Sender, MouseEventArgs E) => EndDrag();

        private void StartDrag(Point StartPoint)
        {
            _StartPoint = StartPoint;
            _Element.CaptureMouse();
            _Element.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUp), true);
            _Element.AddHandler(UIElement.MouseMoveEvent, new MouseEventHandler(OnMouseMove), true);
            _Element.AddHandler(UIElement.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));
        }

        private void EndDrag()
        {
            _Element.ReleaseMouseCapture();
            _Element.RemoveHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUp));
            _Element.RemoveHandler(UIElement.MouseMoveEvent, new MouseEventHandler(OnMouseMove));
        }

        private void OnMouseMove(object Sender, MouseEventArgs E) => HandleDrag(E.GetPosition(_Element));

        private static Vector GetTransformVector(GeneralTransform? transform, double X, double Y)
        {
            if (transform is null) return new();
            var start_point = transform.Transform(new Point());
            var end_point = transform.Transform(new Point(X, Y));
            return end_point - start_point;
        }

        private void HandleDrag(Point CurrentMousePosition)
        {
            var offset_x = CurrentMousePosition.X - _StartPoint.X;
            var offset_y = CurrentMousePosition.Y - _StartPoint.Y;
            var element_to_root = _Root is null ? null : _Element.TransformToVisual(_Root);
            var transform_vector = GetTransformVector(element_to_root, offset_x, offset_y);
            ApplyTranslation(transform_vector);
        }

        private void ApplyTranslation(Vector Transition)
        {
            if (_Element.Parent is not FrameworkElement parent) return;
            var parent_transition_transform = _Root?.TransformToVisual(parent);
            var transition = GetTransformVector(parent_transition_transform, Transition.X, Transition.Y);

            if (_ConstrainToParent)
            {
                var parent_bounds = new Rect(0, 0, parent.ActualWidth, parent.ActualHeight);

                var object_to_parent = _Element.TransformToVisual(parent);
                var object_bounding_box = object_to_parent.TransformBounds(ElementBounds);

                var end_position = object_bounding_box;
                end_position.Location += transition;

                if (!RectContainsRect(parent_bounds, end_position))
                {
                    if (end_position.X < parent_bounds.Left)
                        transition.X -= end_position.X - parent_bounds.Left;
                    else if (end_position.Right > parent_bounds.Right)
                        transition.X -= end_position.Right - parent_bounds.Right;

                    if (end_position.Y < parent_bounds.Top)
                        transition.Y -= end_position.Y - parent_bounds.Top;
                    else if (end_position.Bottom > parent_bounds.Bottom)
                        transition.Y -= end_position.Bottom - parent_bounds.Bottom;
                }
            }

            ApplyTranslationTransform(transition);
        }

        private void ApplyTranslationTransform(Vector Transition)
        {
            var transform = RenderTransform;
            if (transform is not TranslateTransform translate_transform)
                switch (transform)
                {
                    case TransformGroup transform_group:
                        var transforms = transform_group.Children;
                        var tr_count = transforms.Count;
                        if (tr_count > 0 && transforms[tr_count-1] is TranslateTransform t)
                            translate_transform = t;
                        else
                            transforms.Add(translate_transform = new TranslateTransform());
                        break;

                    case MatrixTransform transform_matrix:
                        var matrix = transform_matrix.Matrix;
                        matrix.OffsetX += Transition.X;
                        matrix.OffsetY += Transition.Y;
                        RenderTransform = new MatrixTransform(matrix);
                        return;

                    default:
                        var new_transform_group = new TransformGroup();
                        if (transform != null)
                            new_transform_group.Children.Add(transform);
                        new_transform_group.Children.Add(translate_transform = new TranslateTransform());
                        RenderTransform = new_transform_group;
                        break;
                }

            translate_transform!.X += Transition.X;
            translate_transform.Y += Transition.Y;
        }

        private static bool RectContainsRect(Rect rect1, Rect rect2) =>
            !rect1.IsEmpty
            && !rect2.IsEmpty
            && rect1.X <= rect2.X && rect1.Y <= rect2.Y
            && rect1.X + rect1.Width >= rect2.X + rect2.Width
            && rect1.Y + rect1.Height >= rect2.Y + rect2.Height;

        /// <inheritdoc />
        public void Dispose()
        {
            _Element.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown));
            EndDrag();
        }
    }

    public static IDisposable StartDragging(this FrameworkElement element, bool AllowX = true, bool AllowY = true, bool ConstrainToParent = false) =>
        new ElementDragManager(element, AllowX, AllowY, ConstrainToParent);
}