using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MathCore.WPF.WindowTest.Samples.CollectionViewFilter
{
    internal class Employee
    {
        [Key]
        [Display(AutoGenerateField = false)][ReadOnly(true)]
        public int Id { get; set; }

        [Display(Name = "Имя", Description = "Имя сотрудника")]
        public string EmployeName { get; set; } = null!;

        [Display(Name = "Дата рождения", Description = "Дата рождения сотрудника")]
        [DisplayFormat(DataFormatString = "dd.MM.yyyy", NullDisplayText = "[--]")]
        public DateTime? Birthday { get; set; }

        [Display(Name = "Должность", Description = "Должность сотрудника")]
        public string Position { get; set; } = null!;

        [Display(Name = "Отдел", Description = "Название отдела")] [ReadOnly(true)]
        public string Department { get; set; } = null!;

        public override string ToString() => $"id:{Id} - {EmployeName}({Birthday:d}) - {Department}({Position})";
    }
}
