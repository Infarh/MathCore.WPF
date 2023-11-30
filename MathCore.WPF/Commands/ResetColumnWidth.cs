using System.Windows;
using System.Windows.Controls;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Commands;

public class ResetColumnWidth() : LambdaCommand<ColumnDefinition>(column => column.Width = GridLength.Auto, column => column != null);