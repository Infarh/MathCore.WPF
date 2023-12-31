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

public class B(string Path) : MarkupExtension
{
    public B() : this(null) { }

    [ConstructorArgument(nameof(Path))]
    public string Path { get; set; } = Path;

    public override object ProvideValue(IServiceProvider sp)
    {
        var binding = new Binding
        {
            Path = new(Path)
        };

        return binding.ProvideValue(sp);
    }
}

public class Bself(string Path) : MarkupExtension
{
    public Bself() : this(null) { }

    [ConstructorArgument(nameof(Path))]
    public string Path { get; set; } = Path;

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

public class Brel(string Path) : MarkupExtension
{
    public Brel() : this(null) { }

    public Brel(string Path, Type AncessorType) : this(Path) => this.AncessorType = AncessorType;

    public Brel(string Path, Type AncessorType, int AncessorLevel) : this(Path, AncessorType) => this.AncessorLevel = AncessorLevel;

    [ConstructorArgument(nameof(Path))]
    public string Path { get; set; } = Path;

    public Type AncessorType { get; set; } = null!;

    public int AncessorLevel { get; set; } = 1;

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
