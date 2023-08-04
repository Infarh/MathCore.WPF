using MathCore.WPF.ViewModels;

namespace MathCore.WPF.Extensions;

public static class SetValueResultExtensions
{
    public static ViewModel.SetValueResult<T> Debug<T>(this ViewModel.SetValueResult<T> Result)
    {
        System.Diagnostics.Debug.WriteLine(Result.NewValue);
        return Result;
    }
}
