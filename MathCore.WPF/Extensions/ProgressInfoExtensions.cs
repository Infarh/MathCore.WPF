using MathCore.WPF.Services;

namespace MathCore.WPF.Extensions;

public static class ProgressInfoExtensions
{
    public static IProgressControl GetControl(this IProgressInfo Info) => Info.Progress.GetControl(Info.Cancel);

    public static async Task<T> Do<T>(this IProgressInfo Progress, Func<IProgressInfo, Task<T>> Selector)
    {
        using (Progress)
            return await Selector(Progress).ConfigureAwait(false);
    }

    public static async Task<T> Do<T, TParameter>(this IProgressInfo Progress, TParameter p, Func<IProgressInfo, TParameter, Task<T>> Selector)
    {
        using (Progress)
            return await Selector(Progress, p).ConfigureAwait(false);
    }

    public static ProgressValue<T> With<T>(this IProgressInfo Progress, T value) => new(Progress, value);

    public readonly struct ProgressValue<TValue>(IProgressInfo Progress, TValue Value)
    {
        public async ValueTask<T> Do<T>(Func<IProgressInfo, TValue, Task<T>> Selector)
        {
            using (Progress)
                return await Selector(Progress, Value).ConfigureAwait(false);
        }
    }

    public static IProgressInfo SetStatus(this IProgressInfo progress, string status)
    {
        progress.Status.Report(status);
        return progress;
    }
}