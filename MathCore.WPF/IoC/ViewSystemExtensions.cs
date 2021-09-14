using MathCore.Annotations;
using MathCore.IoC;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.IoC
{
    public static class ViewSystemExtensions
    {
        public static IServiceManager AddViewSystem(this IServiceManager Manager)
        {
            Manager.RegisterSingleton<IViewSystem, ViewSystem>().AllowInheritance = true;
            return Manager;
        }
    }
}