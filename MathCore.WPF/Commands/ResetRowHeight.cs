using System.Windows;
using System.Windows.Controls;

namespace MathCore.WPF.Commands
{
    public class ResetRowHeight : LambdaCommand<RowDefinition>
    {
        public ResetRowHeight() : base(row => row.Height = GridLength.Auto, row => row != null) { }
    }
}