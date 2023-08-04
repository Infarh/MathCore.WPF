namespace MathCore.WPF;

public class NestedBindingNode
{
    public NestedBindingNode(int index) => Index = index;

    public int Index { get; }

    public override string ToString() => Index.ToString();
}