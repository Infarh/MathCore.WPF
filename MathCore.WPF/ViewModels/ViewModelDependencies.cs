using System.ComponentModel;
using System.Linq.Expressions;
using System.Linq.Reactive;
using System.Reflection;
// ReSharper disable UnusedType.Global

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace MathCore.WPF.ViewModels;

partial class ViewModel
{

}

public static class ViewModelExtensions
{
    public class ViewModelPropertyObserver<T, TValue> : IDisposable, IObservableEx<TValue>
        where T : ViewModel
    {
        public event Action<T>? PropertyChanged;

        private readonly T _Model;
        private readonly string _PropertName;

        public ViewModelPropertyObserver(T Model, string PropertName)
        {
            _Model       = Model ?? throw new ArgumentNullException(nameof(Model));
            _PropertName = PropertName ?? throw new ArgumentNullException(nameof(PropertName));
            if (string.IsNullOrWhiteSpace(PropertName)) throw new ArgumentException("Пустая строка в виде имени свойства", nameof(PropertName));
            _Model.PropertyChanged += OnPropertyChanged!;
        }

        private void OnPropertyChanged(object Sender, PropertyChangedEventArgs E)
        {
            if (E.PropertyName != _PropertName) return;
            PropertyChanged?.Invoke(_Model);
        }

        public ViewModelPropertyObserver<T, TValue> Invoke(Action<T> action)
        {
            PropertyChanged += action;
            return this;
        }

        #region IDispose implementation

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Признак того, что объект был разрушен</summary>
        private bool _Disposed;

        /// <summary>Освобождение ресурсов</summary>
        /// <param name="disposing">Выполнить освобождение управляемых ресурсов</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed || !disposing) return;
            _Disposed              =  true;
            _Model.PropertyChanged -= OnPropertyChanged;
            _Property              =  null;
        }

        #endregion

        private Property<TValue>? _Property;

        public IDisposable Subscribe(IObserver<TValue> observer) => (_Property ??= new(_Model, _PropertName)).Subscribe(observer);

        public IDisposable Subscribe(IObserverEx<TValue> observer) => (_Property ??= new(_Model, _PropertName)).Subscribe(observer);
    }

    public static ViewModelPropertyObserver<T, TValue> When<T, TValue>(this T model, Expression<Func<T, TValue>> PropertySelector)
        where T : ViewModel
    {
        if (model is null) throw new ArgumentNullException(nameof(model));
        if (PropertySelector is null) throw new ArgumentNullException(nameof(PropertySelector));

        var member_expression = PropertySelector.Body as MemberExpression ?? throw new InvalidOperationException();
        if (member_expression.Member.MemberType != MemberTypes.Property)
            throw new NotSupportedException("Выражение не является выражением доступа к свойству модели");

        var property_name = member_expression.Member.Name;

        return new(model, property_name);
    }
}