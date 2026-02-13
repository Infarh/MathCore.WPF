// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ParameterHidesMember

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedParameter.Global

namespace MathCore.WPF.ViewModels;

public abstract partial class ViewModel
{
    public readonly ref struct SetValueResult<T>
    {
        private readonly ViewModel _Model;
        private readonly bool _Result;
        private readonly T? _OldValue;
        private readonly T? _NewValue;

        /// <summary>Признак успешного изменения значения</summary>
        public bool Result => _Result;

        /// <summary>Предыдущее значение</summary>
        public T? OldValue => _OldValue;

        /// <summary>Новое значение</summary>
        public T? NewValue => _NewValue;

        internal SetValueResult(bool Result, T? OldValue, ViewModel model) : this(Result, OldValue, OldValue, model) { }
        internal SetValueResult(bool Result, T? OldValue, T? NewValue, ViewModel model)
        {
            _Result = Result;
            _OldValue = OldValue;
            _NewValue = NewValue;
            _Model = model;
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
        public bool Then(Action<object?> execute)
        {
            if (_Result) execute(NewValue);
            return _Result;
        }

        /// <summary>Выполнить действие при успешном изменении</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool Then(Action<T?> execute)
        {
            if (_Result) execute(NewValue);
            return _Result;
        }

        /// <summary>Выполнить действие асинхронно при успешном изменении</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool ThenAsync(Action execute)
        {
            if (_Result) Task.Run(execute);
            return _Result;
        }

        /// <summary>Выполнить действие асинхронно при успешном изменении</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool ThenAsync(Action<T?> execute)
        {
            if (_Result) NewValue.Async(execute);
            return _Result;
        }

        /// <summary>Выполнить действие при выполнении условия</summary>
        /// <param name="predicate">Условие выполнения</param>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool ThenIf(Func<T?, bool> predicate, Action<T?> execute)
        {
            if (_Result && predicate(NewValue)) execute(NewValue);
            return _Result;
        }

        /// <summary>Выполнить действие асинхронно при выполнении условия</summary>
        /// <param name="predicate">Условие выполнения</param>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool ThenIfAsync(Func<T?, bool> predicate, Action<T?> execute)
        {
            if (_Result && predicate(NewValue)) NewValue.Async(execute);
            return _Result;
        }

        /// <summary>Установить значение при успешном изменении</summary>
        /// <param name="SetAction">Действие установки</param>
        /// <returns>Текущий результат</returns>
        public SetValueResult<T> ThenSet(Action<T?> SetAction)
        {
            if (_Result) SetAction(NewValue);
            return this;
        }

        /// <summary>Установить значение асинхронно при успешном изменении</summary>
        /// <param name="SetAction">Действие установки</param>
        /// <returns>Текущий результат</returns>
        public SetValueResult<T> ThenSetAsync(Action<T?> SetAction)
        {
            if (_Result) NewValue.Async(SetAction);
            return this;
        }

        /// <summary>Выполнить действие при успешном изменении</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool Then(Action<T?, T?> execute)
        {
            if (_Result) execute(OldValue, NewValue);
            return _Result;
        }

        /// <summary>Выполнить действие асинхронно при успешном изменении</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool ThenAsync(Action<T?, T?> execute)
        {
            if (_Result) OldValue.Async(NewValue, execute);
            return _Result;
        }

        /// <summary>Уведомить об изменении указанного свойства</summary>
        /// <param name="PropertyName">Имя свойства</param>
        /// <param name="UpdateCommands">Обновить состояния команд</param>
        /// <returns>Текущий результат</returns>
        public SetValueResult<T> ThenUpdate(string PropertyName, bool UpdateCommands = false)
        {
            if (_Result) _Model.OnPropertyChanged(PropertyName, UpdateCommands);
            return this;
        }

        /// <summary>Уведомить об изменении указанных свойств</summary>
        /// <param name="PropertyNames">Имена свойств</param>
        /// <returns>Текущий результат</returns>
        public SetValueResult<T> ThenUpdate(params string[] PropertyNames)
        {
            if (!_Result) return this;
            foreach (var property in PropertyNames)
                _Model.OnPropertyChanged(property);
            return this;
        }

        /// <summary>Уведомить об изменении указанных свойств</summary>
        /// <param name="UpdateCommands">Обновить состояния команд</param>
        /// <param name="PropertyNames">Имена свойств</param>
        /// <returns>Текущий результат</returns>
        public SetValueResult<T> ThenUpdate(bool UpdateCommands, params string[] PropertyNames)
        {
            if (!_Result) return this;
            foreach (var property in PropertyNames)
                _Model.OnPropertyChanged(property, UpdateCommands);
            return this;
        }

        /// <summary>Уведомить об изменении указанного свойства</summary>
        /// <param name="PropertyName">Имя свойства</param>
        /// <param name="UpdateCommands">Обновить состояния команд</param>
        /// <returns>Текущий результат</returns>
        public SetValueResult<T> Update(string PropertyName, bool UpdateCommands = false)
        {
            _Model.OnPropertyChanged(PropertyName, UpdateCommands);
            return this;
        }

        /// <summary>Уведомить об изменении указанных свойств</summary>
        /// <param name="PropertyName">Имена свойств</param>
        /// <returns>Текущий результат</returns>
        public SetValueResult<T> Update(params string[] PropertyName)
        {
            foreach (var name in PropertyName) _Model.OnPropertyChanged(name);
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
            execute(NewValue);
            return _Result;
        }

        /// <summary>Выполнить действие независимо от результата</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool AnywayThen(Action<T?, bool> execute)
        {
            execute(NewValue, _Result);
            return _Result;
        }

        /// <summary>Выполнить действие независимо от результата</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool AnywayThen(Action<T?, T?> execute)
        {
            execute(OldValue, NewValue);
            return _Result;
        }

        /// <summary>Выполнить действие независимо от результата</summary>
        /// <param name="execute">Действие</param>
        /// <returns>Истина, если изменение произошло</returns>
        public bool AnywayThen(Action<T?, T?, bool> execute)
        {
            execute(OldValue, NewValue, _Result);
            return _Result;
        }

        /// <summary>Преобразовать результат в логическое значение</summary>
        /// <param name="result">Результат установки</param>
        /// <returns>Истина, если изменение произошло</returns>
        public static implicit operator bool(SetValueResult<T> result) => result._Result;
    }
}