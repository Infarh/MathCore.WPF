using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace MathCore.WPF
{
    /// <summary>Класс прикрепляемых свойств-зависимости для работы с <see cref="PasswordBox"/></summary>
    // ReSharper disable once UnusedMember.Global
    public static class PasswordBoxEx
    {
        #region AttachProperty

        /// <summary>Прикрепляемое свойство-зависимости, устанавливающее связь для дальнейшей работы с <see cref="PasswordBox"/></summary>
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached(
                "Attach",
                typeof(bool),
                typeof(PasswordBoxEx),
                new PropertyMetadata(default(bool), OnAttachChanged));

        /// <summary>Установка значения свойства присоединения</summary>
        /// <param name="o">Объект для которого производится установка значения</param>
        /// <param name="v">Устанавливаемое значение</param>
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static void SetAttach(DependencyObject o, bool v) => o.SetValue(AttachProperty, v);

        public static bool GetAttach(DependencyObject dp) => (bool)dp.GetValue(AttachProperty);

        private static void OnAttachChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.OldValue) ((PasswordBox)sender).PasswordChanged -= PasswordChanged;
            if ((bool)e.NewValue) ((PasswordBox)sender).PasswordChanged += PasswordChanged;
        }

        #endregion

        #region PasswordProperty

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password",
                typeof(string),
                typeof(PasswordBoxEx),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnPasswordPropertyChanged));

        public static void SetPassword(DependencyObject dp, string value) => dp.SetValue(PasswordProperty, value);

        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static string GetPassword(DependencyObject dp) => (string)dp.GetValue(PasswordProperty);

        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var password_box = (PasswordBox)sender;
            password_box.PasswordChanged -= PasswordChanged;
            if (!GetIsUpdating(password_box)) password_box.Password = (string)e.NewValue;
            password_box.PasswordChanged += PasswordChanged;
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var password_box = (PasswordBox)sender;
            SetIsUpdating(password_box, true);
            SetPassword(password_box, password_box.Password);
            SetIsUpdating(password_box, false);
        }

        #endregion

        #region IsUpdatingProperty

        private static readonly DependencyProperty IsUpdatingProperty =
           DependencyProperty.RegisterAttached(
               "IsUpdating",
               typeof(bool),
               typeof(PasswordBoxEx));

        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        private static void SetIsUpdating(DependencyObject dp, bool value) => dp.SetValue(IsUpdatingProperty, value);

        private static bool GetIsUpdating(DependencyObject dp) => (bool)dp.GetValue(IsUpdatingProperty);

        #endregion

        #region Attached property WhatermarkText : string - Текст, замещающий пустое пространство при отсутствии ввода пароля

        /// <summary>Текст, замещающий пустое пространство при отсутствии ввода пароля</summary>
        public static readonly DependencyProperty WaterMarkTextProperty =
            DependencyProperty.RegisterAttached(
                "WaterMarkText",
                typeof(string),
                typeof(PasswordBoxEx),
                new PropertyMetadata("Введите пароль"));

        public static void SetWaterMarkText(DependencyObject element, string value) => element.SetValue(WaterMarkTextProperty, value);

        public static string GetWaterMarkText(DependencyObject element) => (string)element.GetValue(WaterMarkTextProperty);

        #endregion
    }

    public class PasswordBoxWatcher : Decorator
    {
        #region Password dependency property (Other : Пароль) : string

        /// <summary>Пароль</summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                nameof(Password),
                typeof(string),
                typeof(PasswordBoxWatcher),
                new FrameworkPropertyMetadata(default(string),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnPasswordChanged));

        /// <summary>Пароль</summary>
        [Category("Other")]
        [Description("Пароль")]
        public string Password { get => (string)GetValue(PasswordProperty); set => SetValue(PasswordProperty, value); }

        private static void OnPasswordChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
        {
            var password_watcher = (PasswordBoxWatcher)D;
            var password_box = (PasswordBox)password_watcher.Child;

            if (password_watcher._IsPreventCallback) return;

            password_box.PasswordChanged -= password_watcher._OnHandlePasswordChangedDelegate;
            password_box.Password = E.NewValue?.ToString() ?? "";
            password_box.PasswordChanged += password_watcher._OnHandlePasswordChangedDelegate;
        }

        #endregion

        private bool _IsPreventCallback;
        private readonly RoutedEventHandler _OnHandlePasswordChangedDelegate;

        public PasswordBoxWatcher()
        {
            _OnHandlePasswordChangedDelegate = OnHandlePasswordChanged;

            var password_box = new PasswordBox();
            password_box.PasswordChanged += _OnHandlePasswordChangedDelegate;
            Child = password_box;
        }

        /// <summary>Обработчик события изменения пароля</summary>
        /// <param name="sender">Источник события - должен быть полем ввода пароля</param>
        /// <param name="e">Аргумент события</param>
        private void OnHandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            _IsPreventCallback = true;
            Password = ((PasswordBox)sender).Password;
            _IsPreventCallback = false;
        }
    }

    /*- ---------------------------------------------------------------------------------- -*/

    /// <summary>
    /// Creates a bindable attached property for the <see cref="PasswordBox.SecurePassword"/> property.
    /// </summary>
    public static class PasswordBoxHelper
    {
        // an attached behavior won't work due to view model validation not picking up the right control to adorn
        public static readonly DependencyProperty SecurePasswordBindingProperty = DependencyProperty.RegisterAttached(
            "ShadowSecurePassword",
            typeof(SecureString),
            typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(new SecureString(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AttachedPropertyValueChanged)
        );

        private static readonly DependencyProperty __PasswordBindingMarshallerProperty = DependencyProperty.RegisterAttached(
            "PasswordBindingMarshaller",
            typeof(PasswordBindingMarshaller),
            typeof(PasswordBoxHelper),
            new PropertyMetadata()
        );

        public static void SetSecurePassword(PasswordBox element, SecureString SecureString) => element.SetValue(SecurePasswordBindingProperty, SecureString);

        public static SecureString GetSecurePassword(PasswordBox element) => element.GetValue(SecurePasswordBindingProperty) as SecureString;

        private static void AttachedPropertyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // we'll need to hook up to one of the element's events
            // in order to allow the GC to collect the control, we'll wrap the event handler inside an object living in an attached property
            // don't be tempted to use the Unloaded event as that will be fired  even when the control is still alive and well (e.g. switching tabs in a tab control) 
            var password_box = (PasswordBox)d;
            var binding_marshaller = password_box.GetValue(__PasswordBindingMarshallerProperty) as PasswordBindingMarshaller;
            if (binding_marshaller is null)
            {
                binding_marshaller = new PasswordBindingMarshaller(password_box);
                password_box.SetValue(__PasswordBindingMarshallerProperty, binding_marshaller);
            }

            binding_marshaller.UpdatePasswordBox(e.NewValue as SecureString);
        }

        /// <summary>
        /// Encapsulated event logic
        /// </summary>
        private class PasswordBindingMarshaller
        {
            private readonly PasswordBox _PasswordBox;
            private bool _IsMarshalling;

            public PasswordBindingMarshaller(PasswordBox PasswordBox)
            {
                _PasswordBox = PasswordBox;
                _PasswordBox.PasswordChanged += PasswordBoxPasswordChanged;
            }

            public void UpdatePasswordBox(SecureString NewPassword)
            {
                if (_IsMarshalling)
                    return;

                _IsMarshalling = true;
                try
                {
                    // setting up the SecuredPassword won't trigger a visual update so we'll have to use the Password property
                    _PasswordBox.Password = SecureStringToString(NewPassword);

                    // you may try the statement below, however the benefits are minimal security wise (you still have to extract the unsecured password for copying)
                    //newPassword.CopyInto(_passwordBox.SecurePassword);
                }
                finally
                {
                    _IsMarshalling = false;
                }
            }

            private static string SecureStringToString(SecureString value)
            {
                var bstr = Marshal.SecureStringToBSTR(value);

                try
                {
                    return Marshal.PtrToStringBSTR(bstr);
                }
                finally
                {
                    Marshal.FreeBSTR(bstr);
                }
            }

            private void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
            {
                // copy the password into the attached property
                if (_IsMarshalling)
                    return;

                _IsMarshalling = true;
                try
                {
                    SetSecurePassword(_PasswordBox, _PasswordBox.SecurePassword.Copy());
                }
                finally
                {
                    _IsMarshalling = false;
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class RequiredSecureStringAttribute : ValidationAttribute
    {
        public RequiredSecureStringAttribute() : base("Field is required") { }

        public override bool IsValid(object value) => (value as SecureString)?.Length > 0;
    }
}