using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;

using MathCore.WPF.Behaviors;

using Validators = System.Collections.Generic.Dictionary<string, (System.Delegate Get, System.ComponentModel.DataAnnotations.ValidationAttribute[] Validators)>;

namespace MathCore.WPF.ViewModels
{
    [MarkupExtensionReturnType(typeof(ValidableViewModel))]
    public abstract class ValidableViewModel : ViewModel, IDataErrorInfo, IValidationExceptionHandler, INotifyDataErrorInfo
    {
        #region IDataErrorInfo

        private string[] GetPropertyErrors(string Property)
        {
            if (!_Validators.TryGetValue(Property, out var property)) return Array.Empty<string>();

            var value = property.Get.DynamicInvoke();
            var errors = property.Validators
               .Where(validator => !validator.IsValid(value))
               .Select(validator => validator.ErrorMessage ?? $"{Property} {validator.GetType().Name}");
            return errors.ToArray();
        }

        protected string GetPropertyError(string Property) => string.Join(Environment.NewLine, GetPropertyErrors(Property));

        protected IEnumerable<string> GetModelErrors()
        {
            foreach (var (getter, validators) in _Validators.Values)
                foreach (var validator in validators)
                    if (!validator.IsValid(getter.DynamicInvoke(this)))
                        yield return validator.ErrorMessage;
        }

        protected string GetError() => string.Join(Environment.NewLine, GetModelErrors());

#pragma warning disable CA1033 // Методы интерфейса должны быть доступны для вызова дочерним типам
        string IDataErrorInfo.this[string Property] => GetPropertyError(Property);

        string IDataErrorInfo.Error => GetError();
#pragma warning restore CA1033 // Методы интерфейса должны быть доступны для вызова дочерним типам

        #endregion

        #region INotifyDataErrorInfo

#pragma warning disable CA1033 // Методы интерфейса должны быть доступны для вызова дочерним типам
        IEnumerable INotifyDataErrorInfo.GetErrors(string PropertyName) => GetPropertyErrors(PropertyName);

        bool INotifyDataErrorInfo.HasErrors => GetModelErrors().Any();

        private event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add => ErrorsChanged += value;
            remove => ErrorsChanged -= value;
        }
#pragma warning restore CA1033 // Методы интерфейса должны быть доступны для вызова дочерним типам

        #endregion

        private readonly Validators _Validators;

        public int ValidPropertiesCount
        {
            get
            {
                var valid_validators =
                    from validators in _Validators
                    where validators.Value.Validators.All(validator => validator.IsValid(validators.Value.Get.DynamicInvoke()))
                    select validators;
                return valid_validators.Count() - _ValidationExceptionCount;
            }
        }

        public int ValidablePropertiesCount => _Validators.Count;

        private static Validators CreateValidators(ValidableViewModel model) =>
            model.GetType()
               .GetProperties(BindingFlags.Instance | BindingFlags.Public)
               .Where(p => p.CanRead)
               .Select(property => (Info: property, Validators: property.GetCustomAttributes<ValidationAttribute>().ToArray()))
               .Where(property => property.Validators.Length > 0)
               .Select(property =>
                {
                    var (info, validators) = property;
                    var method_info = info.GetMethod!;
                    var method_type = typeof(Func<>).MakeGenericType(method_info.ReturnType);
                    var method = method_info!.CreateDelegate(method_type, model);
                    return (
                        info.Name,
                        Validators: validators,
                        Get: method
                    );
                })
               .Where(property => property.Validators.Length > 0)
               .ToDictionary(
                    property => property.Name,
                    property => (property.Get, property.Validators));

        protected ValidableViewModel(bool CheckDependencies = true) : base(CheckDependencies) => _Validators = CreateValidators(this);

        private int _ValidationExceptionCount;
        public void ValidationExceptionsChanged(int count, object ErrorContent)
        {
            _ValidationExceptionCount = count;
            OnPropertyChanged(nameof(ValidPropertiesCount));
        }

#pragma warning disable CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.
        protected override void OnPropertyChanged(string PropertyName = null, bool UpdateCommandsState = false)
        {
            base.OnPropertyChanged(PropertyName, UpdateCommandsState);

            Validate(PropertyName);
        }
#pragma warning restore CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.

        private void Validate(string Property)
        {
            var event_handler = ErrorsChanged;
            if (event_handler is null) return;
            var error = GetPropertyError(Property);
            if (string.IsNullOrEmpty(error)) return;
            event_handler(this, new DataErrorsChangedEventArgs(Property));
        }
    }
}
