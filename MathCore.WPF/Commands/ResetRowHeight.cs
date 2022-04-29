using System.Windows;
using System.Windows.Controls;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Commands;

public class ResetRowHeight : LambdaCommand<RowDefinition>
{
    public ResetRowHeight() : base(row => row.Height = GridLength.Auto, row => row != null) { }
}