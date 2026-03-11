namespace MathCore.WPF.Controls;

/// <summary>Направление обновления спектрального водопада</summary>
public enum WaterfallFlowDirection
{
    /// <summary>Обновление от верхней границы к нижней</summary>
    TopToBottom,

    /// <summary>Обновление от нижней границы к верхней</summary>
    BottomToTop,

    /// <summary>Обновление от левой границы к правой</summary>
    LeftToRight,

    /// <summary>Обновление от правой границы к левой</summary>
    RightToLeft
}
