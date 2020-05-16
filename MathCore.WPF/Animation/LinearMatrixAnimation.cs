using System.Windows;
using System.Windows.Media.Animation;
using TransformMatrix = System.Windows.Media.Matrix;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Animation
{
    public class LinearMatrixAnimation : MatrixAnimationBase
    {
        protected override Freezable CreateInstanceCore() => new LinearMatrixAnimation();

        #region From : System.Windows.Media.Matrix? - Начальная матрица

        /// <summary>Начальная матрица</summary>
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register(
                nameof(From),
                typeof(TransformMatrix?),
                typeof(LinearMatrixAnimation),
                new PropertyMetadata(default(TransformMatrix?)));

        /// <summary>Начальная матрица</summary>
        public TransformMatrix? From
        {
            get => (TransformMatrix?)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        #endregion

        #region To : System.Windows.Media.Matrix? - Конечная матрица

        /// <summary>Конечная матрица</summary>
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register(
                nameof(To),
                typeof(TransformMatrix?),
                typeof(LinearMatrixAnimation),
                new PropertyMetadata(default(TransformMatrix?)));

        /// <summary>Конечная матрица</summary>
        public TransformMatrix? To
        {
            get => (TransformMatrix?)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        #endregion

        #region EasingFunction : IEasingFunction  - Функция плавности

        /// <summary>Функция плавности</summary>
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register(
                nameof(EasingFunction),
                typeof(IEasingFunction),
                typeof(LinearMatrixAnimation),
                new PropertyMetadata(default(IEasingFunction)));

        /// <summary>Функция плавности</summary>
        public IEasingFunction EasingFunction
        {
            get => (IEasingFunction)GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }

        #endregion

        public LinearMatrixAnimation() { }

        public LinearMatrixAnimation(TransformMatrix? from, TransformMatrix to, Duration duration)
        {
            Duration = duration;
            From = from;
            To = to;
        }

        public LinearMatrixAnimation(TransformMatrix? to, Duration duration, FillBehavior fill)
        {
            To = to;
            Duration = duration;
            FillBehavior = fill;
        }

        public LinearMatrixAnimation(TransformMatrix? from, TransformMatrix? to, Duration duration, FillBehavior fill)
        {
            From = from;
            To = to;
            Duration = duration;
            FillBehavior = fill;
        }

        protected override TransformMatrix GetCurrentValueCore(TransformMatrix DefaultOriginValue, TransformMatrix DefaultDestinationValue, AnimationClock animation_clock)
        {
            if (animation_clock.CurrentProgress is null) return TransformMatrix.Identity;

            var time = animation_clock.CurrentProgress.Value;
            if (EasingFunction != null)
                time = EasingFunction.Ease(time);

            var from = From ?? DefaultOriginValue;
            var to = To ?? DefaultDestinationValue;

            return new TransformMatrix(
                (to.M11 - from.M11) * time + from.M11,
                (to.M12 - from.M12) * time + from.M12,
                (to.M21 - from.M21) * time + from.M21,
                (to.M22 - from.M22) * time + from.M22,
                (to.OffsetX - from.OffsetX) * time + from.OffsetX,
                (to.OffsetY - from.OffsetY) * time + from.OffsetY);
        }
    }
}