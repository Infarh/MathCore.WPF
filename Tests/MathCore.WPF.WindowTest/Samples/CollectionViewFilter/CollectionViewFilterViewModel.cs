using System.Windows.Markup;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.Samples.CollectionViewFilter;

[MarkupExtensionReturnType(typeof(CollectionViewFilterViewModel))]
internal class CollectionViewFilterViewModel : ViewModel
{
    /// <summary>Сотрудники</summary>
    public IEnumerable<Employee> Employees { get; } = CreateEmployees();

    private static IEnumerable<Employee> CreateEmployees()
    {
        var rnd = new Random();
        return Enumerable.Range(1, 10000)
           .Select(
                i => new Employee
                {
                    Id          = i,
                    EmployeName = $"Employee {i}",
                    Birthday    = new DateTime(rnd.Next(1950, 2001), rnd.Next(1, 13), rnd.Next(1, 28)),
                    Position    = $"Position {i}",
                    Department  = $"Department {i}"
                })
           .ToArray();
    }
}