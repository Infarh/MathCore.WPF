using MathCore.WPF.Services;

namespace MathCore.WPF.Extensions;

public static class ProgressInfoExtensions
{
    public static IProgressControl GetControl(this IProgressInfo Info) => Info.Progress.GetControl(Info.Cancel);
}