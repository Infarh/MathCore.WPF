using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

class Link : ViewModel
{
    public Node Start { get; }

    public Node End { get; }

    #region Weight : double - Вес

    /// <summary>Вес</summary>
    private double _Weight;

    /// <summary>Вес</summary>
    public double Weight { get => _Weight; set => Set(ref _Weight, value); }

    #endregion

    public Link(Node Start, Node End, double Weight = 1)
    {
        this.Start = Start;
        this.End   = End;
        _Weight    = Weight;
    }
}