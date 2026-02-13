// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ParameterHidesMember

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedParameter.Global

namespace MathCore.WPF.ViewModels;

public abstract partial class ViewModel
{
    /// <summary>Результат установки значения свойства со статическим обработчиком</summary>
    public readonly ref struct SetStaticValueResult<T>
    {
        private readonly bool _Result;
        private readonly T? _OldValue;
        private readonly T? _NewValue;
        private readonly Action<string> _OnPropertyChanged;

        internal SetStaticValueResult(bool Result, T? OldValue, Action<string> OnPropertyChanged) : this(Result, OldValue, OldValue, OnPropertyChanged) { }
        internal SetStaticValueResult(bool Result, T? OldValue, T? NewValue, Action<string> OnPropertyChanged)
        {
            _Result = Result;
            _OldValue = OldValue;
            _NewValue = NewValue;
            _OnPropertyChanged = OnPropertyChanged;
        }

        /// <summary>Выполнить действие при успешном изменении</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool Then(Action execute)
        {
            if (_Result) execute();
            return _Result;
        }

        /// <summary>Выполнить действие при успешном изменении</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool Then(Action<T?> execute)
        {
            if (_Result) execute(_NewValue);
            return _Result;
        }

        /// <summary>Выполнить действие при успешном изменении</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool Then(Action<T?, T?> execute)
        {
            if (_Result) execute(_OldValue, _NewValue);
            return _Result;
        }

        /// <summary>Уведомить об изменении указанного свойства</summary>
        /// <param name="PropertyName">Имя свойства</param>
        /// <returns>Текущий результат</returns>
        public SetStaticValueResult<T> Update(string PropertyName)
        {
            _OnPropertyChanged(PropertyName);
            return this;
        }

        /// <summary>Уведомить об изменении указанных свойств</summary>
        /// <param name="PropertyName">Имена свойств</param>
        /// <returns>Текущий результат</returns>
        public SetStaticValueResult<T> Update(params string[] PropertyName)
        {
            foreach (var name in PropertyName) _OnPropertyChanged(name);
            return this;
        }

        /// <summary>Выполнить действие независимо от результата</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool AnywayThen(Action execute)
        {
            execute();
            return _Result;
        }

        /// <summary>Выполнить действие независимо от результата</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool AnywayThen(Action<bool> execute)
        {
            execute(_Result);
            return _Result;
        }

        /// <summary>Выполнить действие независимо от результата</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool AnywayThen(Action<T?> execute)
        {
            execute(_NewValue);
            return _Result;
        }

        /// <summary>Выполнить действие независимо от результата</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool AnywayThen(Action<T?, bool> execute)
        {
            execute(_NewValue, _Result);
            return _Result;
        }

        /// <summary>Выполнить действие независимо от результата</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool AnywayThen(Action<T?, T?> execute)
        {
            execute(_OldValue, _NewValue);
            return _Result;
        }

        /// <summary>Выполнить действие независимо от результата</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool AnywayThen(Action<T?, T?, bool> execute)
        {
            execute(_OldValue, _NewValue, _Result);
            return _Result;
        }

        /// <summary>Преобразовать результат в логическое значение</summary>
        /// <param name="result">Результат установки</param>
        /// <returns>Истина, если изменение произошло</returns>
        public static implicit operator bool(SetStaticValueResult<T> result) => result._Result;
    }
}