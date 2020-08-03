using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var model1 = new Model1();
            (model1 as INotifyDataErrorInfo).ErrorsChanged += (s, e) => Console.WriteLine(e.PropertyName);
            model1.Value1 = null;

            Console.WriteLine("Hello World!");
        }
    }

    class Model1 : ValidableViewModel
    {
        private string _Value1 = "123";

        [Required]
        public string Value1
        {
            get => _Value1;
            set => Set(ref _Value1, value);
        }

        [Range(5, 20)]
        public int Value2 { get; set; }
    }

    class Model2 : ValidableViewModel
    {
        [Required]
        public List<string> Values { get; set; }

        [Range(5, 20)]
        public double X { get; set; }
    }
}
