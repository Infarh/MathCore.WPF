using MathCore.WPF.ViewModels;

namespace MathCore.WPF.Tests.ViewModels;

[TestClass]
public class ViewModelTests
{
    private class TestViewModel : ViewModel
    {
        #region P1 : string - P1

        /// <summary>P1</summary>
        private string? _P1;

        public string? P1
        {
            get => _P1;
            set => Set(ref _P1, value);
        }

        #endregion

        [DependencyOn(nameof(P1))]
        public int P11 => _P1?.Length ?? -1;

        [DependencyOn(nameof(P11))]
        public int P111 => P11 * 10;

        #region P2 : string - P2

        /// <summary>P2</summary>
        private string? _P2;

        /// <summary>P2</summary>
        [ChangedHandler(nameof(OnP2Changed))]
        public string? P2 { get => _P2; set => Set(ref _P2, value); }

        public event EventHandler? P2Changed;

        private void OnP2Changed() => P2Changed?.Invoke(this, EventArgs.Empty);

        #endregion
    }

    [TestMethod]
    public void PropertyChanges_TransientDependencies()
    {
        var model = new TestViewModel();

        var properties = new List<string>();
        model.PropertyChanged += (_, e) => properties.Add(e.PropertyName!);

        model.P1 = "data";

        Assert.AreEqual(3, properties.Count);
        CollectionAssert.AreEqual(new[] { nameof(TestViewModel.P1), nameof(TestViewModel.P11), nameof(TestViewModel.P111) }, properties);
    }

    [TestMethod]
    public void PropertyChanges_ChangedHandler()
    {
        var model   = new TestViewModel();
        var invoked = false;

        model.P2Changed += (_, _) => invoked = true;

        model.P2 = "qwe";
        Assert.IsTrue(invoked);
    }
}