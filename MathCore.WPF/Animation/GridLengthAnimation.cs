using System.Windows;
using System.Windows.Media.Animation;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Animation;

public class GridLengthAnimation : AnimationTimeline
{
    #region property From : GridLength

    public static readonly DependencyProperty FromProperty =
        DependencyProperty.Register(
            nameof(From),
            typeof(GridLength),
            typeof(GridLengthAnimation));

    public GridLength From
    {
        get => (GridLength)GetValue(FromProperty);
        set => SetValue(FromProperty, value);
    }

    #endregion

    #region property To : GridLength

    public static readonly DependencyProperty ToProperty =
        DependencyProperty.Register(
            nameof(To),
            typeof(GridLength),
            typeof(GridLengthAnimation));

    public GridLength To
    {
        get => (GridLength)GetValue(ToProperty);
        set => SetValue(ToProperty, value);
    } 

    #endregion

    public override Type TargetPropertyType => typeof(GridLength);

    protected override Freezable CreateInstanceCore() => new GridLengthAnimation();

    public override object GetCurrentValue(object DefaultOriginValue, object DefaultDestinationValue, AnimationClock clock)
    {
        var from_val = ((GridLength)GetValue(FromProperty)).Value;
        var to_val   = ((GridLength)GetValue(ToProperty)).Value;

        var clock_current_progress = clock.CurrentProgress ?? double.NaN;
        return from_val > to_val
            ? new((1 - clock_current_progress) * (from_val - to_val) + to_val, GridUnitType.Star)
            : new GridLength(clock_current_progress * (to_val - from_val) + from_val, GridUnitType.Star);
    }
}