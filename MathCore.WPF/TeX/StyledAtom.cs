using System.Windows.Media;

namespace MathCore.WPF.TeX;

/// <summary>Atom specifying graphical style</summary>
internal sealed class StyledAtom : Atom, IRow
{
    public DummyAtom PreviousAtom
    {
        get => RowAtom.PreviousAtom;
        set => RowAtom.PreviousAtom = value;
    }

    /// <summary>RowAtom to which colors are applied</summary>
    public RowAtom RowAtom { get; }

    public Brush Background { get; set; }

    public Brush Foreground { get; set; }

    public StyledAtom(Atom atom, Brush BackgroundColor, Brush ForegroundColor)
    {
        RowAtom    = new RowAtom(atom);
        Background = BackgroundColor;
        Foreground = ForegroundColor;
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        var new_environment = environment.Clone();
        if(Background != null)
            new_environment.Background = Background;
        if(Foreground != null)
            new_environment.Foreground = Foreground;
        return RowAtom.CreateBox(new_environment);
    }

    public override TexAtomType GetLeftType() => RowAtom.GetLeftType();

    public override TexAtomType GetRightType() => RowAtom.GetRightType();

    public StyledAtom Clone() => new(RowAtom, Background, Foreground);
}