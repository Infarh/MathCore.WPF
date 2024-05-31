
namespace MathCore.WPF.WindowTest;

public partial class TestWindow6
{
    public double Test { get; set; }

    private void OnTestChanged(TestWindow6 sender, double NewValue, double OldValue)
    {
        
    }

    private double TestValueCoerce(TestWindow6 sender, double value) => value;

    private bool TestValueValidate(double value) => true;

    public TestWindow6() => InitializeComponent();
}
