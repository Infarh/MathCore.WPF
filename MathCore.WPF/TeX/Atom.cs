namespace MathCore.WPF.TeX;

/// <summary>Atom (smallest unit) of TexFormula</summary>
internal abstract class Atom
{
    protected Atom() => Type = TexAtomType.Ordinary;

    public TexAtomType Type { get; set; }

    public abstract Box CreateBox(TexEnvironment environment);

    /// <summary>Gets type of leftmost child item</summary>
    /// <returns></returns>
    public virtual TexAtomType GetLeftType() => Type;

    /// <summary>Gets type of leftmost child item</summary>
    /// <returns></returns>
    public virtual TexAtomType GetRightType() => Type;
}