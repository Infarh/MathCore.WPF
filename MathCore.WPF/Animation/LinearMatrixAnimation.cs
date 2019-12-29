using System.Windows;
using System.Windows.Media.Animation;
// ReSharper disable UnusedType.Global

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
                typeof(System.Windows.Media.Matrix?),
                typeof(LinearMatrixAnimation),
                new PropertyMetadata(default(System.Windows.Media.Matrix?)));

        /// <summary>Начальная матрица</summary>
        public System.Windows.Media.Matrix? From
        {
            get => (System.Windows.Media.Matrix?)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        #endregion

        #region To : System.Windows.Media.Matrix? - Конечная матрица


        /// <summary>Конечная матрица</summary>
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register(
                nameof(To),
                typeof(System.Windows.Media.Matrix?),
                typeof(LinearMatrixAnimation),
                new PropertyMetadata(default(System.Windows.Media.Matrix?)));

        /// <summary>Конечная матрица</summary>
        public System.Windows.Media.Matrix? To
        {
            get => (System.Windows.Media.Matrix?)GetValue(ToProperty);
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

        public LinearMatrixAnimation(System.Windows.Media.Matrix? from, System.Windows.Media.Matrix to, Duration duration)
        {
            Duration = duration;
            From = from;
            To = to;
        }

        public LinearMatrixAnimation(System.Windows.Media.Matrix? to, Duration duration, FillBehavior fill)
        {
            To = to;
            Duration = duration;
            FillBehavior = fill;
        }

        public LinearMatrixAnimation(System.Windows.Media.Matrix? from, System.Windows.Media.Matrix? to, Duration duration, FillBehavior fill)
        {
            From = from;
            To = to;
            Duration = duration;
            FillBehavior = fill;
        }

        protected override System.Windows.Media.Matrix GetCurrentValueCore(System.Windows.Media.Matrix DefaultOriginValue, System.Windows.Media.Matrix DefaultDestinationValue, AnimationClock animation_clock)
        {
            if (animation_clock.CurrentProgress is null) return System.Windows.Media.Matrix.Identity;

            var time = animation_clock.CurrentProgress.Value;
            if (EasingFunction != null)
                time = EasingFunction.Ease(time);

            var from = From ?? DefaultOriginValue;
            var to = To ?? DefaultDestinationValue;

            return new System.Windows.Media.Matrix(
                (to.M11 - from.M11) * time + from.M11,
                (to.M12 - from.M12) * time + from.M12,
                (to.M21 - from.M21) * time + from.M21,
                (to.M22 - from.M22) * time + from.M22,
                (to.OffsetX - from.OffsetX) * time + from.OffsetX,
                (to.OffsetY - from.OffsetY) * time + from.OffsetY);
        }
    }
}
