using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MathCore.Annotations;
using Microsoft.Xaml.Behaviors;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Behaviors
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Стиль", "IDE1006:Стили именования", Justification = "<Ожидание>")]
    public class DragBehavior : Behavior<FrameworkElement>
    {
        private abstract class ObjectMover : IDisposable
        {
            private readonly Point _StartPos;
            [NotNull] private readonly IInputElement _ParentElement;
            [NotNull] private readonly UIElement _MovingElement;
            [NotNull] private readonly DragBehavior _Behavior;

            protected ObjectMover([NotNull] UIElement element, [NotNull] DragBehavior behavior)
            {
                _MovingElement = element;
                _Behavior = behavior;
                _ParentElement = element.FindLogicalParent<IInputElement>() ?? throw new InvalidOperationException("Не найден родительский элемент");
                _StartPos = Mouse.GetPosition(_ParentElement);
                Mouse.Capture(element, CaptureMode.SubTree);
                element.MouseMove += OnMouseMove;
                element.MouseLeftButtonUp += OnLeftMouseUp;
            }

            private void OnLeftMouseUp([CanBeNull] object Sender, [CanBeNull] MouseButtonEventArgs E) => Dispose();

            private void OnMouseMove([NotNull] object Sender, [NotNull] MouseEventArgs E)
            {
                var element = (FrameworkElement)Sender;
                if (Equals(Mouse.Captured, element))
                {
                    var pos = E.GetPosition(_ParentElement);
                    var dx = pos.X - _StartPos.X;
                    var dy = pos.Y - _StartPos.Y;
                    OnMouseMove(element, dx, dy);
                    _Behavior.dx = dx;
                    _Behavior.dy = dy;
                    _Behavior.Radius = Math.Sqrt(dx * dx + dy * dy);
                    _Behavior.Angle = Math.Atan2(dy, dx);
                    return;
                }
                Dispose();
            }

            protected abstract void OnMouseMove(FrameworkElement element, double dx, double dy);

            public void Dispose()
            {
                _MovingElement.MouseMove -= OnMouseMove;
                _MovingElement.MouseLeftButtonUp -= OnLeftMouseUp;
                _MovingElement.ReleaseMouseCapture();
            }
        }

        private class ThicknessObjectMover : ObjectMover
        {

            private readonly Thickness _StartThickness;
            public ThicknessObjectMover([NotNull] FrameworkElement element, [NotNull] DragBehavior behavior) : base(element, behavior) => _StartThickness = element.Margin;

            protected override void OnMouseMove([NotNull] FrameworkElement element, double dx, double dy) => element.Margin = new Thickness
            (
                _StartThickness.Left + dx,
                _StartThickness.Top + dy,
                _StartThickness.Right - dx,
                _StartThickness.Bottom - dy
            );
        }

        private class CanvasObjectMover : ObjectMover
        {
            private readonly double _StartLeft;
            private readonly double _StartRight;
            private readonly double _StartTop;
            private readonly double _StartBottom;

            public CanvasObjectMover([NotNull] FrameworkElement element, [NotNull] DragBehavior behavior) : base(element, behavior)
            {
                _StartLeft = Canvas.GetLeft(element);
                _StartRight = Canvas.GetRight(element);
                _StartTop = Canvas.GetTop(element);
                _StartBottom = Canvas.GetBottom(element);
            }

            protected override void OnMouseMove([NotNull] FrameworkElement element, double dx, double dy)
            {
                if (!double.IsNaN(_StartLeft)) Canvas.SetLeft(element, _StartLeft + dx);
                if (!double.IsNaN(_StartRight)) Canvas.SetLeft(element, _StartRight - dx);
                if (!double.IsNaN(_StartTop)) Canvas.SetLeft(element, _StartTop + dy);
                if (!double.IsNaN(_StartBottom)) Canvas.SetLeft(element, _StartBottom - dy);
            }
        }

        #region Enabled

        /// <summary></summary>
        [NotNull]
        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.Register(
                nameof(Enabled),
                typeof(bool),
                typeof(DragBehavior),
                new PropertyMetadata(default(bool), (s, e) => { if (!(bool)e.NewValue) ((DragBehavior)s)?._ObjectMover?.Dispose(); }));

        /// <summary></summary>
        public bool Enabled
        {
            get => (bool)GetValue(EnabledProperty);
            set => SetValue(EnabledProperty, value);
        }

        #endregion

        #region dx : double - Величина смещения по горизонтали

        /// <summary>Величина смещения по горизонтали</summary>
        [NotNull]
        private static readonly DependencyPropertyKey dxPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(dx),
                typeof(double),
                typeof(DragBehavior),
                new PropertyMetadata(default(double)));

        /// <summary>Величина смещения по горизонтали</summary>
        [NotNull] public static readonly DependencyProperty dxProperty = dxPropertyKey.DependencyProperty;

        /// <summary>Величина смещения по горизонтали</summary>
        public double dx
        {
            get => (double)GetValue(dxProperty);
            private set => SetValue(dxPropertyKey, value);
        }

        #endregion

        #region dy : double - Величина смещения по вертикали

        /// <summary>Величина смещения по вертикали</summary>
        [NotNull]
        private static readonly DependencyPropertyKey dyPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(dy),
                typeof(double),
                typeof(DragBehavior),
                new PropertyMetadata(default(double)));

        /// <summary>Величина смещения по вертикали</summary>
        [NotNull] public static readonly DependencyProperty dyProperty = dyPropertyKey.DependencyProperty;

        /// <summary>Величина смещения по вертикали</summary>
        public double dy
        {
            get => (double)GetValue(dyProperty);
            private set => SetValue(dyPropertyKey, value);
        }

        #endregion

        #region Radius

        [NotNull]
        private static readonly DependencyPropertyKey RadiusPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(Radius),
                typeof(double),
                typeof(DragBehavior),
                new PropertyMetadata(default(double)));

        [NotNull] public static readonly DependencyProperty RadiusProperty = RadiusPropertyKey.DependencyProperty;

        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            private set => SetValue(RadiusPropertyKey, value);
        }

        #endregion

        #region Angle

        [NotNull]
        private static readonly DependencyPropertyKey AnglePropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(Angle),
                typeof(double),
                typeof(DragBehavior),
                new PropertyMetadata(default(double)));

        [NotNull] public static readonly DependencyProperty AngleProperty = AnglePropertyKey.DependencyProperty;

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            private set => SetValue(AnglePropertyKey, value);
        }

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            _ObjectMover?.Dispose();
            _ObjectMover = null;
        }

        private ObjectMover? _ObjectMover;
        private void OnMouseLeftButtonDown([CanBeNull] object Sender, [CanBeNull] MouseButtonEventArgs E)
        {
            if (!(Sender is FrameworkElement element)) return;
            var parent = element.FindLogicalParent<IInputElement>();
            if (parent is null) return;
            _ObjectMover = parent switch
            {
                Canvas _ => new CanvasObjectMover(element, this),
                Panel _ => new ThicknessObjectMover(element, this),
                GroupBox _ => new ThicknessObjectMover(element, this),
                _ => _ObjectMover
            };
        }
    }
}