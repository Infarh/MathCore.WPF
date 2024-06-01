namespace MathCore.WPF.TeX;

/// <summary>Atom representing other atom that is not rendered</summary>
internal class PhantomAtom : Atom, IRow
{
    private readonly bool _UseWidth;
    private readonly bool _UseHeight;
    private readonly bool _UseDepth;

    public DummyAtom PreviousAtom { get => RowAtom.PreviousAtom; set => RowAtom.PreviousAtom = value; }

    public RowAtom RowAtom { get; }

    public PhantomAtom(Atom BaseAtom, bool UseWidth = true, bool UseHeight = true, bool UseDepth = true)
    {
        RowAtom        = BaseAtom is null ? new() : new RowAtom(BaseAtom);
        this._UseWidth  = UseWidth;
        this._UseHeight = UseHeight;
        this._UseDepth  = UseDepth;
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        var result_box = RowAtom.CreateBox(environment);
        return new StrutBox((_UseWidth ? result_box.Width : 0), (_UseHeight ? result_box.Height : 0),
            (_UseDepth ? result_box.Depth : 0), result_box.Shift);
    }

    public override TexAtomType GetLeftType() => RowAtom.GetLeftType();

    public override TexAtomType GetRightType() => RowAtom.GetRightType();
}