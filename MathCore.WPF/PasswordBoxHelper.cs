using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace MathCore.WPF;

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
        if (!(password_box.GetValue(__PasswordBindingMarshallerProperty) is PasswordBindingMarshaller binding_marshaller))
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