#if !NET7_0_OR_GREATER

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
internal sealed class RequiredMemberAttribute : Attribute;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
internal sealed class CompilerFeatureRequiredAttribute : Attribute
{
    public const string RequiredMembers = nameof(RequiredMembers);

    public const string RefStructs = nameof(RefStructs);

    public string FeatureName { get; }

    public bool IsOptional { get; init; }

    public CompilerFeatureRequiredAttribute(string FeatureName) => this.FeatureName = FeatureName;
}

#endif