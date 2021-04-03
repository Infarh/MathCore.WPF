using MathCore.WPF.Services;

using Microsoft.Extensions.DependencyInjection;

namespace MathCore.WPF.WindowTest
{
    public partial class App
    {
        private static void ConfigureServices(IServiceCollection Services)
        {
            Services.AddSingleton<IUserDialogBase, UserDialogService>();
        }
    }
}
