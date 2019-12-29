using MathCore.IoC;

namespace MathCore.WPF.IoC
{
    public static class ViewSystemExtensions
    {
        public static IServiceManager AddViewSystem(this IServiceManager service_manager)
        {
            service_manager.RegisterSingleton<IViewSystem, ViewSystem>().AllowInheritance = true;
            return service_manager;
        }
    }
}