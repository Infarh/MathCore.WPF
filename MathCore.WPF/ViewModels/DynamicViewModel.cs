using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using MathCore.Annotations;
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable ArgumentsStyleAnonymousFunction
// ReSharper disable LocalizableElement

// ReSharper disable UnusedMember.Global
// ReSharper disable ExplicitCallerInfoArgument

namespace MathCore.WPF.ViewModels
{
    /// <summary>Динамическая визуальная объектная модель</summary>
    public class DynamicViewModel : DynamicObject, INotifyPropertyChanged
    {
        /// <summary>Событие возникает в момент изменения значения свойства модели</summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>Генерация события изменения значения свойства</summary>
        /// <param name="PropertyName">Имя изменившегося свойства</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? PropertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        /// <summary>Словарь значений свойств модели</summary>
        protected readonly Dictionary<string, object> _PropertiesValues;

        /// <summary>Словарь значений индексаторов объекта</summary>
        protected readonly Dictionary<object[], object> _IndexersValues =
            new(
                new LambdaEqualityComparer<object[]>(
                    Comparer: (k1, k2) => k1.Length == k2.Length && !k1.Where((t, i) => !Equals(t, k2[i])).Any(),
                    HashFunction: k => k.GetComplexHashCode()));

        /// <summary>Инициализация новой динамической модели-представления</summary>
        public DynamicViewModel() => _PropertiesValues = new Dictionary<string, object>();

        /// <summary>Инициализация новой динамической модели-представления</summary><param name="PropertiesDictionary">Словарь для хранения значений свойств объекта</param>
        public DynamicViewModel(Dictionary<string, object> PropertiesDictionary) => _PropertiesValues = PropertiesDictionary;

        /// <summary>Установка значения свойства</summary>
        /// <param name="property">Имя изменяемого свойства</param>
        /// <param name="value">Значение, устанавливаемое для свойства</param>
        /// <returns>Истина, если значение было установлено</returns>
        protected virtual bool SetPropertyValue([NotNull] string property, object value)
        {
            if (property is null) throw new ArgumentNullException(nameof(property));
            if (property == string.Empty) throw new ArgumentException("Указанное имя свойства пусто", nameof(property));
            if (_PropertiesValues.TryGetValue(property, out var old_value) && ReferenceEquals(old_value, value) || Equals(old_value, value)) return true;
            _PropertiesValues[property] = value;
            OnPropertyChanged(property);
            return true;
        }

        /// <summary>Имя свойства индексатора</summary>
        protected const string __Items_PropertyName = "Items";

        /// <summary>Установка индексированного значения</summary>
        /// <param name="indexes">Индексы значения</param>
        /// <param name="value">Устанавливаемое значение</param>
        /// <returns>Истина, если значение было установлено</returns>
        protected virtual bool SetIndexedValue([NotNull] object[] indexes, object value)
        {
            if (indexes is null) throw new ArgumentNullException(nameof(indexes));
            if (indexes.Length == 0) throw new ArgumentException("Количество индексов должно быть больше 0", nameof(indexes));
            if (_IndexersValues.TryGetValue(indexes, out var old_value) && ReferenceEquals(old_value, value) || Equals(old_value, value)) return true;
            _IndexersValues[indexes] = value;
            OnPropertyChanged(__Items_PropertyName);
            return true;
        }

        /// <summary>Попытка получить значение свойства</summary>
        /// <param name="property">Имя свойства, значение которого требуется получить</param>
        /// <param name="value">Значение свойства</param>
        /// <returns>Истина, если значение свойства было определено до вызова метода</returns>
        protected virtual bool TryGetPropertyValue([NotNull] string property, [CanBeNull] out object? value) => _PropertiesValues.TryGetValue(property, out value);

        /// <summary>Попытка получить индексированное значение</summary>
        /// <param name="indexes">Индексы значения</param>
        /// <param name="value">Получаемое значение</param>
        /// <returns>Истина, если значение получить удалось</returns>
        protected virtual bool TryGetIndexedValue([NotNull] object[] indexes, [CanBeNull] out object? value) => _IndexersValues.TryGetValue(indexes, out value);

        /// <inheritdoc />
        public override bool TryGetMember(GetMemberBinder binder, [CanBeNull] out object? result) => TryGetPropertyValue(binder.Name, out result);

        /// <inheritdoc />
        public override bool TrySetMember(SetMemberBinder binder, object value) => SetPropertyValue(binder.Name, value);

        /// <inheritdoc />
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, [CanBeNull] out object? result) => TryGetIndexedValue(indexes, out result);

        /// <inheritdoc />
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value) => SetIndexedValue(indexes, value);
    }
}
