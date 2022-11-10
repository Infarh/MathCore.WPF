using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors;

public class DataGridBeh : Behavior<DataGrid>
{
    protected override void OnAttached()
    {
        //AssociatedObject.SelectedCellsChanged
        //AssociatedObject.SelectionChanged += OnSelectionChanged;
    }

    private void OnSelectionChanged(object Sender, SelectionChangedEventArgs E)
    {
        //string.
    }
}
