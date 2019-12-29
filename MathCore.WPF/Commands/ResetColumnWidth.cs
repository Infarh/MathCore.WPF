using System.Windows;
using System.Windows.Controls;

namespace MathCore.WPF.Commands
{
    public class ResetColumnWidth : LambdaCommand<ColumnDefinition>
    {
        public ResetColumnWidth() : base(column => column.Width = GridLength.Auto, column => column != null) { }
    }
}