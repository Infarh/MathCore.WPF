using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest
{
    [MarkupExtensionReturnType(typeof(TestWindow3ViewModel))]
    internal class TestWindow3ViewModel : ViewModel
    {
        #region Title : string - Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _Title = "Test Window 3";

        /// <summary>Заголовок окна</summary>
        public string Title { get => _Title; set => Set(ref _Title, value); }

        #endregion

        public ObservableCollection<StudentInfo> Students { get; } = Enumerable
           .Range(1, 100)
           .Select(i => new StudentInfo
           {
               Name = $"Student {i}",
               Avatar = $"AVA{i:000}",
               Rating = i / 100d
           })
           .ToArray()
           .ToObservableCollection();
    }

    class StudentInfo : ViewModel
    {
        #region Name : string - Имя

        /// <summary>Имя</summary>
        private string _Name;

        /// <summary>Имя</summary>
        public string Name { get => _Name; set => Set(ref _Name, value); }

        #endregion

        #region Rating : double - Рейтинг

        /// <summary>Рейтинг</summary>
        private double _Rating;

        /// <summary>Рейтинг</summary>
        public double Rating { get => _Rating; set => Set(ref _Rating, value); }

        #endregion

        #region Avatar : string - Аватар

        /// <summary>Аватар</summary>
        private string _Avatar;

        /// <summary>Аватар</summary>
        public string Avatar { get => _Avatar; set => Set(ref _Avatar, value); }

        #endregion
    }
}
