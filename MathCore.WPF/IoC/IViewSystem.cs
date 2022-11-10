using System.Windows;

namespace MathCore.WPF.IoC;

public interface IViewSystem : IEnumerable<Window>
{
    void Register<TObject, TViewModel>() where TViewModel : class;

    void RegisterViewModel<TViewModel, TView>() where TViewModel : class where TView : Window;

    Window? CreateView(object Model);

    Window CreateView<TWindow>() where TWindow : Window;

    Window? View(object obj);

    bool? ViewDialog(object obj);
}