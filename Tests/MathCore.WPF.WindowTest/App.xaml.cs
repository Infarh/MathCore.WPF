using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

using MathCore.WPF.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace MathCore.WPF.WindowTest
{
    public partial class App
    {
        public ServiceProvider Services { get; }

        public App()
        {
            var vm = new TestValidableViewModel();

            var error_info = vm as IDataErrorInfo;
            var error_informer = vm as INotifyDataErrorInfo;
            error_informer.ErrorsChanged += (s, e) => Debug.WriteLine(e.PropertyName + " invalid: " + error_info[e.PropertyName]);

            vm.Name = "123";
            vm.Name = null;
            vm.Name = "QWE";
            vm.Name = "";

            var service_collection = new ServiceCollection();
            ConfigureServices(service_collection);
            Services = service_collection.BuildServiceProvider();
        }


    }

    internal class TestValidableViewModel : ValidableViewModel
    {
        #region Name : string - Имя

        /// <summary>Имя</summary>
        private string _Name;

        /// <summary>Имя</summary>
        [Required(AllowEmptyStrings = false)]
        public string Name { get => _Name; set => Set(ref _Name, value); }

        #endregion
    }

}
