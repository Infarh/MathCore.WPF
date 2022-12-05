using System.ComponentModel.DataAnnotations;
using System.Security;

namespace MathCore.WPF;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class RequiredSecureStringAttribute : ValidationAttribute
{
    public RequiredSecureStringAttribute() : base("Field is required") { }

    public override bool IsValid(object? value) => (value as SecureString)?.Length > 0;
}