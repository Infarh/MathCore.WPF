namespace MathCore.WPF;

public class NestedBindingNode(int index)
{
    public int Index => index;

    public override string ToString() => index.ToString();
}