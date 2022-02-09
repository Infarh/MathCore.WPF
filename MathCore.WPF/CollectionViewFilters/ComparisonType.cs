using System.ComponentModel;

namespace MathCore.WPF;

public enum ComparisonType : byte
{
    [Description("<")]
    Less,
    [Description("<=")]
    LessOrEqual,
    [Description("==")]
    Equal,
    [Description(">=")]
    GreaterOrEqual,
    [Description(">")]
    Greater,
    [Description("!=")]
    NotEqual
}