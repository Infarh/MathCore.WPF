using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Markup;

using MathCore.Extensions.Expressions;

namespace MathCore.WPF.ViewModels
{
    [MarkupExtensionReturnType(typeof(ValidableViewModel))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1033:Методы интерфейса должны быть доступны для вызова дочерним типам", Justification = "<Ожидание>")]
    public abstract class ValidableViewModel : ViewModel, IDataErrorInfo, INotifyDataErrorInfo
    {
        private class PropertyValidator
        {
            private readonly Func<object> _Getter;
            private readonly Func<object, bool> _Validator;
            public string? ErrorMessage { get; }

            public PropertyValidator(Func<object> Getter, Func<object, bool> Validator, string? ErrorMessage)
            {
                _Getter = Getter;
                _Validator = Validator;
                this.ErrorMessage = ErrorMessage;
            }

            public bool IsValid => _Validator(_Getter());

            public bool IsPropertyValid(out string? ErrorMessage)
            {
                var is_valid = IsValid;
                ErrorMessage = is_valid ? null : this.ErrorMessage;
                return is_valid;
            }

            public string? GetErrorMessageIfInvalid() => IsPropertyValid(out var error) ? null : error;
        }

        private readonly Dictionary<string, List<PropertyValidator>> _Validators = new Dictionary<string, List<PropertyValidator>>();

        protected ValidableViewModel(bool CheckDependencies = true)
            : base(CheckDependencies)
        {
            var type = GetType();
            foreach (var property in type.GetProperties().Where(p => p.CanRead))
            {
                var validator_attributes = property.GetCustomAttributes<ValidationAttribute>().ToArray();
                if (validator_attributes.Length == 0) continue;

                var getter = Expression
                   .Property(this.ToExpression(), property.Name)
                   .CreateLambda<Func<object>>()
                   .Compile();

                static string GetAttributeName(ValidationAttribute attribute)
                {
                    var type = attribute.GetType();
                    var type_name = type.Name;
                    const string attribute_name = "Attribute";
                    return type_name.EndsWith(attribute_name, StringComparison.InvariantCulture)
                        ? type_name.Substring(0, type_name.Length - attribute_name.Length)
                        : type_name;
                }

                _Validators.Add(property.Name, validator_attributes.Select(validator => new PropertyValidator(getter, validator.IsValid, validator.ErrorMessage ?? $"{property.Name} {GetAttributeName(validator)}")).ToList());
            }
        }

        #region IDataErrorInfo

        string? IDataErrorInfo.this[string Property] =>
            _Validators.TryGetValue(Property, out var validators)
                ? validators
                   .Where(validator => !validator.IsValid)
                   .Select(validator => validator.ErrorMessage)
                   .JoinStrings(Environment.NewLine)
                : null;

        string IDataErrorInfo.Error => _Validators.Keys
           .Select(property => ((IDataErrorInfo)this)[property])
           .JoinStrings(Environment.NewLine);

        #endregion

        #region INotifyDataErrorInfo

        IEnumerable INotifyDataErrorInfo.GetErrors(string PropertyName) =>
            _Validators.TryGetValue(PropertyName, out var validators)
                ? validators
                   .Where(validator => !validator.IsValid)
                   .Select(validator => validator.ErrorMessage)
                : Enumerable.Empty<string>();

        bool INotifyDataErrorInfo.HasErrors => _Validators.Values.Any(propertry_validators => propertry_validators.Any(validator => !validator.IsValid));

        protected event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add => ErrorsChanged += value;
            remove => ErrorsChanged -= value;
        }

        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e) => ErrorsChanged?.Invoke(this, e);

        #endregion

        protected override void OnPropertyChanged(string PropertyName = null, bool UpdateCommandsState = false)
        {
            base.OnPropertyChanged(PropertyName, UpdateCommandsState);

            Validate(PropertyName);
        }

        private void Validate(string Property)
        {
            var event_handler = ErrorsChanged;
            if (event_handler is null) return;

            if (_Validators.TryGetValue(Property, out var validators) && validators.Any(v => !v.IsValid))
                OnErrorsChanged(new DataErrorsChangedEventArgs(Property));
        }
    }
}
