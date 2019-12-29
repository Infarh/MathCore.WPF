using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MathCore.Annotations;

namespace MathCore.WPF.ViewModels
{
    public abstract class DependencyViewModel : Freezable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
}