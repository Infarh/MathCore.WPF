using System;
using System.Windows;
using System.Windows.Markup;

using MathCore.WPF.ViewModels;
using MathCore.WPF.WindowTest.ViewModels;

namespace MathCore.WPF.WindowTest
{
    [MarkupExtensionReturnType(typeof(TestViewModel))]
    class TestViewModel : ViewModel
    {
        public Graph Graph { get; } = new Graph();

        public TestViewModel()
        {
            var rnd = new Random();
            var graph = Graph;
            for (var i = 0; i < 20; i++)
                graph.Add(new Node { Position = new Point(rnd.Next(10, 491), rnd.Next(10, 491)), Radius = rnd.Next(2, 11) });
        }
    }
}
