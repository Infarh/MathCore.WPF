using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.Samples.CollectionViewFilter
{
    [MarkupExtensionReturnType(typeof(CollectionViewFilterViewModel))]
    internal class CollectionViewFilterViewModel : ViewModel
    {
        /// <summary>Сотрудники</summary>
        public IEnumerable<Employee> Employees { get; } = Enumerable.Range(1, 100)
           .Select(
                i => new Employee
                {
                    Id = i,
                    EmployeName = $"Employee {i}",
                    Birthday = DateTime.Now
                       .Subtract(TimeSpan.FromDays(365 * (i + 20)))
                       .Subtract(TimeSpan.FromDays(i * 3 + 17)),
                    Department = $"Department {i}",
                    Position = $"Position {i}"
                })
           .ToArray();
    }
}
