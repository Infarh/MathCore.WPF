using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF;

public class BParent : MarkupExtension
{
    [ConstructorArgument(nameof(Path))]
    public string Path { get; set; } = null!;

    public BParent() { }

    public BParent(string Path) => this.Path = Path;

    public override object ProvideValue(IServiceProvider sp)
    {
        var binding = new Binding
        {
            RelativeSource = new(RelativeSourceMode.TemplatedParent),
            Path = new(Path)
        };

        return binding.ProvideValue(sp);
    }
}

public class B : MarkupExtension
{
    [ConstructorArgument(nameof(Path))]
    public string Path { get; set; } = null!;

    public B() { }

    public B(string Path) => this.Path = Path;

    public override object ProvideValue(IServiceProvider sp)
    {
        var binding = new Binding
        {
            Path = new(Path)
        };

        return binding.ProvideValue(sp);
    }
}

public class Bself : MarkupExtension
{
    [ConstructorArgument(nameof(Path))]
    public string Path { get; set; } = null!;

    public Bself() { }

    public Bself(string Path) => this.Path = Path;

    public override object ProvideValue(IServiceProvider sp)
    {
        var binding = new Binding
        {
            RelativeSource = new(RelativeSourceMode.Self),
            Path = new(Path)
        };

        return binding.ProvideValue(sp);
    }
}

public class Brel : MarkupExtension
{
    [ConstructorArgument(nameof(Path))]
    public string Path { get; set; } = null!;

    public Type AncessorType { get; set; } = null!;

    public int AncessorLevel { get; set; } = 1;

    public Brel() { }

    public Brel(string Path) => this.Path = Path;
    public Brel(string Path, Type AncessorType) : this(Path) => this.AncessorType = AncessorType;
    public Brel(string Path, Type AncessorType, int AncessorLevel) : this(Path, AncessorType) => this.AncessorLevel = AncessorLevel;

    public override object ProvideValue(IServiceProvider sp)
    {
        var binding = new Binding
        {
            RelativeSource = new(RelativeSourceMode.FindAncestor, AncessorType, AncessorLevel),
            Path = new(Path)
        };

        return binding.ProvideValue(sp);
    }
}
