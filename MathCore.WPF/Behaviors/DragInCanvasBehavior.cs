using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Behaviors
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Качество кода", "IDE0052:Удалить непрочитанные закрытые члены", Justification = "<Ожидание>")]
    public class DragInCanvasBehavior : Behavior<UIElement>
    {
        /// <summary>Ссылка на канву</summary>
        private Canvas? _Canvas;

        /// <summary>Запись точной позиции, в которой нажата кнопка</summary>
        private Point _StartPoint;

        #region IsDragging

        /// <summary>Отслеживание перетаскивания элемента</summary>
        private bool _IsDragging;

        private bool IsDragging
        {
            get => _IsDragging;
            set
            {
                if (_IsDragging)
                {
                    if (value) return;
                    _IsDragging = false;
                    AssociatedObject.ReleaseMouseCapture();
                }
                else
                {
                    if (!value) return;
                    _IsDragging = true;
                    AssociatedObject.CaptureMouse();
                }
            }
        }

        #endregion

        #region AllowX : bool - Разрешено перемещение по оси X

        /// <summary>Разрешено перемещение по оси X</summary>
        public static readonly DependencyProperty AllowXProperty =
            DependencyProperty.Register(
                nameof(AllowX),
                typeof(bool),
                typeof(DragInCanvasBehavior),
                new PropertyMetadata(true));

        /// <summary>Разрешено перемещение по оси X</summary>
        public bool AllowX
        {
            get => (bool)GetValue(AllowXProperty);
            set => SetValue(AllowXProperty, value);
        }

        #endregion

        #region AllowY : bool - Разрешено перетаскивание по оси Y

        /// <summary>summary</summary>
        public static readonly DependencyProperty AllowYProperty =
            DependencyProperty.Register(
                nameof(AllowY),
                typeof(bool),
                typeof(DragInCanvasBehavior),
                new PropertyMetadata(true));

        /// <summary>Разрешено перетаскивание по оси Y</summary>
        public bool AllowY
        {
            get => (bool)GetValue(AllowYProperty);
            set => SetValue(AllowYProperty, value);
        }

        #endregion

        #region Enabled : bool - Перетаскивание активно

        /// <summary>Перетаскивание активно</summary>
        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.Register(
                nameof(Enabled),
                typeof(bool),
                typeof(DragInCanvasBehavior),
                new PropertyMetadata(true, (d, e) => ((DragInCanvasBehavior)d).IsDragging &= (bool)e.NewValue));

        /// <summary>Перетаскивание активно</summary>
        public bool Enabled
        {
            get => (bool)GetValue(EnabledProperty);
            set => SetValue(EnabledProperty, value);
        }

        #endregion

        /// <summary>Присоединение поведения к объекту</summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        /// <summary>Отсоединение поведения от объекта</summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        }

        /// <summary>При нажатии левой кнопки мыши</summary><param name="sender">Источник события</param><param name="e">Аргумент события</param>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Если канва не определена, то её надо найти вверх по визуальному дереву
            // ReSharper disable once ArrangeRedundantParentheses
            if ((_Canvas ??= VisualTreeHelper.GetParent(AssociatedObject) as Canvas) is null) return;

            // Фиксируем точку нажатия левой кнопки мыши относительно элемента
            _StartPoint = e.GetPosition(AssociatedObject);
            IsDragging = true;
        }

        /// <summary>При перемещении мыши</summary>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            // Если режим перетаскивания не активирован, то возврат
            if (!_IsDragging) return;
            // Иначе определяем положение указателя относительно канвы
            var current_point = e.GetPosition(_Canvas);

            // Изменяем присоединённые к элементу свойства канвы, отвечающие за положение элемента на ней
            var obj = AssociatedObject;
            if (AllowX)
            {
                if (obj.ReadLocalValue(Canvas.RightProperty) == DependencyProperty.UnsetValue)
                    obj.SetValue(Canvas.LeftProperty, current_point.X - _StartPoint.X);
                else
                    obj.SetValue(Canvas.LeftProperty, current_point.X + _StartPoint.X);
            }

            if (AllowY)
            {
                if (obj.ReadLocalValue(Canvas.BottomProperty) == DependencyProperty.UnsetValue)
                    obj.SetValue(Canvas.TopProperty, current_point.Y - _StartPoint.Y);
                else
                    obj.SetValue(Canvas.TopProperty, current_point.Y + _StartPoint.Y);
            }
        }

        /// <summary>При отпускании левой кнопки мыши</summary>
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => IsDragging = false;
    }
}