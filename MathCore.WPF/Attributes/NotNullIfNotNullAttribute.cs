namespace MathCore.WPF.Attributes;

/// <summary>Initializes the attribute with the associated parameter name.</summary>
/// <param name="ParameterName">
/// The associated parameter name.  The output will be non-null if the argument to the parameter specified is non-null.
/// </param>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
internal sealed class NotNullIfNotNullAttribute(string ParameterName) : Attribute
{

    /// <summary>Gets the associated parameter name.</summary>
    public string ParameterName { get; } = ParameterName;
}
