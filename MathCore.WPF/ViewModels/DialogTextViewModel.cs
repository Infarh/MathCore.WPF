using System.Windows.Markup;

namespace MathCore.WPF.ViewModels;

/// <summary>Модель-представления диалога с текстовым значением</summary>
[MarkupExtensionReturnType(typeof(DialogTextViewModel))]
public class DialogTextViewModel : DialogValueViewModel<string> { }